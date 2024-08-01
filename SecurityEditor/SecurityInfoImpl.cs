using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using static Microsoft.Win32.NativeMethods;

namespace Community.Security.AccessControl;

internal delegate void SecurityEvent(SecurityEventArg e);

internal class SecurityEventArg(IntPtr sd, int parts) : EventArgs
{
	public int Parts = parts;
	public IntPtr SecurityDesciptor = sd;
}

internal class SecurityInfoImpl(ObjInfoFlags flags, string objectName, string fullName, string serverName = null, string pageTitle = null) :
	ISecurityInformation, ISecurityInformation3, ISecurityInformation4, ISecurityObjectTypeInfo, IEffectivePermission, IEffectivePermission2
{
	internal SI_OBJECT_INFO ObjectInfo = new(flags, objectName, serverName, pageTitle);
	private ObjInfoFlags currentElevation = 0;
	private readonly string fullObjectName = fullName;
	private IAccessControlEditorDialogProvider prov;
	private MarshaledMem pSD;

	public event SecurityEvent OnSetSecurity;

	public byte[] SecurityDescriptor
	{
		get => pSD.Array;
		set => pSD = new MarshaledMem(value);
	}

	public void SetProvider(IAccessControlEditorDialogProvider provider) => prov = provider;

	public RawSecurityDescriptor ShowDialog(IntPtr hWnd, SecurityPageType pageType = SecurityPageType.BasicPermissions, SecurityPageActivation pageAct = SecurityPageActivation.ShowDefault)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("ShowDialog: {0} {1}", pageType, pageAct));
		SecurityEventArg sd = null;
		void fn(SecurityEventArg e) => sd = e;
		try
		{
			OnSetSecurity += fn;
			if (System.Environment.OSVersion.Version.Major == 5 || pageType == SecurityPageType.BasicPermissions && pageAct == SecurityPageActivation.ShowDefault)
			{
				if (!EditSecurity(hWnd, this))
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			else
			{
				uint uPage = (uint)pageType | ((uint)pageAct << 16);
				EditSecurityAdvanced(hWnd, this, uPage);
			}
			if (sd != null)
			{
				string sddl = SecurityDescriptorPtrToSDLL(sd.SecurityDesciptor, sd.Parts);
				if (!string.IsNullOrEmpty(sddl))
				{
					System.Diagnostics.Debug.WriteLine(string.Format("ShowDialog: Return: {0}", sddl));
					return new RawSecurityDescriptor(sddl);
				}
			}
		}
		finally
		{
			OnSetSecurity -= fn;
		}
		System.Diagnostics.Debug.WriteLine(string.Format("ShowDialog: Return: null"));
		return null;
	}

	private const int E_FAIL = unchecked((int)0x80004005);
	private const int E_INVALIDARG = unchecked((int)0x80070057);
	private const int E_NOINTERFACE = unchecked((int)0x80004002);

	int IEffectivePermission2.ComputeEffectivePermissionWithSecondarySecurity(IntPtr pSid, IntPtr pDeviceSid, string pszServerName,
		SECURITY_OBJECT[] pSecurityObjects, uint dwSecurityObjectCount, in TOKEN_GROUPS pUserGroups, AuthzSidOperation[] pAuthzUserGroupsOperations,
		in TOKEN_GROUPS pDeviceGroups, AuthzSidOperation[] pAuthzDeviceGroupsOperations, in AUTHZ_SECURITY_ATTRIBUTES_INFORMATION pAuthzUserClaims,
		AuthzSecurityAttributeOperation[] pAuthzUserClaimsOperations, in AUTHZ_SECURITY_ATTRIBUTES_INFORMATION pAuthzDeviceClaims,
		AuthzSecurityAttributeOperation[] pAuthzDeviceClaimsOperations, EFFPERM_RESULT_LIST[] pEffpermResultLists)
	{
		System.Diagnostics.Debug.WriteLine($"ComputeEffectivePermissionWithSecondarySecurity({dwSecurityObjectCount}):{new SecurityIdentifier(pSid).Value};{new SecurityIdentifier(pDeviceSid).Value}");
		if (dwSecurityObjectCount != 1)
			return E_FAIL;
		if (pSecurityObjects[0].Id != SecurityObjectType.ObjectSD)
			return E_FAIL;
		if (pSid == IntPtr.Zero)
			return E_INVALIDARG;
		if (prov is null) return E_NOINTERFACE;

		if (!AuthzInitializeResourceManager(AuthzResourceManagerFlags.AUTHZ_RM_FLAG_NO_AUDIT, null, null, null, prov.ToString(), out var hAuthzResourceManager))
			return 0;

		var identifier = new LUID();
		SafeAUTHZ_CLIENT_CONTEXT_HANDLE hAuthzCompoundContext = default;
		if (!AuthzInitializeContextFromSid(AuthzContextFlags.DEFAULT, pSid, hAuthzResourceManager, IntPtr.Zero, identifier, IntPtr.Zero, out var hAuthzUserContext))
			return 0;

		pAuthzDeviceGroupsOperations ??= [];
		pAuthzUserClaimsOperations ??= [];
		pAuthzDeviceClaimsOperations ??= [];
		if (pDeviceSid != IntPtr.Zero)
		{
			if (AuthzInitializeContextFromSid(AuthzContextFlags.DEFAULT, pDeviceSid, hAuthzResourceManager, IntPtr.Zero, identifier, IntPtr.Zero, out var hAuthzDeviceContext))
				if (AuthzInitializeCompoundContext(hAuthzUserContext, hAuthzDeviceContext, out hAuthzCompoundContext))
					if (pAuthzDeviceClaims.Version != 0)
						AuthzModifyClaims(hAuthzCompoundContext, AuthzContextInformationClass.AuthzContextInfoDeviceClaims, pAuthzDeviceClaimsOperations, pAuthzDeviceClaims);
		}
		else
		{
			hAuthzCompoundContext = hAuthzUserContext;
		}

		if (hAuthzCompoundContext is null)
			return 0;

		if (pAuthzUserClaims.Version != 0)
			if (!AuthzModifyClaims(hAuthzCompoundContext, AuthzContextInformationClass.AuthzContextInfoUserClaims, pAuthzUserClaimsOperations, pAuthzUserClaims))
				return 0;

		if (pDeviceGroups.GroupCount != 0)
			if (!AuthzModifySids(hAuthzCompoundContext, AuthzContextInformationClass.AuthzContextInfoDeviceSids, pAuthzDeviceGroupsOperations, pDeviceGroups))
				return 0;

		if (pUserGroups.GroupCount != 0 && pAuthzUserGroupsOperations != null)
			if (!AuthzModifySids(hAuthzCompoundContext, AuthzContextInformationClass.AuthzContextInfoGroupsSids, pAuthzUserGroupsOperations, pUserGroups))
				return 0;

		var request = new AUTHZ_ACCESS_REQUEST(0x02000000 /*MAXIMUM_ALLOWED*/);
		var sd = pSecurityObjects[0].pData;
		var reply = new AUTHZ_ACCESS_REPLY(1);
		if (!AuthzAccessCheck(AuthzAccessCheckFlags.NONE, hAuthzCompoundContext, request, default, sd, default, 0, reply, out _))
			return 0;

		pEffpermResultLists[0].fEvaluated = true;
		pEffpermResultLists[0].pGrantedAccessList = reply.GrantedAccessMaskValues;
		pEffpermResultLists[0].pObjectTypeList = [ObjectTypeList.Self];
		pEffpermResultLists[0].cObjectTypeListLength = 1;

		return 0;
	}

	void ISecurityInformation.GetAccessRights(Guid guidObject, int dwFlags, out AccessRightInfo[] access, ref uint access_count, out uint DefaultAccess)
	{
		System.Diagnostics.Debug.WriteLine($"GetAccessRight: {guidObject}, {(ObjInfoFlags)dwFlags}");
		prov.GetAccessListInfo((ObjInfoFlags)dwFlags, out AccessRightInfo[] ari, out uint defAcc);
		DefaultAccess = defAcc;
		access = ari;
		access_count = (uint)access.Length;
	}

	void IEffectivePermission.GetEffectivePermission(Guid pguidObjectType, IntPtr pUserSid, string pszServerName, IntPtr pSD, out ObjectTypeList[] ppObjectTypeList, out uint pcObjectTypeListLength, out uint[] ppGrantedAccessList, out uint pcGrantedAccessListLength)
	{
		System.Diagnostics.Debug.WriteLine($"GetEffectivePermission: {pguidObjectType}, {pszServerName}");
		if (pguidObjectType == Guid.Empty)
		{
			ppGrantedAccessList = prov.GetEffectivePermission(pUserSid, pszServerName, pSD);
			pcGrantedAccessListLength = (uint)ppGrantedAccessList.Length;
			ppObjectTypeList = [ObjectTypeList.Self];
			pcObjectTypeListLength = (uint)ppObjectTypeList.Length;
		}
		else
		{
			ppGrantedAccessList = prov.GetEffectivePermission(pguidObjectType, pUserSid, pszServerName, pSD, out ppObjectTypeList);
			pcGrantedAccessListLength = (uint)ppGrantedAccessList.Length;
			pcObjectTypeListLength = (uint)ppObjectTypeList.Length;
		}
	}

	void ISecurityInformation3.GetFullResourceName(out string szResourceName) => szResourceName = fullObjectName;

	void ISecurityObjectTypeInfo.GetInheritSource(int si, IntPtr pACL, out InheritedFromInfo[] ppInheritArray)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("GetInheritSource: {0}", (SecurityInfosEx)si));
		ppInheritArray = prov.GetInheritSource(fullObjectName, ObjectInfo.ServerName, ObjectInfo.IsContainer, (uint)si, pACL);
	}

	void ISecurityInformation.GetInheritTypes(out InheritTypeInfo[] InheritTypes, out uint InheritTypesCount)
	{
		System.Diagnostics.Debug.WriteLine("GetInheritTypes");
		InheritTypes = prov.GetInheritTypes();
		InheritTypesCount = (uint)InheritTypes.Length;
	}

	void ISecurityInformation.GetObjectInformation(ref SI_OBJECT_INFO object_info)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("GetObjectInformation: {0} {1}", ObjectInfo.Flags, currentElevation));
		object_info = ObjectInfo;
		object_info.Flags &= ~currentElevation;
	}

	void ISecurityInformation.GetSecurity(int RequestInformation, out IntPtr ppSecurityDescriptor, bool fDefault)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("GetSecurity: {0}{1}", (SecurityInfosEx)RequestInformation, fDefault ? " (Def)" : ""));
		ppSecurityDescriptor = GetPrivateObjectSecurity(fDefault ? prov.GetDefaultSecurity() : pSD.Ptr, RequestInformation);
		System.Diagnostics.Debug.WriteLine(string.Format("GetSecurity={0}<-{1}", SecurityDescriptorPtrToSDLL(ppSecurityDescriptor, RequestInformation), SecurityDescriptorPtrToSDLL(pSD.Ptr, RequestInformation)));
	}

	void ISecurityInformation.MapGeneric(Guid guidObjectType, ref sbyte AceFlags, ref uint Mask)
	{
		uint stMask = Mask;
		GenericMapping gm = prov.GetGenericMapping(AceFlags);
		MapGenericMask(ref Mask, ref gm);
		//if (Mask != gm.GenericAll)
		//	Mask &= ~(uint)FileSystemRights.Synchronize;
		System.Diagnostics.Debug.WriteLine(string.Format("MapGeneric: {0}, {1}, 0x{2:X}->0x{3:X}", guidObjectType, (AceFlags)AceFlags, stMask, Mask));
	}

	void ISecurityInformation3.OpenElevatedEditor(IntPtr hWnd, uint uPage)
	{
		SecurityPageType pgType = (SecurityPageType)(uPage & 0x0000FFFF);
		SecurityPageActivation pgActv = (SecurityPageActivation)((uPage >> 16) & 0x0000FFFF);
		System.Diagnostics.Debug.WriteLine(string.Format("OpenElevatedEditor: {0} - {1}", pgType, pgActv));
		ObjInfoFlags lastElev = currentElevation;
		switch (pgActv)
		{
			case SecurityPageActivation.ShowDefault:
				currentElevation |= ObjInfoFlags.PermsElevationRequired | ObjInfoFlags.ViewOnly;
				break;

			case SecurityPageActivation.ShowPermActivated:
				currentElevation |= ObjInfoFlags.PermsElevationRequired | ObjInfoFlags.ViewOnly;
				pgType = SecurityPageType.AdvancedPermissions;
				break;

			case SecurityPageActivation.ShowAuditActivated:
				currentElevation |= ObjInfoFlags.AuditElevationRequired;
				pgType = SecurityPageType.Audit;
				break;

			case SecurityPageActivation.ShowOwnerActivated:
				currentElevation |= ObjInfoFlags.OwnerElevationRequired;
				pgType = SecurityPageType.Owner;
				break;

			case SecurityPageActivation.ShowEffectiveActivated:
				break;

			case SecurityPageActivation.ShowShareActivated:
				break;

			case SecurityPageActivation.ShowCentralPolicyActivated:
				break;

			default:
				break;
		}
		ShowDialog(hWnd, pgType, pgActv);
		currentElevation = lastElev;
	}

	void ISecurityInformation.PropertySheetPageCallback(IntPtr hwnd, PropertySheetCallbackMessage uMsg, SecurityPageType uPage)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("PropertySheetPageCallback: {0}, {1}, {2}", hwnd, uMsg, uPage));
		prov.PropertySheetPageCallback(hwnd, uMsg, uPage);
	}

	void ISecurityInformation.SetSecurity(int RequestInformation, IntPtr sd) => OnSetSecurity?.Invoke(new SecurityEventArg(sd, RequestInformation));

	void ISecurityInformation4.GetSecondarySecurity(out SECURITY_OBJECT[] securityObjects, out uint securityObjectCount)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("GetSecondarySecurity:"));
		securityObjects = [];
		securityObjectCount = 0;
	}
}