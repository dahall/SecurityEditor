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

		public static uint GetAceCount(IntPtr pAcl)
		{
			return GetAclInfo(pAcl).AceCount;
		}

		public static ACCESS_ALLOWED_ACE GetAce(IntPtr pAcl, int aceIndex)
		{
			IntPtr acePtr;
			if (GetAce(pAcl, aceIndex, out acePtr))
				return (ACCESS_ALLOWED_ACE)Marshal.PtrToStructure(acePtr, typeof(ACCESS_ALLOWED_ACE));
			throw new System.ComponentModel.Win32Exception();
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

		public static InheritedFromInfo[] GetInheritanceSource(string objectName, ResourceType objectType,
			SecurityInfos securityInfo, bool container, IntPtr pAcl, ref GenericRightsMapping pGenericMapping)
		{
			int objSize = Marshal.SizeOf(typeof(InheritedFromInfo));
			var aceCount = Helper.GetAceCount(pAcl);
			var ret = new InheritedFromInfo[aceCount];
			IntPtr pInherit = Marshal.AllocHGlobal(objSize * (int)aceCount);
			try
			{
				int hr = GetInheritanceSource(objectName, objectType, securityInfo, container, IntPtr.Zero, 0, pAcl, IntPtr.Zero, ref pGenericMapping, pInherit);
				if (hr != 0)
					throw new System.ComponentModel.Win32Exception(hr);
				IntPtr pInheritTmp = pInherit;
				for (int i = 0; i < aceCount; i++)
				{
					ret[i] = (InheritedFromInfo)Marshal.PtrToStructure(pInheritTmp, typeof(InheritedFromInfo));
					System.Diagnostics.Debug.WriteLine(":  " + ret[i].ToString());
					pInheritTmp = new IntPtr(pInheritTmp.ToInt32() + objSize);
				}
			}
			catch { }
			finally
			{
				FreeInheritedFromArray(pInherit, (ushort)aceCount, IntPtr.Zero);
				Marshal.FreeHGlobal(pInherit);
			}
			return ret;
		}

		public static IntPtr GetPrivateObjectSecurity(IntPtr pSD, int si)
		{
			IntPtr pResSD = IntPtr.Zero;
			uint ret = 0;
			GetPrivateObjectSecurity(pSD, si, IntPtr.Zero, 0, ref ret);
			if (ret > 0)
			{
				pResSD = Marshal.AllocHGlobal((int)ret);
				if (pResSD != IntPtr.Zero && !GetPrivateObjectSecurity(pSD, si, pResSD, ret, ref ret))
				{
					Marshal.FreeHGlobal(pResSD);
					pResSD = IntPtr.Zero;
					Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
				}
			}
			return pResSD;
		}

		private static ACL_SIZE_INFORMATION GetAclInfo(IntPtr pAcl)
		{
			ACL_SIZE_INFORMATION si = new ACL_SIZE_INFORMATION();
			if (!GetAclInformation(pAcl, ref si, (uint)Marshal.SizeOf(si), 2))
				throw new System.ComponentModel.Win32Exception();
			return si;
		}

		public static uint GetAclSize(IntPtr pAcl)
		{
			return GetAclInfo(pAcl).AclBytesInUse;
		}

		public static RawAcl RawAclFromPtr(IntPtr pAcl)
		{
			uint len = GetAclSize(pAcl);
			byte[] dest = new byte[len];
			Marshal.Copy(pAcl, dest, 0, (int)len);
			return new RawAcl(dest, 0);
		}

		public static string SecurityDescriptorPtrToSDLL(IntPtr pSD, int si)
		{
			IntPtr ssd, ssdLen;
			if (Helper.ConvertSecurityDescriptorToStringSecurityDescriptor(pSD, 1, si, out ssd, out ssdLen))
			{
				string s = Marshal.PtrToStringAuto(ssd);
				Marshal.FreeHGlobal(ssd);
				return s;
			}
			return null;
		}

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern bool ConvertSecurityDescriptorToStringSecurityDescriptor(IntPtr SecurityDescriptor, uint RequestedStringSDRevision,
			int SecurityInformation, out IntPtr StringSecurityDescriptor, out IntPtr StringSecurityDescriptorLen);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool GetAclInformation(IntPtr pAcl, ref ACL_SIZE_INFORMATION pAclInformation, uint nAclInformationLength, uint dwAclInformationClass);

		[DllImport("aclui.dll")]
		[return:MarshalAs(UnmanagedType.Bool)]
		public static extern bool EditSecurity(IntPtr hwnd, ISecurityInformation psi);

		[DllImport("aclui.dll", PreserveSig = false)]
		public static extern void EditSecurityAdvanced(IntPtr hwnd, ISecurityInformation psi, SecurityPageType pageType);

		[DllImport("advapi32.dll", PreserveSig = false)]
		private static extern void FreeInheritedFromArray(IntPtr pIA, ushort p, IntPtr intPtr);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, PreserveSig = true)]
		private static extern int GetInheritanceSource([MarshalAs(UnmanagedType.LPWStr)] string objectName, ResourceType objectType, SecurityInfos securityInfo,
			[MarshalAs(UnmanagedType.Bool)] bool container, IntPtr pObjectClassGuids, uint guidCount, IntPtr pAcl, IntPtr pfnArray, ref GenericRightsMapping pGenericMapping,
			IntPtr pInheritArray);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetPrivateObjectSecurity(IntPtr ObjectDescriptor, int SecurityInformation, IntPtr ResultantDescriptor, uint DescriptorLength, ref uint ReturnLength);

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

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetAce(IntPtr aclPtr, int aceIndex, out IntPtr acePtr);

		[StructLayout(LayoutKind.Sequential)]
		public struct ACE_HEADER
		{
			public byte AceType;
			public byte AceFlags;
			public short AceSize;
		}

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
		private struct ACL_SIZE_INFORMATION
		{
			public uint AceCount;
			public uint AclBytesInUse;
			public uint AclBytesFree;
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
