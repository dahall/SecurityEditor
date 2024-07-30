using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Community.Security.AccessControl;

internal delegate void SecurityEvent(SecurityEventArg e);

internal class SecurityEventArg(IntPtr sd, int parts) : EventArgs
{
	public int Parts = parts;
	public IntPtr SecurityDesciptor = sd;
}

internal class SecurityInfoImpl(ObjInfoFlags flags, string objectName, string fullName, string serverName = null, string pageTitle = null) : NativeMethods.ISecurityInformation, NativeMethods.ISecurityInformation3, NativeMethods.ISecurityObjectTypeInfo, NativeMethods.IEffectivePermission//, NativeMethods.ISecurityInformation4, NativeMethods.IEffectivePermission2
{
	internal NativeMethods.SI_OBJECT_INFO ObjectInfo = new NativeMethods.SI_OBJECT_INFO(flags, objectName, serverName, pageTitle);
	private ObjInfoFlags currentElevation = 0;
	private readonly string fullObjectName = fullName;
	private IAccessControlEditorDialogProvider prov;
	private NativeMethods.MarshaledMem pSD;

	public event SecurityEvent OnSetSecurity;

	public byte[] SecurityDescriptor
	{
		get => pSD.Array;
		set => pSD = new NativeMethods.MarshaledMem(value);
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
				if (!NativeMethods.EditSecurity(hWnd, this))
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			else
			{
				uint uPage = (uint)pageType + ((uint)pageAct << 16);
				NativeMethods.EditSecurityAdvanced(hWnd, this, uPage);
			}
			if (sd != null)
			{
				string sddl = NativeMethods.SecurityDescriptorPtrToSDLL(sd.SecurityDesciptor, sd.Parts);
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

	void NativeMethods.ISecurityInformation.GetAccessRights(Guid guidObject, int dwFlags, out AccessRightInfo[] access, ref uint access_count, out uint DefaultAccess)
	{
		System.Diagnostics.Debug.WriteLine($"GetAccessRight: {guidObject}, {(ObjInfoFlags)dwFlags}");
		prov.GetAccessListInfo((ObjInfoFlags)dwFlags, out AccessRightInfo[] ari, out uint defAcc);
		DefaultAccess = defAcc;
		access = ari;
		access_count = (uint)access.Length;
	}

	void NativeMethods.IEffectivePermission.GetEffectivePermission(Guid pguidObjectType, IntPtr pUserSid, string pszServerName, IntPtr pSD, out ObjectTypeList[] ppObjectTypeList, out uint pcObjectTypeListLength, out uint[] ppGrantedAccessList, out uint pcGrantedAccessListLength)
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

	void NativeMethods.ISecurityInformation3.GetFullResourceName(out string szResourceName) => szResourceName = fullObjectName;

	void NativeMethods.ISecurityObjectTypeInfo.GetInheritSource(int si, IntPtr pACL, out InheritedFromInfo[] ppInheritArray)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("GetInheritSource: {0}", (SecurityInfosEx)si));
		ppInheritArray = prov.GetInheritSource(fullObjectName, ObjectInfo.ServerName, ObjectInfo.IsContainer, (uint)si, pACL);
	}

	void NativeMethods.ISecurityInformation.GetInheritTypes(out InheritTypeInfo[] InheritTypes, out uint InheritTypesCount)
	{
		System.Diagnostics.Debug.WriteLine("GetInheritTypes");
		InheritTypes = prov.GetInheritTypes();
		InheritTypesCount = (uint)InheritTypes.Length;
	}

	void NativeMethods.ISecurityInformation.GetObjectInformation(ref NativeMethods.SI_OBJECT_INFO object_info)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("GetObjectInformation: {0} {1}", ObjectInfo.Flags, currentElevation));
		object_info = ObjectInfo;
		object_info.Flags &= ~currentElevation;
	}

	void NativeMethods.ISecurityInformation.GetSecurity(int RequestInformation, out IntPtr ppSecurityDescriptor, bool fDefault)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("GetSecurity: {0}{1}", (SecurityInfosEx)RequestInformation, fDefault ? " (Def)" : ""));
		ppSecurityDescriptor = NativeMethods.GetPrivateObjectSecurity(fDefault ? prov.GetDefaultSecurity() : pSD.Ptr, RequestInformation);
		System.Diagnostics.Debug.WriteLine(string.Format("GetSecurity={0}<-{1}", NativeMethods.SecurityDescriptorPtrToSDLL(ppSecurityDescriptor, RequestInformation), NativeMethods.SecurityDescriptorPtrToSDLL(pSD.Ptr, RequestInformation)));
	}

	void NativeMethods.ISecurityInformation.MapGeneric(Guid guidObjectType, ref sbyte AceFlags, ref uint Mask)
	{
		uint stMask = Mask;
		GenericMapping gm = prov.GetGenericMapping(AceFlags);
		NativeMethods.MapGenericMask(ref Mask, ref gm);
		//if (Mask != gm.GenericAll)
		//	Mask &= ~(uint)FileSystemRights.Synchronize;
		System.Diagnostics.Debug.WriteLine(string.Format("MapGeneric: {0}, {1}, 0x{2:X}->0x{3:X}", guidObjectType, (AceFlags)AceFlags, stMask, Mask));
	}

	void NativeMethods.ISecurityInformation3.OpenElevatedEditor(IntPtr hWnd, uint uPage)
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

	void NativeMethods.ISecurityInformation.PropertySheetPageCallback(IntPtr hwnd, PropertySheetCallbackMessage uMsg, SecurityPageType uPage)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("PropertySheetPageCallback: {0}, {1}, {2}", hwnd, uMsg, uPage));
		prov.PropertySheetPageCallback(hwnd, uMsg, uPage);
	}

	void NativeMethods.ISecurityInformation.SetSecurity(int RequestInformation, IntPtr sd) => OnSetSecurity?.Invoke(new SecurityEventArg(sd, RequestInformation));

	/*void NativeMethods.ISecurityInformation4.GetSecondarySecurity(out NativeMethods.SECURITY_OBJECT[] securityObjects, out uint securityObjectCount)
	{
		System.Diagnostics.Debug.WriteLine(string.Format("GetSecondarySecurity:"));
		securityObjects = new NativeMethods.SECURITY_OBJECT[0];
		securityObjectCount = 0;
	}*/
}