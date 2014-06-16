using Community.Windows.Forms;
using System.Windows.Forms;

namespace SecurityEditorCSTester
{
	class Program
	{
		[System.STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}

		static void DirectTest()
		{
			var dlg = new AccessControlEditorDialog() { ElevationRequired = true, OwnerElevationRequired = true, AllowEditOwner = true };
			//dlg.ResourceType = AccessControlEditorDialog.TaskResourceType; dlg.ObjectName = @"AUScheduledInstall";
			dlg.Initialize(new System.IO.FileInfo(@"C:\Temp\ida.ico"));
			//dlg.Initialize(new System.IO.DirectoryInfo(@"C:\Temp"));
			//dlg.Initialize(Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Test"));
			dlg.ShowDialog();
		}
	}
}
