using Community.Windows.Forms;

namespace SecurityEditorCSTester
{
	class Program
	{
		[System.STAThread]
		static void Main(string[] args)
		{
			var dlg = new AccessControlEditorDialog();
			dlg.ResourceType = AccessControlEditorDialog.TaskResourceType; dlg.ObjectName = @"Maint";
			//dlg.Initialize(new System.IO.FileInfo(@"C:\RAT2Llog.txt"));
			//dlg.Initialize(new System.IO.DirectoryInfo(@"C:\Temp"));
			dlg.ShowDialog();
		}
	}
}
