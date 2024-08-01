#pragma warning disable IDE1006 // Naming Styles
using Community.Security.AccessControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SecurityEditorCSTester;

public partial class Form1 : Form
{
	private bool updatingFlags = false;

	public Form1()
	{
		InitializeComponent();

		Properties.Settings.Default.SettingsLoaded += Default_SettingsLoaded;

		numericUpDown1.Maximum = ulong.MaxValue;

		var res = GetEnumVals<System.Security.AccessControl.ResourceType>();
		res.Add(new KeyValuePair<string, System.Security.AccessControl.ResourceType>("Task", Community.Windows.Forms.AccessControlEditorDialog.TaskResourceType));
		resTypeCombo.DataSource = res;
		resTypeCombo.DisplayMember = "Key";
		resTypeCombo.ValueMember = "Value";

		pageTypeCombo.DataSource = GetEnumVals<SecurityPageType>();
		pageTypeCombo.DisplayMember = "Key";
		pageTypeCombo.ValueMember = "Value";

		pageActCombo.DataSource = GetEnumVals<SecurityPageActivation>();
		pageActCombo.DisplayMember = "Key";
		pageActCombo.ValueMember = "Value";

		var oif = GetEnumVals<ObjInfoFlags>();
		oif.RemoveAll(val => val.Value == 0);
		checkBoxList.Items.AddRange(oif.Select(kvp => new GroupControls.CheckBoxListItem() { Text = kvp.Key, Tag = kvp.Value }));
	}

	private static List<KeyValuePair<string, T>> GetEnumVals<T>() where T : struct, Enum => Enum.GetValues(typeof(T)).Cast<T>().Select(v => new KeyValuePair<string, T>(v.ToString(), v)).ToList();

	private void checkBoxList_ItemCheckStateChanged(object sender, GroupControls.CheckBoxListItemCheckStateChangedEventArgs e)
	{
		if (!updatingFlags)
		{
			updatingFlags = true;
			var flags = GetFlags();

			// Process dependencies
			checkBoxList.ProcessFlagsOnCheckStateChanged<ObjInfoFlags>(e.Item);

			if ((flags & ObjInfoFlags.Advanced) != 0 && (SecurityPageType)pageTypeCombo.SelectedValue == SecurityPageType.BasicPermissions)
				SetChecks(ObjInfoFlags.ViewOnly, true);

			if ((flags & ObjInfoFlags.NoTreeApply) != 0)
				SetChecks(ObjInfoFlags.Advanced, true);

			if ((flags & ObjInfoFlags.OwnerRecurse) != 0)
				SetChecks(ObjInfoFlags.Container, true);

			if ((flags & (ObjInfoFlags.OwnerReadOnly | ObjInfoFlags.ResetOwner)) != 0)
				SetChecks(ObjInfoFlags.EditOwner, true);

			if ((flags & ObjInfoFlags.ReadOnly) != 0)
				SetChecks(ObjInfoFlags.ViewOnly, false);

			if ((flags & (ObjInfoFlags.ResetSacl | ObjInfoFlags.ResetSaclTree)) != 0)
				SetChecks(ObjInfoFlags.Advanced, true);

			numericUpDown1.Value = (decimal)GetFlags();
			updatingFlags = false;
		}
	}

	private void closeBtn_Click(object sender, EventArgs e) => Close();

	private void Default_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
	{
	}

	private void Form1_FormClosing(object sender, FormClosingEventArgs e)
	{
		Properties.Settings.Default.resType = (int)ResType;
		Properties.Settings.Default.pageType = (int)(SecurityPageType)pageTypeCombo.SelectedValue;
		Properties.Settings.Default.pageAct = (int)(SecurityPageActivation)pageActCombo.SelectedValue;
		Properties.Settings.Default.dlgFlags = (decimal)GetFlags();
		Properties.Settings.Default.objName = objNameText.Text;
		Properties.Settings.Default.dispName = dispNameText.Text;
		Properties.Settings.Default.Save();
	}

	private void Form1_Load(object sender, EventArgs e)
	{
		ResType = (System.Security.AccessControl.ResourceType)Properties.Settings.Default.resType;
		pageTypeCombo.SelectedValue = (SecurityPageType)Properties.Settings.Default.pageType;
		pageActCombo.SelectedValue = (SecurityPageActivation)Properties.Settings.Default.pageAct;
		objNameText.Text = Properties.Settings.Default.objName;
		dispNameText.Text = Properties.Settings.Default.dispName;
		checkBoxList.SetFlagsValue((ObjInfoFlags)(numericUpDown1.Value = Properties.Settings.Default.dlgFlags));
	}

	private ObjInfoFlags GetFlags() => checkBoxList.GetFlagsValue<ObjInfoFlags>(0);

	private void launchBtn_Click(object sender, EventArgs e)
	{
		if (objNameText.TextLength > 0)
		{
			aceDlg.Initialize(objNameText.Text, svrNameText.TextLength == 0 ? null : svrNameText.Text, ResType);
			if (dispNameText.TextLength > 0)
				aceDlg.DisplayName = dispNameText.Text;
		}
		else
			aceDlg.Initialize(dispNameText.Text, dispNameText.Text, (GetFlags() & ObjInfoFlags.Container) != 0, ResType, null, svrNameText.TextLength == 0 ? null : svrNameText.Text);
		if (pageTypeCombo.SelectedIndex > 0)
			aceDlg.PageType = (SecurityPageType)pageTypeCombo.SelectedValue;
		aceDlg.Flags = (ObjInfoFlags)numericUpDown1.Value;
		aceDlg.ShowDialog(this);
	}

	private void SetChecks(ObjInfoFlags flags, bool check)
	{
		var i = checkBoxList.Items.Find(item => flags == (ObjInfoFlags)item.Tag);
		if (i != null)
			i.Checked = check;
	}

	private void toolStripButton1_Click(object sender, EventArgs e)
	{
		if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
		{
			ResType = System.Security.AccessControl.ResourceType.FileObject;
			objNameText.Text = folderBrowserDialog1.SelectedPath;
			SetChecks(ObjInfoFlags.Container, true);
		}
	}

	private void toolStripButton2_Click(object sender, EventArgs e)
	{
		if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
		{
			ResType = System.Security.AccessControl.ResourceType.FileObject;
			objNameText.Text = openFileDialog1.FileName;
			SetChecks(ObjInfoFlags.Container, false);
		}
	}

	private void toolStripButton3_Click(object sender, EventArgs e)
	{
		using var dlg = new RegKeySelectionDlg();
		if (dlg.ShowDialog(this) == DialogResult.OK)
		{
			ResType = System.Security.AccessControl.ResourceType.RegistryKey;
			objNameText.Text = dlg.Key.Name;
			SetChecks(ObjInfoFlags.Container, true);
		}
	}

	private void toolStripButton4_Click(object sender, EventArgs e)
	{
		using var dlg = new Microsoft.Win32.TaskScheduler.TaskBrowserDialog() { AllowFolderSelection = true, ShowTasks = true };
		if (dlg.ShowDialog(this) == DialogResult.OK)
		{
			ResType = Community.Windows.Forms.AccessControlEditorDialog.TaskResourceType;
			objNameText.Text = dlg.SelectedPath;
			SetChecks(ObjInfoFlags.Container, dlg.SelectedPathType.Name == "TaskFolder");
		}
	}

	private System.Security.AccessControl.ResourceType ResType
	{
		get => (System.Security.AccessControl.ResourceType)resTypeCombo.SelectedValue;
		set => resTypeCombo.SelectedValue = value;
	}
}

internal static class CBLExt
{
	public static T GetFlagsValue<T>(this GroupControls.CheckBoxList l, int i) where T : unmanaged, Enum =>
		(T)Enum.ToObject(typeof(T), l.Items.Where(i => i.Checked).Aggregate(0U, (f, i) => f | Convert.ToUInt32(i.Tag)));
	public static void SetFlagsValue<T>(this GroupControls.CheckBoxList l, T value) where T : struct, Enum
	{
		for (int i = 0; i < l.Items.Count; i++)
			l.Items[i].Checked = (Convert.ToUInt32(l.Items[i].Tag) & Convert.ToUInt32(value)) != 0;
	}
	public static void ProcessFlagsOnCheckStateChanged<T>(this GroupControls.CheckBoxList l, GroupControls.CheckBoxListItem i) where T : unmanaged, Enum
	{
	}
}