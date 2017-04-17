using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace Expresso
{
	/// <summary>
	/// Summary description for MakeAssemblyForm.
	/// </summary>
	public class MakeAssemblyForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox NamespaceBox;
		private System.Windows.Forms.TextBox FileNameBox;
		private System.Windows.Forms.TextBox ClassNameBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.OpenFileDialog OpenFileDialog;
		private System.Windows.Forms.Button BrowseBtn;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.Button CompileBtn;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox IsPublicChk;
		public string Directory;

		public string FileName
		{
			get{return FileNameBox.Text;}
			set{FileNameBox.Text=value;}
		}

		public string Namespace
		{
			get{return NamespaceBox.Text;}
			set{NamespaceBox.Text=value;}
		}

		public string ClassName
		{
			get{return ClassNameBox.Text;}
			set{ ClassNameBox.Text=value;}
		}

		public bool IsPublic
		{
			get
			{
				return IsPublicChk.Checked;
			}
			set
			{
				IsPublicChk.Checked=value;
			}
		}

		public MakeAssemblyForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.NamespaceBox = new System.Windows.Forms.TextBox();
			this.FileNameBox = new System.Windows.Forms.TextBox();
			this.ClassNameBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.BrowseBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.CompileBtn = new System.Windows.Forms.Button();
			this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.IsPublicChk = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// NamespaceBox
			// 
			this.NamespaceBox.Location = new System.Drawing.Point(135, 39);
			this.NamespaceBox.Name = "NamespaceBox";
			this.NamespaceBox.Size = new System.Drawing.Size(206, 20);
			this.NamespaceBox.TabIndex = 0;
			this.NamespaceBox.Text = "MyNamespace";
			// 
			// FileNameBox
			// 
			this.FileNameBox.Location = new System.Drawing.Point(135, 10);
			this.FileNameBox.Name = "FileNameBox";
			this.FileNameBox.Size = new System.Drawing.Size(206, 20);
			this.FileNameBox.TabIndex = 1;
			this.FileNameBox.Text = "MyFileName.dll";
			// 
			// ClassNameBox
			// 
			this.ClassNameBox.Location = new System.Drawing.Point(135, 68);
			this.ClassNameBox.Name = "ClassNameBox";
			this.ClassNameBox.Size = new System.Drawing.Size(206, 20);
			this.ClassNameBox.TabIndex = 2;
			this.ClassNameBox.Text = "MyClassName";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 37);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "&Namespace:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "&Class Name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(4, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(125, 23);
			this.label3.TabIndex = 6;
			this.label3.Text = "Name of Assembly &File:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// BrowseBtn
			// 
			this.BrowseBtn.Location = new System.Drawing.Point(362, 8);
			this.BrowseBtn.Name = "BrowseBtn";
			this.BrowseBtn.TabIndex = 8;
			this.BrowseBtn.Text = "&Browse ...";
			this.BrowseBtn.Click += new System.EventHandler(this.BrowseBtn_Click);
			// 
			// CancelBtn
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(362, 37);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.TabIndex = 9;
			this.CancelBtn.Text = "&Cancel";
			// 
			// CompileBtn
			// 
			this.CompileBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.CompileBtn.Location = new System.Drawing.Point(362, 66);
			this.CompileBtn.Name = "CompileBtn";
			this.CompileBtn.TabIndex = 10;
			this.CompileBtn.Text = "&Compile";
			// 
			// OpenFileDialog
			// 
			this.OpenFileDialog.CheckFileExists = false;
			this.OpenFileDialog.DefaultExt = "dll";
			this.OpenFileDialog.Filter = "DLL Files|*.dll|All Files|*.*";
			this.OpenFileDialog.Title = "Select a Name for the Assembly File";
			// 
			// IsPublicChk
			// 
			this.IsPublicChk.Checked = true;
			this.IsPublicChk.CheckState = System.Windows.Forms.CheckState.Checked;
			this.IsPublicChk.Location = new System.Drawing.Point(135, 97);
			this.IsPublicChk.Name = "IsPublicChk";
			this.IsPublicChk.Size = new System.Drawing.Size(246, 24);
			this.IsPublicChk.TabIndex = 12;
			this.IsPublicChk.Text = "Make the regular expression publicly visible";
			// 
			// MakeAssemblyForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(447, 130);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.IsPublicChk,
																																	this.CompileBtn,
																																	this.CancelBtn,
																																	this.BrowseBtn,
																																	this.label3,
																																	this.label2,
																																	this.label1,
																																	this.ClassNameBox,
																																	this.FileNameBox,
																																	this.NamespaceBox});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "MakeAssemblyForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Create an Assembly for this Regular Expression";
			this.ResumeLayout(false);

		}
		#endregion

		private void BrowseBtn_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog.FileName=this.FileName;
			OpenFileDialog.InitialDirectory=this.Directory;
			if(OpenFileDialog.ShowDialog()==DialogResult.OK)
			{
				this.FileName=Path.GetFileName(OpenFileDialog.FileName);
				this.Directory=Path.GetDirectoryName(OpenFileDialog.FileName);
			}
		}
	}
}
