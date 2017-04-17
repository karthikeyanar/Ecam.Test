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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Expresso
{
	/// <summary>
	/// This is the regular expression builder, a UserControl that holds
	/// a set of tab pages for building regular expressions within the Expresso application
	/// </summary>
	public class RegBuilder : System.Windows.Forms.UserControl
	{
		private Characters characters;
		private Groups groups;
		private Special special;
		private Substitutions substitute;
		private Misc misc;
		private Table table;
		private System.Windows.Forms.TabControl tabControl1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private MainForm Main;
		public Size Offset;

		/// <summary>
		/// Create the regular expression builder
		/// </summary>
		/// <param name="main">The main form of the application</param>
		public RegBuilder(MainForm main)
		{
			InitializeComponent();
			Main=main;
			characters = new Characters(this);
			groups = new Groups(this);
			special = new Special(this);
			substitute = new Substitutions(this);
			misc = new Misc(this);
			table = new Table(this);

			// Add the Characters and Repetitions tab page
			TabPage page6=new TabPage();
			page6.Controls.Add(characters);
			page6.Text="Characters and Repetitions";
			this.tabControl1.Controls.Add(page6);

			// Add the Groups and Options tab page
			TabPage page4=new TabPage();
			page4.Controls.Add(groups);
			page4.Text="Groups and Options";
			this.tabControl1.Controls.Add(page4);

			// Add the Special Characters tab page
			TabPage page5=new TabPage();
			page5.Controls.Add(special);
			page5.Text="Special Characters";
			this.tabControl1.Controls.Add(page5);

			// Add the Substitutions tab page 
			TabPage page3=new TabPage();
			page3.Controls.Add(substitute);
			page3.Text="Substitutions";
			this.tabControl1.Controls.Add(page3);

			// Add the Miscellaneous tab page
			TabPage page2=new TabPage();
			page2.Controls.Add(misc);
			page2.Text="Miscellaneous";
			this.tabControl1.Controls.Add(page2);

			// Add the ASCII Table tab page
			TabPage page1=new TabPage();
			page1.Controls.Add(table);
			page1.Text="ASCII Table";
			this.tabControl1.Controls.Add(page1);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(584, 364);
			this.tabControl1.TabIndex = 0;
			// 
			// RegBuilder
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.tabControl1});
			this.Name = "RegBuilder";
			this.Size = new System.Drawing.Size(584, 364);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Insert text into the regular expression box on the main form
		/// </summary>
		/// <param name="input">The text to be inserted</param>
		/// <param name="CursorIndex">The insertion point for the text</param>
		public void InsertRegex(string input, int CursorIndex)
		{
			Main.InsertRegex(input,CursorIndex);
		}

		/// <summary>
		/// Insert text into the replacement expression box on the main form
		/// </summary>
		/// <param name="input">The text to be inserted</param>
		/// <param name="CursorIndex">The insertion point for the text</param>
		public void InsertReplaceText(string input, int CursorIndex)
		{
			Main.InsertReplaceText(input,CursorIndex);
		}

		/// <summary>
		/// Hide the RegBuilder on double-click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RegBuilder_DoubleClick(object sender, System.EventArgs e)
		{
			this.Visible=false;
			Main.BuildBtn.Text="Show &Builder";
		}

		/// <summary>
		/// Show or hide the RegBuilder
		/// </summary>
		/// <param name="ShowIt">If true, show the RegBuilder, otherwise hide it</param>
		public void ShowBuilder(bool ShowIt)
		{
			Main.ShowBuilder(ShowIt);
		}

		private void RegBuilder_Move(object sender, System.EventArgs e)
		{
			Offset.Height= Main.Location.Y - this.Location.Y;
			Offset.Width = Main.Location.X - this.Location.X;
		}
	}
}
