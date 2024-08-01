using Microsoft.Win32;
using System;
using System.Reflection;
using System.Security.AccessControl;

namespace Community.Security.AccessControl;

/// <summary>An interface for defining an information provider for object types supplied to the <see cref="Windows.Forms.AccessControlEditorDialog"/>.</summary>
public interface IAccessControlEditorDialogProvider
{
	/// <summary>Gets the type of the resource.</summary>
	/// <value>The type of the resource.</value>
	ResourceType ResourceType { get; }

	/// <summary>
	/// Gets an array of <see cref="AccessRightInfo"/> structures which define how to display different access rights supplied to the
	/// editor along with the index of the access right that should be applied to new ACEs.
	/// </summary>
	/// <param name="flags">
	/// A set of bit flags that indicate the property page being initialized. This value is zero if the basic security page is being initialized.
	/// </param>
	/// <param name="rights">The access right information for each right.</param>
	/// <param name="defaultIndex">The default index in the <paramref name="rights"/> array for new ACEs.</param>
	void GetAccessListInfo(ObjInfoFlags flags, out AccessRightInfo[] rights, out uint defaultIndex);

	/// <summary>Gets a default Security Descriptor for resetting the security of the object.</summary>
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
	uint[] GetEffectivePermission(Guid objTypeId, IntPtr pUserSid, string serverName, IntPtr pSecurityDescriptor, out ObjectTypeList[] objectTypeList);

	/// <summary>Gets the generic mapping for standard rights.</summary>
	/// <param name="AceFlags">The ace flags.</param>
	/// <returns>A <see cref="GenericMapping"/> structure for this object type.</returns>
	GenericMapping GetGenericMapping(sbyte AceFlags);

	/// <summary>
	/// Determines the source of inherited access control entries (ACEs) in discretionary access control lists (DACLs) and system access
	/// control lists (SACLs).
	/// </summary>
	/// <param name="objName">Name of the object.</param>
	/// <param name="serverName">Name of the server.</param>
	/// <param name="isContainer">If set to <c>true</c> object is a container.</param>
	/// <param name="si">The object-related security information being queried. See SECURITY_INFORMATION type in Windows documentation.</param>
	/// <param name="pAcl">A pointer to the ACL.</param>
	/// <returns>
	/// An array of <see cref="InheritedFromInfo"/> structures. The length of this array is the same as the number of ACEs in the ACL
	/// referenced by pACL. Each <see cref="InheritedFromInfo"/> entry provides inheritance information for the corresponding ACE entry
	/// in pACL.
	/// </returns>
	InheritedFromInfo[] GetInheritSource(string objName, string serverName, bool isContainer, uint si, IntPtr pAcl);

	/// <summary>Gets inheritance information for supported object type.</summary>
	/// <returns>
	/// An array of <see cref="InheritTypeInfo"/> that includes one entry for each combination of inheritance flags and child object type
	/// that you support.
	/// </returns>
	InheritTypeInfo[] GetInheritTypes();

	/// <summary>Callback method for the property pages.</summary>
	/// <param name="hwnd">The HWND.</param>
	/// <param name="uMsg">The message.</param>
	/// <param name="uPage">The page type.</param>
	void PropertySheetPageCallback(IntPtr hwnd, PropertySheetCallbackMessage uMsg, SecurityPageType uPage);
}

/// <summary>Base implementation of <see cref="IAccessControlEditorDialogProvider"/>.</summary>
public class GenericProvider : IAccessControlEditorDialogProvider
{
	/// <summary>Gets the type of the resource.</summary>
	/// <value>The type of the resource.</value>
	public virtual ResourceType ResourceType => ResourceType.Unknown;

	/// <summary>
	/// Gets an array of <see cref="AccessRightInfo"/> structures which define how to display different access rights supplied to the
	/// editor along with the index of the access right that should be applied to new ACEs.
	/// </summary>
	/// <param name="flags">
	/// A set of bit flags that indicate the property page being initialized. This value is zero if the basic security page is being initialized.
	/// </param>
	/// <param name="rights">The access right information for each right.</param>
	/// <param name="defaultIndex">The default index in the <paramref name="rights"/> array for new ACEs.</param>
	public virtual void GetAccessListInfo(ObjInfoFlags flags, out AccessRightInfo[] rights, out uint defaultIndex)
	{
		rights = [ new(0, ResStr("Object"), 0) ];
		defaultIndex = 0;
	}

	/// <summary>Gets a default Security Descriptor for resetting the security of the object.</summary>
	/// <returns>Pointer to a Security Descriptor.</returns>
	public virtual IntPtr GetDefaultSecurity() => throw new NotImplementedException();

	/// <summary>Gets the effective permissions for the provided Sid within the Security Descriptor.</summary>
	/// <param name="pUserSid">A pointer to the Sid of the identity to check.</param>
	/// <param name="serverName">Name of the server. This can be <c>null</c>.</param>
	/// <param name="pSecurityDescriptor">A pointer to the security descriptor.</param>
	/// <returns>An array of access masks.</returns>
	public virtual uint[] GetEffectivePermission(IntPtr pUserSid, string serverName, IntPtr pSecurityDescriptor)
	{
		uint mask = NativeMethods.GetEffectiveRights(pUserSid, pSecurityDescriptor);
		return [mask];
	}

	/// <summary>
	/// Gets the effective permissions for the provided Sid within the Security Descriptor. Called only when an object type identifier is specified.
	/// </summary>
	/// <param name="objTypeId">The object type identifier.</param>
	/// <param name="pUserSid">A pointer to the Sid of the identity to check.</param>
	/// <param name="serverName">Name of the server. This can be <c>null</c>.</param>
	/// <param name="pSecurityDescriptor">A pointer to the security descriptor.</param>
	/// <param name="objectTypeList">The object type list.</param>
	/// <returns>An array of access masks.</returns>
	/// <exception cref="NotImplementedException"></exception>
	public virtual uint[] GetEffectivePermission(Guid objTypeId, IntPtr pUserSid, string serverName, IntPtr pSecurityDescriptor, out ObjectTypeList[] objectTypeList) => throw new NotImplementedException();

	/// <summary>Gets the generic mapping for standard rights.</summary>
	/// <returns>A <see cref="GenericMapping"/> structure for this object type.</returns>
	public virtual GenericMapping GetGenericMapping(sbyte AceFlags) => new(0x80000000, 0x40000000, 0x20000000, 0x10000000);

	/// <summary>
	/// Determines the source of inherited access control entries (ACEs) in discretionary access control lists (DACLs) and system access
	/// control lists (SACLs).
	/// </summary>
	/// <param name="objName">Name of the object.</param>
	/// <param name="serverName">Name of the server.</param>
	/// <param name="isContainer">If set to <c>true</c> object is a container.</param>
	/// <param name="si">The object-related security information being queried. See SECURITY_INFORMATION type in Windows documentation.</param>
	/// <param name="pAcl">A pointer to the ACL.</param>
	/// <returns>
	/// An array of <see cref="InheritedFromInfo"/> structures. The length of this array is the same as the number of ACEs in the ACL
	/// referenced by pACL. Each <see cref="InheritedFromInfo"/> entry provides inheritance information for the corresponding ACE entry
	/// in pACL.
	/// </returns>
	public virtual InheritedFromInfo[] GetInheritSource(string objName, string serverName, bool isContainer, uint si, IntPtr pAcl)
	{
		GenericMapping gMap = GetGenericMapping(0);
		return NativeMethods.GetInheritanceSource(objName, ResourceType, (SecurityInfosEx)si, isContainer, pAcl, ref gMap);
	}

	/// <summary>Gets inheritance information for supported object type.</summary>
	/// <returns>
	/// An array of <see cref="InheritTypeInfo"/> that includes one entry for each combination of inheritance flags and child object type
	/// that you support.
	/// </returns>
	public virtual InheritTypeInfo[] GetInheritTypes() => [
			new(0, ResStr("StdInheritance")),
			new(InheritFlags.Container | InheritFlags.Object, ResStr("StdInheritanceCIOI")),
			new(InheritFlags.Container, ResStr("StdInheritanceCI")),
			new(InheritFlags.Object, ResStr("StdInheritanceOI")),
			new(InheritFlags.InheritOnly | InheritFlags.Container | InheritFlags.Object, ResStr("StdInheritanceIOCIOI")),
			new(InheritFlags.InheritOnly | InheritFlags.Container, ResStr("StdInheritanceIOCI")),
			new(InheritFlags.InheritOnly | InheritFlags.Object, ResStr("StdInheritanceIOOI"))
		];

	/// <summary>Callback method for the property pages.</summary>
	/// <param name="hwnd">The HWND.</param>
	/// <param name="uMsg">The message.</param>
	/// <param name="uPage">The page type.</param>
	public virtual void PropertySheetPageCallback(IntPtr hwnd, PropertySheetCallbackMessage uMsg, SecurityPageType uPage)
	{
	}

	/// <summary>Gets a resource string.</summary>
	/// <param name="id">The string identifier.</param>
	/// <returns>Localized resource string or identifier string if not found.</returns>
	protected static string ResStr(string id) => Properties.Resources.ResourceManager.GetString(id) ?? id;
}

internal class FileProvider : GenericProvider
{
	public override ResourceType ResourceType => ResourceType.FileObject;

	public override void GetAccessListInfo(ObjInfoFlags flags, out AccessRightInfo[] rights, out uint defaultIndex)
	{
		rights = [
			new((uint)FileSystemRights.FullControl, ResStr("FileRightFullControl"), AccessFlags.General | AccessFlags.Specific | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new((uint)FileSystemRights.Modify, ResStr("FileRightModify"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new((uint)FileSystemRights.ReadAndExecute, ResStr("FileRightReadAndExecute"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new((uint)FileSystemRights.ReadAndExecute, ResStr("FileRightListFolderContents"), AccessFlags.Container | AccessFlags.ContainerInheritAce),
			new((uint)FileSystemRights.Read, ResStr("FileRightRead"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new((uint)FileSystemRights.Write, ResStr("FileRightWrite"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new((uint)FileSystemRights.ExecuteFile, ResStr("FileRightExecuteFile"), AccessFlags.Specific),
			new((uint)FileSystemRights.ReadData, ResStr("FileRightReadData"), AccessFlags.Specific),
			new((uint)FileSystemRights.ReadAttributes, ResStr("FileRightReadAttributes"), AccessFlags.Specific),
			new((uint)FileSystemRights.ReadExtendedAttributes, ResStr("FileRightReadExtendedAttributes"), AccessFlags.Specific),
			new((uint)FileSystemRights.WriteData, ResStr("FileRightWriteData"), AccessFlags.Specific),
			new((uint)FileSystemRights.AppendData, ResStr("FileRightAppendData"), AccessFlags.Specific),
			new((uint)FileSystemRights.WriteAttributes, ResStr("FileRightWriteAttributes"), AccessFlags.Specific),
			new((uint)FileSystemRights.WriteExtendedAttributes, ResStr("FileRightWriteExtendedAttributes"), AccessFlags.Specific),
			new((uint)FileSystemRights.DeleteSubdirectoriesAndFiles, ResStr("FileRightDeleteSubdirectoriesAndFiles"), AccessFlags.Specific),
			new((uint)FileSystemRights.Delete, ResStr("StdRightDelete"), AccessFlags.Specific),
			new((uint)FileSystemRights.ReadPermissions, ResStr("FileRightReadPermissions"), AccessFlags.Specific),
			new((uint)FileSystemRights.ChangePermissions, ResStr("FileRightChangePermissions"), AccessFlags.Specific),
			new((uint)FileSystemRights.TakeOwnership, ResStr("StdRightTakeOwnership"), AccessFlags.Specific),
			new((uint)FileSystemRights.Modify, ResStr("FileRightModify"), AccessFlags.Ignore),
			new((uint)FileSystemRights.ReadAndExecute, ResStr("FileRightReadAndExecute"), AccessFlags.Ignore),
			new((uint)(FileSystemRights.Write | FileSystemRights.ExecuteFile), ResStr("FileRightWriteAndExecute"), AccessFlags.Ignore),
			new((uint)(FileSystemRights.ReadAndExecute | FileSystemRights.Write), ResStr("FileRightReadWriteAndExecute"), AccessFlags.Ignore),
			new(0, ResStr("File"), AccessFlags.Ignore)
		];
		defaultIndex = 3;
	}

	public override IntPtr GetDefaultSecurity() => base.GetDefaultSecurity();

	public override GenericMapping GetGenericMapping(sbyte AceFlags) => new((uint)(FileSystemRights.Read | FileSystemRights.Synchronize),
(uint)(FileSystemRights.Write | FileSystemRights.Synchronize),
0x1200A0,
(uint)FileSystemRights.FullControl);

	public override InheritTypeInfo[] GetInheritTypes() => [
			new(0, ResStr("FileInheritance")),
			new(InheritFlags.Container | InheritFlags.Object, ResStr("FileInheritanceCIOI")),
			new(InheritFlags.Container, ResStr("FileInheritanceCI")),
			new(InheritFlags.Object, ResStr("FileInheritanceOI")),
			new(InheritFlags.InheritOnly | InheritFlags.Container | InheritFlags.Object, ResStr("FileInheritanceIOCIOI")),
			new(InheritFlags.InheritOnly | InheritFlags.Container, ResStr("FileInheritanceIOCI")),
			new(InheritFlags.InheritOnly | InheritFlags.Object, ResStr("FileInheritanceIOOI"))
		];
}

internal class KernelProvider : GenericProvider
{
	public override ResourceType ResourceType => ResourceType.KernelObject;
}

internal class RegistryProvider : GenericProvider
{
	public override ResourceType ResourceType => ResourceType.RegistryKey;

	public override void GetAccessListInfo(ObjInfoFlags flags, out AccessRightInfo[] rights, out uint defaultIndex)
	{
		rights = [
			new((uint)RegistryRights.FullControl, ResStr("FileRightFullControl"), AccessFlags.General | AccessFlags.Specific | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new((uint)RegistryRights.ReadKey, ResStr("FileRightRead"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new((uint)RegistryRights.QueryValues, ResStr("RegistryRightQueryValues"), AccessFlags.Specific),
			new((uint)RegistryRights.SetValue, ResStr("RegistryRightSetValue"), AccessFlags.Specific),
			new((uint)RegistryRights.CreateSubKey, ResStr("RegistryRightCreateSubKey"), AccessFlags.Specific),
			new((uint)RegistryRights.EnumerateSubKeys, ResStr("RegistryRightEnumerateSubKeys"), AccessFlags.Specific),
			new((uint)RegistryRights.Notify, ResStr("RegistryRightNotify"), AccessFlags.Specific),
			new((uint)RegistryRights.CreateLink, ResStr("RegistryRightCreateLink"), AccessFlags.Specific),
			new((uint)RegistryRights.Delete, ResStr("StdRightDelete"), AccessFlags.Specific),
			new((uint)RegistryRights.ChangePermissions, ResStr("RegistryRightChangePermissions"), AccessFlags.Specific),
			new((uint)RegistryRights.TakeOwnership, ResStr("RegistryRightTakeOwnership"), AccessFlags.Specific),
			new((uint)RegistryRights.ReadPermissions, ResStr("RegistryRightReadControl"), AccessFlags.Specific),
			new(0, ResStr("File"), 0)
		];
		defaultIndex = 11;
	}

	public override IntPtr GetDefaultSecurity() => base.GetDefaultSecurity();

	public override GenericMapping GetGenericMapping(sbyte AceFlags) => new((uint)RegistryRights.ReadKey, (uint)RegistryRights.WriteKey, (uint)RegistryRights.ExecuteKey, (uint)RegistryRights.FullControl);

	public override InheritedFromInfo[] GetInheritSource(string objName, string serverName, bool isContainer, uint si, IntPtr pAcl)
	{
		var ret = base.GetInheritSource(objName, serverName, isContainer, si, pAcl);
		for (int i = 0; i < ret.Length; i++)
		{
			if (ret[i].GenerationGap == -1)
			{
				int idx = objName.StartsWith(@"\\") ? 1 : 0;
				string[] parts = objName.TrimStart('\\').Split('\\');
				if (parts.Length > idx)
					ret[i].AncestorName = parts[idx].Replace("HKEY_", "");
			}
		}
		return ret;
	}

	public override InheritTypeInfo[] GetInheritTypes() => [
			new(0, ResStr("RegistryInheritance")),
			new(InheritFlags.Container | InheritFlags.Object, ResStr("RegistryInheritanceCI")),
			new(InheritFlags.InheritOnly | InheritFlags.Container, ResStr("RegistryInheritanceIOCI")),
		];
}

internal class TaskProvider : GenericProvider
{
	public override ResourceType ResourceType => Community.Windows.Forms.AccessControlEditorDialog.TaskResourceType;

	public override void GetAccessListInfo(ObjInfoFlags flags, out AccessRightInfo[] rights, out uint defaultIndex)
	{
		rights = [
			new(0x1F01FF, ResStr("FileRightFullControl"), AccessFlags.General | AccessFlags.Specific | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new(0x1200A9, ResStr("FileRightListFolderContents"), AccessFlags.Container | AccessFlags.ContainerInheritAce),
			new(0x120089, ResStr("FileRightRead"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new(0x120116, ResStr("FileRightWrite"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),
			new(0x1200A0, ResStr("TaskRightExecute"), AccessFlags.General | AccessFlags.ContainerInheritAce | AccessFlags.ObjectInheritAce),

			new(0x000001, ResStr("FileRightReadData"), AccessFlags.Specific),
			new(0x000002, ResStr("FileRightWriteData"), AccessFlags.Specific),
			new(0x000004, ResStr("FileRightAppendData"), AccessFlags.Specific),
			new(0x000008, ResStr("FileRightReadExtendedAttributes"), AccessFlags.Specific),
			new(0x000010, ResStr("FileRightWriteExtendedAttributes"), AccessFlags.Specific),
			new(0x000020, ResStr("FileRightExecuteFile"), AccessFlags.Specific),
			new(0x000040, ResStr("FileRightDeleteSubdirectoriesAndFiles"), AccessFlags.Specific),
			new(0x000080, ResStr("FileRightReadAttributes"), AccessFlags.Specific),
			new(0x000100, ResStr("FileRightWriteAttributes"), AccessFlags.Specific),
			new(0x010000, ResStr("StdRightDelete"), AccessFlags.Specific),
			new((uint)FileSystemRights.ReadPermissions, ResStr("FileRightReadPermissions"), AccessFlags.Specific),
			new((uint)FileSystemRights.ChangePermissions, ResStr("FileRightChangePermissions"), AccessFlags.Specific),
			new((uint)FileSystemRights.TakeOwnership, ResStr("StdRightTakeOwnership"), AccessFlags.Specific),
			new(0x100000, ResStr("Synchronize"), AccessFlags.Specific),

			new(0x1F019F, ResStr("FileRightReadWriteAndExecute"), AccessFlags.Ignore),
			new(0, ResStr("File"), AccessFlags.Ignore)
		];
		defaultIndex = 3;
	}

	public override IntPtr GetDefaultSecurity() => base.GetDefaultSecurity();

	public override GenericMapping GetGenericMapping(sbyte AceFlags) => new(0x120089, 0x120116, 0x1200A0, 0x1F01FF);

	public override InheritedFromInfo[] GetInheritSource(string objName, string serverName, bool isContainer, uint si, IntPtr pAcl)
	{
		object obj = SecuredObject.GetKnownObject(Community.Windows.Forms.AccessControlEditorDialog.TaskResourceType, objName, serverName);
		RawAcl acl = NativeMethods.RawAclFromPtr(pAcl);

		// Get list of all parents
		var parents = new System.Collections.Generic.List<object>();
		object folder = obj.GetPropertyValue(isContainer ? "Parent" : "Folder");
		while (folder != null)
		{
			parents.Add(folder);
			folder = folder.GetPropertyValue("Parent");
		}

		// For each ACE, walk up list of lists of parents to determine if there's a matching one.
		for (int i = 0; i < acl.Count; i++)
		{
		}
		return new InheritedFromInfo[NativeMethods.GetAceCount(pAcl)];
	}
}