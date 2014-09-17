namespace SecurityEditorCSTester
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			SecurityEditorCSTester.Properties.Settings settings1 = new SecurityEditorCSTester.Properties.Settings();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.closeBtn = new System.Windows.Forms.Button();
			this.launchBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.resTypeCombo = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.pageTypeCombo = new System.Windows.Forms.ComboBox();
			this.checkBoxList = new GroupControls.CheckBoxList();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.dispNameText = new System.Windows.Forms.TextBox();
			this.svrNameText = new System.Windows.Forms.TextBox();
			this.objNameText = new System.Windows.Forms.TextBox();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.pageActCombo = new System.Windows.Forms.ComboBox();
			this.toolStripContainer2 = new System.Windows.Forms.ToolStripContainer();
			this.accessControlEditorDialog1 = new Community.Windows.Forms.AccessControlEditorDialog();
			this.checkBoxList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.toolStripContainer2.ContentPanel.SuspendLayout();
			this.toolStripContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// closeBtn
			// 
			this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeBtn.Location = new System.Drawing.Point(539, 266);
			this.closeBtn.Name = "closeBtn";
			this.closeBtn.Size = new System.Drawing.Size(75, 23);
			this.closeBtn.TabIndex = 0;
			this.closeBtn.Text = "Close";
			this.closeBtn.UseVisualStyleBackColor = true;
			this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
			// 
			// launchBtn
			// 
			this.launchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.launchBtn.Location = new System.Drawing.Point(458, 266);
			this.launchBtn.Name = "launchBtn";
			this.launchBtn.Size = new System.Drawing.Size(75, 23);
			this.launchBtn.TabIndex = 0;
			this.launchBtn.Text = "Launch";
			this.launchBtn.UseVisualStyleBackColor = true;
			this.launchBtn.Click += new System.EventHandler(this.launchBtn_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Object name:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Server name:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(73, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Display name:";
			// 
			// resTypeCombo
			// 
			this.resTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.resTypeCombo.FormattingEnabled = true;
			this.resTypeCombo.Location = new System.Drawing.Point(394, 11);
			this.resTypeCombo.Name = "resTypeCombo";
			this.resTypeCombo.Size = new System.Drawing.Size(220, 21);
			this.resTypeCombo.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(309, 14);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(79, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Resource type:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(309, 41);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "Page type:";
			// 
			// pageTypeCombo
			// 
			this.pageTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.pageTypeCombo.FormattingEnabled = true;
			this.pageTypeCombo.Location = new System.Drawing.Point(394, 38);
			this.pageTypeCombo.Name = "pageTypeCombo";
			this.pageTypeCombo.Size = new System.Drawing.Size(111, 21);
			this.pageTypeCombo.TabIndex = 3;
			// 
			// checkBoxList
			// 
			this.checkBoxList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxList.AutoScrollMinSize = new System.Drawing.Size(599, 0);
			this.checkBoxList.AutoSize = false;
			this.checkBoxList.Location = new System.Drawing.Point(15, 95);
			this.checkBoxList.Name = "checkBoxList";
			this.checkBoxList.RepeatColumns = 4;
			this.checkBoxList.Size = new System.Drawing.Size(599, 160);
			this.checkBoxList.SpaceEvenly = true;
			this.checkBoxList.TabIndex = 4;
			this.checkBoxList.TabStop = false;
			this.checkBoxList.ItemCheckStateChanged += new System.EventHandler<GroupControls.CheckBoxListItemCheckStateChangedEventArgs>(this.checkBoxList_ItemCheckStateChanged);
			// 
			// numericUpDown1
			// 
			settings1.dispName = "";
			settings1.dlgFlags = new decimal(new int[] {
            0,
            0,
            0,
            0});
			settings1.flags = 0;
			settings1.objName = "";
			settings1.pageAct = 0;
			settings1.pageType = 0;
			settings1.resType = 0;
			settings1.SettingsKey = "";
			settings1.svrName = "";
			this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", settings1, "dlgFlags", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDown1.Hexadecimal = true;
			this.numericUpDown1.Location = new System.Drawing.Point(15, 269);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            66000,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.ReadOnly = true;
			this.numericUpDown1.Size = new System.Drawing.Size(70, 20);
			this.numericUpDown1.TabIndex = 5;
			this.numericUpDown1.Value = settings1.dlgFlags;
			// 
			// dispNameText
			// 
			this.dispNameText.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings1, "dispName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.dispNameText.Location = new System.Drawing.Point(95, 64);
			this.dispNameText.Name = "dispNameText";
			this.dispNameText.Size = new System.Drawing.Size(203, 20);
			this.dispNameText.TabIndex = 1;
			this.dispNameText.Text = settings1.dispName;
			// 
			// svrNameText
			// 
			this.svrNameText.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings1, "svrName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.svrNameText.Location = new System.Drawing.Point(95, 38);
			this.svrNameText.Name = "svrNameText";
			this.svrNameText.Size = new System.Drawing.Size(203, 20);
			this.svrNameText.TabIndex = 1;
			this.svrNameText.Text = settings1.svrName;
			// 
			// objNameText
			// 
			this.objNameText.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings1, "objName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.objNameText.Location = new System.Drawing.Point(95, 12);
			this.objNameText.Name = "objNameText";
			this.objNameText.Size = new System.Drawing.Size(203, 20);
			this.objNameText.TabIndex = 1;
			this.objNameText.Text = settings1.objName;
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(152, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton1.Text = "folder";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// toolStripButton2
			// 
			this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton2.Text = "file";
			this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton3.Text = "registry";
			this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
			// 
			// toolStripButton4
			// 
			this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
			this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton4.Text = "task";
			this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// pageActCombo
			// 
			this.pageActCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.pageActCombo.FormattingEnabled = true;
			this.pageActCombo.Location = new System.Drawing.Point(511, 38);
			this.pageActCombo.Name = "pageActCombo";
			this.pageActCombo.Size = new System.Drawing.Size(103, 21);
			this.pageActCombo.TabIndex = 3;
			// 
			// toolStripContainer2
			// 
			// 
			// toolStripContainer2.ContentPanel
			// 
			this.toolStripContainer2.ContentPanel.Controls.Add(this.toolStrip1);
			this.toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(152, 0);
			this.toolStripContainer2.Location = new System.Drawing.Point(394, 65);
			this.toolStripContainer2.Name = "toolStripContainer2";
			this.toolStripContainer2.Size = new System.Drawing.Size(152, 24);
			this.toolStripContainer2.TabIndex = 7;
			this.toolStripContainer2.Text = "toolStripContainer2";
			// 
			// accessControlEditorDialog1
			// 
			this.accessControlEditorDialog1.DisplayName = null;
			this.accessControlEditorDialog1.Flags = ((Community.Security.AccessControl.ObjInfoFlags)(((((((Community.Security.AccessControl.ObjInfoFlags.EditOwner | Community.Security.AccessControl.ObjInfoFlags.EditAudit) 
            | Community.Security.AccessControl.ObjInfoFlags.Advanced) 
            | Community.Security.AccessControl.ObjInfoFlags.Reset) 
            | Community.Security.AccessControl.ObjInfoFlags.EditProperties) 
            | Community.Security.AccessControl.ObjInfoFlags.EditEffective) 
            | Community.Security.AccessControl.ObjInfoFlags.ResetSacl)));
			// 
			// Form1
			// 
			this.AcceptButton = this.launchBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.closeBtn;
			this.ClientSize = new System.Drawing.Size(626, 301);
			this.Controls.Add(this.toolStripContainer2);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.checkBoxList);
			this.Controls.Add(this.pageActCombo);
			this.Controls.Add(this.pageTypeCombo);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.resTypeCombo);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dispNameText);
			this.Controls.Add(this.svrNameText);
			this.Controls.Add(this.objNameText);
			this.Controls.Add(this.launchBtn);
			this.Controls.Add(this.closeBtn);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "Form1";
			this.Text = "Test SecurityEditor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.checkBoxList.ResumeLayout(true);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.toolStripContainer2.ContentPanel.ResumeLayout(false);
			this.toolStripContainer2.ContentPanel.PerformLayout();
			this.toolStripContainer2.ResumeLayout(false);
			this.toolStripContainer2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Community.Windows.Forms.AccessControlEditorDialog accessControlEditorDialog1;
		private System.Windows.Forms.Button closeBtn;
		private System.Windows.Forms.Button launchBtn;
		private System.Windows.Forms.TextBox objNameText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox svrNameText;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox dispNameText;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox resTypeCombo;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox pageTypeCombo;
		private GroupControls.CheckBoxList checkBoxList;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripButton toolStripButton4;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.ComboBox pageActCombo;
		private System.Windows.Forms.ToolStripContainer toolStripContainer2;
	}
}