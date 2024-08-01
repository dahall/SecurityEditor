using Community.Security.AccessControl;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Community.Windows.Forms;

/// <summary>
/// Displays a property sheet that contains a basic security property page. This property page enables the user to view and edit the
/// access rights allowed or denied by the ACEs in an object's DACL.
/// </summary>
[ToolboxItem(true), ToolboxItemFilter("System.Windows.Forms.Control.TopLevel")]
[Designer("System.ComponentModel.Design.ComponentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[DesignTimeVisible(true)]
[DefaultProperty("ObjectName"), Description("Displays a property sheet that contains a basic security property page.")]
[System.Drawing.ToolboxBitmap(typeof(AccessControlEditorDialog))]
public class AccessControlEditorDialog : CommonDialog
{
	/// <summary>Pseudo type cast for a Task specific ResourceType.</summary>
	public const System.Security.AccessControl.ResourceType TaskResourceType = (System.Security.AccessControl.ResourceType)99;

	private static readonly ObjInfoFlags defaultFlags = ObjInfoFlags.EditAll | ObjInfoFlags.Advanced | ObjInfoFlags.ViewOnly;

	private ObjInfoFlags flags = defaultFlags;
	private SecurityInfoImpl iSecInfo;
	private string objectName, serverName, title;
	private System.Security.AccessControl.ResourceType resType = System.Security.AccessControl.ResourceType.Unknown;

	/// <summary>Initializes a new instance of the <see cref="AccessControlEditorDialog"/> class.</summary>
	public AccessControlEditorDialog()
	{
		if (System.Threading.Thread.CurrentThread.GetApartmentState() != System.Threading.ApartmentState.STA)
			throw new InvalidOperationException("Current thread must be STA in order to use the AccessControlEditorDialog.");
		PageType = SecurityPageType.BasicPermissions;
	}

	/// <summary>
	/// When set, this flag displays the Reset permissions on all child objects and enable propagation of inheritable permissions check
	/// box in the Permissions page of the Access Control Settings window. This function does not reset the permissions and enable
	/// propagation of inheritable permissions.
	/// </summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool AllowDaclInheritanceReset
	{
		get => HasFlag(ObjInfoFlags.ResetDaclTree);
		set => SetFlag(ObjInfoFlags.ResetDaclTree, value, true);
	}

	/// <summary>Combines the AllowEditPerms, AllowEditOwner, and AllowEditAudit properties.</summary>
	[DefaultValue(true), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool AllowEditAll
	{
		get => HasFlag(ObjInfoFlags.EditAll);
		set => SetFlag(ObjInfoFlags.EditAll, value, true);
	}

	/// <summary>
	/// If this flag is set and the user clicks the Advanced button, the system displays an advanced security property sheet that
	/// includes an Auditing property page for editing the object's SACL.
	/// </summary>
	[DefaultValue(true), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool AllowEditAudit
	{
		get => HasFlag(ObjInfoFlags.EditAudit);
		set => SetFlag(ObjInfoFlags.EditAudit, value, true);
	}

	/// <summary>
	/// If this flag is set and the user clicks the Advanced button, the system displays an advanced security property sheet that
	/// includes an Owner property page for changing the object's owner.
	/// </summary>
	[DefaultValue(true), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool AllowEditOwner
	{
		get => HasFlag(ObjInfoFlags.EditOwner);
		set => SetFlag(ObjInfoFlags.EditOwner, value, true);
	}

	/// <summary>
	/// When set, this flag displays the Reset auditing entries on all child objects and enables propagation of the inheritable auditing
	/// entries check box in the Auditing page of the Access Control Settings window. This function does not reset the permissions and
	/// enable propagation of inheritable permissions.
	/// </summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool AllowSaclInheritanceReset
	{
		get => HasFlag(ObjInfoFlags.ResetSaclTree);
		set => SetFlag(ObjInfoFlags.ResetSaclTree, value, true);
	}

	/// <summary>When set, this flag displays a shield on the Edit button of the advanced Auditing pages.</summary>
	[DefaultValue(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool AuditElevationRequired
	{
		get => HasFlag(ObjInfoFlags.AuditElevationRequired);
		set => SetFlag(ObjInfoFlags.AuditElevationRequired, value, true);
	}

	/// <summary>
	/// If this flag is set, the Title property value is used as the title of the basic security property page. Otherwise, a default
	/// title is used.
	/// </summary>
	[DefaultValue(false), Browsable(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CustomPageTitle
	{
		get => HasFlag(ObjInfoFlags.PageTitle);
		private set => SetFlag(ObjInfoFlags.PageTitle, value);
	}

	/// <summary>
	/// If this flag is set, the access control editor hides the check box that controls the NO_PROPAGATE_INHERIT_ACE flag. This flag is
	/// relevant only when the Advanced flag is also set.
	/// </summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool DisallowInheritance
	{
		get => HasFlag(ObjInfoFlags.NoTreeApply);
		set => SetFlag(ObjInfoFlags.NoTreeApply, value, true);
	}

	/// <summary>
	/// If this flag is set, the access control editor hides the check box that allows inheritable ACEs to propagate from the parent
	/// object to this object. If this flag is not set, the check box is visible. The check box is clear if the SE_DACL_PROTECTED flag is
	/// set in the object's security descriptor. In this case, the object's DACL is protected from being modified by inheritable ACEs. If
	/// the user clears the check box, any inherited ACEs in the security descriptor are deleted or converted to non-inherited ACEs.
	/// Before proceeding with this conversion, the system displays a warning message box to confirm the change.
	/// </summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool DisallowProtectedAcls
	{
		get => HasFlag(ObjInfoFlags.NoAclProtect);
		set => SetFlag(ObjInfoFlags.NoAclProtect, value);
	}

	/// <summary>Gets or sets the display name.</summary>
	/// <value>The display name.</value>
	public string DisplayName
	{
		get => objectName;
		set
		{
			objectName = value;
			if (iSecInfo != null)
				iSecInfo.ObjectInfo.ObjectName = value;
		}
	}

	/// <summary>
	/// If this flag is set, the system enables controls for editing ACEs that apply to the object's property sets and properties. These
	/// controls are available only on the property sheet displayed when the user clicks the Advanced button.
	/// </summary>
	[DefaultValue(true), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool EditProperties
	{
		get => HasFlag(ObjInfoFlags.EditProperties);
		set => SetFlag(ObjInfoFlags.EditProperties, value, true);
	}

	/// <summary>When set, this flag displays a shield on the Edit button of the simple and advanced Permissions pages.</summary>
	[DefaultValue(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ElevationRequired
	{
		get => HasFlag(ObjInfoFlags.PermsElevationRequired);
		set => SetFlag(ObjInfoFlags.PermsElevationRequired, value, true);
	}

	/// <summary>Gets or sets the flags.</summary>
	/// <value>The flags.</value>
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public ObjInfoFlags Flags
	{
		get => flags;
		set { flags = value; if (iSecInfo != null) iSecInfo.ObjectInfo.Flags = value; }
	}

	/// <summary>
	/// If this flag is set, the access control editor hides the Special Permissions tab on the Advanced Security Settings page.
	/// </summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool HideSpecialPermissionTab
	{
		get => HasFlag(ObjInfoFlags.NoAdditionalPermission);
		set => SetFlag(ObjInfoFlags.NoAdditionalPermission, value, true);
	}

	/// <summary>Indicates that the access control editor cannot read the DACL but might be able to write to the DACL.</summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool MayWrite
	{
		get => HasFlag(ObjInfoFlags.MayWrite);
		set => SetFlag(ObjInfoFlags.MayWrite, value);
	}

	/// <summary>
	/// When set, indicates that the ObjectGuid property is valid. This is set in comparisons with object-specific ACEs in determining
	/// whether the ACE applies to the current object.
	/// </summary>
	[DefaultValue(false), Browsable(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ObjectGuid
	{
		get => HasFlag(ObjInfoFlags.ObjectGuid);
		private set => SetFlag(ObjInfoFlags.ObjectGuid, value);
	}

	/// <summary>
	/// Indicates that the object is a container. If this flag is set, the access control editor enables the controls relevant to the
	/// inheritance of permissions onto child objects.
	/// </summary>
	[DefaultValue(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ObjectIsContainer
	{
		get => HasFlag(ObjInfoFlags.Container);
		set => SetFlag(ObjInfoFlags.Container, value);
	}

	/// <summary>Gets or sets the name of the object.</summary>
	[DefaultValue(null), Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string ObjectName
	{
		get => objectName;
		set
		{
			objectName = value;
			if (iSecInfo != null)
				iSecInfo.ObjectInfo.ObjectName = value;
		}
	}

	/// <summary>When set, this flag displays a shield on the Edit button of the advanced Owner page.</summary>
	[DefaultValue(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool OwnerElevationRequired
	{
		get => HasFlag(ObjInfoFlags.OwnerElevationRequired);
		set => SetFlag(ObjInfoFlags.OwnerElevationRequired, value, true);
	}

	/// <summary>
	/// If this flag is set, the user cannot change the owner of the object. Set this flag if EditOwner is set but the user does not have
	/// permission to change the owner.
	/// </summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool OwnerReadOnly
	{
		get => HasFlag(ObjInfoFlags.OwnerReadOnly);
		set { var opt = value ? ObjInfoFlags.OwnerReadOnly | ObjInfoFlags.EditOwner : ObjInfoFlags.OwnerReadOnly; SetFlag(opt, value, true); }
	}

	/// <summary>
	/// Combine this flag with Container to display a check box on the owner page that indicates whether the user intends the new owner
	/// to be applied to all child objects as well as the current object. The access control editor does not perform the recursion.
	/// </summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool OwnerRecurse
	{
		get => HasFlag(ObjInfoFlags.OwnerRecurse);
		set => SetFlag(ObjInfoFlags.OwnerRecurse, value, true);
	}

	/// <summary>Gets or sets the type of the page to display.</summary>
	/// <value>The type of the page.</value>
	[DefaultValue(typeof(SecurityPageType), "BasicPermissions"), Category("Behavior")]
	public SecurityPageType PageType { get; set; }

	/// <summary>
	/// If this flag is set, the editor displays the object's security information, but the controls for editing the information are
	/// disabled. This flag cannot be combined with the ViewOnly flag.
	/// </summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ReadOnly
	{
		get => HasFlag(ObjInfoFlags.ReadOnly);
		set { if (value) SetFlag(ObjInfoFlags.ViewOnly, false); SetFlag(ObjInfoFlags.ReadOnly, value); }
	}

	/// <summary>Gets or sets the resource type of the object.</summary>
	/// <value>The type of the resource.</value>
	[DefaultValue(System.Security.AccessControl.ResourceType.Unknown), Category("Behavior")]
	public System.Security.AccessControl.ResourceType ResourceType
	{
		get => resType;
		set => resType = value;
	}

	/// <summary>Gets the resulting Security Descriptor.</summary>
	/// <value>The resulting Security Descriptor.</value>
	[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public System.Security.AccessControl.RawSecurityDescriptor Result { get; private set; }

	/// <summary>Gets the resulting Security Descriptor in SDDL form.</summary>
	/// <value>The resulting Security Descriptor in SDDL form.</value>
	[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string SDDL => Result.GetSddlForm(System.Security.AccessControl.AccessControlSections.All);

	/// <summary>
	/// Set this flag if the computer defined by the ServerName property is known to be a domain controller. If this flag is set, the
	/// domain name is included in the scope list of the Add Users and Groups dialog box. Otherwise, the pszServerName computer is used
	/// to determine the scope list of the dialog box.
	/// </summary>
	[DefaultValue(false), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ServerIsDC
	{
		get => HasFlag(ObjInfoFlags.ServerIsDC);
		set => SetFlag(ObjInfoFlags.ServerIsDC, value);
	}

	/// <summary>Gets or sets the name of the server on which the object resides.</summary>
	/// <value>The name of the server.</value>
	[DefaultValue(null), Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string ServerName
	{
		get => serverName;
		set { serverName = value; if (iSecInfo != null) iSecInfo.ObjectInfo.ServerName = value; }
	}

	/// <summary>
	/// Determines if the Advanced button is displayed on the basic security property page. If the user clicks this button, the system
	/// displays an advanced security property sheet that enables advanced editing of the discretionary access control list (DACL) of the object.
	/// </summary>
	[DefaultValue(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ShowAdvancedButton
	{
		get => HasFlag(ObjInfoFlags.Advanced);
		set { SetFlag(ObjInfoFlags.Advanced, value); if (value && PageType == SecurityPageType.BasicPermissions) SetFlag(ObjInfoFlags.ViewOnly, value); }
	}

	/// <summary>When set, this flag displays the Reset Defaults button on the Auditing page.</summary>
	[DefaultValue(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ShowAuditingResetButton
	{
		get => HasFlag(ObjInfoFlags.ResetSacl);
		set => SetFlag(ObjInfoFlags.ResetSacl, value, true);
	}

	/// <summary>
	/// If this flag is set, the Default button is displayed. If the user clicks this button, the access control editor calls the
	/// IAccessControlEditorDialogProvider.DefaultSecurity to retrieve an application-defined default security descriptor. The access
	/// control editor uses this security descriptor to reinitialize the property sheet, and the user is allowed to apply the change or cancel.
	/// </summary>
	[DefaultValue(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ShowDefaultButton
	{
		get => HasFlag(ObjInfoFlags.Reset);
		set => SetFlag(ObjInfoFlags.Reset, value, true);
	}

	/// <summary>If this flag is set, the Effective Permissions page is displayed.</summary>
	[DefaultValue(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ShowEffectivePermissionsPage
	{
		get => HasFlag(ObjInfoFlags.EditEffective);
		set => SetFlag(ObjInfoFlags.EditEffective, value, true);
	}

	/// <summary>When set, this flag displays the Reset Defaults button on the Owner page.</summary>
	[DefaultValue(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ShowOwnerResetButton
	{
		get => HasFlag(ObjInfoFlags.ResetOwner);
		set => SetFlag(ObjInfoFlags.ResetOwner, value, true);
	}

	/// <summary>When set, this flag displays the Reset Defaults button on the Permissions page.</summary>
	[DefaultValue(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ShowPermissionsResetButton
	{
		get => HasFlag(ObjInfoFlags.ResetDacl);
		set => SetFlag(ObjInfoFlags.ResetDacl, value, true);
	}

	/// <summary>Gets or sets the title of the property page tab. If this value is not set, a default will be used.</summary>
	/// <value>The title.</value>
	[DefaultValue(null), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string TabTitle
	{
		get => title;
		set { title = value; if (iSecInfo != null) iSecInfo.ObjectInfo.PageTitle = value; CustomPageTitle = !string.IsNullOrEmpty(value); }
	}

	/// <summary>
	/// If this flag is set, the editor displays the object's security information, but the controls for editing the information are
	/// disabled. This flag cannot be combined with the EditOnly flag.
	/// </summary>
	[DefaultValue(true), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ViewOnly
	{
		get => HasFlag(ObjInfoFlags.ViewOnly);
		set { if (value) SetFlag(ObjInfoFlags.ReadOnly, false); SetFlag(ObjInfoFlags.ViewOnly, value); }
	}

	/// <summary>Initializes the dialog with the specified known object.</summary>
	/// <param name="knownObject">The known object. See Remarks section for acceptable object types.</param>
	/// <remarks>
	/// <para>Known objects can include:</para>
	/// <list type="bullet">
	/// <item>
	/// <description>System.IO.Pipes.PipeStream</description>
	/// </item>
	/// <item>
	/// <description><see cref="System.Threading.EventWaitHandle"/></description>
	/// </item>
	/// <item>
	/// <description><see cref="System.IO.DirectoryInfo"/></description>
	/// </item>
	/// <item>
	/// <description><see cref="System.IO.FileInfo"/></description>
	/// </item>
	/// <item>
	/// <description><see cref="System.IO.FileStream"/></description>
	/// </item>
	/// <item>
	/// <description><see cref="System.Threading.Mutex"/></description>
	/// </item>
	/// <item>
	/// <description>System.Win32.RegistryKey</description>
	/// </item>
	/// <item>
	/// <description><see cref="System.Threading.Semaphore"/></description>
	/// </item>
	/// <item>
	/// <description>System.IO.MemoryMappedFiles.MemoryMappedFile</description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="System.Security.AccessControl.CommonObjectSecurity"/> or derived class. <c>Note:</c> When using this option, be sure
	/// to set the <see cref="ObjectIsContainer"/>, <see cref="ResourceType"/>, <see cref="ObjectName"/>, and <see cref="ServerName"/> properties.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <para>Any object that supports the following methods and properties:</para>
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// <code>
	///GetAccessControl()
	/// </code>
	/// or
	/// <code>
	///GetAccessControl(AccessControlSections)
	/// </code>
	/// method
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <code>
	///SetAccessControl(CommonObjectSecurity)
	/// </code>
	/// method
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <code>
	///Name
	/// </code>
	/// or
	/// <code>
	///FullName
	/// </code>
	/// property
	/// </description>
	/// </item>
	/// </list>
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	public void Initialize(object knownObject)
	{
		SecuredObject secObject = new(knownObject);
		Initialize(secObject.DisplayName, secObject.ObjectName, secObject.IsContainer, ProviderFromResourceType(secObject.ResourceType),
			secObject.ObjectSecurity.GetSecurityDescriptorBinaryForm(), secObject.TargetServer);
		ResourceType = secObject.ResourceType;
	}

	/// <summary>Initializes the dialog with the specified known object.</summary>
	/// <param name="fullObjectName">Full name of the object.</param>
	/// <param name="serverName">Name of the server. This value can be <c>null</c>.</param>
	/// <param name="resourceType">Type of the object resource.</param>
	/// <exception cref="ArgumentException">Unable to create an object from supplied arguments.</exception>
	public void Initialize(string fullObjectName, string serverName, System.Security.AccessControl.ResourceType resourceType) => Initialize(SecuredObject.GetKnownObject(resourceType, fullObjectName, serverName));

	/// <summary>Initializes the dialog with a custom provider.</summary>
	/// <param name="displayName">The object name.</param>
	/// <param name="fullObjectName">Full name of the object.</param>
	/// <param name="isContainer">Set to <c>true</c> if object is a container.</param>
	/// <param name="customProvider">The custom provider.</param>
	/// <param name="sd">The binary Security Descriptor.</param>
	/// <param name="targetServer">The target server.</param>
	public void Initialize(string displayName, string fullObjectName, bool isContainer, IAccessControlEditorDialogProvider customProvider, byte[] sd, string targetServer = null)
	{
		if (isContainer)
			ObjectIsContainer = true;
		iSecInfo = new SecurityInfoImpl(flags, displayName, fullObjectName, targetServer);
		ResourceType = System.Security.AccessControl.ResourceType.Unknown;
		if (sd != null)
		{
			Result = new System.Security.AccessControl.RawSecurityDescriptor(sd, 0);
			iSecInfo.SecurityDescriptor = sd;
		}
		else
		{
			Result = new System.Security.AccessControl.RawSecurityDescriptor("");
			iSecInfo.SecurityDescriptor = new byte[20];
		}
		iSecInfo.SetProvider(customProvider);
	}

	/// <summary>Initializes the dialog with a known resource type.</summary>
	/// <param name="displayName">The display name.</param>
	/// <param name="fullName">The full name.</param>
	/// <param name="isContainer">if set to <c>true</c> [is container].</param>
	/// <param name="resourceType">Type of the resource.</param>
	/// <param name="sd">The raw security descriptor.</param>
	/// <param name="targetServer">The target server.</param>
	public void Initialize(string displayName, string fullName, bool isContainer, System.Security.AccessControl.ResourceType resourceType, byte[] sd, string targetServer = null)
	{
		Initialize(displayName, fullName, isContainer, ProviderFromResourceType(resourceType), sd, targetServer);
		ResourceType = resourceType;
	}

	/// <summary>When overridden in a derived class, resets the properties of a common dialog box to their default values.</summary>
	/// <exception cref="InvalidOperationException">
	/// AccessControlEditorDialog cannot be reset. It must be instantiated with a valid securable object.
	/// </exception>
	public override void Reset() => throw new InvalidOperationException("AccessControlEditorDialog cannot be reset. It must be instantiated with a valid securable object.");

	internal void ResetFlags() => flags = defaultFlags;

	internal bool ShouldSerializeFlags() => flags != defaultFlags;

	/// <summary>Runs the dialog.</summary>
	/// <param name="hWndOwner">The h WND owner.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">The Initialize method must be called before the dialog can be shown.</exception>
	protected override bool RunDialog(IntPtr hWndOwner)
	{
		if (iSecInfo == null && !string.IsNullOrEmpty(ObjectName) && ResourceType != System.Security.AccessControl.ResourceType.Unknown)
			Initialize(ObjectName, ServerName, ResourceType);

		if (iSecInfo == null)
			throw new InvalidOperationException("The Initialize method must be called before the dialog can be shown.");

		var ret = iSecInfo.ShowDialog(hWndOwner, PageType);
		if (ret != null)
		{
#if DEBUG
			MessageBox.Show(ret.GetSddlForm(System.Security.AccessControl.AccessControlSections.All));
#endif
			Result = ret;
		}
		return ret != null;
	}

	private bool HasFlag(ObjInfoFlags flag) => (Flags & flag) == flag;

	private static GenericProvider ProviderFromResourceType(System.Security.AccessControl.ResourceType resType) => resType switch
	{
		System.Security.AccessControl.ResourceType.FileObject => new FileProvider(),
		System.Security.AccessControl.ResourceType.KernelObject => new KernelProvider(),
		System.Security.AccessControl.ResourceType.RegistryKey => new RegistryProvider(),
		TaskResourceType => new TaskProvider(),
		_ => new GenericProvider(),
	};

	private void SetFlag(ObjInfoFlags flag, bool set, bool reqAdvanced = false)
	{
		if (set)
		{
			if (reqAdvanced)
				flag |= ObjInfoFlags.Advanced;
			Flags |= flag;
		}
		else
			Flags &= ~flag;
	}
}