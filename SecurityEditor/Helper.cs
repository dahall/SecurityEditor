using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Community.Security.AccessControl
{
	internal static class Helper
	{
		private static MarshaledMem EmptyGuidMem = new MarshaledMem(Guid.Empty.ToByteArray());

		private enum MULTIPLE_TRUSTEE_OPERATION
		{
			NO_MULTIPLE_TRUSTEE,
			TRUSTEE_IS_IMPERSONATE
		}

		private enum TRUSTEE_FORM
		{
			TRUSTEE_IS_SID,
			TRUSTEE_IS_NAME,
			TRUSTEE_BAD_FORM,
			TRUSTEE_IS_OBJECTS_AND_SID,
			TRUSTEE_IS_OBJECTS_AND_NAME
		}

		private enum TRUSTEE_TYPE
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

		public static IntPtr EmptyGuidPtr { get { return EmptyGuidMem.Ptr; } }

		[DllImport("advapi32.dll", PreserveSig = false)]
		public static extern void FreeInheritedFromArray(IntPtr pIA, ushort p, IntPtr intPtr);

		public static Int16 GetAceCount(IntPtr pAcl)
		{
			ACL _dacl = (ACL)Marshal.PtrToStructure(pAcl, typeof(ACL));
			return _dacl.AceCount;
		}

		public static uint GetEffectiveRights(IntPtr pSid, IntPtr pSD)
		{
			TRUSTEE t = new TRUSTEE();
			BuildTrusteeWithSid(ref t, pSid);

			bool daclPresent, daclDefaulted;
			IntPtr pDacl = IntPtr.Zero;
			GetSecurityDescriptorDacl(pSD, out daclPresent, ref pDacl, out daclDefaulted);

			uint access = 0;
			GetEffectiveRightsFromAcl(pDacl, ref t, ref access);

			return access;
		}

		public static string SecurityDescriptorPtrToSDLL(IntPtr pSD, int si)
		{
			IntPtr ssd, ssdLen;
			if (Helper.ConvertSecurityDescriptorToStringSecurityDescriptor(pSD, 1, si, out ssd, out ssdLen))
				return Marshal.PtrToStringAuto(ssd);
			return null;
		}

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool ConvertSecurityDescriptorToStringSecurityDescriptor(IntPtr SecurityDescriptor, uint RequestedStringSDRevision,
			int SecurityInformation, out IntPtr StringSecurityDescriptor, out IntPtr StringSecurityDescriptorLen);

		[DllImport("aclui.dll")]
		public static extern bool EditSecurity(IntPtr hwnd, ISecurityInformation psi);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, PreserveSig = false)]
		public static extern void GetInheritanceSource([MarshalAs(UnmanagedType.LPWStr)] string objectName, ResourceType objectType, SecurityInfos securityInfo,
			bool container, IntPtr pObjectClassGuids, uint guidCount, IntPtr pAcl, IntPtr pfnArray, ref GenericRightsMapping pGenericMapping, IntPtr pInheritArray);

		[DllImport("advapi32.dll")]
		public static extern void MapGenericMask(ref uint Mask, ref GenericRightsMapping map);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern void BuildTrusteeWithSid(ref TRUSTEE pTrustee, IntPtr sid);

		[DllImport("advapi32.dll", PreserveSig = false)]
		private static extern void GetEffectiveRightsFromAcl(IntPtr pacl, ref TRUSTEE pTrustee, ref uint pAccessRights);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetSecurityDescriptorDacl(IntPtr pSecurityDescriptor, [MarshalAs(UnmanagedType.Bool)] out bool bDaclPresent,
			ref IntPtr pDacl, [MarshalAs(UnmanagedType.Bool)] out bool bDaclDefaulted);

		[StructLayoutAttribute(LayoutKind.Sequential)]
		private struct ACL
		{
			public Byte AclRevision;
			public Byte Sbz1;
			public Int16 AclSize;
			public Int16 AceCount;
			public Int16 Sbz2;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 0)] //Platform independent (32 & 64 bit) - use Pack = 0 for both platforms. IntPtr works as well.
		private struct TRUSTEE : IDisposable
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

	internal class MarshaledMem : IDisposable
	{
		public MarshaledMem(byte[] array)
		{
			Array = array;
			int size = Marshal.SizeOf(typeof(byte)) * array.Length;
			Ptr = Marshal.AllocHGlobal(size);
			Marshal.Copy(array, 0, Ptr, array.Length);
		}

		public byte[] Array { get; private set; }

		public IntPtr Ptr { get; private set; }

		public void Dispose()
		{
			Marshal.FreeHGlobal(Ptr);
		}
	}
}
