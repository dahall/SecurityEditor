using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace Community.Security.AccessControl
{
	/// <summary>Privilege determining the type of system operations that can be performed.</summary>
	internal enum Privilege
	{
		/// <summary>Privilege to replace a process-level token.</summary>
		AssignPrimaryToken,

		/// <summary>Privilege to generate security audits.</summary>
		Audit,

		/// <summary>Privilege to backup files and directories.</summary>
		Backup,

		/// <summary>Privilege to bypass traverse checking.</summary>
		ChangeNotify,

		/// <summary>Privilege to create global objects.</summary>
		CreateGlobal,

		/// <summary>Privilege to create a pagefile.</summary>
		CreatePageFile,

		/// <summary>Privilege to create permanent shared objects.</summary>
		CreatePermanent,

		/// <summary>Privilege to create symbolic links.</summary>
		CreateSymbolicLink,

		/// <summary>Privilege to create a token object.</summary>
		CreateToken,

		/// <summary>Privilege to debug programs.</summary>
		Debug,

		/// <summary>Privilege to enable computer and user accounts to be trusted for delegation.</summary>
		EnableDelegation,

		/// <summary>Privilege to impersonate a client after authentication.</summary>
		Impersonate,

		/// <summary>Privilege to increase scheduling priority.</summary>
		IncreaseBasePriority,

		/// <summary>Privilege to adjust memory quotas for a process.</summary>
		IncreaseQuota,

		/// <summary>Privilege to increase a process working set.</summary>
		IncreaseWorkingSet,

		/// <summary>Privilege to load and unload device drivers.</summary>
		LoadDriver,

		/// <summary>Privilege to lock pages in memory.</summary>
		LockMemory,

		/// <summary>Privilege to add workstations to domain.</summary>
		MachineAccount,

		/// <summary>Privilege to manage the files on a volume.</summary>
		ManageVolume,

		/// <summary>Privilege to profile single process.</summary>
		ProfileSingleProcess,

		/// <summary>Privilege to modify an object label.</summary>
		Relabel,

		/// <summary>Privilege to force shutdown from a remote system.</summary>
		RemoteShutdown,

		/// <summary>Privilege to restore files and directories.</summary>
		Restore,

		/// <summary>Privilege to manage auditing and security log.</summary>
		Security,

		/// <summary>Privilege to shut down the system.</summary>
		Shutdown,

		/// <summary>Privilege to synchronize directory service data.</summary>
		SyncAgent,

		/// <summary>Privilege to modify firmware environment values.</summary>
		SystemEnvironment,

		/// <summary>Privilege to profile system performance.</summary>
		SystemProfile,

		/// <summary>Privilege to change the system time.</summary>
		SystemTime,

		/// <summary>Privilege to take ownership of files or other objects.</summary>
		TakeOwnership,

		/// <summary>Privilege to act as part of the operating system.</summary>
		TrustedComputerBase,

		/// <summary>Privilege to change the time zone.</summary>
		TimeZone,

		/// <summary>Privilege to access Credential Manager as a trusted caller.</summary>
		TrustedCredentialManagerAccess,

		/// <summary>Privilege to remove computer from docking station.</summary>
		Undock,

		/// <summary>Privilege to read unsolicited input from a terminal device.</summary>
		UnsolicitedInput
	}

	/// <summary>
	/// Manage user privileges
	/// </summary>
	internal sealed class PrivilegedCodeBlock : IDisposable
	{
		private static readonly Dictionary<Privilege, NativeMethods.LUID> luidLookup =
			new Dictionary<Privilege, NativeMethods.LUID>();

		private static readonly Dictionary<Privilege, string> privLookup =
			new Dictionary<Privilege, string>(35)
		{
			{ Privilege.AssignPrimaryToken, "SeAssignPrimaryTokenPrivilege" },
			{ Privilege.Audit, "SeAuditPrivilege" },
			{ Privilege.Backup, "SeBackupPrivilege" },
			{ Privilege.ChangeNotify, "SeChangeNotifyPrivilege" },
			{ Privilege.CreateGlobal, "SeCreateGlobalPrivilege" },
			{ Privilege.CreatePageFile, "SeCreatePagefilePrivilege" },
			{ Privilege.CreatePermanent, "SeCreatePermanentPrivilege" },
			{ Privilege.CreateSymbolicLink, "SeCreateSymbolicLinkPrivilege" },
			{ Privilege.CreateToken, "SeCreateTokenPrivilege" },
			{ Privilege.Debug, "SeDebugPrivilege" },
			{ Privilege.EnableDelegation, "SeEnableDelegationPrivilege" },
			{ Privilege.Impersonate, "SeImpersonatePrivilege" },
			{ Privilege.IncreaseBasePriority, "SeIncreaseBasePriorityPrivilege" },
			{ Privilege.IncreaseQuota, "SeIncreaseQuotaPrivilege" },
			{ Privilege.IncreaseWorkingSet, "SeIncreaseWorkingSetPrivilege" },
			{ Privilege.LoadDriver, "SeLoadDriverPrivilege" },
			{ Privilege.LockMemory, "SeLockMemoryPrivilege" },
			{ Privilege.MachineAccount, "SeMachineAccountPrivilege" },
			{ Privilege.ManageVolume, "SeManageVolumePrivilege" },
			{ Privilege.ProfileSingleProcess, "SeProfileSingleProcessPrivilege" },
			{ Privilege.Relabel, "SeRelabelPrivilege" },
			{ Privilege.RemoteShutdown, "SeRemoteShutdownPrivilege" },
			{ Privilege.Restore, "SeRestorePrivilege" },
			{ Privilege.Security, "SeSecurityPrivilege" },
			{ Privilege.Shutdown, "SeShutdownPrivilege" },
			{ Privilege.SyncAgent, "SeSyncAgentPrivilege" },
			{ Privilege.SystemEnvironment, "SeSystemEnvironmentPrivilege" },
			{ Privilege.SystemProfile, "SeSystemProfilePrivilege" },
			{ Privilege.SystemTime, "SeSystemTimePrivilege" },
			{ Privilege.TakeOwnership, "SeTakeOwnershipPrivilege" },
			{ Privilege.TrustedComputerBase, "SeTcbPrivilege" },
			{ Privilege.TimeZone, "SeTimeZonePrivilege" },
			{ Privilege.TrustedCredentialManagerAccess, "SeTrustedCredManAccessPrivilege" },
			{ Privilege.Undock, "SeUndockPrivilege" },
			{ Privilege.UnsolicitedInput, "SeUnsolicitedInputPrivilege" }
		};

		private readonly Thread currentThread = Thread.CurrentThread;
		private bool disposed = false;
		private Stack<NativeMethods.TOKEN_PRIVILEGES> prevPrivs = new Stack<NativeMethods.TOKEN_PRIVILEGES>();
		private NativeMethods.SafeTokenHandle token;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrivilegedCodeBlock"/> class.
		/// </summary>
		/// <param name="privileges">The privileges.</param>
		[PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityCritical]
		public PrivilegedCodeBlock(params Privilege[] privileges)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				token = NativeMethods.SafeTokenHandle.FromCurrentThread(NativeMethods.AccessTypes.TokenAdjustPrivileges | NativeMethods.AccessTypes.TokenQuery);
				foreach (var priv in privileges)
					EnablePrivilege(priv);
			}
		}

		/// <summary>Finalizes an instance of the PrivilegedCodeBlock class.</summary>
		[SecuritySafeCritical]
		~PrivilegedCodeBlock()
		{
			this.Revert();
		}

		/// <summary>Disposes of an instance of the PrivilegedCodeBlock class.</summary>
		/// <exception cref="Win32Exception">Thrown when an underlying Win32 function call does not succeed.</exception>
		/// <permission cref="SecurityAction.Demand">Requires the call stack to have FullTrust.</permission>
		[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
		public void Dispose()
		{
			this.Revert();
			GC.SuppressFinalize(this);
		}

		internal static string EnvironmentGetResourceString(string key, params object[] values)
		{
			string str = new System.Resources.ResourceManager(typeof(Environment)).GetString(key);
			return values.Length == 0 ? str : string.Format(System.Globalization.CultureInfo.CurrentCulture, str, values);
		}

		internal static NativeMethods.LUID LuidFromPrivilege(Privilege privilege)
		{
			NativeMethods.LUID val;
			lock (luidLookup)
			{
				if (!luidLookup.TryGetValue(privilege, out val))
				{
					val = NativeMethods.LUID.FromName(privLookup[privilege]);
					luidLookup.Add(privilege, val);
				}
			}
			return val;
		}

		private void EnablePrivilege(Privilege priv)
		{
			NativeMethods.TOKEN_PRIVILEGES newState = new NativeMethods.TOKEN_PRIVILEGES(LuidFromPrivilege(priv), NativeMethods.PrivilegeAttributes.Enabled);
			NativeMethods.TOKEN_PRIVILEGES prevState = new NativeMethods.TOKEN_PRIVILEGES();
			uint retLen = 0;
			if (!NativeMethods.AdjustTokenPrivileges(token, false, ref newState, NativeMethods.TOKEN_PRIVILEGES.SizeInBytes, ref prevState, ref retLen))
				throw new System.ComponentModel.Win32Exception();
			if ((prevState.Privileges.Attributes & NativeMethods.PrivilegeAttributes.Enabled) != NativeMethods.PrivilegeAttributes.Enabled)
				prevPrivs.Push(prevState);
		}

		/// <summary>
		/// Revert back to prior privileges.
		/// </summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityCritical]
		private void Revert()
		{
			if (!disposed)
			{
				if (!this.currentThread.Equals(Thread.CurrentThread))
					throw new InvalidOperationException(EnvironmentGetResourceString("InvalidOperation_MustBeSameThread"));

				lock (prevPrivs)
				{
					RuntimeHelpers.PrepareConstrainedRegions();
					try
					{
					}
					finally
					{
						Exception failure = null;
						while (prevPrivs.Count > 0)
						{
							try { RevertPrivilege(prevPrivs.Pop()); }
							catch (Exception e) { failure = e; }
						}
						if (failure != null)
							throw failure;
					}
				}
				disposed = true;
			}
		}

		private void RevertPrivilege(NativeMethods.TOKEN_PRIVILEGES priv)
		{
			if (!NativeMethods.AdjustTokenPrivileges(token, false, ref priv, 0, IntPtr.Zero, IntPtr.Zero))
				throw new System.ComponentModel.Win32Exception();
		}
	}
}