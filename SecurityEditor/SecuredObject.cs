using System;
using System.Security.AccessControl;

namespace Community.Security.AccessControl
{
	/* Derivitives of NativeObjectSecurity: (default is Group|Owner|Access)
		Security												Object.GetSecurityControl()						V	Sec	Nm
		===========												===========================						==	===	===
		System.IO.Pipes.PipeSecurity							System.IO.Pipes.PipeStream						3.5	N
		EventWaitHandleSecurity									System.Threading.EventWaitHandle				2	N
		FileSystemSecurity										System.IO.DirectoryInfo, FileInfo				2	Y	Name
		FileSystemSecurity										System.IO.FileStream							2	N	Name
		MutexSecurity											System.Threading.Mutex							2	N
		ObjectSecurity<T>																						4	N
		RegistrySecurity										System.Win32.RegistryKey						2	Y	Name
		SemaphoreSecurity										System.Threading.Semaphore						2	N
		System.IO.MemoryMappedFiles.MemoryMappedFileSecurity	System.IO.MemoryMappedFiles.MemoryMappedFile	4	N
	*/

	internal class SecuredObject
	{
		private static string[] nonContainerTypes = new string[] { "FileSecurity", "PipeSecurity", "CryptoKeySecurity", "MemoryMappedFileSecurity", "TaskSecurity" };

		public SecuredObject(object knownObject)
		{
			// If just being passed a security object, grab it and stop
			if (knownObject is CommonObjectSecurity)
			{
				ObjectSecurity = knownObject as CommonObjectSecurity;
			}
			else
			{
				// Special handling for Tasks
				if (knownObject.GetType().FullName == "Microsoft.Win32.TaskScheduler.Task" || knownObject.GetType().FullName == "Microsoft.Win32.TaskScheduler.TaskFolder")
				{
					IsContainer = knownObject.GetType().Name == "TaskFolder";

					var piTS = knownObject.GetType().GetProperty("TaskService", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					if (piTS == null)
						throw new ArgumentException("Invalid Task type.");
					var piSv = piTS.PropertyType.GetProperty("TargetServer", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					if (piSv == null)
						throw new ArgumentException("Invalid Task type.");
					TargetServer = (string)piSv.GetValue(piTS.GetValue(knownObject, null), null);
				}

				// Get the security object using the standard "GetAccessControl" method
				System.Reflection.MethodInfo mi = null;
				object[] methodParams = null;
				mi = knownObject.GetType().GetMethod("GetAccessControl", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, System.Reflection.CallingConventions.Any, new Type[] { typeof(AccessControlSections) }, null);
				if (mi != null)
					methodParams = new object[] { AccessControlSections.All };
				else
					mi = knownObject.GetType().GetMethod("GetAccessControl", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, System.Reflection.CallingConventions.Any, Type.EmptyTypes, null);
				if (mi != null)
				{
					try { ObjectSecurity = mi.Invoke(knownObject, methodParams) as CommonObjectSecurity; }
					catch
					{
						if (methodParams != null)
						{
							methodParams = new object[] { AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner };
							try { ObjectSecurity = mi.Invoke(knownObject, methodParams) as CommonObjectSecurity; }
							catch { }
						}
					}
				}
				else
					throw new ArgumentException("Object must have a GetAccessControl member.");

				// Get the object names
				switch (knownObject.GetType().Name)
				{
					case "RegistryKey":
						ObjectName = GetProperty(knownObject, "Name");
						DisplayName = System.IO.Path.GetFileNameWithoutExtension(ObjectName);
						break;
					case "Task":
						DisplayName = GetProperty(knownObject, "Name");
						ObjectName = GetProperty(knownObject, "Path");
						break;
					default:
						ObjectName = GetProperty(knownObject, "FullName");
						DisplayName = GetProperty(knownObject, "Name");
						if (ObjectName == null) ObjectName = DisplayName;
						break;
				}

				// Set the base object
				BaseObject = knownObject;
			}
			this.IsContainer = SecuredObject.IsContainerObject(ObjectSecurity);
			this.MandatoryLabel = new SystemMandatoryLabel(this.ObjectSecurity);
		}

		internal static string GetProperty(object knownObject, string propName)
		{
			var pi = knownObject.GetType().GetProperty(propName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			if (pi != null)
				return Convert.ToString(pi.GetValue(knownObject, null));
			return null;
		}

		public enum SystemMandatoryLabelLevel
		{
			None = 0,
			Low = 0x1000,
			Medium = 0x2000,
			High = 0x3000
		}

		[Flags]
		public enum SystemMandatoryLabelPolicy
		{
			None = 0,
			NoWriteUp = 1,
			NoReadUp = 2,
			NoExecuteUp = 4
		}

		public object BaseObject { get; private set; }

		public string DisplayName { get; set; }

		public string ObjectName { get; set; }

		public bool IsContainer { get; set; }

		public SystemMandatoryLabel MandatoryLabel { get; private set; }

		public CommonObjectSecurity ObjectSecurity { get; private set; }

		public ResourceType ResourceType { get { return GetResourceType(ObjectSecurity); } }

		public string TargetServer { get; set; }

		public static object GetAccessMask(CommonObjectSecurity acl, AuthorizationRule rule)
		{
			if (rule.GetType() == acl.AccessRuleType || rule.GetType() == acl.AuditRuleType)
			{
				Type accRightType = acl.AccessRightType;
				foreach (var pi in rule.GetType().GetProperties())
					if (pi.PropertyType == accRightType)
						return Enum.ToObject(accRightType, pi.GetValue(rule, null));
			}
			throw new ArgumentException();
		}

		public static object GetKnownObject(ResourceType resType, string objName, string serverName)
		{
			object obj = null;
			switch (resType)
			{
				case System.Security.AccessControl.ResourceType.FileObject:
					if (!string.IsNullOrEmpty(serverName))
						objName = System.IO.Path.Combine(serverName, objName);
					if (System.IO.File.Exists(objName))
						obj = new System.IO.FileInfo(objName);
					else if (System.IO.Directory.Exists(objName))
						obj = new System.IO.DirectoryInfo(objName);
					break;
				case System.Security.AccessControl.ResourceType.RegistryKey:
					obj = GetKeyFromKeyName(objName, serverName);
					break;
				case Community.Windows.Forms.AccessControlEditorDialog.TaskResourceType:
					obj = GetTaskObj(objName, serverName);
					break;
				default:
					break;
			}
			if (obj == null)
				throw new ArgumentException("Unable to create an object from supplied arguments.");
			return obj;
		}

		private static object GetTaskObj(string objName, string serverName)
		{
			try
			{
				System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFrom("Microsoft.Win32.TaskScheduler.dll");
				if (asm != null)
				{
					Type tsType = asm.GetType("Microsoft.Win32.TaskScheduler.TaskService", false, false);
					if (tsType != null)
					{
						object ts = Activator.CreateInstance(tsType, serverName, (string)null, (string)null, (string)null, false);
						if (ts != null)
						{
							System.Reflection.MethodInfo mi = tsType.GetMethod("GetFolder", new Type[] { typeof(string) });
							if (mi != null)
							{
								try
								{
									object r = mi.Invoke(ts, new object[] { objName });
									if (r != null)
										return r;
								}
								catch { }
							}

							mi = tsType.GetMethod("GetTask", new Type[] { typeof(string) });
							if (mi != null)
							{
								try
								{
									object r = mi.Invoke(ts, new object[] { objName });
									if (r != null)
										return r;
								}
								catch { }
							}

							mi = tsType.GetMethod("FindTask", new Type[] { typeof(string), typeof(bool) });
							if (mi != null)
							{
								try
								{
									object r = mi.Invoke(ts, new object[] { objName, true });
									if (r != null)
										return r;
								}
								catch { }
							}
						}
					}
				}
			}
			catch { }
			return null;
		}

		private static Microsoft.Win32.RegistryKey GetKeyFromKeyName(string keyName, string serverName)
		{
			if (keyName == null)
				return null;

			string str;
			int index = keyName.IndexOf('\\');
			if (index != -1)
				str = keyName.Substring(0, index).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
			else
				str = keyName.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

			Microsoft.Win32.RegistryHive hive;
			switch (str)
			{
				case "HKEY_CURRENT_USER":
					hive = Microsoft.Win32.RegistryHive.CurrentUser;
					break;

				case "HKEY_LOCAL_MACHINE":
					hive = Microsoft.Win32.RegistryHive.LocalMachine;
					break;

				case "HKEY_CLASSES_ROOT":
					hive = Microsoft.Win32.RegistryHive.ClassesRoot;
					break;

				case "HKEY_USERS":
					hive = Microsoft.Win32.RegistryHive.Users;
					break;

				case "HKEY_PERFORMANCE_DATA":
					hive = Microsoft.Win32.RegistryHive.PerformanceData;
					break;

				case "HKEY_CURRENT_CONFIG":
					hive = Microsoft.Win32.RegistryHive.CurrentConfig;
					break;

				default:
					return null;
			}

			if (serverName == null)
				serverName = string.Empty;

			Microsoft.Win32.RegistryKey retVal = Microsoft.Win32.RegistryKey.OpenRemoteBaseKey(hive, serverName);

			if ((index == -1) || (index == keyName.Length))
				return retVal;

			string subKeyName = keyName.Substring(index + 1, (keyName.Length - index) - 1);
			return retVal.OpenSubKey(subKeyName);
		}

		public static ResourceType GetResourceType(CommonObjectSecurity sec)
		{
			switch (sec.GetType().Name)
			{
				case "FileSecurity":
				case "DirectorySecurity":
				case "CryptoKeySecurity":
					return ResourceType.FileObject;

				case "PipeSecurity":
				case "EventWaitHandleSecurity":
				case "MutexSecurity":
				case "MemoryMappedFileSecurity":
				case "SemaphoreSecurity":
					return ResourceType.KernelObject;

				case "RegistrySecurity":
					return ResourceType.RegistryKey;

				case "TaskSecurity":
					return Community.Windows.Forms.AccessControlEditorDialog.TaskResourceType;
			}
			return ResourceType.Unknown;
		}

		public static bool IsContainerObject(CommonObjectSecurity sec)
		{
			string secTypeName = sec.GetType().Name;
			return !Array.Exists<string>(nonContainerTypes, delegate(string s) { return secTypeName == s; });
		}

		public object GetAccessMask(AuthorizationRule rule)
		{
			return GetAccessMask(this.ObjectSecurity, rule);
		}

		public void Persist(object newBase = null)
		{
			object obj = (newBase != null) ? newBase : BaseObject;
			if (obj == null)
				throw new ArgumentNullException("Either newBase or BaseObject must not be null.");

			var mi = obj.GetType().GetMethod("SetAccessControl", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, Type.EmptyTypes, null);
			if (mi == null)
				throw new InvalidOperationException("Either newBase or BaseObject must represent a securable object.");

			mi.Invoke(obj, new object[] { this.ObjectSecurity });
		}

		public class SystemMandatoryLabel
		{
			public SystemMandatoryLabel(CommonObjectSecurity sec)
			{
				Policy = SystemMandatoryLabelPolicy.None;
				Level = SystemMandatoryLabelLevel.None;

				try
				{
					var sd = new RawSecurityDescriptor(sec.GetSecurityDescriptorBinaryForm(), 0);
					if (sd.SystemAcl != null)
					{
						foreach (var ace in sd.SystemAcl)
						{
							if ((int)ace.AceType == 0x11)
							{
								byte[] aceBytes = new byte[ace.BinaryLength];
								ace.GetBinaryForm(aceBytes, 0);
								//_policy = new IntegrityPolicy(aceBytes, 4);
								//_level = new IntegrityLevel(aceBytes, 8);
							}
						}
					}
				}
				catch { }
				/*byte[] saclBinaryForm = new byte[sd.SystemAcl.BinaryLength];
				sd.SystemAcl.GetBinaryForm(saclBinaryForm, 0);
				GenericAce ace = null;
				if (null != saclBinaryForm)
				{
					RawAcl aclRaw = new RawAcl(saclBinaryForm, 0);
					if (0 >= aclRaw.Count) throw new ArgumentException("No ACEs in ACL", "saclBinaryForm");
					ace = aclRaw[0];
					if (Win32.SYSTEM_MANDATORY_LABEL_ACE_TYPE != (int)ace.AceType)
						throw new ArgumentException("No Mandatory Integrity Label in ACL", "saclBinaryForm");
					byte[] aceBytes = new byte[ace.BinaryLength];
					ace.GetBinaryForm(aceBytes, 0);
					_policy = new IntegrityPolicy(aceBytes, 4);
					_level = new IntegrityLevel(aceBytes, 8);
					return;
				}
				throw new ArgumentNullException("saclBinaryForm");*/
			}

			public bool IsSet { get { return Policy != SystemMandatoryLabelPolicy.None && Level != SystemMandatoryLabelLevel.None; } }

			public SystemMandatoryLabelLevel Level { get; private set; }

			public SystemMandatoryLabelPolicy Policy { get; private set; }
		}
	}
}