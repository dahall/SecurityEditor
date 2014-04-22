using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Community.Security.AccessControl
{
	internal delegate void SecurityEvent(SecurityEventArg e);

	/// <summary>
	/// Flags that indicate where the access right is displayed or whether other containers or objects can inherit the access right.
	/// </summary>
	[Flags]
	public enum AccessFlags : uint
	{
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

		/// <summary>If this flag is set, the user cannot change the owner of the object. Set this flag if EditOwner is set but the user does not have permission to change the owner.</summary>
		OwnerReadOnly = 0x40,

		/// <summary>Combine this flag with Container to display a check box on the owner page that indicates whether the user intends the new owner to be applied to all child objects as well as the current object. The access control editor does not perform the recursion.</summary>
		OwnerRecurse = 0x100,

		/// <summary>If this flag is set, the Title property value is used as the title of the basic security property page. Otherwise, a default title is used.</summary>
		PageTitle = 0x800,

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
	/// Page type of property sheet.
	/// </summary>
	public enum PropertySheetPageType : uint
	{
		/// <summary>Permissions page.</summary>
		Permissions = 0,

		/// <summary>Advanced page.</summary>
		Advanced,

		/// <summary>Audit page.</summary>
		Audit,

		/// <summary>Owner page.</summary>
		Owner,

		/// <summary>EffectiveRights page.</summary>
		EffectiveRights,

		/// <summary>TakeOwnership page.</summary>
		TakeOwnership,

		/// <summary>Share page.</summary>
		Share
	}

	/// <summary>
	/// Values that indicate the types of property pages in an access control editor property sheet.
	/// </summary>
	public enum SecurityPageType
	{
		BasicPermissions = 0,
		AdvancedPermissions,
		Audit,
		Owner,
		EffectivePermissions,
		TakeOwnership,
		Share,
	}

	[Flags]
	internal enum GET_SECURITY_REQUEST_INFORMATION
	{
		OWNER_SECURITY_INFORMATION = 1,
		GROUP_SECURITY_INFORMATION = 2,
		DACL_SECURITY_INFORMATION = 4,
		SACL_SECURITY_INFORMATION = 8,
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3853DC76-9F35-407c-88A1-D19344365FBC")]
	internal interface IEffectivePermission
	{
		void GetEffectivePermission([In, MarshalAs(UnmanagedType.LPStruct)] Guid pguidObjectType, [In] IntPtr pUserSid,
			[In, MarshalAs(UnmanagedType.LPWStr)] string pszServerName, [In] IntPtr pSD,
			[MarshalAs(UnmanagedType.LPArray)] out ObjectTypeInfo[] ppObjectTypeList,
			out uint pcObjectTypeListLength,
			[MarshalAs(UnmanagedType.LPArray)] out uint[] ppGrantedAccessList,
			out uint pcGrantedAccessListLength);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("941FABCA-DD47-4FCA-90BB-B0E10255F20D")]
	internal interface IEffectivePermission2
	{
		/*STDMETHOD(ComputeEffectivePermissionWithSecondarySecurity) (THIS_
		_In_ PSID pSid,
		_In_opt_ PSID pDeviceSid,
		_In_ PCWSTR pszServerName,
		_Inout_updates_(dwSecurityObjectCount) PSECURITY_OBJECT pSecurityObjects,
		_In_ DWORD dwSecurityObjectCount,
		_In_opt_ PTOKEN_GROUPS pUserGroups,
		_When_(pUserGroups != NULL && *pAuthzUserGroupsOperations != AUTHZ_SID_OPERATION_REPLACE_ALL, _In_reads_(pUserGroups->GroupCount))
		_In_opt_ PAUTHZ_SID_OPERATION pAuthzUserGroupsOperations,
		_In_opt_ PTOKEN_GROUPS pDeviceGroups,
		_When_(pDeviceGroups != NULL && *pAuthzDeviceGroupsOperations != AUTHZ_SID_OPERATION_REPLACE_ALL, _In_reads_(pDeviceGroups->GroupCount))
		_In_opt_ PAUTHZ_SID_OPERATION pAuthzDeviceGroupsOperations,
		_In_opt_ PAUTHZ_SECURITY_ATTRIBUTES_INFORMATION pAuthzUserClaims,
		_When_(pAuthzUserClaims != NULL && *pAuthzUserClaimsOperations != AUTHZ_SECURITY_ATTRIBUTE_OPERATION_REPLACE_ALL, _In_reads_(pAuthzUserClaims->AttributeCount))
		_In_opt_ PAUTHZ_SECURITY_ATTRIBUTE_OPERATION pAuthzUserClaimsOperations,
		_In_opt_ PAUTHZ_SECURITY_ATTRIBUTES_INFORMATION pAuthzDeviceClaims,
		_When_(pAuthzDeviceClaims != NULL && *pAuthzDeviceClaimsOperations != AUTHZ_SECURITY_ATTRIBUTE_OPERATION_REPLACE_ALL, _In_reads_(pAuthzDeviceClaims->AttributeCount))
		_In_opt_ PAUTHZ_SECURITY_ATTRIBUTE_OPERATION pAuthzDeviceClaimsOperations,
		_Inout_updates_(dwSecurityObjectCount) PEFFPERM_RESULT_LIST pEffpermResultLists);
		*/
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("965FC360-16FF-11d0-91CB-00AA00BBB723")]
	internal interface ISecurityInformation
	{
		void GetObjectInformation(ref SI_OBJECT_INFO object_info);

		void GetSecurity([In] int RequestInformation, out IntPtr SecurityDescriptor, [In] bool fDefault);

		void SetSecurity([In] int RequestInformation, [In] IntPtr SecurityDescriptor);

		void GetAccessRights([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidObject, [In] int dwFlags, [MarshalAs(UnmanagedType.LPArray)] out AccessRightInfo[] access, ref uint access_count, out uint DefaultAccess);

		void MapGeneric([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidObjectType, [In] ref SByte AceFlags, [In] ref uint Mask);

		void GetInheritTypes([MarshalAs(UnmanagedType.LPArray)]out InheritTypeInfo[] InheritType, out uint InheritTypesCount);

		void PropertySheetPageCallback([In] IntPtr hwnd, [In] PropertySheetCallbackMessage uMsg, [In] PropertySheetPageType uPage);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("c3ccfdb4-6f88-11d2-a3ce-00c04fb1782a")]
	internal interface ISecurityInformation2
	{
		[return:MarshalAs(UnmanagedType.Bool)]
		bool IsDaclCanonical([In] IntPtr pDacl);
		void LookupSids([In] uint cSids, [In] IntPtr rgpSids, out IntPtr ppdo);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("E2CDC9CC-31BD-4f8f-8C8B-B641AF516A1A")]
	internal interface ISecurityInformation3
	{
		//STDMETHOD(GetFullResourceName) (THIS_ _Outptr_ LPWSTR *ppszResourceName) PURE;
		//STDMETHOD(OpenElevatedEditor) (THIS_ _In_ HWND hWnd, _In_ SI_PAGE_TYPE uPage) PURE;
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("EA961070-CD14-4621-ACE4-F63C03E583E4")]
	internal interface ISecurityInformation4
	{
		//STDMETHOD(GetSecondarySecurity) (THIS_ _Outptr_result_buffer_(*pSecurityObjectCount) PSECURITY_OBJECT *pSecurityObjects, _Out_ PULONG pSecurityObjectCount) PURE;
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("FC3066EB-79EF-444b-9111-D18A75EBF2FA")]
	internal interface ISecurityObjectTypeInfo
	{
		void GetInheritSource([In] int si, [In] IntPtr pACL, [MarshalAs(UnmanagedType.LPArray)] out InheritedFromInfo[] ppInheritArray);
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
			this.guidObjectType = Helper.EmptyGuidPtr;
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
					this.guidObjectType = Helper.EmptyGuidPtr;
			}
		}
	}

	/// <summary>
	/// Defines the mapping of generic access rights to specific and standard access rights for an object. When a client application requests generic access to an object, that request is mapped to the access rights defined in this structure.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct GenericRightsMapping
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
		/// Initializes a new instance of the <see cref="GenericRightsMapping"/> struct.
		/// </summary>
		/// <param name="read">The read mapping.</param>
		/// <param name="write">The write mapping.</param>
		/// <param name="execute">The execute mapping.</param>
		/// <param name="all">The 'all' mapping.</param>
		public GenericRightsMapping(uint read, uint write, uint execute, uint all)
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
			this.guidObjectType = Helper.EmptyGuidPtr;
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
					this.guidObjectType = Helper.EmptyGuidPtr;
			}
		}
	}

	/// <summary>
	/// Identifies an object type element in a hierarchy of object types. An array of ObjectTypeInfo structures to define a hierarchy of an object and its subobjects, such as property sets and properties.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct ObjectTypeInfo
	{
		private UInt16 level;
		private UInt16 Sbz;
		private IntPtr guidObjectType;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectTypeInfo"/> struct.
		/// </summary>
		/// <param name="level">The level of the object type in the hierarchy of an object and its subobjects.</param>
		/// <param name="objType">The object or subobject identifier.</param>
		public ObjectTypeInfo(UInt16 level, Guid objType)
		{
			this.level = 0;
			this.Sbz = 0;
			this.guidObjectType = Helper.EmptyGuidPtr;
			this.Level = level;
			this.ObjectId = objType;
		}

		/// <summary>
		/// Represents an object that is itself.
		/// </summary>
		public static readonly ObjectTypeInfo Self = new ObjectTypeInfo(0, Guid.Empty);

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
					this.guidObjectType = Helper.EmptyGuidPtr;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SI_OBJECT_INFO
	{
		public ObjInfoFlags Flags;
		public IntPtr hInstance;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string ServerName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string ObjectName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string PageTitle;

		public Guid ObjectTypeGuid;

		public SI_OBJECT_INFO(ObjInfoFlags flags, string objectName)
			: this(flags, objectName, null, null, Guid.Empty)
		{
		}

		public SI_OBJECT_INFO(ObjInfoFlags flags, string objectName, string serverName, string pageTitle)
			: this(flags, objectName, serverName, pageTitle, Guid.Empty)
		{
		}

		public SI_OBJECT_INFO(ObjInfoFlags flags, string objectName, string serverName, string pageTitle, Guid objectGuid)
		{
			this.Flags = flags;
			this.hInstance = IntPtr.Zero;
			this.ObjectName = objectName;
			this.ServerName = serverName;
			this.PageTitle = pageTitle;
			this.ObjectTypeGuid = objectGuid;
			if (this.PageTitle != null)
				this.Flags |= ObjInfoFlags.PageTitle;
			if (this.ObjectTypeGuid != Guid.Empty)
				this.Flags |= ObjInfoFlags.ObjectGuid;
		}

		public bool IsContainer { get { return ((this.Flags & ObjInfoFlags.Container) == ObjInfoFlags.Container); } }

		public override string ToString()
		{
			return string.Format("{0}: {1}{2}", this.ObjectName, this.Flags, IsContainer ? " (Cont)" : "");
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SID_INFO
	{
		public IntPtr pSid;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pwzCommonName;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pwzClass;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pwzUPN;
	}

	internal class SecurityEventArg : EventArgs
	{
		public int Parts;
		public IntPtr SecurityDesciptor;

		public SecurityEventArg(IntPtr sd, int parts)
		{
			Parts = parts;
			SecurityDesciptor = sd;
		}
	}

	internal class SecurityInfoImpl : ISecurityInformation //, ISecurityObjectTypeInfo, IEffectivePermission
	{
		internal SI_OBJECT_INFO ObjectInfo;
		private IAccessControlEditorDialogProvider prov;
		private MarshaledMem pSD;

		public SecurityInfoImpl(ObjInfoFlags flags, string objectName, string serverName = null, string pageTitle = null)
		{
			ObjectInfo = new SI_OBJECT_INFO(flags, objectName, serverName, pageTitle);
		}

		public event SecurityEvent OnSetSecurity;

		public byte[] SecurityDescriptor
		{
			get { return pSD.Array; }
			set { pSD = new MarshaledMem(value); }
		}

		/*void IEffectivePermission.GetEffectivePermission(Guid pguidObjectType, IntPtr pUserSid, string pszServerName, IntPtr pSD, out ObjectTypeInfo[] ppObjectTypeList, out uint pcObjectTypeListLength, out uint[] ppGrantedAccessList, out uint pcGrantedAccessListLength)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("GetEffectivePermission: {0}, {1}", pguidObjectType, pszServerName));
			if (pguidObjectType == Guid.Empty)
			{
				ppGrantedAccessList = this.prov.GetEffectivePermission(pUserSid, pszServerName, pSD);
				pcGrantedAccessListLength = (uint)ppGrantedAccessList.Length;
				ppObjectTypeList = new ObjectTypeInfo[] { ObjectTypeInfo.Self };
				pcObjectTypeListLength = (uint)ppObjectTypeList.Length;
			}
			else
			{
				ppGrantedAccessList = this.prov.GetEffectivePermission(pguidObjectType, pUserSid, pszServerName, pSD, out ppObjectTypeList);
				pcGrantedAccessListLength = (uint)ppGrantedAccessList.Length;
				pcObjectTypeListLength = (uint)ppObjectTypeList.Length;
			}
		}*/

		void ISecurityInformation.GetAccessRights(Guid guidObject, int dwFlags, out AccessRightInfo[] access, ref uint access_count, out uint DefaultAccess)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("GetAccessRight: {0}, {1}", guidObject, dwFlags));
			uint defAcc;
			AccessRightInfo[] ari;
			this.prov.GetAccessListInfo(out ari, out defAcc);
			DefaultAccess = defAcc;
			access = ari;
			access_count = (uint)access.Length;
		}

		void ISecurityInformation.GetInheritTypes(out InheritTypeInfo[] InheritTypes, out uint InheritTypesCount)
		{
			System.Diagnostics.Debug.WriteLine("GetInheritTypes");
			InheritTypes = this.prov.GetInheritTypes();
			InheritTypesCount = (uint)InheritTypes.Length;
		}

		void ISecurityInformation.GetObjectInformation(ref SI_OBJECT_INFO object_info)
		{
			System.Diagnostics.Debug.WriteLine("GetObjectInformation");
			object_info = this.ObjectInfo;
		}

		void ISecurityInformation.GetSecurity(int RequestInformation, out IntPtr ppSecurityDescriptor, bool fDefault)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("GetSecurity: {0}{1}", RequestInformation, fDefault ? " (Def)" : ""));
			ppSecurityDescriptor = Helper.GetPrivateObjectSecurity(fDefault ? this.prov.GetDefaultSecurity() : pSD.Ptr, RequestInformation);
			//System.Diagnostics.Debug.WriteLine(string.Format("GetSecurity={0}<-{1}", Helper.SecurityDescriptorPtrToSDLL(ppSecurityDescriptor, RequestInformation), Helper.SecurityDescriptorPtrToSDLL(pSD.Ptr, RequestInformation)));
		}

		void ISecurityInformation.MapGeneric(Guid guidObjectType, ref sbyte AceFlags, ref uint Mask)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("MapGeneric: {0}, {1}, {2}", guidObjectType, AceFlags, Mask));
			GenericRightsMapping gm = this.prov.GetGenericMapping();
			Helper.MapGenericMask(ref Mask, ref gm);
		}

		void ISecurityInformation.PropertySheetPageCallback(IntPtr hwnd, PropertySheetCallbackMessage uMsg, PropertySheetPageType uPage)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("PropertySheetPageCallback: {0}, {1}, {2}", hwnd, uMsg, uPage));
			this.prov.PropertySheetPageCallback(hwnd, uMsg, uPage);
		}

		void ISecurityInformation.SetSecurity(int RequestInformation, IntPtr sd)
		{
			if (OnSetSecurity != null)
				OnSetSecurity(new SecurityEventArg(sd, RequestInformation));
		}

		/*void ISecurityObjectTypeInfo.GetInheritSource(int si, IntPtr pACL, out InheritedFromInfo[] ppInheritArray)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("GetInheritSource: {0}", si));
			ppInheritArray = this.prov.GetInheritSource(this.ObjectInfo.ObjectName, this.ObjectInfo.IsContainer, (uint)si, pACL);
		}*/

		public void SetProvider(IAccessControlEditorDialogProvider provider)
		{
			prov = provider;
		}

		public RawSecurityDescriptor ShowDialog(IntPtr hWnd, SecurityPageType pageType = SecurityPageType.BasicPermissions)
		{
			SecurityEventArg sd = null;
			SecurityEvent fn = delegate(SecurityEventArg e) { sd = e; };
			try
			{
				this.OnSetSecurity += fn;
				if (System.Environment.OSVersion.Version.Major == 5 || pageType == SecurityPageType.BasicPermissions)
				{
					if (!Helper.EditSecurity(hWnd, this))
						Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				else
				{
					int res = Helper.EditSecurityAdvanced(hWnd, this, pageType);
					if (res != 0)
						Marshal.ThrowExceptionForHR(res);
				}
				if (sd != null)
				{
					string sddl = Helper.SecurityDescriptorPtrToSDLL(sd.SecurityDesciptor, sd.Parts);
					if (!string.IsNullOrEmpty(sddl))
						return new RawSecurityDescriptor(sddl);
				}
			}
			finally
			{
				this.OnSetSecurity -= fn;
			}
			return null;
		}
	}
}