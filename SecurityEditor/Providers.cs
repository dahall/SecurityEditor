using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Community.Security.AccessControl
{
	/// <summary>
	/// An interface for defining an information provider for object types supplied to the <see cref="Community.Windows.Forms.AccessControlEditorDialog"/>.
	/// </summary>
	public interface IAccessControlEditorDialogProvider
	{
		/// <summary>
		/// Gets the type of the resource.
		/// </summary>
		/// <value>
		/// The type of the resource.
		/// </value>
		ResourceType ResourceType { get; }

		/// <summary>
		/// Gets an array of <see cref="AccessRightInfo" /> structures which define how to display differnt access rights supplied to the editor along with the index of the access right that should be applied to new ACEs.
		/// </summary>
		/// <param name="rights">The access right information for each right.</param>
		/// <param name="defaultIndex">The default index in the <paramref name="rights" /> array for new ACEs.</param>
		void GetAccessListInfo(out AccessRightInfo[] rights, out uint defaultIndex);

		/// <summary>
		/// Gets a default Security Descriptor for resetting the security of the object.
		/// </summary>
		/// <returns>Pointer to a Security Descriptor.</returns>
		IntPtr GetDefaultSecurity();

		/// <summary>
		/// Gets the effective permissions for the provided Sid within the Security Descriptor. Called only when no object type identifier is specified.
		/// </summary>
		/// <param name="pUserSid">A pointer to the Sid of the identity to check.</param>
		/// <param name="serverName">Name of the server. This can be <c>null</c>.</param>
		/// <param name="pSecurityDescriptor">A pointer to the security descriptor.</param>
		/// <returns>An array of access masks.</returns>
		uint[] GetEffectivePermission(IntPtr pUserSid, string serverName, IntPtr pSecurityDescriptor);

		/// <summary>
		/// Gets the effective permissions for the provided Sid within the Security Descriptor. Called only when an object type identifier is specified.
		/// </summary>
		/// <param name="objTypeId">The object type identifier.</param>
		/// <param name="pUserSid">A pointer to the Sid of the identity to check.</param>
		/// <param name="serverName">Name of the server. This can be <c>null</c>.</param>
		/// <param name="pSecurityDescriptor">A pointer to the security descriptor.</param>
		/// <param name="objectTypeList">The object type list.</param>
		/// <returns>An array of access masks.</returns>
		uint[] GetEffectivePermission(Guid objTypeId, IntPtr pUserSid, string serverName, IntPtr pSecurityDescriptor, out ObjectTypeInfo[] objectTypeList);

		/// <summary>
		/// Gets the generic mapping for standard rights.
		/// </summary>
		/// <returns>A <see cref="GenericRightsMapping"/> structure fror this object type.</returns>
		GenericRightsMapping GetGenericMapping();

		/// <summary>
		/// Determines the source of inherited access control entries (ACEs) in discretionary access control lists (DACLs) and system access control lists (SACLs).
		/// </summary>
		/// <param name="objName">Name of the object.</param>
		/// <param name="isContainer">If set to <c>true</c> object is a container.</param>
		/// <param name="si">The object-related security information being queried. See SECURITY_INFORMATION type in Windows documentation.</param>
		/// <param name="pAcl">A pointer to the Acl.</param>
		/// <returns>
		/// An array of <see cref="InheritedFromInfo" /> structures. The length of this array is the same as the number of ACEs in the ACL referenced by pACL. Each <see cref="InheritedFromInfo" /> entry provides inheritance information for the corresponding ACE entry in pACL.
		/// </returns>
		InheritedFromInfo[] GetInheritSource(string objName, bool isContainer, uint si, IntPtr pAcl);

		/// <summary>
		/// Gets inheritance information for supported object type.
		/// </summary>
		/// <returns>An array of <see cref="InheritTypeInfo" /> that includes one entry for each combination of inheritance flags and child object type that you support.</returns>
		InheritTypeInfo[] GetInheritTypes();

		/// <summary>
		/// Callback method for the property pages.
		/// </summary>
		/// <param name="hwnd">The HWND.</param>
		/// <param name="uMsg">The message.</param>
		/// <param name="uPage">The page type.</param>
		void PropertySheetPageCallback(IntPtr hwnd, PropertySheetCallbackMessage uMsg, PropertySheetPageType uPage);
	}

	/// <summary>
	/// Base implementation of <see cref="IAccessControlEditorDialogProvider"/>.
	/// </summary>
	public class GenericProvider : IAccessControlEditorDialogProvider
	{
		/// <summary>
		/// Gets the type of the resource.
		/// </summary>
		/// <value>
		/// The type of the resource.
		/// </value>
		virtual public ResourceType ResourceType
		{
			get { return ResourceType.Unknown; }
		}

		/// <summary>
		/// Gets an array of <see cref="AccessRightInfo" /> structures which define how to display differnt access rights supplied to the editor along with the index of the access right that should be applied to new ACEs.
		/// </summary>
		/// <param name="rights">The access right information for each right.</param>
		/// <param name="defaultIndex">The default index in the <paramref name="rights" /> array for new ACEs.</param>
		virtual public void GetAccessListInfo(out AccessRightInfo[] rights, out uint defaultIndex)
		{
			rights = new AccessRightInfo[] {
				new AccessRightInfo(0, ResStr("Object"), (AccessFlags)0)
			};
			defaultIndex = 0;
		}

		/// <summary>
		/// Gets a default Security Descriptor for resetting the security of the object.
		/// </summary>
		/// <returns>
		/// Pointer to a Security Descriptor.
		/// </returns>
		virtual public IntPtr GetDefaultSecurity()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the effective permissions for the provided Sid within the Security Descriptor.
		/// </summary>
		/// <param name="pUserSid">A pointer to the Sid of the identity to check.</param>
		/// <param name="serverName">Name of the server. This can be <c>null</c>.</param>
		/// <param name="pSecurityDescriptor">A pointer to the security descriptor.</param>
		/// <returns>
		/// An array of access masks.
		/// </returns>
		virtual public uint[] GetEffectivePermission(IntPtr pUserSid, string serverName, IntPtr pSecurityDescriptor)
		{
			uint mask = Helper.GetEffectiveRights(pUserSid, pSecurityDescriptor);
			return new uint[] { mask };
		}

		/// <summary>
		/// Gets the effective permissions for the provided Sid within the Security Descriptor. Called only when an object type identifier is specified.
		/// </summary>
		/// <param name="objTypeId">The object type identifier.</param>
		/// <param name="pUserSid">A pointer to the Sid of the identity to check.</param>
		/// <param name="serverName">Name of the server. This can be <c>null</c>.</param>
		/// <param name="pSecurityDescriptor">A pointer to the security descriptor.</param>
		/// <param name="objectTypeList">The object type list.</param>
		/// <returns>
		/// An array of access masks.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		virtual public uint[] GetEffectivePermission(Guid objTypeId, IntPtr pUserSid, string serverName, IntPtr pSecurityDescriptor, out ObjectTypeInfo[] objectTypeList)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the generic mapping for standard rights.
		/// </summary>
		/// <returns>
		/// A <see cref="GenericRightsMapping" /> structure fror this object type.
		/// </returns>
		virtual public GenericRightsMapping GetGenericMapping()
		{
			return new GenericRightsMapping(0x80000000, 0x40000000, 0x20000000, 0x10000000);
		}

		/// <summary>
		/// Determines the source of inherited access control entries (ACEs) in discretionary access control lists (DACLs) and system access control lists (SACLs).
		/// </summary>
		/// <param name="objName">Name of the object.</param>
		/// <param name="isContainer">If set to <c>true</c> object is a container.</param>
		/// <param name="si">The object-related security information being queried. See SECURITY_INFORMATION type in Windows documentation.</param>
		/// <param name="pAcl">A pointer to the Acl.</param>
		/// <returns>
		/// An array of <see cref="InheritedFromInfo" /> structures. The length of this array is the same as the number of ACEs in the ACL referenced by pACL. Each <see cref="InheritedFromInfo" /> entry provides inheritance information for the corresponding ACE entry in pACL.
		/// </returns>
		virtual public InheritedFromInfo[] GetInheritSource(string objName, bool isContainer, uint si, IntPtr pAcl)
		{
			GenericRightsMapping gMap = this.GetGenericMapping();
			var aceCount = Helper.GetAceCount(pAcl);
			var strSize = Marshal.SizeOf(typeof(InheritedFromInfo));
			IntPtr pIA = Marshal.AllocHGlobal(strSize * aceCount);
			var ppInheritArray = new InheritedFromInfo[aceCount];
			try
			{
				Helper.GetInheritanceSource(objName, this.ResourceType, (SecurityInfos)si, isContainer, IntPtr.Zero, 0, pAcl, IntPtr.Zero, ref gMap, pIA);
				IntPtr ppIA = pIA;
				for (int i = 0; i < aceCount; i++)
				{
					ppInheritArray[i] = (InheritedFromInfo)Marshal.PtrToStructure(ppIA, typeof(InheritedFromInfo));
					ppIA = new IntPtr(ppIA.ToInt32() + strSize);
				}
			}
			catch { }
			finally
			{
				Helper.FreeInheritedFromArray(pIA, (ushort)aceCount, IntPtr.Zero);
				Marshal.FreeHGlobal(pIA);
			}
			return ppInheritArray;
		}

		/// <summary>
		/// Gets inheritance information for supported object type.
		/// </summary>
		/// <returns>
		/// An array of <see cref="InheritTypeInfo" /> that includes one entry for each combination of inheritance flags and child object type that you support.
		/// </returns>
		virtual public InheritTypeInfo[] GetInheritTypes()
		{
			return new InheritTypeInfo[] {
				new InheritTypeInfo((InheritFlags)0, ResStr("StdInheritance")),
				new InheritTypeInfo(InheritFlags.Container | InheritFlags.Object, ResStr("StdInheritanceCIOI")),
				new InheritTypeInfo(InheritFlags.Container, ResStr("StdInheritanceCI")),
				new InheritTypeInfo(InheritFlags.Object, ResStr("StdInheritanceOI")),
				new InheritTypeInfo(InheritFlags.InheritOnly | InheritFlags.Container | InheritFlags.Object, ResStr("StdInheritanceIOCIOI")),
				new InheritTypeInfo(InheritFlags.InheritOnly | InheritFlags.Container, ResStr("StdInheritanceIOCI")),
				new InheritTypeInfo(InheritFlags.InheritOnly | InheritFlags.Object, ResStr("StdInheritanceIOOI"))
			};
		}

		/// <summary>
		/// Callback method for the property pages.
		/// </summary>
		/// <param name="hwnd">The HWND.</param>
		/// <param name="uMsg">The message.</param>
		/// <param name="uPage">The page type.</param>
		public virtual void PropertySheetPageCallback(IntPtr hwnd, PropertySheetCallbackMessage uMsg, PropertySheetPageType uPage)
		{
		}

		/// <summary>
		/// Gets a resource string.
		/// </summary>
		/// <param name="id">The string identifier.</param>
		/// <returns>Localized resource string or identifier string if not found.</returns>
		protected string ResStr(string id)
		{
			String ret = Properties.Resources.ResourceManager.GetString(id);
			if (ret == null)
				ret = id;
			return ret;
		}
	}

	internal class FileProvider : GenericProvider
	{
		public override ResourceType ResourceType
		{
			get { return ResourceType.FileObject; }
		}

		public override void GetAccessListInfo(out AccessRightInfo[] rights, out uint defaultIndex)
		{
			rights = new AccessRightInfo[] {
				new AccessRightInfo((uint)FileSystemRights.FullControl, ResStr("FileRightFullControl"), AccessFlags.General | AccessFlags.Specific | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
				new AccessRightInfo((uint)FileSystemRights.Modify, ResStr("FileRightModify"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
				new AccessRightInfo((uint)FileSystemRights.ExecuteFile, ResStr("FileRightExecuteFile"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
				new AccessRightInfo((uint)FileSystemRights.ReadAndExecute, ResStr("FileRightReadAndExecute"), AccessFlags.Container | AccessFlags.ContainerInheritAce),
				new AccessRightInfo((uint)FileSystemRights.Read, ResStr("FileRightRead"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
				new AccessRightInfo((uint)FileSystemRights.Write, ResStr("FileRightWrite"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
				new AccessRightInfo((uint)FileSystemRights.ExecuteFile, ResStr("FileRightExecuteFile"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.ReadData, ResStr("FileRightReadData"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.ReadAttributes, ResStr("FileRightReadAttributes"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.ReadExtendedAttributes, ResStr("FileRightReadExtendedAttributes"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.WriteData, ResStr("FileRightWriteData"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.AppendData, ResStr("FileRightAppendData"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.WriteAttributes, ResStr("FileRightWriteAttributes"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.WriteExtendedAttributes, ResStr("FileRightWriteExtendedAttributes"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.DeleteSubdirectoriesAndFiles, ResStr("FileRightDeleteSubdirectoriesAndFiles"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.Delete, ResStr("StdRightDelete"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.ReadPermissions, ResStr("FileRightReadPermissions"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.ChangePermissions, ResStr("FileRightChangePermissions"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.TakeOwnership, ResStr("StdRightTakeOwnership"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.Synchronize, ResStr("StdRightSynchronize"), AccessFlags.Specific),
				new AccessRightInfo((uint)FileSystemRights.ExecuteFile, ResStr("FileExecute"), (AccessFlags)0),
				new AccessRightInfo((uint)(FileSystemRights.Write | FileSystemRights.ExecuteFile), ResStr("FileRightWriteAndExecute"), (AccessFlags)0),
				new AccessRightInfo((uint)(FileSystemRights.ReadAndExecute | FileSystemRights.Write), ResStr("FileRightReadWriteAndExecute"), (AccessFlags)0),
				new AccessRightInfo(0, ResStr("File"), (AccessFlags)0)
			};
			defaultIndex = 3;
		}

		public override IntPtr GetDefaultSecurity()
		{
			return base.GetDefaultSecurity();
			// TODO: This should return the parent's security or a default access if root.
		}

		public override GenericRightsMapping GetGenericMapping()
		{
			return new GenericRightsMapping(0x00120089, 0x00120116, 0x001200A0, 0x001F01FF);
		}

		public override InheritTypeInfo[] GetInheritTypes()
		{
			return new InheritTypeInfo[] {
				new InheritTypeInfo((InheritFlags)0, ResStr("FileInheritance")),
				new InheritTypeInfo(InheritFlags.Container | InheritFlags.Object, ResStr("FileInheritanceCIOI")),
				new InheritTypeInfo(InheritFlags.Container, ResStr("FileInheritanceCI")),
				new InheritTypeInfo(InheritFlags.Object, ResStr("FileInheritanceOI")),
				new InheritTypeInfo(InheritFlags.InheritOnly | InheritFlags.Container | InheritFlags.Object, ResStr("FileInheritanceIOCIOI")),
				new InheritTypeInfo(InheritFlags.InheritOnly | InheritFlags.Container, ResStr("FileInheritanceIOCI")),
				new InheritTypeInfo(InheritFlags.InheritOnly | InheritFlags.Object, ResStr("FileInheritanceIOOI"))
			};
		}
	}

	internal class KernelProvider : GenericProvider
	{
		public override ResourceType ResourceType
		{
			get { return ResourceType.KernelObject; }
		}
	}

	internal class RegistryProvider : GenericProvider
	{
		public override ResourceType ResourceType
		{
			get { return ResourceType.RegistryKey; }
		}
	}

	internal class TaskProvider : GenericProvider
	{
		public override ResourceType ResourceType
		{
			get { return Community.Windows.Forms.AccessControlEditorDialog.TaskResourceType; }
		}
	}
}