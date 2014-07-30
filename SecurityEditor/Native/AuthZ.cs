using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Win32
{
	internal static partial class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct AUTHZ_SECURITY_ATTRIBUTES_INFORMATION
		{
			public ushort Version;
			public ushort Reserved;
			public uint AttributeCount;
			public IntPtr pAttributeV1;
		}

		public enum AuthzSecurityAttributeValueType : ushort
		{
			Invalid = 0x0,
			Int = 0x1,
			String = 0x3,
			Boolean = 0x6,
		}

		[Flags]
		public enum AuthzSecurityAttributeFlags : uint // ULONG
		{
			None = 0x0,
			NonInheritable = 0x1,
			ValueCaseSensitive = 0x2,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct AUTHZ_SECURITY_ATTRIBUTE_V1
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string Name;
			public AuthzSecurityAttributeValueType Type;
			public ushort Reserved;
			public AuthzSecurityAttributeFlags Flags;
			public uint ValueCount;
			public IntPtr Values;
		}

		public enum AuthzContextInformationClass : uint
		{
			AuthzContextInfoUserClaims = 13,
			AuthzContextInfoDeviceClaims,
		};

		public enum AuthzSecurityAttributeOperation : uint
		{
			None = 0,
			ReplaceAll,
			Add,
			Delete,
			Replace
		}

		public enum AuthzSidOperation : uint
		{
			None = 0,
			ReplaceAll,
			Add,
			Delete,
			Replace
		}

		[DllImport("Authz.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AuthzModifyClaims(
			[In] IntPtr handleClientContext,
			[In] AuthzContextInformationClass infoClass,
			[In] AuthzSecurityAttributeOperation[] claimOperation,
			[In, Optional] ref AUTHZ_SECURITY_ATTRIBUTES_INFORMATION claims);
	}
}
