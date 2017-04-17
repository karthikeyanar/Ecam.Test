// =====================================================================
//
// Expresso - A Tool for Building and Testing Regular Expressions
//
// by Jim Hollenhorst, jim@ultrapico.com
// Copyright Ultrapico, February 2003
// http://www.ultrapico.com
//
// =====================================================================
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Expresso
{
	/// <summary>
	/// The Expresso AboutBox.
	/// </summary>
	public class AboutBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.ToolTip Tip1;
		private System.Windows.Forms.Label lbVersion;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.PictureBox UltrapicoLogo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button LicenseBtn;
		private System.Windows.Forms.PictureBox pictureBox1;
		private MainForm Main;

		public AboutBox(MainForm main)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Main=main;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AboutBox));
			this.lbVersion = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.OKButton = new System.Windows.Forms.Button();
			this.Tip1 = new System.Windows.Forms.ToolTip(this.components);
			this.LicenseBtn = new System.Windows.Forms.Button();
			this.UltrapicoLogo = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// lbVersion
			// 
			this.lbVersion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lbVersion.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lbVersion.Location = new System.Drawing.Point(9, 135);
			this.lbVersion.Name = "lbVersion";
			this.lbVersion.Size = new System.Drawing.Size(236, 130);
			this.lbVersion.TabIndex = 3;
			this.lbVersion.Text = "Copyright 2002, by Ultrapico Company    All Rights Reserved";
			this.lbVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// linkLabel1
			// 
			this.linkLabel1.Location = new System.Drawing.Point(251, 226);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(177, 23);
			this.linkLabel1.TabIndex = 2;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "http://www.ultrapico.com";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.Tip1.SetToolTip(this.linkLabel1, "Click to visit the TimeTraveler web site.");
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// OKButton
			// 
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Location = new System.Drawing.Point(341, 136);
			this.OKButton.Name = "OKButton";
			this.OKButton.TabIndex = 0;
			this.OKButton.Text = "&OK";
			this.Tip1.SetToolTip(this.OKButton, "Close this form");
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// LicenseBtn
			// 
			this.LicenseBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.LicenseBtn.Location = new System.Drawing.Point(259, 136);
			this.LicenseBtn.Name = "LicenseBtn";
			this.LicenseBtn.TabIndex = 9;
			this.LicenseBtn.Text = "&License Info";
			this.Tip1.SetToolTip(this.LicenseBtn, "Close this form");
			this.LicenseBtn.Click += new System.EventHandler(this.LicenseBtn_Click);
			// 
			// UltrapicoLogo
			// 
			this.UltrapicoLogo.Image = ((System.Drawing.Bitmap)(resources.GetObject("UltrapicoLogo.Image")));
			this.UltrapicoLogo.Location = new System.Drawing.Point(259, 177);
			this.UltrapicoLogo.Name = "UltrapicoLogo";
			this.UltrapicoLogo.Size = new System.Drawing.Size(161, 42);
			this.UltrapicoLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.UltrapicoLogo.TabIndex = 7;
			this.UltrapicoLogo.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Times New Roman", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(128)), ((System.Byte)(64)), ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(170, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(209, 56);
			this.label1.TabIndex = 8;
			this.label1.Text = "Expresso";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox1.Image = ((System.Drawing.Bitmap)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(35, 23);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(105, 82);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 10;
			this.pictureBox1.TabStop = false;
			// 
			// AboutBox
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.OKButton;
			this.ClientSize = new System.Drawing.Size(428, 275);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.pictureBox1,
																																	this.LicenseBtn,
																																	this.label1,
																																	this.UltrapicoLogo,
																																	this.OKButton,
																																	this.linkLabel1,
																																	this.lbVersion});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About Expresso";
			this.VisibleChanged += new System.EventHandler(this.AboutBox_VisibleChanged);
			this.ResumeLayout(false);

		}
		#endregion

		private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
				// Change the color of the link text by setting LinkVisited 
				// to True.
				linkLabel1.LinkVisited = true;
   
				// Call the Process.Start method to open the default browser 
				// with a URL:
				System.Diagnostics.Process.Start("http://www.ultrapico.com");
		}

		private void OKButton_Click(object sender, System.EventArgs e)
		{
			this.Visible=false;
		}

		public void LicenseBtn_Click(object sender, System.EventArgs e)
		{
			FileInfo appfile=new FileInfo(Application.ExecutablePath);
			string LicenseFilePath=appfile.DirectoryName+"\\license.rtf";
			//MessageBox.Show("Getting the file: "+LicenseFilePath);
			Process.Start("wordpad.exe","\""+LicenseFilePath+"\"");
		}

		private void ShowVersion()
		{
			// Get the file version information for the application
			string TheAppFile=Application.ExecutablePath;
			FileVersionInfo myInfo = FileVersionInfo.GetVersionInfo(TheAppFile);
 
			lbVersion.Text="Expresso 1.0 - For Building and Testing Regular Expressions"+
				"\nVersion Number: " + myInfo.FileVersion +"\n"+
				"\n"+myInfo.LegalCopyright;
		}

		private void AboutBox_VisibleChanged(object sender, System.EventArgs e)
		{
			ShowVersion();
		}
	}
}
