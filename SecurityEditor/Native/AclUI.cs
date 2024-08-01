﻿using Community.Security.AccessControl;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32;

internal static partial class NativeMethods
{
	[Flags]
	internal enum GET_SECURITY_REQUEST_INFORMATION
	{
		OWNER_SECURITY_INFORMATION = 1,
		GROUP_SECURITY_INFORMATION = 2,
		DACL_SECURITY_INFORMATION = 4,
		SACL_SECURITY_INFORMATION = 8,
	}

	internal enum SecurityObjectType : uint
	{
		ObjectSD = 1,
		Share = 2,
		CentralPolicy = 3,
		CentralAccessRule = 4
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3853DC76-9F35-407c-88A1-D19344365FBC")]
	internal interface IEffectivePermission
	{
		void GetEffectivePermission([In, MarshalAs(UnmanagedType.LPStruct)] Guid pguidObjectType, [In] IntPtr pUserSid,
			[In, MarshalAs(UnmanagedType.LPWStr)] string pszServerName, [In] IntPtr pSD,
			[MarshalAs(UnmanagedType.LPArray)] out ObjectTypeList[] ppObjectTypeList,
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

		[PreserveSig]
		int ComputeEffectivePermissionWithSecondarySecurity(
			[In] IntPtr pSid,
			[In, Optional] IntPtr pDeviceSid,
			[In, Optional, MarshalAs(UnmanagedType.LPWStr)] string pszServerName,
			[In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] SECURITY_OBJECT[] pSecurityObjects,
			uint dwSecurityObjectCount,
			in TOKEN_GROUPS pUserGroups,
			[In, Optional] AuthzSidOperation[] pAuthzUserGroupsOperations,
			in TOKEN_GROUPS pDeviceGroups,
			[In, Optional] AuthzSidOperation[] pAuthzDeviceGroupsOperations,
			in AUTHZ_SECURITY_ATTRIBUTES_INFORMATION pAuthzUserClaims,
			[In, Optional] AuthzSecurityAttributeOperation[] pAuthzUserClaimsOperations,
			in AUTHZ_SECURITY_ATTRIBUTES_INFORMATION pAuthzDeviceClaims,
			[In, Optional] AuthzSecurityAttributeOperation[] pAuthzDeviceClaimsOperations,
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] EFFPERM_RESULT_LIST[] pEffpermResultLists);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("965FC360-16FF-11d0-91CB-00AA00BBB723")]
	internal interface ISecurityInformation
	{
		void GetObjectInformation(ref SI_OBJECT_INFO object_info);

		void GetSecurity([In] int RequestInformation, out IntPtr SecurityDescriptor, [In] bool fDefault);

		void SetSecurity([In] int RequestInformation, [In] IntPtr SecurityDescriptor);

		void GetAccessRights([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidObject, [In] int dwFlags, [MarshalAs(UnmanagedType.LPArray)] out AccessRightInfo[] access, ref uint access_count, out uint DefaultAccess);

		void MapGeneric([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidObjectType, [In] ref sbyte AceFlags, [In] ref uint Mask);

		void GetInheritTypes([MarshalAs(UnmanagedType.LPArray)] out InheritTypeInfo[] InheritType, out uint InheritTypesCount);

		void PropertySheetPageCallback([In] IntPtr hwnd, [In] PropertySheetCallbackMessage uMsg, [In] SecurityPageType uPage);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("c3ccfdb4-6f88-11d2-a3ce-00c04fb1782a")]
	internal interface ISecurityInformation2
	{
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsDaclCanonical([In] IntPtr pDacl);

		void LookupSids([In] uint cSids, [In] IntPtr rgpSids, out IntPtr ppdo);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("E2CDC9CC-31BD-4f8f-8C8B-B641AF516A1A")]
	internal interface ISecurityInformation3
	{
		void GetFullResourceName(out string szResourceName);

		void OpenElevatedEditor([In] IntPtr hWnd, [In] uint uPage);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("EA961070-CD14-4621-ACE4-F63C03E583E4")]
	internal interface ISecurityInformation4
	{
		void GetSecondarySecurity([MarshalAs(UnmanagedType.LPArray)] out SECURITY_OBJECT[] securityObjects, out uint securityObjectCount);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("FC3066EB-79EF-444b-9111-D18A75EBF2FA")]
	internal interface ISecurityObjectTypeInfo
	{
		void GetInheritSource([In] int si, [In] IntPtr pACL, [MarshalAs(UnmanagedType.LPArray)] out InheritedFromInfo[] ppInheritArray);
	}

	[DllImport("aclui.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EditSecurity(IntPtr hwnd, ISecurityInformation psi);

	[DllImport("aclui.dll", PreserveSig = false)]
	public static extern void EditSecurityAdvanced(IntPtr hwnd, ISecurityInformation psi, uint pageType);

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct EFFPERM_RESULT_LIST(int resultLen = 0)
	{
		[MarshalAs(UnmanagedType.Bool)]
		public bool fEvaluated = resultLen > 0;

		public uint cObjectTypeListLength = (uint)resultLen;

		[MarshalAs(UnmanagedType.LPArray)]
		public ObjectTypeList[] pObjectTypeList = new ObjectTypeList[resultLen];

		[MarshalAs(UnmanagedType.LPArray)]
		public uint[] pGrantedAccessList = new uint[resultLen];
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SECURITY_OBJECT
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pwszName;

		public IntPtr pData;
		public uint cbData;
		public IntPtr pData2;
		public uint cbData2;
		public SecurityObjectType Id;

		[MarshalAs(UnmanagedType.Bool)]
		public bool fWellKnown;
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
			Flags = flags;
			hInstance = IntPtr.Zero;
			ObjectName = objectName;
			ServerName = serverName;
			PageTitle = pageTitle;
			ObjectTypeGuid = objectGuid;
			if (PageTitle != null)
				Flags |= ObjInfoFlags.PageTitle;
			if (ObjectTypeGuid != Guid.Empty)
				Flags |= ObjInfoFlags.ObjectGuid;
		}

		public bool IsContainer => (Flags & ObjInfoFlags.Container) == ObjInfoFlags.Container;

		public override string ToString() => string.Format("{0}: {1}{2}", ObjectName, Flags, IsContainer ? " (Cont)" : "");
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
}