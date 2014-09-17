using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Community.Security.AccessControl
{
	/// <summary>
	/// Flags that indicate where the access right is displayed or whether other containers or objects can inherit the access right.
	/// </summary>
	[Flags]
	public enum AccessFlags : uint
	{
		/// <summary>Indicates to ignore this access right.</summary>
		Ignore = 0,

		/// <summary>Indicates an access right that applies only to containers. If this flag is set, the access right is displayed on the basic security page only if the <see cref="Community.Windows.Forms.AccessControlEditorDialog.ObjectIsContainer"/> is <c>true</c>.</summary>
		Container = 0x40000,

		/// <summary>Other containers that are contained by the primary object inherit the entry.</summary>
		ContainerInheritAce = 2,

		/// <summary>The access right is displayed on the basic security page.</summary>
		General = 0x20000,

		/// <summary>The ACE is inherited. Operations that change the security on a tree of objects may modify inherited ACEs without changing ACEs that were directly applied to the object.</summary>
		InheritedAce = 0x10,

		/// <summary>The ACE does not apply to the primary object to which the ACL is attached, but objects contained by the primary object inherit the entry.</summary>
		InheritOnlyAce = 8,

		/// <summary>The ObjectInheritAce and ContainerInheritAce bits are not propagated to an inherited ACE.</summary>
		NoPropagateInheritAce = 4,

		/// <summary>Noncontainer objects contained by the primary object inherit the entry.</summary>
		ObjectInheritAce = 1,

		/// <summary>Indicates a property-specific access right. Used with <see cref="Community.Windows.Forms.AccessControlEditorDialog.EditProperties"/>.</summary>
		Property = 0x80000,

		/// <summary>The access right is displayed on the advanced security pages.</summary>
		Specific = 0x10000
	}

	/// <summary>
	/// Indicate the types of ACEs that can be inherited in a <see cref="InheritTypeInfo"/> structure.
	/// </summary>
	[Flags]
	public enum InheritFlags : uint
	{
		/// <summary>The specified object type can inherit ACEs that have the <c>ContainerInheritAce</c> flag set.</summary>
		Container = 2,

		/// <summary>The specified object type can inherit ACEs that have the <c>InheritOnlyAce</c> flag set.</summary>
		InheritOnly = 8,

		/// <summary>The specified object type can inherit ACEs that have the <c>ObjectInheritAce</c> flag set.</summary>
		Object = 1
	}

	/// <summary>
	/// A set of bit flags that determine the editing options available to the user.
	/// </summary>
	[Flags]
	public enum ObjInfoFlags : uint
	{
		/// <summary>The Advanced button is displayed on the basic security property page. If the user clicks this button, the system displays an advanced security property sheet that enables advanced editing of the discretionary access control list (DACL) of the object.</summary>
		Advanced = 0x10,

		/// <summary>If this flag is set, a shield is displayed on the Edit button of the advanced Auditing pages. For NTFS objects, this flag is requested when the user does not have READ_CONTROL or ACCESS_SYSTEM_SECURITY access. Windows Server 2003 and Windows XP:  This flag is not supported.</summary>
		AuditElevationRequired = 0x2000000,

		/// <summary>Indicates that the object is a container. If this flag is set, the access control editor enables the controls relevant to the inheritance of permissions onto child objects.</summary>
		Container = 4,

		/// <summary>Combines the EditPerms, EditOwner, and EditAudit flags.</summary>
		EditAll = 3,

		/// <summary>If this flag is set and the user clicks the Advanced button, the system displays an advanced security property sheet that includes an Auditing property page for editing the object's SACL.</summary>
		EditAudit = 2,

		/// <summary>If this flag is set, the Effective Permissions page is displayed.</summary>
		EditEffective = 0x20000,

		/// <summary>If this flag is set and the user clicks the Advanced button, the system displays an advanced security property sheet that includes an Owner property page for changing the object's owner.</summary>
		EditOwner = 1,

		/// <summary>This is the default value. The basic security property page always displays the controls for basic editing of the object's DACL. To disable these controls, set the ReadOnly flag.</summary>
		EditPerms = 0,

		/// <summary>If this flag is set, the system enables controls for editing ACEs that apply to the object's property sets and properties. These controls are available only on the property sheet displayed when the user clicks the Advanced button.</summary>
		EditProperties = 0x80,

		/// <summary>Indicates that the access control editor cannot read the DACL but might be able to write to the DACL. If a call to the ISecurityInformation::GetSecurity method returns AccessDenied, the user can try to add a new ACE, and a more appropriate warning is displayed.</summary>
		MayWrite = 0x10000000,

		/// <summary>
		/// If this flag is set, the access control editor hides the check box that allows inheritable ACEs to propagate from the parent object to this object. If this flag is not set, the check box is visible.
		/// The check box is clear if the SE_DACL_PROTECTED flag is set in the object's security descriptor. In this case, the object's DACL is protected from being modified by inheritable ACEs.
		/// If the user clears the check box, any inherited ACEs in the security descriptor are deleted or converted to noninherited ACEs. Before proceeding with this conversion, the system displays a warning message box to confirm the change.
		/// </summary>
		NoAclProtect = 0x200,

		/// <summary>If this flag is set, the access control editor hides the Special Permissions tab on the Advanced Security Settings page.</summary>
		NoAdditionalPermission = 0x200000,

		/// <summary>If this flag is set, the access control editor hides the check box that controls the NO_PROPAGATE_INHERIT_ACE flag. This flag is relevant only when the Advanced flag is also set.</summary>
		NoTreeApply = 0x400,

		/// <summary>When set, indicates that the ObjectGuid property is valid. This is set in comparisons with object-specific ACEs in determining whether the ACE applies to the current object.</summary>
		ObjectGuid = 0x10000,

		/// <summary>If this flag is set, a shield is displayed on the Edit button of the advanced Owner page. For NTFS objects, this flag is requested when the user does not have WRITE_OWNER access. This flag is valid only if the owner page is requested. Windows Server 2003 and Windows XP:  This flag is not supported.</summary>
		OwnerElevationRequired = 0x4000000,

		/// <summary>If this flag is set, the user cannot change the owner of the object. Set this flag if EditOwner is set but the user does not have permission to change the owner.</summary>
		OwnerReadOnly = 0x40,

		/// <summary>Combine this flag with Container to display a check box on the owner page that indicates whether the user intends the new owner to be applied to all child objects as well as the current object. The access control editor does not perform the recursion.</summary>
		OwnerRecurse = 0x100,

		/// <summary>If this flag is set, the Title property value is used as the title of the basic security property page. Otherwise, a default title is used.</summary>
		PageTitle = 0x800,

		/// <summary>If this flag is set, an image of a shield is displayed on the Edit button of the simple and advanced Permissions pages. For NTFS objects, this flag is requested when the user does not have READ_CONTROL or WRITE_DAC access. Windows Server 2003 and Windows XP: This flag is not supported.</summary>
		PermsElevationRequired = 0x1000000,

		/// <summary>If this flag is set, the editor displays the object's security information, but the controls for editing the information are disabled. This flag cannot be combined with the ViewOnly flag.</summary>
		ReadOnly = 8,

		/// <summary>If this flag is set, the Default button is displayed. If the user clicks this button, the access control editor calls the IAccessControlEditorDialogProvider.DefaultSecurity to retrieve an application-defined default security descriptor. The access control editor uses this security descriptor to reinitialize the property sheet, and the user is allowed to apply the change or cancel.</summary>
		Reset = 0x20,

		/// <summary>When set, this flag displays the Reset Defaults button on the Permissions page.</summary>
		ResetDacl = 0x40000,

		/// <summary>When set, this flag displays the Reset permissions on all child objects and enable propagation of inheritable permissions check box in the Permissions page of the Access Control Settings window. This function does not reset the permissions and enable propagation of inheritable permissions.</summary>
		ResetDaclTree = 0x4000,

		/// <summary>When set, this flag displays the Reset Defaults button on the Owner page.</summary>
		ResetOwner = 0x100000,

		/// <summary>When set, this flag displays the Reset Defaults button on the Auditing page.</summary>
		ResetSacl = 0x80000,

		/// <summary>When set, this flag displays the Reset auditing entries on all child objects and enables propagation of the inheritable auditing entries check box in the Auditing page of the Access Control Settings window. This function does not reset the permissions and enable propagation of inheritable permissions.</summary>
		ResetSaclTree = 0x8000,

		/// <summary>Set this flag if the computer defined by the ServerName property is known to be a domain controller. If this flag is set, the domain name is included in the scope list of the Add Users and Groups dialog box. Otherwise, the pszServerName computer is used to determine the scope list of the dialog box.</summary>
		ServerIsDC = 0x1000,

		// ISecurityInformation3
		/// <summary>View Only.</summary>
		ViewOnly = 0x00400000,

		// ISecurityInformation4
		//DisableDenyAce = 0x80000000,
		//EnableCentralPolicy = 0x40000000,
		//EnableEditAttributeCondition = 0x20000000,
		//ScopeElevationRequired = 0x08000000,
	}

	/// <summary>
	/// Messages sent from property sheet.
	/// </summary>
	public enum PropertySheetCallbackMessage : uint
	{
		/// <summary>Add reference.</summary>
		AddRef = 0,

		/// <summary>Release reference.</summary>
		Release = 1,

		/// <summary>Create page.</summary>
		Create = 2,

		/// <summary>Initialize dialog.</summary>
		InitDialog = 0x00401 // wm_user + 1
	}

	/// <summary>
	/// Identifies the object-related security information being set or queried.
	/// </summary>
	[Flags]
	public enum SecurityInfosEx : uint
	{
		/// <summary>The owner identifier of the object is being referenced.</summary>
		Owner = 1,
		/// <summary>The primary group identifier of the object is being referenced.</summary>
		Group = 2,
		/// <summary>The DACL of the object is being referenced.</summary>
		DiscretionaryAcl = 4,
		/// <summary>The SACL of the object is being referenced.</summary>
		SystemAcl = 8,
		/// <summary>The mandatory integrity label is being referenced.</summary>
		MandatoryIntegrityLabel = 0x10,
		/// <summary>A SYSTEM_RESOURCE_ATTRIBUTE_ACE is being referenced.</summary>
		ResourceAttribute = 0x20,
		/// <summary>A SYSTEM_SCOPED_POLICY_ID_ACE is being referenced.</summary>
		ScopedPolicyID = 0x40,
		/// <summary>The security descriptor is being accessed for use in a backup operation.</summary>
		Backup = 0x10000,
		/// <summary>The SACL inherits access control entries (ACEs) from the parent object.</summary>
		UnprotectedSystemAcl = 0x10000000,
		/// <summary>The DACL inherits ACEs from the parent object.</summary>
		UnprotectedDiscretionaryAcl = 0x20000000,
		/// <summary>The SACL cannot inherit ACEs.</summary>
		ProtectedSystemAcl = 0x40000000,
		/// <summary>The DACL cannot inherit ACEs.</summary>
		ProtectedDiscretionaryAcl = 0x80000000,
	}

	/// <summary>
	/// Page types used by the new advanced ACL UI
	/// </summary>
	public enum SecurityPageActivation : uint
	{
		/// <summary></summary>
		ShowDefault = 0,
		/// <summary></summary>
		ShowPermActivated,
		/// <summary></summary>
		ShowAuditActivated,
		/// <summary></summary>
		ShowOwnerActivated,
		/// <summary></summary>
		ShowEffectiveActivated,
		/// <summary></summary>
		ShowShareActivated,
		/// <summary></summary>
		ShowCentralPolicyActivated,
	}

	/// <summary>
	/// Values that indicate the types of property pages in an access control editor property sheet.
	/// </summary>
	public enum SecurityPageType : uint
	{
		/// <summary>Permissions page.</summary>
		BasicPermissions = 0,
		/// <summary>Advanced page.</summary>
		AdvancedPermissions,
		/// <summary>Audit page.</summary>
		Audit,
		/// <summary>Owner page.</summary>
		Owner,
		/// <summary>Effective Rights page.</summary>
		EffectiveRights,
		/// <summary>Take Ownership page.</summary>
		TakeOwnership,
		/// <summary>Share page.</summary>
		Share
	}

	/// <summary>
	/// Structure contains information about an access right or default access mask for a securable object.
	/// The <see cref="IAccessControlEditorDialogProvider.GetAccessListInfo"/> method uses this structure to specify information that the access control editor uses to initialize its property pages.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct AccessRightInfo
	{
		private IntPtr guidObjectType;

		/// <summary>A bitmask that specifies the access right described by this structure. The mask can contain any combination of standard and specific rights, but should not contain generic rights such as GENERIC_ALL.</summary>
		public uint Mask;

		/// <summary>A display string that describes the access right.</summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Name;

		/// <summary>A set of <see cref="AccessFlags"/> that indicate where the access right is displayed.</summary>
		public AccessFlags Flags;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessRightInfo"/> struct.
		/// </summary>
		/// <param name="mask">The access mask.</param>
		/// <param name="name">The display name.</param>
		/// <param name="flags">The access flags.</param>
		public AccessRightInfo(uint mask, string name, AccessFlags flags)
		{
			this.guidObjectType = NativeMethods.EmptyGuidPtr;
			this.Mask = mask;
			this.Name = name;
			this.Flags = flags;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessRightInfo"/> struct.
		/// </summary>
		/// <param name="mask">The access mask.</param>
		/// <param name="name">The display name.</param>
		/// <param name="flags">The access flags.</param>
		/// <param name="objType">Type of the object.</param>
		public AccessRightInfo(uint mask, string name, AccessFlags flags, Guid objType)
			: this(mask, name, flags)
		{
			this.ObjectTypeId = objType;
		}

		/// <summary>
		/// The type of object. This member can be <see cref="Guid.Empty"/>. The GUID corresponds to the InheritedObjectType member of an object-specific ACE.
		/// </summary>
		/// <value>
		/// The ojbect type identifier.
		/// </value>
		public Guid ObjectTypeId
		{
			get { return guidObjectType == IntPtr.Zero ? Guid.Empty : (Guid)Marshal.PtrToStructure(guidObjectType, typeof(Guid)); }
			set
			{
				if (value != Guid.Empty)
					Marshal.StructureToPtr(value, this.guidObjectType, true);
				else
					this.guidObjectType = NativeMethods.EmptyGuidPtr;
			}
		}
	}

	/// <summary>
	/// Defines the mapping of generic access rights to specific and standard access rights for an object. When a client application requests generic access to an object, that request is mapped to the access rights defined in this structure.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct GenericMapping
	{
		/// <summary>Specifies an access mask defining read access to an object.</summary>
		public uint GenericRead;

		/// <summary>Specifies an access mask defining write access to an object.</summary>
		public uint GenericWrite;

		/// <summary>Specifies an access mask defining execute access to an object.</summary>
		public uint GenericExecute;

		/// <summary>Specifies an access mask defining all possible types of access to an object.</summary>
		public uint GenericAll;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericMapping"/> struct.
		/// </summary>
		/// <param name="read">The read mapping.</param>
		/// <param name="write">The write mapping.</param>
		/// <param name="execute">The execute mapping.</param>
		/// <param name="all">The 'all' mapping.</param>
		public GenericMapping(uint read, uint write, uint execute, uint all)
		{
			GenericRead = read;
			GenericWrite = write;
			GenericExecute = execute;
			GenericAll = all;
		}
	}

	/// <summary>
	/// Provides information about an object's inherited access control entry (ACE).
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct InheritedFromInfo
	{
		/// <summary>Number of levels, or generations, between the object and the ancestor. Set this to zero for an explicit ACE. If the ancestor cannot be determined for the inherited ACE, set this member to –1.</summary>
		public Int32 GenerationGap;

		/// <summary>Name of the ancestor from which the ACE was inherited. For an explicit ACE, set this to <c>null</c>.</summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string AncestorName;

		/// <summary>
		/// Initializes a new instance of the <see cref="InheritedFromInfo"/> struct.
		/// </summary>
		/// <param name="generationGap">The generation gap.</param>
		/// <param name="ancestorName">Name of the ancestor.</param>
		public InheritedFromInfo(int generationGap, string ancestorName)
		{
			this.GenerationGap = generationGap;
			this.AncestorName = ancestorName;
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if (AncestorName == null)
			{
				if (GenerationGap == 0)
					return "Explicit";
				if (GenerationGap == -1)
					return "Indeterminate";
			}
			return string.Format("{0} : 0x{1:X}", AncestorName, GenerationGap);
		}

		/// <summary>ACE is explicit.</summary>
		public static readonly InheritedFromInfo Explicit = new InheritedFromInfo(0, null);

		/// <summary>ACE inheritance cannot be determined.</summary>
		public static readonly InheritedFromInfo Indeterminate = new InheritedFromInfo(-1, null);
	}

	/// <summary>
	/// Contains information about how access control entries (ACEs) can be inherited by child objects. The <see cref="IAccessControlEditorDialogProvider.GetInheritTypes"/> method uses this structure to specify display strings that the access control editor uses to initialize its property pages.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct InheritTypeInfo
	{
		private IntPtr guidObjectType;

		/// <summary>A set of <see cref="InheritFlags"/> that indicate the types of ACEs that can be inherited by the <see cref="ChildObjectTypeId"/>. These flags correspond to the AceFlags member of an ACE_HEADER structure.</summary>
		public InheritFlags Flags;

		/// <summary>A display string that describes the child object.</summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Name;

		/// <summary>
		/// Initializes a new instance of the <see cref="InheritTypeInfo"/> struct.
		/// </summary>
		/// <param name="flags">The inheritance flags.</param>
		/// <param name="name">The display name.</param>
		public InheritTypeInfo(InheritFlags flags, string name)
		{
			this.guidObjectType = NativeMethods.EmptyGuidPtr;
			this.Flags = flags;
			this.Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InheritTypeInfo"/> struct.
		/// </summary>
		/// <param name="childObjectType">Type of the child object.</param>
		/// <param name="flags">The inheritance flags.</param>
		/// <param name="name">The display name.</param>
		public InheritTypeInfo(Guid childObjectType, InheritFlags flags, string name)
			: this(flags, name)
		{
			this.ChildObjectTypeId = childObjectType;
		}

		/// <summary>
		/// The type of child object. This member can be <see cref="Guid.Empty"/>. The GUID corresponds to the InheritedObjectType member of an object-specific ACE.
		/// </summary>
		/// <value>
		/// The child ojbect type identifier.
		/// </value>
		public Guid ChildObjectTypeId
		{
			get { return guidObjectType == IntPtr.Zero ? Guid.Empty : (Guid)Marshal.PtrToStructure(guidObjectType, typeof(Guid)); }
			set
			{
				if (value != Guid.Empty)
					Marshal.StructureToPtr(value, this.guidObjectType, true);
				else
					this.guidObjectType = NativeMethods.EmptyGuidPtr;
			}
		}
	}

	/// <summary>
	/// Identifies an object type element in a hierarchy of object types. An array of ObjectTypeList structures to define a hierarchy of an object and its subobjects, such as property sets and properties.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct ObjectTypeList
	{
		private UInt16 level;
		private UInt16 Sbz;
		private IntPtr guidObjectType;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectTypeList"/> struct.
		/// </summary>
		/// <param name="level">The level of the object type in the hierarchy of an object and its subobjects.</param>
		/// <param name="objType">The object or subobject identifier.</param>
		public ObjectTypeList(UInt16 level, Guid objType)
		{
			this.level = 0;
			this.Sbz = 0;
			this.guidObjectType = NativeMethods.EmptyGuidPtr;
			this.Level = level;
			this.ObjectId = objType;
		}

		/// <summary>
		/// Represents an object that is itself.
		/// </summary>
		public static readonly ObjectTypeList Self = new ObjectTypeList(0, Guid.Empty);

		/// <summary>
		/// Specifies the level of the object type in the hierarchy of an object and its subobjects. Level zero indicates the object itself. Level one indicates a subobject of the object, such as a property set. Level two indicates a subobject of the level one subobject, such as a property. There can be a maximum of five levels numbered zero through four.
		/// </summary>
		/// <value>
		/// The level. There can be a maximum of five levels numbered zero through four.
		/// </value>
		public UInt16 Level
		{
			get { return level; }
			set
			{
				if (value < 0 || value > 4)
					throw new ArgumentOutOfRangeException("Level", "There can be a maximum of five levels numbered zero through four.");
				level = value;
			}
		}

		/// <summary>
		/// The object or subobject identifier.
		/// </summary>
		/// <value>
		/// The ojbect type identifier.
		/// </value>
		public Guid ObjectId
		{
			get { return guidObjectType == IntPtr.Zero ? Guid.Empty : (Guid)Marshal.PtrToStructure(guidObjectType, typeof(Guid)); }
			set
			{
				if (value != Guid.Empty)
					Marshal.StructureToPtr(value, this.guidObjectType, true);
				else
					this.guidObjectType = NativeMethods.EmptyGuidPtr;
			}
		}
	}
}
