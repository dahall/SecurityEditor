﻿using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Win32;

internal static partial class NativeMethods
{
	private const string ADVAPI32 = "advapi32.dll";

	[Flags]
	public enum AccessTypes : uint
	{
		TokenAssignPrimary = 0x0001,
		TokenDuplicate = 0x0002,
		TokenImpersonate = 0x0004,
		TokenQuery = 0x0008,
		TokenQuerySource = 0x0010,
		TokenAdjustPrivileges = 0x0020,
		TokenAdjustGroups = 0x0040,
		TokenAdjustDefault = 0x0080,
		TokenAdjustSessionID = 0x0100,
		TokenAllAccessP = 0x000F00FF,
		TokenAllAccess = 0x000F01FF,
		TokenRead = 0x00020008,
		TokenWrite = 0x000200E0,
		TokenExecute = 0x00020000,

		Delete = 0x00010000,
		ReadControl = 0x00020000,
		WriteDac = 0x00040000,
		WriteOwner = 0x00080000,
		Synchronize = 0x00100000,
		StandardRightsRequired = 0x000F0000,
		StandardRightsRead = 0x00020000,
		StandardRightsWrite = 0x00020000,
		StandardRightsExecute = 0x00020000,
		StandardRightsAll = 0x001F0000,
		SpecificRightsAll = 0x0000FFFF,
		AccessSystemSecurity = 0x01000000,
		MaximumAllowed = 0x02000000,
		GenericRead = 0x80000000,
		GenericWrite = 0x40000000,
		GenericExecute = 0x20000000,
		GenericAll = 0x10000000,
	}

	[Flags]
	public enum PrivilegeAttributes : uint
	{
		Disabled = 0x00000000,
		EnabledByDefault = 0x00000001,
		Enabled = 0x00000002,
		UsedForAccess = 0x80000000,
	}

	public enum SECURITY_IMPERSONATION_LEVEL
	{
		Anonymous,
		Identification,
		Impersonation,
		Delegation
	}

	public enum TOKEN_ELEVATION_TYPE
	{
		Default = 1,
		Full,
		Limited
	}

	public enum TOKEN_INFORMATION_CLASS
	{
		TokenUser = 1,
		TokenGroups,
		TokenPrivileges,
		TokenOwner,
		TokenPrimaryGroup,
		TokenDefaultDacl,
		TokenSource,
		TokenType,
		TokenImpersonationLevel,
		TokenStatistics,
		TokenRestrictedSids,
		TokenSessionId,
		TokenGroupsAndPrivileges,
		TokenSessionReference,
		TokenSandBoxInert,
		TokenAuditPolicy,
		TokenOrigin,
		TokenElevationType,
		TokenLinkedToken,
		TokenElevation,
		TokenHasRestrictions,
		TokenAccessInformation,
		TokenVirtualizationAllowed,
		TokenVirtualizationEnabled,
		TokenIntegrityLevel,
		TokenUIAccess,
		TokenMandatoryPolicy,
		TokenLogonSid,
		MaxTokenInfoClass
	}

	[Serializable]
	public enum TokenType
	{
		TokenImpersonation = 2,
		TokenPrimary = 1
	}

	[DllImport(ADVAPI32, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool AdjustTokenPrivileges([In] SafeTokenHandle TokenHandle, [In] bool DisableAllPrivileges, [In] ref TOKEN_PRIVILEGES NewState, [In] uint BufferLength, [In, Out] ref TOKEN_PRIVILEGES PreviousState, [In, Out] ref uint ReturnLength);

	[DllImport(ADVAPI32, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool AdjustTokenPrivileges([In] SafeTokenHandle TokenHandle, [In] bool DisableAllPrivileges, [In] ref TOKEN_PRIVILEGES NewState, [In] uint BufferLength, [In] IntPtr PreviousState, [In] IntPtr ReturnLength);

	[DllImport(ADVAPI32, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool ConvertStringSidToSid([In, MarshalAs(UnmanagedType.LPTStr)] string pStringSid, ref IntPtr sid);

	[DllImport(ADVAPI32, CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool DuplicateToken(SafeTokenHandle ExistingTokenHandle, SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, out SafeTokenHandle DuplicateTokenHandle);

	[DllImport(ADVAPI32, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr GetSidSubAuthority(IntPtr pSid, uint nSubAuthority);

	[DllImport(ADVAPI32, CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetTokenInformation(SafeTokenHandle hToken, TOKEN_INFORMATION_CLASS tokenInfoClass, IntPtr pTokenInfo, int tokenInfoLength, out int returnLength);

	[DllImport(ADVAPI32, SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern int LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

	[DllImport(ADVAPI32, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool LookupAccountSid(string systemName, byte[] accountSid, StringBuilder accountName, ref int nameLength, StringBuilder domainName, ref int domainLength, out int accountType);

	[DllImport(ADVAPI32, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool LookupAccountSid([In, MarshalAs(UnmanagedType.LPTStr)] string systemName, IntPtr sid, StringBuilder name, ref int cchName, StringBuilder referencedDomainName, ref int cchReferencedDomainName, out int use);

	[DllImport(ADVAPI32, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool LookupPrivilegeValue(string systemName, string name, out LUID luid);

	[DllImport(ADVAPI32, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool OpenProcessToken(IntPtr ProcessHandle, AccessTypes DesiredAccess, out SafeTokenHandle TokenHandle);

	[DllImport(ADVAPI32, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool OpenThreadToken(IntPtr ThreadHandle, AccessTypes DesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool OpenAsSelf, out SafeTokenHandle TokenHandle);

	[DllImport(ADVAPI32, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool PrivilegeCheck(IntPtr ClientToken, ref PRIVILEGE_SET RequiredPrivileges, out int result);

	[DllImport(ADVAPI32, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RevertToSelf();

	[DllImport(ADVAPI32, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetThreadToken(IntPtr ThreadHandle, SafeTokenHandle TokenHandle);

	[DllImport(ADVAPI32, CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool DuplicateTokenEx([In] SafeTokenHandle ExistingTokenHandle, [In] AccessTypes DesiredAccess, [In] IntPtr TokenAttributes, [In] SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, [In] TokenType TokenType, [In, Out] ref SafeTokenHandle DuplicateTokenHandle);

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LUID
	{
		public uint LowPart;
		public int HighPart;

		public static LUID FromName(string name, string systemName = null)
		{
			return !LookupPrivilegeValue(systemName, name, out LUID val) ? throw new System.ComponentModel.Win32Exception() : val;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LUID_AND_ATTRIBUTES(LUID luid, PrivilegeAttributes attr)
	{
		public LUID Luid = luid;
		public PrivilegeAttributes Attributes = attr;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PRIVILEGE_SET : IDisposable
	{
		public uint PrivilegeCount;
		public uint Control;
		public IntPtr Privilege;

		public PRIVILEGE_SET(uint control, params LUID_AND_ATTRIBUTES[] privileges)
		{
			PrivilegeCount = (uint)privileges.Length;
			Control = control;
			Privilege = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LUID_AND_ATTRIBUTES)) * (int)PrivilegeCount);
			for (int i = 0; i < PrivilegeCount; i++)
				Marshal.StructureToPtr(privileges[i], (IntPtr)((int)Privilege + Marshal.SizeOf(typeof(LUID_AND_ATTRIBUTES)) * i), false);
		}

		public void Dispose() => Marshal.FreeHGlobal(Privilege);
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SID_AND_ATTRIBUTES
	{
		public IntPtr Sid;
		public uint Attributes;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TOKEN_ELEVATION
	{
		public int TokenIsElevated;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TOKEN_GROUPS(uint count = 0)
	{
		public uint GroupCount = count;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		public SID_AND_ATTRIBUTES[] Groups = new SID_AND_ATTRIBUTES[count];
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TOKEN_MANDATORY_LABEL
	{
		public SID_AND_ATTRIBUTES Label;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TOKEN_PRIVILEGES
	{
		public uint PrivilegeCount;
		public LUID_AND_ATTRIBUTES Privileges;

		public TOKEN_PRIVILEGES(LUID luid, PrivilegeAttributes attribute)
		{
			PrivilegeCount = 1;
			Privileges.Luid = luid;
			Privileges.Attributes = attribute;
		}

		public static uint SizeInBytes => (uint)Marshal.SizeOf(typeof(TOKEN_PRIVILEGES));
	}

	public partial class SafeTokenHandle
	{
		private const int ERROR_INSUFFICIENT_BUFFER = 122;
		private const int ERROR_NO_TOKEN = 0x000003F0;
		private static SafeTokenHandle currentProcessToken = null;

		public static SafeTokenHandle FromCurrentProcess(AccessTypes desiredAccess = AccessTypes.TokenDuplicate)
		{
			lock (currentProcessToken)
			{
				currentProcessToken ??= FromProcess(GetCurrentProcess(), desiredAccess);
				return currentProcessToken;
			}
		}

		public static SafeTokenHandle FromCurrentThread(AccessTypes desiredAccess = AccessTypes.TokenDuplicate, bool openAsSelf = true) => FromThread(GetCurrentThread(), desiredAccess, openAsSelf);

		public static SafeTokenHandle FromProcess(IntPtr hProcess, AccessTypes desiredAccess = AccessTypes.TokenDuplicate)
		{
			return !OpenProcessToken(hProcess, desiredAccess, out SafeTokenHandle val) ? throw new System.ComponentModel.Win32Exception() : val;
		}

		public static SafeTokenHandle FromThread(IntPtr hThread, AccessTypes desiredAccess = AccessTypes.TokenDuplicate, bool openAsSelf = true)
		{
			if (!OpenThreadToken(hThread, desiredAccess, openAsSelf, out SafeTokenHandle val))
			{
				if (Marshal.GetLastWin32Error() == ERROR_NO_TOKEN)
				{
					SafeTokenHandle pval = FromCurrentProcess();
					if (!DuplicateTokenEx(pval, AccessTypes.TokenImpersonate | desiredAccess, IntPtr.Zero, SECURITY_IMPERSONATION_LEVEL.Impersonation, TokenType.TokenImpersonation, ref val))
						throw new System.ComponentModel.Win32Exception();
					if (!SetThreadToken(IntPtr.Zero, val))
						throw new System.ComponentModel.Win32Exception();
				}
				else
					throw new System.ComponentModel.Win32Exception();
			}
			return val;
		}

		public T GetInfo<T>(TOKEN_INFORMATION_CLASS type)
		{
			int cbSize = Marshal.SizeOf(typeof(T));
			IntPtr pType = Marshal.AllocHGlobal(cbSize);

			try
			{
				// Retrieve token information.
				if (!GetTokenInformation(this, type, pType, cbSize, out cbSize))
					throw new System.ComponentModel.Win32Exception();

				// Marshal from native to .NET.
				switch (type)
				{
					case TOKEN_INFORMATION_CLASS.TokenType:
					case TOKEN_INFORMATION_CLASS.TokenImpersonationLevel:
					case TOKEN_INFORMATION_CLASS.TokenSessionId:
					case TOKEN_INFORMATION_CLASS.TokenSandBoxInert:
					case TOKEN_INFORMATION_CLASS.TokenOrigin:
					case TOKEN_INFORMATION_CLASS.TokenElevationType:
					case TOKEN_INFORMATION_CLASS.TokenHasRestrictions:
					case TOKEN_INFORMATION_CLASS.TokenUIAccess:
					case TOKEN_INFORMATION_CLASS.TokenVirtualizationAllowed:
					case TOKEN_INFORMATION_CLASS.TokenVirtualizationEnabled:
						return (T)Convert.ChangeType(Marshal.ReadInt32(pType), typeof(T));

					case TOKEN_INFORMATION_CLASS.TokenLinkedToken:
						return (T)Convert.ChangeType(Marshal.ReadIntPtr(pType), typeof(T));

					case TOKEN_INFORMATION_CLASS.TokenUser:
					case TOKEN_INFORMATION_CLASS.TokenGroups:
					case TOKEN_INFORMATION_CLASS.TokenPrivileges:
					case TOKEN_INFORMATION_CLASS.TokenOwner:
					case TOKEN_INFORMATION_CLASS.TokenPrimaryGroup:
					case TOKEN_INFORMATION_CLASS.TokenDefaultDacl:
					case TOKEN_INFORMATION_CLASS.TokenSource:
					case TOKEN_INFORMATION_CLASS.TokenStatistics:
					case TOKEN_INFORMATION_CLASS.TokenRestrictedSids:
					case TOKEN_INFORMATION_CLASS.TokenGroupsAndPrivileges:
					case TOKEN_INFORMATION_CLASS.TokenElevation:
					case TOKEN_INFORMATION_CLASS.TokenAccessInformation:
					case TOKEN_INFORMATION_CLASS.TokenIntegrityLevel:
					case TOKEN_INFORMATION_CLASS.TokenMandatoryPolicy:
					case TOKEN_INFORMATION_CLASS.TokenLogonSid:
						return (T)Marshal.PtrToStructure(pType, typeof(T));

					case TOKEN_INFORMATION_CLASS.TokenSessionReference:
					case TOKEN_INFORMATION_CLASS.TokenAuditPolicy:
					default:
						return default;
				}
			}
			finally
			{
				Marshal.FreeHGlobal(pType);
			}
		}
	}
}