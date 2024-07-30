using Community.Windows.Forms;
using System.Runtime.Versioning;
using System.Windows.Forms;

#if NET50_OR_GREATER
[assembly: SupportedOSPlatform("windows")]
#endif
namespace SecurityEditorCSTester;

internal class Program
{
	private static void DirectTest()
	{
		var dlg = new AccessControlEditorDialog() { ElevationRequired = true, OwnerElevationRequired = true, AllowEditOwner = true };
		//dlg.ResourceType = AccessControlEditorDialog.TaskResourceType; dlg.ObjectName = @"AUScheduledInstall";
		dlg.Initialize(new System.IO.FileInfo(@"C:\Temp\ida.ico"));
		//dlg.Initialize(new System.IO.DirectoryInfo(@"C:\Temp"));
		//dlg.Initialize(Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Test"));
		dlg.ShowDialog();
	}

	[System.STAThread]
	private static void Main(string[] args)
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new Form1());
	}
}