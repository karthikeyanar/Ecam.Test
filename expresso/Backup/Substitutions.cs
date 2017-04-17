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

namespace Expresso
{
	/// <summary>
	/// This is the "Substitutions" tab page for the RegBuilder
	/// </summary>
	public class Substitutions : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox Results;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton Following;
		private System.Windows.Forms.RadioButton Prior;
		private System.Windows.Forms.RadioButton Match;
		private System.Windows.Forms.RadioButton Named;
		private System.Windows.Forms.RadioButton Numbered;
		private System.Windows.Forms.NumericUpDown NumberBox;
		private System.Windows.Forms.TextBox NameBox;
		private System.Windows.Forms.RadioButton Last;
		private System.Windows.Forms.RadioButton Input;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button InsertBtn;
		private System.Windows.Forms.Button HideBtn;
		private System.Windows.Forms.ToolTip toolTip1;
		private RegBuilder Builder;

		public Substitutions(RegBuilder builder)
		{
			InitializeComponent();
			Builder=builder;
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
			this.label3 = new System.Windows.Forms.Label();
			this.InsertBtn = new System.Windows.Forms.Button();
			this.Results = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.Input = new System.Windows.Forms.RadioButton();
			this.Last = new System.Windows.Forms.RadioButton();
			this.NameBox = new System.Windows.Forms.TextBox();
			this.Following = new System.Windows.Forms.RadioButton();
			this.Prior = new System.Windows.Forms.RadioButton();
			this.Match = new System.Windows.Forms.RadioButton();
			this.Named = new System.Windows.Forms.RadioButton();
			this.Numbered = new System.Windows.Forms.RadioButton();
			this.NumberBox = new System.Windows.Forms.NumericUpDown();
			this.HideBtn = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumberBox)).BeginInit();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 23);
			this.label3.TabIndex = 18;
			this.label3.Text = "Replacement Pattern";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// InsertBtn
			// 
			this.InsertBtn.Location = new System.Drawing.Point(400, 5);
			this.InsertBtn.Name = "InsertBtn";
			this.InsertBtn.TabIndex = 16;
			this.InsertBtn.Text = "&Insert";
			this.toolTip1.SetToolTip(this.InsertBtn, "Insert the subexpression");
			this.InsertBtn.Click += new System.EventHandler(this.InsertBtn_Click);
			// 
			// Results
			// 
			this.Results.Location = new System.Drawing.Point(114, 5);
			this.Results.Name = "Results";
			this.Results.Size = new System.Drawing.Size(273, 20);
			this.Results.TabIndex = 15;
			this.Results.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						this.Input,
																																						this.Last,
																																						this.NameBox,
																																						this.Following,
																																						this.Prior,
																																						this.Match,
																																						this.Named,
																																						this.Numbered,
																																						this.NumberBox});
			this.groupBox1.Location = new System.Drawing.Point(116, 59);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(280, 216);
			this.groupBox1.TabIndex = 19;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Substitutions";
			// 
			// Input
			// 
			this.Input.Location = new System.Drawing.Point(16, 176);
			this.Input.Name = "Input";
			this.Input.Size = new System.Drawing.Size(151, 24);
			this.Input.TabIndex = 7;
			this.Input.Text = "Entire input string $_";
			this.toolTip1.SetToolTip(this.Input, "Substitute the entire input string");
			this.Input.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// Last
			// 
			this.Last.Location = new System.Drawing.Point(16, 152);
			this.Last.Name = "Last";
			this.Last.Size = new System.Drawing.Size(150, 24);
			this.Last.TabIndex = 6;
			this.Last.Text = "Last group captured $+";
			this.toolTip1.SetToolTip(this.Last, "Substitute the last captured group");
			this.Last.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// NameBox
			// 
			this.NameBox.Location = new System.Drawing.Point(161, 59);
			this.NameBox.Name = "NameBox";
			this.NameBox.Size = new System.Drawing.Size(64, 20);
			this.NameBox.TabIndex = 5;
			this.NameBox.Text = "Name";
			this.toolTip1.SetToolTip(this.NameBox, "Enter the name of the group");
			this.NameBox.TextChanged += new System.EventHandler(this.NameBox_TextChanged);
			// 
			// Following
			// 
			this.Following.Location = new System.Drawing.Point(16, 128);
			this.Following.Name = "Following";
			this.Following.Size = new System.Drawing.Size(159, 24);
			this.Following.TabIndex = 4;
			this.Following.Text = "Text following the match $\'";
			this.toolTip1.SetToolTip(this.Following, "Substitute all text following the match");
			this.Following.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// Prior
			// 
			this.Prior.Location = new System.Drawing.Point(16, 104);
			this.Prior.Name = "Prior";
			this.Prior.Size = new System.Drawing.Size(148, 24);
			this.Prior.TabIndex = 3;
			this.Prior.Text = "Text prior to the match $`";
			this.toolTip1.SetToolTip(this.Prior, "Substitute all text prior to the match");
			this.Prior.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// Match
			// 
			this.Match.Location = new System.Drawing.Point(16, 80);
			this.Match.Name = "Match";
			this.Match.Size = new System.Drawing.Size(128, 24);
			this.Match.TabIndex = 2;
			this.Match.Text = "Entire match $&&";
			this.toolTip1.SetToolTip(this.Match, "Substitute the match itself");
			this.Match.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// Named
			// 
			this.Named.Location = new System.Drawing.Point(16, 56);
			this.Named.Name = "Named";
			this.Named.Size = new System.Drawing.Size(138, 24);
			this.Named.TabIndex = 1;
			this.Named.Text = "Named group ${Name}";
			this.toolTip1.SetToolTip(this.Named, "Substitute the group with this name");
			this.Named.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// Numbered
			// 
			this.Numbered.Location = new System.Drawing.Point(16, 32);
			this.Numbered.Name = "Numbered";
			this.Numbered.Size = new System.Drawing.Size(134, 24);
			this.Numbered.TabIndex = 0;
			this.Numbered.Text = "Numbered group $1";
			this.toolTip1.SetToolTip(this.Numbered, "Substitute the group with this number");
			this.Numbered.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// NumberBox
			// 
			this.NumberBox.Location = new System.Drawing.Point(161, 35);
			this.NumberBox.Minimum = new System.Decimal(new int[] {
																															1,
																															0,
																															0,
																															0});
			this.NumberBox.Name = "NumberBox";
			this.NumberBox.Size = new System.Drawing.Size(64, 20);
			this.NumberBox.TabIndex = 4;
			this.toolTip1.SetToolTip(this.NumberBox, "Enter the number of the group");
			this.NumberBox.Value = new System.Decimal(new int[] {
																														1,
																														0,
																														0,
																														0});
			this.NumberBox.ValueChanged += new System.EventHandler(this.NumberBox_ValueChanged);
			// 
			// HideBtn
			// 
			this.HideBtn.Location = new System.Drawing.Point(480, 4);
			this.HideBtn.Name = "HideBtn";
			this.HideBtn.TabIndex = 20;
			this.HideBtn.Text = "&Hide";
			this.toolTip1.SetToolTip(this.HideBtn, "Hide the Regex Builder");
			this.HideBtn.Click += new System.EventHandler(this.HideBtn_Click);
			// 
			// Substitutions
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.HideBtn,
																																	this.groupBox1,
																																	this.label3,
																																	this.InsertBtn,
																																	this.Results});
			this.Name = "Substitutions";
			this.Size = new System.Drawing.Size(570, 315);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumberBox)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Handle changes to check boxes by figuring out which button was changed and
		/// generating the appropriate output
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Numbered_CheckedChanged(object sender, System.EventArgs e)
		{
			RadioButton radio = (RadioButton)sender;
			if(!radio.Checked)return;
			string Output="";
			switch(radio.Name)
			{
				case "Numbered" : Output = "$"+NumberBox.Value.ToString(); break;
				case "Named" : Output = "${"+NameBox.Text+"}"; break;
				case "Match" : Output = "$&"; break;
				case "Prior" : Output = "$`"; break;
				case "Following" : Output = "$\'"; break;
				case "Last" : Output = "$+"; break;
				case "Input" : Output = "$_"; break;
				default : Output=""; break;
			}
			Results.Text=Output;
		}

		private void NumberBox_ValueChanged(object sender, System.EventArgs e)
		{
			Numbered.Checked=true;
			Results.Text="$"+NumberBox.Value.ToString();
		}

		private void NameBox_TextChanged(object sender, System.EventArgs e)
		{
			Named.Checked=true;
			Results.Text="${"+NameBox.Text+"}";
		}

		/// <summary>
		/// Insert text into the regular expression
		/// </summary>
		private void InsertRegex()
		{
			Builder.InsertRegex(Results.Text,0);
		}

		/// <summary>
		/// Insert text into the replacement expression
		/// </summary>
		private void InsertReplaceText()
		{
			Builder.InsertReplaceText(Results.Text,0);
		}

		private void InsertBtn_Click(object sender, System.EventArgs e)
		{
			InsertReplaceText();
		}

		private void HideBtn_Click(object sender, System.EventArgs e)
		{
			Builder.ShowBuilder(false);
		}
	}
}
