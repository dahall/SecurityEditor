﻿using Community.Security.AccessControl;
using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Microsoft.Win32;

internal static partial class NativeMethods
{
	private static readonly MarshaledMem EmptyGuidMem = new(Guid.Empty.ToByteArray());
	public static IntPtr EmptyGuidPtr => EmptyGuidMem.Ptr;

	public static ACCESS_ALLOWED_ACE GetAce(IntPtr pAcl, int aceIndex)
	{
		return GetAce(pAcl, aceIndex, out IntPtr acePtr)
			? (ACCESS_ALLOWED_ACE)Marshal.PtrToStructure(acePtr, typeof(ACCESS_ALLOWED_ACE))
			: throw new System.ComponentModel.Win32Exception();
	}

	public static uint GetAceCount(IntPtr pAcl) => GetAclInfo(pAcl).AceCount;

	public static ACL_SIZE_INFORMATION GetAclInfo(IntPtr pAcl)
	{
		ACL_SIZE_INFORMATION si = new();
		return !GetAclInformation(pAcl, ref si, (uint)Marshal.SizeOf(si), 2) ? throw new System.ComponentModel.Win32Exception() : si;
	}

	public static uint GetAclSize(IntPtr pAcl) => GetAclInfo(pAcl).AclBytesInUse;

	public static uint GetEffectiveRights(IntPtr pSid, IntPtr pSD)
	{
		TRUSTEE t = new();
		BuildTrusteeWithSid(ref t, pSid);

		IntPtr pDacl = IntPtr.Zero;
		GetSecurityDescriptorDacl(pSD, out bool daclPresent, ref pDacl, out bool daclDefaulted);

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
		if (ConvertSecurityDescriptorToStringSecurityDescriptor(pSD, 1, si, out IntPtr ssd, out IntPtr ssdLen))
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

		public byte[] Array { get; }

		public IntPtr Ptr { get; }

		public void Dispose() => Marshal.FreeHGlobal(Ptr);
	}
}