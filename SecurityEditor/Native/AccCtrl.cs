using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Microsoft.Win32
{
	internal static partial class NativeMethods
	{
		public enum MULTIPLE_TRUSTEE_OPERATION
		{
			NO_MULTIPLE_TRUSTEE,
			TRUSTEE_IS_IMPERSONATE
		}

		public enum TRUSTEE_FORM
		{
			TRUSTEE_IS_SID,
			TRUSTEE_IS_NAME,
			TRUSTEE_BAD_FORM,
			TRUSTEE_IS_OBJECTS_AND_SID,
			TRUSTEE_IS_OBJECTS_AND_NAME
		}

		public enum TRUSTEE_TYPE
		{
			TRUSTEE_IS_UNKNOWN,
			TRUSTEE_IS_USER,
			TRUSTEE_IS_GROUP,
			TRUSTEE_IS_DOMAIN,
			TRUSTEE_IS_ALIAS,
			TRUSTEE_IS_WELL_KNOWN_GROUP,
			TRUSTEE_IS_DELETED,
			TRUSTEE_IS_INVALID,
			TRUSTEE_IS_COMPUTER
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern void BuildTrusteeWithSid(ref TRUSTEE pTrustee, IntPtr sid);

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool ConvertSecurityDescriptorToStringSecurityDescriptor(IntPtr SecurityDescriptor, uint RequestedStringSDRevision,
			int SecurityInformation, out IntPtr StringSecurityDescriptor, out IntPtr StringSecurityDescriptorLen);

		[DllImport("advapi32.dll", PreserveSig = false)]
		public static extern void FreeInheritedFromArray(IntPtr pIA, ushort p, IntPtr intPtr);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetAce(IntPtr aclPtr, int aceIndex, out IntPtr acePtr);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool GetAclInformation(IntPtr pAcl, ref ACL_SIZE_INFORMATION pAclInformation, uint nAclInformationLength, uint dwAclInformationClass);

		[DllImport("advapi32.dll", PreserveSig = false)]
		public static extern void GetEffectiveRightsFromAcl(IntPtr pacl, ref TRUSTEE pTrustee, ref uint pAccessRights);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, PreserveSig = true)]
		public static extern int GetInheritanceSource([MarshalAs(UnmanagedType.LPWStr)] string objectName, ResourceType objectType, SecurityInfos securityInfo,
			[MarshalAs(UnmanagedType.Bool)] bool container, IntPtr pObjectClassGuids, uint guidCount, IntPtr pAcl, IntPtr pfnArray, ref Community.Security.AccessControl.GenericMapping pGenericMapping,
			IntPtr pInheritArray);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetPrivateObjectSecurity(IntPtr ObjectDescriptor, int SecurityInformation, IntPtr ResultantDescriptor, uint DescriptorLength, ref uint ReturnLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetSecurityDescriptorDacl(IntPtr pSecurityDescriptor, [MarshalAs(UnmanagedType.Bool)] out bool bDaclPresent,
			ref IntPtr pDacl, [MarshalAs(UnmanagedType.Bool)] out bool bDaclDefaulted);

		[DllImport("advapi32.dll")]
		public static extern void MapGenericMask(ref uint Mask, ref Community.Security.AccessControl.GenericMapping map);

		[StructLayout(LayoutKind.Sequential)]
		public struct ACCESS_ALLOWED_ACE
		{
			public ACE_HEADER Header;
			public int Mask;
			public int SidStart;

			public override bool Equals(object obj)
			{
				if (obj is ACCESS_ALLOWED_ACE)
				{
					ACCESS_ALLOWED_ACE that = (ACCESS_ALLOWED_ACE)obj;
					return (this.Header.AceType == that.Header.AceType && this.Header.AceFlags == that.Header.AceFlags && this.Mask == that.Mask);
				}
				return base.Equals(obj);
			}

			public override int GetHashCode()
			{
				return new { A = Header.AceFlags, B = Header.AceType, C = Mask }.GetHashCode();
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ACE_HEADER
		{
			public byte AceType;
			public byte AceFlags;
			public short AceSize;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ACL_SIZE_INFORMATION
		{
			public uint AceCount;
			public uint AclBytesInUse;
			public uint AclBytesFree;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 0)] //Platform independent (32 & 64 bit) - use Pack = 0 for both platforms. IntPtr works as well.
		public struct TRUSTEE : IDisposable
		{
			public IntPtr pMultipleTrustee;
			public MULTIPLE_TRUSTEE_OPERATION MultipleTrusteeOperation;
			public TRUSTEE_FORM TrusteeForm;
			public TRUSTEE_TYPE TrusteeType;
			public IntPtr ptstrName;

			void IDisposable.Dispose()
			{
				if (ptstrName != IntPtr.Zero) Marshal.Release(ptstrName);
			}

			public string Name { get { return Marshal.PtrToStringAuto(ptstrName); } }
		}
	}
}
