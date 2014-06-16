using Community.Security.AccessControl;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SecurityEditorCSTester
{
	public partial class Form1 : Form
	{
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
			resTypeCombo.SelectedIndex = 0;

			pageTypeCombo.DataSource = GetEnumVals<SecurityPageType>();
			pageTypeCombo.DisplayMember = "Key";
			pageTypeCombo.ValueMember = "Value";
			pageTypeCombo.SelectedIndex = 0;

			var oif = GetEnumVals<ObjInfoFlags>();
			oif.RemoveAll(delegate(KeyValuePair<string, ObjInfoFlags> val) { return val.Value == 0; });
			checkBoxList.Items.AddRange(oif.ConvertAll<GroupControls.CheckBoxListItem>(chliConvert));
		}

		void Default_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
		{
			SetFlags();
		}

		private ObjInfoFlags GetFlags()
		{
			return checkBoxList.GetFlagsValue<ObjInfoFlags>(0);
		}

		private void SetFlags()
		{
			checkBoxList.SetFlagsValue<ObjInfoFlags>((ObjInfoFlags)Properties.Settings.Default.dlgFlags);
		}

		public static GroupControls.CheckBoxListItem chliConvert(KeyValuePair<string, ObjInfoFlags> kvp)
		{
			return new GroupControls.CheckBoxListItem() { Text = kvp.Key, Tag = kvp.Value };
		}

		private static object[] GetEnumObjs<T>()
		{
			var rt = new List<object>();
			foreach (T item in Enum.GetValues(typeof(T)))
				rt.Add(new KeyValuePair<string, T>(item.ToString(), item));
			return rt.ToArray();
		}

		private static List<KeyValuePair<string, T>> GetEnumVals<T>()
		{
			var rt = new List<KeyValuePair<string, T>>();
			foreach (T item in Enum.GetValues(typeof(T)))
				rt.Add(new KeyValuePair<string, T>(item.ToString(), item));
			return rt;
		}

		private void launchBtn_Click(object sender, EventArgs e)
		{
			accessControlEditorDialog1.Initialize(this.objNameText.Text, this.svrNameText.TextLength == 0 ? null : this.svrNameText.Text, (System.Security.AccessControl.ResourceType)resTypeCombo.SelectedValue);
			if (this.dispNameText.TextLength > 0)
				accessControlEditorDialog1.DisplayName = this.dispNameText.Text;
			if (this.pageTypeCombo.SelectedIndex > 0)
				accessControlEditorDialog1.PageType = (SecurityPageType)this.pageTypeCombo.SelectedValue;
			accessControlEditorDialog1.Flags = (ObjInfoFlags)numericUpDown1.Value;
			accessControlEditorDialog1.ShowDialog(this);
		}

		private void closeBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private bool updatingFlags = false;

		private void checkBoxList_ItemCheckStateChanged(object sender, GroupControls.CheckBoxListItemCheckStateChangedEventArgs e)
		{
			if (!updatingFlags)
			{
				updatingFlags = true;
				var flags = GetFlags();

				// Process dependencies
				checkBoxList.ProcessFlagsOnCheckStateChanged(e.Item);

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

		private void SetChecks(ObjInfoFlags flags, bool check)
		{
			var i = checkBoxList.Items.Find(delegate(GroupControls.CheckBoxListItem item) { return flags == (ObjInfoFlags)item.Tag; });
			if (i != null)
				i.Checked = check;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Properties.Settings.Default.Reload();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				this.resTypeCombo.SelectedValue = System.Security.AccessControl.ResourceType.FileObject;
				this.objNameText.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				this.resTypeCombo.SelectedValue = System.Security.AccessControl.ResourceType.FileObject;
				this.objNameText.Text = openFileDialog1.FileName;
			}
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{

		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{

		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			Properties.Settings.Default.Save();
		}
	}
}
