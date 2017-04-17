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
	/// This is the RegBuilder tab page for Miscellaneous expressions
	/// </summary>
	public class Misc : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox Results;
		private System.Windows.Forms.NumericUpDown NumberBox;
		private System.Windows.Forms.TextBox NameBox;
		private System.Windows.Forms.RadioButton Named;
		private System.Windows.Forms.RadioButton Numbered;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton Alternatives;
		private System.Windows.Forms.RadioButton Expression;
		private System.Windows.Forms.RadioButton Group;
		private System.Windows.Forms.TextBox YesBox;
		private System.Windows.Forms.TextBox NoBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox ExpressionBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox GroupBox;
		private System.Windows.Forms.RadioButton XMode;
		private System.Windows.Forms.TextBox CommentBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button InsertBtn;
		private System.Windows.Forms.Button HideBtn;
		private System.Windows.Forms.ToolTip toolTip1;
		private RegBuilder Builder;

		public Misc(RegBuilder builder)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Builder=builder;

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
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.XMode = new System.Windows.Forms.RadioButton();
			this.Alternatives = new System.Windows.Forms.RadioButton();
			this.CommentBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.Group = new System.Windows.Forms.RadioButton();
			this.NoBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.YesBox = new System.Windows.Forms.TextBox();
			this.GroupBox = new System.Windows.Forms.TextBox();
			this.Expression = new System.Windows.Forms.RadioButton();
			this.ExpressionBox = new System.Windows.Forms.TextBox();
			this.Named = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.Numbered = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.NameBox = new System.Windows.Forms.TextBox();
			this.NumberBox = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.InsertBtn = new System.Windows.Forms.Button();
			this.Results = new System.Windows.Forms.TextBox();
			this.HideBtn = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox2.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumberBox)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						this.XMode,
																																						this.Alternatives,
																																						this.CommentBox,
																																						this.label2,
																																						this.label4,
																																						this.label5,
																																						this.Group,
																																						this.NoBox,
																																						this.label6,
																																						this.YesBox,
																																						this.GroupBox,
																																						this.Expression,
																																						this.ExpressionBox,
																																						this.Named,
																																						this.label1,
																																						this.Numbered,
																																						this.panel1,
																																						this.panel2});
			this.groupBox2.Location = new System.Drawing.Point(31, 32);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(508, 272);
			this.groupBox2.TabIndex = 21;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Miscellaneous constructions";
			// 
			// XMode
			// 
			this.XMode.Location = new System.Drawing.Point(30, 172);
			this.XMode.Name = "XMode";
			this.XMode.Size = new System.Drawing.Size(217, 48);
			this.XMode.TabIndex = 16;
			this.XMode.Text = "X-mode comment (requires Ignore Pattern Whitespace option) #Comment";
			this.toolTip1.SetToolTip(this.XMode, "Place a comment at the end of the regular expression");
			this.XMode.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// Alternatives
			// 
			this.Alternatives.Location = new System.Drawing.Point(30, 140);
			this.Alternatives.Name = "Alternatives";
			this.Alternatives.Size = new System.Drawing.Size(214, 32);
			this.Alternatives.TabIndex = 5;
			this.Alternatives.Text = "Delimit a group of alternatives |";
			this.toolTip1.SetToolTip(this.Alternatives, "Use this delimiter to separate a list of alternatives within a group");
			this.Alternatives.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// CommentBox
			// 
			this.CommentBox.Location = new System.Drawing.Point(72, 232);
			this.CommentBox.Name = "CommentBox";
			this.CommentBox.TabIndex = 16;
			this.CommentBox.Text = "Comment";
			this.CommentBox.TextChanged += new System.EventHandler(this.CommentBox_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(290, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 23);
			this.label2.TabIndex = 10;
			this.label2.Text = "Yes String:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(306, 73);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 23);
			this.label4.TabIndex = 11;
			this.label4.Text = "No String:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(298, 145);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 23);
			this.label5.TabIndex = 13;
			this.label5.Text = "Expression:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// Group
			// 
			this.Group.Location = new System.Drawing.Point(290, 185);
			this.Group.Name = "Group";
			this.Group.Size = new System.Drawing.Size(160, 32);
			this.Group.TabIndex = 7;
			this.Group.Text = "Select alternatives based on match of a group";
			this.Group.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// NoBox
			// 
			this.NoBox.Location = new System.Drawing.Point(378, 73);
			this.NoBox.Name = "NoBox";
			this.NoBox.TabIndex = 9;
			this.NoBox.Text = "no";
			this.toolTip1.SetToolTip(this.NoBox, "Subexpression used if match is not successful");
			this.NoBox.TextChanged += new System.EventHandler(this.NoBox_TextChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(290, 217);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(80, 23);
			this.label6.TabIndex = 15;
			this.label6.Text = "Group name:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// YesBox
			// 
			this.YesBox.Location = new System.Drawing.Point(378, 41);
			this.YesBox.Name = "YesBox";
			this.YesBox.TabIndex = 8;
			this.YesBox.Text = "yes";
			this.toolTip1.SetToolTip(this.YesBox, "Subexpression used if match is successful");
			this.YesBox.TextChanged += new System.EventHandler(this.YesBox_TextChanged);
			// 
			// GroupBox
			// 
			this.GroupBox.Location = new System.Drawing.Point(378, 217);
			this.GroupBox.Name = "GroupBox";
			this.GroupBox.TabIndex = 14;
			this.GroupBox.Text = "Name";
			this.toolTip1.SetToolTip(this.GroupBox, "Group used to test for match");
			this.GroupBox.TextChanged += new System.EventHandler(this.GroupBox_TextChanged);
			// 
			// Expression
			// 
			this.Expression.Location = new System.Drawing.Point(290, 113);
			this.Expression.Name = "Expression";
			this.Expression.Size = new System.Drawing.Size(160, 32);
			this.Expression.TabIndex = 6;
			this.Expression.Text = "Select alternatives based on match of an expression";
			this.Expression.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// ExpressionBox
			// 
			this.ExpressionBox.Location = new System.Drawing.Point(378, 145);
			this.ExpressionBox.Name = "ExpressionBox";
			this.ExpressionBox.TabIndex = 12;
			this.ExpressionBox.Text = "expression";
			this.toolTip1.SetToolTip(this.ExpressionBox, "Subexpression used to test for match");
			this.ExpressionBox.TextChanged += new System.EventHandler(this.ExpressionBox_TextChanged);
			// 
			// Named
			// 
			this.Named.Location = new System.Drawing.Point(30, 79);
			this.Named.Name = "Named";
			this.Named.Size = new System.Drawing.Size(118, 24);
			this.Named.TabIndex = 1;
			this.Named.Text = "Named \\k<Name>";
			this.toolTip1.SetToolTip(this.Named, "A named group");
			this.Named.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "Reference a group";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.label1, "Match a previously captured group");
			// 
			// Numbered
			// 
			this.Numbered.Location = new System.Drawing.Point(30, 55);
			this.Numbered.Name = "Numbered";
			this.Numbered.Size = new System.Drawing.Size(109, 24);
			this.Numbered.TabIndex = 0;
			this.Numbered.Text = "Numbered \\k1";
			this.toolTip1.SetToolTip(this.Numbered, "A numbered group");
			this.Numbered.CheckedChanged += new System.EventHandler(this.Numbered_CheckedChanged);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Location = new System.Drawing.Point(274, 25);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(216, 232);
			this.panel1.TabIndex = 17;
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																																				 this.NameBox,
																																				 this.NumberBox});
			this.panel2.Location = new System.Drawing.Point(19, 24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(203, 100);
			this.panel2.TabIndex = 18;
			// 
			// NameBox
			// 
			this.NameBox.Location = new System.Drawing.Point(128, 57);
			this.NameBox.Name = "NameBox";
			this.NameBox.Size = new System.Drawing.Size(64, 20);
			this.NameBox.TabIndex = 2;
			this.NameBox.Text = "Name";
			this.NameBox.TextChanged += new System.EventHandler(this.NameBox_TextChanged_1);
			// 
			// NumberBox
			// 
			this.NumberBox.Location = new System.Drawing.Point(128, 33);
			this.NumberBox.Minimum = new System.Decimal(new int[] {
																															1,
																															0,
																															0,
																															0});
			this.NumberBox.Name = "NumberBox";
			this.NumberBox.Size = new System.Drawing.Size(64, 20);
			this.NumberBox.TabIndex = 3;
			this.NumberBox.Value = new System.Decimal(new int[] {
																														1,
																														0,
																														0,
																														0});
			this.NumberBox.ValueChanged += new System.EventHandler(this.NumberBox_ValueChanged_1);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 23);
			this.label3.TabIndex = 18;
			this.label3.Text = "Regular Expression";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// InsertBtn
			// 
			this.InsertBtn.Location = new System.Drawing.Point(400, 4);
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
			// HideBtn
			// 
			this.HideBtn.Location = new System.Drawing.Point(480, 4);
			this.HideBtn.Name = "HideBtn";
			this.HideBtn.TabIndex = 22;
			this.HideBtn.Text = "&Hide";
			this.toolTip1.SetToolTip(this.HideBtn, "Hide the Regex Builder");
			this.HideBtn.Click += new System.EventHandler(this.HideBtn_Click);
			// 
			// Misc
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.HideBtn,
																																	this.groupBox2,
																																	this.InsertBtn,
																																	this.label3,
																																	this.Results});
			this.Name = "Misc";
			this.Size = new System.Drawing.Size(570, 315);
			this.groupBox2.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumberBox)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		
		/// <summary>
		/// Respond to changes in radio buttons and format the results
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
				case "Numbered" : Output = "\\k"+NumberBox.Value.ToString(); break;
				case "Named" : Output = "\\k<"+NameBox.Text+">"; break;
				case "Alternatives" : Output = "|"; break;
				case "XMode" : Output = "#"+CommentBox.Text; break;
				case "Expression" : 
					Output = "(?("+ExpressionBox.Text+")"+
						YesBox.Text+"|"+NoBox.Text+")"; 
					break;
				case "Group" : 
					Output = "(?("+NameBox.Text+")"+
						YesBox.Text+"|"+NoBox.Text+")"; 
					break;
				default : Output=""; break;
			}
			Results.Text=Output;
		}

		private void NumberBox_ValueChanged(object sender, System.EventArgs e)
		{
			Numbered.Checked=true;
			Results.Text="\\k"+NumberBox.Value.ToString();
		}

		private void NameBox_TextChanged(object sender, System.EventArgs e)
		{
			Named.Checked=true;
			Results.Text="\\k<"+NameBox.Text+">";
		}

		private void CommentBox_TextChanged(object sender, System.EventArgs e)
		{
			XMode.Checked=true;
			Results.Text="#"+CommentBox.Text;
		}

		private void YesBox_TextChanged(object sender, System.EventArgs e)
		{
			if(Group.Checked)
			{
				Results.Text= "(?("+NameBox.Text+")"+
					YesBox.Text+"|"+NoBox.Text+")";
			}
			else 
			{
				if(!Expression.Checked)Expression.Checked=true;
				Results.Text="(?("+ExpressionBox.Text+")"+
					YesBox.Text+"|"+NoBox.Text+")";			}
		}

		private void NoBox_TextChanged(object sender, System.EventArgs e)
		{
			if(Group.Checked)
			{
				Results.Text= "(?("+NameBox.Text+")"+
					YesBox.Text+"|"+NoBox.Text+")";
			}
			else 
			{
				if(!Expression.Checked)Expression.Checked=true;
				Results.Text="(?("+ExpressionBox.Text+")"+
					YesBox.Text+"|"+NoBox.Text+")";			}
		}

		private void ExpressionBox_TextChanged(object sender, System.EventArgs e)
		{
			Expression.Checked=true;
		}

		private void GroupBox_TextChanged(object sender, System.EventArgs e)
		{
			Group.Checked=true;
		}

		private void NameBox_TextChanged_1(object sender, System.EventArgs e)
		{
			Named.Checked=true;
		}

		private void NumberBox_ValueChanged_1(object sender, System.EventArgs e)
		{
			Numbered.Checked=true;
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
			InsertRegex();
		}

		private void HideBtn_Click(object sender, System.EventArgs e)
		{
			Builder.ShowBuilder(false);
		}
	}
}
