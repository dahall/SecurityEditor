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
			this.accessControlEditorDialog1 = new Community.Windows.Forms.AccessControlEditorDialog();
			this.SuspendLayout();
			// 
			// accessControlEditorDialog1
			// 
			this.accessControlEditorDialog1.Flags = ((Community.Security.AccessControl.ObjInfoFlags)(((((((Community.Security.AccessControl.ObjInfoFlags.EditOwner | Community.Security.AccessControl.ObjInfoFlags.EditAudit) 
            | Community.Security.AccessControl.ObjInfoFlags.Advanced) 
            | Community.Security.AccessControl.ObjInfoFlags.Reset) 
            | Community.Security.AccessControl.ObjInfoFlags.EditProperties) 
            | Community.Security.AccessControl.ObjInfoFlags.EditEffective) 
            | Community.Security.AccessControl.ObjInfoFlags.ResetSacl)));
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private Community.Windows.Forms.AccessControlEditorDialog accessControlEditorDialog1;
	}
}