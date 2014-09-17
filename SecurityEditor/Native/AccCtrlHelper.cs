using Community.Security.AccessControl;
using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Microsoft.Win32
{
	internal static partial class NativeMethods
	{
		private static MarshaledMem EmptyGuidMem = new MarshaledMem(Guid.Empty.ToByteArray());
		public static IntPtr EmptyGuidPtr { get { return EmptyGuidMem.Ptr; } }

		public static ACCESS_ALLOWED_ACE GetAce(IntPtr pAcl, int aceIndex)
		{
			IntPtr acePtr;
			if (GetAce(pAcl, aceIndex, out acePtr))
				return (ACCESS_ALLOWED_ACE)Marshal.PtrToStructure(acePtr, typeof(ACCESS_ALLOWED_ACE));
			throw new System.ComponentModel.Win32Exception();
		}

		public static uint GetAceCount(IntPtr pAcl)
		{
			return GetAclInfo(pAcl).AceCount;
		}

		public static ACL_SIZE_INFORMATION GetAclInfo(IntPtr pAcl)
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
			SecurityInfosEx securityInfo, bool container, IntPtr pAcl, ref GenericMapping pGenericMapping)
		{
			int objSize = Marshal.SizeOf(typeof(InheritedFromInfo));
			var aceCount = GetAceCount(pAcl);
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
			if (ConvertSecurityDescriptorToStringSecurityDescriptor(pSD, 1, si, out ssd, out ssdLen))
			{
				string s = Marshal.PtrToStringAuto(ssd);
				Marshal.FreeHGlobal(ssd);
				return s;
			}
			return null;
		}

		public class MarshaledMem : IDisposable
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
}
