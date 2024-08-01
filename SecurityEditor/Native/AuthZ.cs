#nullable enable
using Community.Security.AccessControl;
using System;
using System.Runtime.InteropServices;
using System.Security;
using static Microsoft.Win32.NativeMethods;

namespace Microsoft.Win32;

internal static partial class NativeMethods
{
	private const string Authz = "authz.dll";

	[UnmanagedFunctionPointer(CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Unicode)]
	[SuppressUnmanagedCodeSecurity]
	[return: MarshalAs(UnmanagedType.Bool)]
	public delegate bool AuthzAccessCheckCallback(IntPtr hAuthzClientContext, IntPtr pAce, IntPtr pArgs, [MarshalAs(UnmanagedType.Bool)] ref bool pbAceApplicable);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public delegate bool AuthzComputeGroupsCallback(IntPtr hAuthzClientContext, IntPtr pArgs, out IntPtr pSidAttrArray, out uint pSidCount, out IntPtr pRestrictedSidAttrArray, out uint pRestrictedSidCount);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Unicode)]
	[SuppressUnmanagedCodeSecurity]
	public delegate void AuthzFreeGroupsCallback(IntPtr pSidAttrArray);

	[Flags]
	public enum AuthzAccessCheckFlags
	{
		/// <summary>
		/// If phAccessCheckResults is not NULL, a deep copy of the security descriptor is copied to the handle referenced by phAccessCheckResults.
		/// </summary>
		NONE = 0,

		/// <summary>
		/// A deep copy of the security descriptor is not performed. The calling application must pass the address of an
		/// AUTHZ_ACCESS_CHECK_RESULTS_HANDLE handle in phAccessCheckResults. The AuthzAccessCheck function sets this handle to a
		/// security descriptor that must remain valid during subsequent calls to AuthzCachedAccessCheck.
		/// </summary>
		AUTHZ_ACCESS_CHECK_NO_DEEP_COPY_SD = 0x00000001,
	}

	[Flags]
	public enum AuthzContextFlags
	{
		/// <summary>
		/// Default value.
		/// <para>AuthzInitializeContextFromSid attempts to retrieve the user's token group information by performing an S4U logon.</para>
		/// <para>
		/// If S4U logon is not supported by the user's domain or the calling computer, AuthzInitializeContextFromSid queries the user's
		/// account object for group information. When an account is queried directly, some groups that represent logon characteristics,
		/// such as Network, Interactive, Anonymous, Network Service, or Local Service, are omitted. Applications can explicitly add such
		/// group SIDs by implementing the AuthzComputeGroupsCallback function or calling the AuthzAddSidsToContext function.
		/// </para>
		/// </summary>
		DEFAULT = 0,

		/// <summary>
		/// Causes AuthzInitializeContextFromSid to skip all group evaluations. When this flag is used, the context returned contains
		/// only the SID specified by the UserSid parameter. The specified SID can be an arbitrary or application-specific SID. Other
		/// SIDs can be added to this context by implementing the AuthzComputeGroupsCallback function or by calling the
		/// AuthzAddSidsToContext function.
		/// </summary>
		AUTHZ_SKIP_TOKEN_GROUPS = 0x2,

		/// <summary>
		/// Causes AuthzInitializeContextFromSid to fail if Windows Services For User is not available to retrieve token group information.
		/// <para><c>Windows XP:</c> This flag is not supported.</para>
		/// </summary>
		AUTHZ_REQUIRE_S4U_LOGON = 0x4,

		/// <summary>
		/// Causes AuthzInitializeContextFromSid to retrieve privileges for the new context. If this function performs an S4U logon, it
		/// retrieves privileges from the token. Otherwise, the function retrieves privileges from all SIDs in the context.
		/// </summary>
		AUTHZ_COMPUTE_PRIVILEGES = 0x8
	}

	public enum AuthzContextInformationClass : uint
	{
		AuthzContextInfoGroupsSids = 2,
		AuthzContextInfoDeviceSids = 12,
		AuthzContextInfoUserClaims,
		AuthzContextInfoDeviceClaims,
	};

	[Flags]
	public enum AuthzResourceManagerFlags
	{
		/// <summary>
		/// Default call to the function. The resource manager is initialized as the principal identified in the process token, and
		/// auditing is in effect. Note that unless the AUTHZ_RM_FLAG_NO_AUDIT flag is set, SeAuditPrivilege must be enabled for the
		/// function to succeed.
		/// </summary>
		DEFAULT = 0,

		/// <summary>
		/// Auditing is not in effect. If this flag is set, the caller does not need to have SeAuditPrivilege enabled to call this function.
		/// </summary>
		AUTHZ_RM_FLAG_NO_AUDIT = 0x1,

		/// <summary>The resource manager is initialized as the identity of the thread token.</summary>
		AUTHZ_RM_FLAG_INITIALIZE_UNDER_IMPERSONATION = 0x2,

		/// <summary>The resource manager ignores CAP IDs and does not evaluate centralized access policies.</summary>
		AUTHZ_RM_FLAG_NO_CENTRALIZED_ACCESS_POLICIES = 0x4
	}

	[Flags]
	public enum AuthzSecurityAttributeFlags : uint // ULONG
	{
		None = 0x0,
		NonInheritable = 0x1,
		ValueCaseSensitive = 0x2,
	}

	public enum AuthzSecurityAttributeOperation : uint
	{
		None = 0,
		ReplaceAll,
		Add,
		Delete,
		Replace
	}

	public enum AuthzSecurityAttributeValueType : ushort
	{
		Invalid = 0x0,
		Int = 0x1,
		String = 0x3,
		Boolean = 0x6,
	}

	public enum AuthzSidOperation : uint
	{
		None = 0,
		ReplaceAll,
		Add,
		Delete,
		Replace
	}

	[DllImport(Authz, SetLastError = true, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AuthzAccessCheck(AuthzAccessCheckFlags Flags, IntPtr hAuthzClientContext, in AUTHZ_ACCESS_REQUEST pRequest,
		[Optional] IntPtr hAuditEvent, IntPtr pSecurityDescriptor,
		[Optional, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] IntPtr OptionalSecurityDescriptorArray,
		uint OptionalSecurityDescriptorCount, [In, Out] AUTHZ_ACCESS_REPLY pReply, out SafeAUTHZ_ACCESS_CHECK_RESULTS_HANDLE phAccessCheckResults);

	[DllImport(Authz, SetLastError = true, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AuthzFreeContext(IntPtr hAuthzClientContext);

	[DllImport(Authz, SetLastError = true, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AuthzFreeHandle(IntPtr hAccessCheckResults);

	[DllImport(Authz, SetLastError = true, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AuthzFreeResourceManager(IntPtr hAuthzResourceManager);

	[DllImport(Authz, SetLastError = true, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AuthzInitializeCompoundContext(IntPtr UserContext, IntPtr DeviceContext, out SafeAUTHZ_CLIENT_CONTEXT_HANDLE phCompoundContext);

	[DllImport(Authz, SetLastError = true, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AuthzInitializeContextFromSid(
		AuthzContextFlags Flags,
		IntPtr UserSid,
		[Optional] IntPtr hAuthzResourceManager,
		[Optional] IntPtr pExpirationTime,
		LUID Identifier,
		[Optional] IntPtr DynamicGroupArgs,
		out SafeAUTHZ_CLIENT_CONTEXT_HANDLE phAuthzClientContext);

	[DllImport(Authz, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AuthzInitializeResourceManager(AuthzResourceManagerFlags flags, [Optional] AuthzAccessCheckCallback? pfnAccessCheck,
		[Optional] AuthzComputeGroupsCallback? pfnComputeDynamicGroups, [Optional] AuthzFreeGroupsCallback? pfnFreeDynamicGroups,
		[Optional, MarshalAs(UnmanagedType.LPWStr)] string? name, out SafeAUTHZ_RESOURCE_MANAGER_HANDLE rm);

	[DllImport(Authz, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AuthzModifyClaims(
		[In] IntPtr handleClientContext,
		[In] AuthzContextInformationClass infoClass,
		[In] AuthzSecurityAttributeOperation[] claimOperation,
		[In, Optional] AUTHZ_SECURITY_ATTRIBUTES_INFORMATION? claims);

	[DllImport(Authz, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AuthzModifySids(IntPtr hAuthzClientContext, AuthzContextInformationClass SidClass,
		[MarshalAs(UnmanagedType.LPArray)] AuthzSidOperation[] pSidOperations, in TOKEN_GROUPS pSids);

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct AUTHZ_ACCESS_REQUEST
	{
		/// <summary>The type of access to test for.</summary>
		public uint DesiredAccess;

		/// <summary>The security identifier (SID) to use for the principal self SID in the access control list (ACL).</summary>
		public IntPtr PrincipalSelfSid;

		/// <summary>
		/// An array of OBJECT_TYPE_LIST structures in the object tree for the object. Set to NULL unless the application checks access
		/// at the property level.
		/// </summary>
		public IntPtr ObjectTypeList;

		/// <summary>
		/// The number of elements in the ObjectTypeList array. This member is necessary only if the application checks access at the
		/// property level.
		/// </summary>
		public uint ObjectTypeListLength;

		/// <summary>A pointer to memory to pass to AuthzAccessCheckCallback when checking callback access control entries (ACEs).</summary>
		public IntPtr OptionalArguments;

		/// <summary>Initializes a new instance of the <see cref="AUTHZ_ACCESS_REQUEST"/> struct.</summary>
		/// <param name="access">The access.</param>
		public AUTHZ_ACCESS_REQUEST(uint access) : this() => DesiredAccess = access;

		/// <summary>Gets or sets the object types.</summary>
		/// <value>The object types.</value>
		//public ObjectTypeList[] ObjectTypes => ObjectTypeList.ToIEnum<IntPtr>((int)ObjectTypeListLength).Select(p => p.ToStructure<OBJECT_TYPE_LIST>()).ToArray();
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

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public sealed class AUTHZ_ACCESS_REPLY : IDisposable
	{
		/// <summary>
		/// The number of elements in the GrantedAccessMask, SaclEvaluationResults, and Error arrays. This number matches the number of
		/// entries in the object type list structure used in the access check. If no object type is used to represent the object, then
		/// set ResultListLength to one.
		/// </summary>
		public int ResultListLength;

		/// <summary>An array of granted access masks. Memory for this array is allocated by the application before calling AccessCheck.</summary>
		public IntPtr GrantedAccessMask;

		/// <summary>
		/// An array of system access control list (SACL) evaluation results. Memory for this array is allocated by the application
		/// before calling AccessCheck. SACL evaluation will only be performed if auditing is requested.
		/// </summary>
		public IntPtr SaclEvaluationResults;

		/// <summary>
		/// An array of results for each element of the array. Memory for this array is allocated by the application before calling AccessCheck.
		/// </summary>
		public IntPtr Error;

		/// <summary>Initializes a new instance of the <see cref="AUTHZ_ACCESS_REPLY"/> struct.</summary>
		/// <param name="count">The count of array members in each of the three arrays.</param>
		public AUTHZ_ACCESS_REPLY(uint count)
		{
			if (count == 0) return;
			ResultListLength = (int)count;
			var sz = Marshal.SizeOf(typeof(uint)) * (int)count;
			GrantedAccessMask = Marshal.AllocHGlobal(sz);
			SaclEvaluationResults = Marshal.AllocHGlobal(sz);
			Error = Marshal.AllocHGlobal(sz);
		}

		/// <summary>Gets or sets the granted access mask values. The length of this array must match the value in <see cref="ResultListLength"/>.</summary>
		/// <value>The granted access mask values.</value>
		public uint[] GrantedAccessMaskValues
		{
			get => GrantedAccessMask != IntPtr.Zero && ResultListLength > 0 ? GrantedAccessMask.ToArray<uint>(ResultListLength)! : new uint[0];
			set
			{
				if (value.Length != ResultListLength)
					throw new ArgumentOutOfRangeException(nameof(GrantedAccessMaskValues), $"Number of items must match value of {nameof(ResultListLength)} field.");
				Marshal.FreeHGlobal(GrantedAccessMask);
				if (ResultListLength == 0)
					GrantedAccessMask = IntPtr.Zero;
				else
					CopyArrayToPtr(value, GrantedAccessMask);
			}
		}

		private static void CopyArrayToPtr<T>(T[] items, IntPtr ptr) where T : unmanaged
		{
			ptr = Marshal.AllocHGlobal(sizeof(uint) * items.Length);
			unsafe
			{
				T* p = (T*)ptr;
				for (int i = 0; i < items.Length; i++)
					p[i] = items[i];
			}
		}

		/// <summary>
		/// Gets or sets the system access control list (SACL) evaluation results values. The length of this array must match the value
		/// in <see cref="ResultListLength"/>.
		/// </summary>
		/// <value>The system access control list (SACL) evaluation results values.</value>
		public uint[] SaclEvaluationResultsValues
		{
			get => SaclEvaluationResults != IntPtr.Zero && ResultListLength > 0
					? SaclEvaluationResults.ToArray<uint>(ResultListLength)!
					: new uint[0];
			set
			{
				if (value.Length != ResultListLength)
					throw new ArgumentOutOfRangeException(nameof(SaclEvaluationResultsValues), $"Number of items must match value of {nameof(ResultListLength)} field.");
				Marshal.FreeHGlobal(SaclEvaluationResults);
				if (ResultListLength == 0)
					SaclEvaluationResults = IntPtr.Zero;
				else
					CopyArrayToPtr(value, SaclEvaluationResults);
			}
		}

		/// <summary>Gets or sets the results for each element of the array. The length of this array must match the value in <see cref="ResultListLength"/>.</summary>
		/// <value>The results values.</value>
		public uint[] ErrorValues
		{
			get => Error != IntPtr.Zero && ResultListLength > 0 ? Error.ToArray<uint>(ResultListLength)! : new uint[0];
			set
			{
				if (value.Length != ResultListLength)
					throw new ArgumentOutOfRangeException(nameof(ErrorValues), $"Number of items must match value of {nameof(ResultListLength)} field.");
				Marshal.FreeHGlobal(Error);
				if (ResultListLength == 0)
					Error = IntPtr.Zero;
				else
					CopyArrayToPtr(value, Error);
			}
		}

		void IDisposable.Dispose()
		{
			Marshal.FreeHGlobal(GrantedAccessMask);
			Marshal.FreeHGlobal(SaclEvaluationResults);
			Marshal.FreeHGlobal(Error);
			ResultListLength = 0;
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public class AUTHZ_SECURITY_ATTRIBUTES_INFORMATION
	{
		public ushort Version;
		public ushort Reserved;
		public uint AttributeCount;
		public IntPtr pAttributeV1;
	}
	
	public class SafeAUTHZ_ACCESS_CHECK_RESULTS_HANDLE : SafeHandle
	{
		public SafeAUTHZ_ACCESS_CHECK_RESULTS_HANDLE(IntPtr preexistingHandle, bool ownsHandle = true) : base(preexistingHandle, ownsHandle) { }

		private SafeAUTHZ_ACCESS_CHECK_RESULTS_HANDLE() : base(default, false) { }

		public override bool IsInvalid => handle == default;

		public static implicit operator IntPtr(SafeAUTHZ_ACCESS_CHECK_RESULTS_HANDLE h) => h.handle;
		protected override bool ReleaseHandle() => AuthzFreeHandle(handle);
	}

	public class SafeAUTHZ_CLIENT_CONTEXT_HANDLE : SafeHandle
	{
		public SafeAUTHZ_CLIENT_CONTEXT_HANDLE(IntPtr preexistingHandle, bool ownsHandle = true) : base(preexistingHandle, ownsHandle) { }

		private SafeAUTHZ_CLIENT_CONTEXT_HANDLE() : base(default, false) { }

		public override bool IsInvalid => handle == default;

		public static implicit operator IntPtr(SafeAUTHZ_CLIENT_CONTEXT_HANDLE h) => h.handle;
		protected override bool ReleaseHandle() => AuthzFreeContext(handle);
	}

	public class SafeAUTHZ_RESOURCE_MANAGER_HANDLE : SafeHandle
	{
		public SafeAUTHZ_RESOURCE_MANAGER_HANDLE(IntPtr preexistingHandle, bool ownsHandle = true) : base(preexistingHandle, ownsHandle) { }

		private SafeAUTHZ_RESOURCE_MANAGER_HANDLE() : base(default, false) { }

		public override bool IsInvalid => handle == default;

		public static implicit operator IntPtr(SafeAUTHZ_RESOURCE_MANAGER_HANDLE h) => h.handle;
		protected override bool ReleaseHandle() => AuthzFreeResourceManager(handle);
	}
}

internal static class Ext
{
	internal static T[]? ToArray<T>(this IntPtr p, int len) where T : unmanaged
	{
		if (p == IntPtr.Zero) return null;
		T[] ret = new T[len];
		unsafe
		{
			T* pt = (T*)p;
			for (int i = 0; i < len; i++)
				ret[i] = pt[i];
		}
		return ret;
	}
}
