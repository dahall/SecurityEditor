using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SecurityEditorCSTester;

public partial class RegKeySelectionDlg : Form
{
	private bool closed = false;
	private RegistryKey key;

	public RegKeySelectionDlg() => InitializeComponent();

	private delegate void NodeMeth(TreeNode node);

	public RegistryKey Key
	{
		get => key;
		set
		{
			var nodes = treeView1.Nodes.Find(value.Name, true);
			if (nodes != null && nodes.Length > 0)
			{
				treeView1.SelectedNode = nodes[0];
				key = value;
			}
		}
	}

	private void AddNodeAndChildren(object okey)
	{
		RegistryKey key = (RegistryKey)okey;
		var tn = new TreeNode(key.Name, 0, 0, GetChildNodes(key));
		if (!closed)
			treeView1.Invoke(new NodeMeth(AddTreeNode), tn);
	}

	private void AddTreeNode(TreeNode node) => treeView1.Nodes.Add(node);

	private void cancelButton_Click(object sender, EventArgs e) => Close();

	private TreeNode[] GetChildNodes(RegistryKey key)
	{
		if (closed) return [];
		try
		{
			string[] subs = key.GetSubKeyNames();
			if (closed) return [];
			TreeNode[] output = new TreeNode[subs.Length];
			for (int i = 0; i < subs.Length; i++)
			{
				string skeyn = subs[i];
				using RegistryKey skey = key.OpenSubKey(skeyn);
				if (closed) return [];
				var parts = skeyn.Split('\\');
				output[i] = new TreeNode(parts[parts.Length - 1], 0, 0, GetChildNodes(skey));
			}
			return output;
		}
		catch { }
		return [];
	}

	private RegistryKey GetKeyFromName(string name)
	{
		var parts = name.Split('\\');
		if (parts.Length > 1)
			name = name.Remove(0, parts[0].Length + 1);
		switch (parts[0])
		{
			case "HKEY_CLASSES_ROOT":
				return Registry.ClassesRoot.OpenSubKey(name);

			case "HKEY_CURRENT_CONFIG":
				return Registry.CurrentConfig.OpenSubKey(name);

			case "HKEY_CURRENT_USER":
				return Registry.CurrentUser.OpenSubKey(name);

			case "HKEY_LOCAL_MACHINE":
				return Registry.LocalMachine.OpenSubKey(name);

			case "HKEY_PERFORMANCE_DATA":
				return Registry.PerformanceData.OpenSubKey(name);

			case "HKEY_USERS":
				return Registry.Users.OpenSubKey(name);

			default:
				break;
		}
		throw new InvalidOperationException();
	}

	private void okButton_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode != null)
			key = GetKeyFromName(treeView1.SelectedNode.FullPath);
		Close();
	}

	protected override void OnClosing(CancelEventArgs e) { base.OnClosing(e); closed = true; }

	private void RegKeySelectionDlg_Load(object sender, EventArgs e)
	{
		foreach (RegistryHive keyVal in Enum.GetValues(typeof(RegistryHive)))
		{
			var key = RegistryKey.OpenRemoteBaseKey(keyVal, string.Empty);
			new System.Threading.Thread(AddNodeAndChildren).Start(key);
		}
	}

	private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) => okButton_Click(sender, EventArgs.Empty);
}