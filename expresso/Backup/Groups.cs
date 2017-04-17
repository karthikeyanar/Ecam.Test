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
	enum GroupTypes {Numbered, Noncapturing, Named, Balancing, 
		MatchPrefix, MatchSuffix, NotMatchSuffix, NotMatchPrefix, Greedy, Comment};
	enum Options {IgnoreCase, Multiline, Singleline, ExplicitCapture, IgnorePattern};
	
	
	/// <summary>
	/// This form is used to generate the Group constructs
	/// </summary>
	public class Groups : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox Results;
		private System.Windows.Forms.RadioButton Numbered;
		private System.Windows.Forms.RadioButton MatchSuffix;
		private System.Windows.Forms.RadioButton Named;
		private System.Windows.Forms.RadioButton Noncapturing;
		private System.Windows.Forms.RadioButton Balancing;
		private System.Windows.Forms.RadioButton MatchPrefix;
		private System.Windows.Forms.RadioButton NotMatchSuffix;
		private System.Windows.Forms.RadioButton NotMatchPrefix;
		private System.Windows.Forms.RadioButton Greedy;
		private System.Windows.Forms.CheckBox IgnoreCase;
		private System.Windows.Forms.CheckBox Multiline;
		private System.Windows.Forms.CheckBox ExplicitCapture;
		private System.Windows.Forms.CheckBox IgnorePattern;
		private System.Windows.Forms.CheckBox Singleline;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TextBox Name1;
		private System.Windows.Forms.TextBox Name2;
		private System.Windows.Forms.GroupBox OptionBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox Within;
		private System.Windows.Forms.CheckBox Until;
		private System.Windows.Forms.GroupBox MatchBox;
		private System.Windows.Forms.RadioButton Comment;
		private System.Windows.Forms.TextBox CommentBox;
		private GroupTypes type;
		private System.Windows.Forms.Button InsertBtn;
		private System.Windows.Forms.Button HideBtn;
		private System.Windows.Forms.ToolTip toolTip1;
		private RegBuilder Builder;

		public Groups(RegBuilder builder)
		{
			InitializeComponent();
			Numbered.Checked=true;
			type=GroupTypes.Numbered;
			FormatResult();
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
			this.MatchBox = new System.Windows.Forms.GroupBox();
			this.CommentBox = new System.Windows.Forms.TextBox();
			this.Comment = new System.Windows.Forms.RadioButton();
			this.Greedy = new System.Windows.Forms.RadioButton();
			this.NotMatchPrefix = new System.Windows.Forms.RadioButton();
			this.NotMatchSuffix = new System.Windows.Forms.RadioButton();
			this.MatchPrefix = new System.Windows.Forms.RadioButton();
			this.Name2 = new System.Windows.Forms.TextBox();
			this.Balancing = new System.Windows.Forms.RadioButton();
			this.Name1 = new System.Windows.Forms.TextBox();
			this.MatchSuffix = new System.Windows.Forms.RadioButton();
			this.Named = new System.Windows.Forms.RadioButton();
			this.Noncapturing = new System.Windows.Forms.RadioButton();
			this.Numbered = new System.Windows.Forms.RadioButton();
			this.OptionBox = new System.Windows.Forms.GroupBox();
			this.IgnorePattern = new System.Windows.Forms.CheckBox();
			this.ExplicitCapture = new System.Windows.Forms.CheckBox();
			this.Singleline = new System.Windows.Forms.CheckBox();
			this.Multiline = new System.Windows.Forms.CheckBox();
			this.IgnoreCase = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.Until = new System.Windows.Forms.CheckBox();
			this.Within = new System.Windows.Forms.CheckBox();
			this.HideBtn = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.MatchBox.SuspendLayout();
			this.OptionBox.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 23);
			this.label3.TabIndex = 9;
			this.label3.Text = "Regular Expression";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// InsertBtn
			// 
			this.InsertBtn.Location = new System.Drawing.Point(400, 4);
			this.InsertBtn.Name = "InsertBtn";
			this.InsertBtn.TabIndex = 7;
			this.InsertBtn.Text = "&Insert";
			this.toolTip1.SetToolTip(this.InsertBtn, "Insert the subexpression");
			this.InsertBtn.Click += new System.EventHandler(this.InsertBtn_Click);
			// 
			// Results
			// 
			this.Results.Location = new System.Drawing.Point(114, 5);
			this.Results.Name = "Results";
			this.Results.Size = new System.Drawing.Size(273, 20);
			this.Results.TabIndex = 6;
			this.Results.Text = "";
			// 
			// MatchBox
			// 
			this.MatchBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																					 this.CommentBox,
																																					 this.Comment,
																																					 this.Greedy,
																																					 this.NotMatchPrefix,
																																					 this.NotMatchSuffix,
																																					 this.MatchPrefix,
																																					 this.Name2,
																																					 this.Balancing,
																																					 this.Name1,
																																					 this.MatchSuffix,
																																					 this.Named,
																																					 this.Noncapturing,
																																					 this.Numbered});
			this.MatchBox.Location = new System.Drawing.Point(16, 30);
			this.MatchBox.Name = "MatchBox";
			this.MatchBox.Size = new System.Drawing.Size(264, 280);
			this.MatchBox.TabIndex = 10;
			this.MatchBox.TabStop = false;
			this.MatchBox.Text = "Define a match group";
			// 
			// CommentBox
			// 
			this.CommentBox.Location = new System.Drawing.Point(136, 240);
			this.CommentBox.Name = "CommentBox";
			this.CommentBox.TabIndex = 13;
			this.CommentBox.Text = "A Comment";
			// 
			// Comment
			// 
			this.Comment.Location = new System.Drawing.Point(16, 240);
			this.Comment.Name = "Comment";
			this.Comment.Size = new System.Drawing.Size(112, 24);
			this.Comment.TabIndex = 12;
			this.Comment.Text = "Comment (?#)";
			this.toolTip1.SetToolTip(this.Comment, "Comments may be inserted and will be ignored");
			this.Comment.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// Greedy
			// 
			this.Greedy.Location = new System.Drawing.Point(16, 216);
			this.Greedy.Name = "Greedy";
			this.Greedy.Size = new System.Drawing.Size(183, 24);
			this.Greedy.TabIndex = 11;
			this.Greedy.Text = "Greedy subexpression (?>)";
			this.toolTip1.SetToolTip(this.Greedy, "Nonbacktracking subexpression");
			this.Greedy.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// NotMatchPrefix
			// 
			this.NotMatchPrefix.Location = new System.Drawing.Point(16, 192);
			this.NotMatchPrefix.Name = "NotMatchPrefix";
			this.NotMatchPrefix.Size = new System.Drawing.Size(189, 24);
			this.NotMatchPrefix.TabIndex = 10;
			this.NotMatchPrefix.Text = "Match if prefix is not present (?!)";
			this.toolTip1.SetToolTip(this.NotMatchPrefix, "Prefix must be absent for a match.");
			this.NotMatchPrefix.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// NotMatchSuffix
			// 
			this.NotMatchSuffix.Location = new System.Drawing.Point(16, 168);
			this.NotMatchSuffix.Name = "NotMatchSuffix";
			this.NotMatchSuffix.Size = new System.Drawing.Size(200, 24);
			this.NotMatchSuffix.TabIndex = 9;
			this.NotMatchSuffix.Text = "Match if suffix is not present (?<!)";
			this.toolTip1.SetToolTip(this.NotMatchSuffix, "Suffix must be absent for a match.");
			this.NotMatchSuffix.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// MatchPrefix
			// 
			this.MatchPrefix.Location = new System.Drawing.Point(16, 144);
			this.MatchPrefix.Name = "MatchPrefix";
			this.MatchPrefix.Size = new System.Drawing.Size(185, 24);
			this.MatchPrefix.TabIndex = 8;
			this.MatchPrefix.Text = "Match prefix but exclude it (?<=)";
			this.toolTip1.SetToolTip(this.MatchPrefix, "Prefix is required for match but will be excluded from the final matched string");
			this.MatchPrefix.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// Name2
			// 
			this.Name2.Location = new System.Drawing.Point(183, 95);
			this.Name2.Name = "Name2";
			this.Name2.Size = new System.Drawing.Size(75, 20);
			this.Name2.TabIndex = 7;
			this.Name2.Text = "Name2";
			this.Name2.TextChanged += new System.EventHandler(this.Name1_TextChanged);
			// 
			// Balancing
			// 
			this.Balancing.Location = new System.Drawing.Point(16, 96);
			this.Balancing.Name = "Balancing";
			this.Balancing.Size = new System.Drawing.Size(160, 24);
			this.Balancing.TabIndex = 6;
			this.Balancing.Text = "Balancing Grp (?<N1-N2>)";
			this.toolTip1.SetToolTip(this.Balancing, "This is complicated!");
			this.Balancing.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// Name1
			// 
			this.Name1.Location = new System.Drawing.Point(183, 71);
			this.Name1.Name = "Name1";
			this.Name1.Size = new System.Drawing.Size(75, 20);
			this.Name1.TabIndex = 5;
			this.Name1.Text = "Name";
			this.Name1.TextChanged += new System.EventHandler(this.Name1_TextChanged);
			// 
			// MatchSuffix
			// 
			this.MatchSuffix.Location = new System.Drawing.Point(16, 120);
			this.MatchSuffix.Name = "MatchSuffix";
			this.MatchSuffix.Size = new System.Drawing.Size(180, 24);
			this.MatchSuffix.TabIndex = 4;
			this.MatchSuffix.Text = "Match suffix but exclude it (?=)";
			this.toolTip1.SetToolTip(this.MatchSuffix, "Suffix is required for match but will be excluded from the final matched string");
			this.MatchSuffix.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// Named
			// 
			this.Named.Location = new System.Drawing.Point(16, 72);
			this.Named.Name = "Named";
			this.Named.Size = new System.Drawing.Size(162, 24);
			this.Named.TabIndex = 3;
			this.Named.Text = "Named Capture (?<Name>)";
			this.toolTip1.SetToolTip(this.Named, "Capture the substring and assign an explicit name to the group.");
			this.Named.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// Noncapturing
			// 
			this.Noncapturing.Location = new System.Drawing.Point(16, 48);
			this.Noncapturing.Name = "Noncapturing";
			this.Noncapturing.Size = new System.Drawing.Size(155, 24);
			this.Noncapturing.TabIndex = 2;
			this.Noncapturing.Text = "Noncapturing Group (?:)";
			this.toolTip1.SetToolTip(this.Noncapturing, "Include text in the match, but do not capture the group");
			this.Noncapturing.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// Numbered
			// 
			this.Numbered.Location = new System.Drawing.Point(16, 24);
			this.Numbered.Name = "Numbered";
			this.Numbered.Size = new System.Drawing.Size(139, 24);
			this.Numbered.TabIndex = 0;
			this.Numbered.Text = "Numbered Capture ()";
			this.toolTip1.SetToolTip(this.Numbered, "Captures a substring. Groups will be numbered sequentially.");
			this.Numbered.CheckedChanged += new System.EventHandler(this.GroupChanged);
			// 
			// OptionBox
			// 
			this.OptionBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						this.IgnorePattern,
																																						this.ExplicitCapture,
																																						this.Singleline,
																																						this.Multiline,
																																						this.IgnoreCase});
			this.OptionBox.Enabled = false;
			this.OptionBox.Location = new System.Drawing.Point(294, 126);
			this.OptionBox.Name = "OptionBox";
			this.OptionBox.Size = new System.Drawing.Size(260, 184);
			this.OptionBox.TabIndex = 11;
			this.OptionBox.TabStop = false;
			this.OptionBox.Text = "Options";
			// 
			// IgnorePattern
			// 
			this.IgnorePattern.Location = new System.Drawing.Point(16, 120);
			this.IgnorePattern.Name = "IgnorePattern";
			this.IgnorePattern.Size = new System.Drawing.Size(176, 24);
			this.IgnorePattern.TabIndex = 4;
			this.IgnorePattern.Text = "Ignore Pattern Whitespace x";
			this.toolTip1.SetToolTip(this.IgnorePattern, "Unescaped whitespace is ignored (allows use of X-mode comments)");
			this.IgnorePattern.CheckedChanged += new System.EventHandler(this.Name1_TextChanged);
			// 
			// ExplicitCapture
			// 
			this.ExplicitCapture.Location = new System.Drawing.Point(16, 96);
			this.ExplicitCapture.Name = "ExplicitCapture";
			this.ExplicitCapture.Size = new System.Drawing.Size(112, 24);
			this.ExplicitCapture.TabIndex = 3;
			this.ExplicitCapture.Text = "Explicit Capture n";
			this.toolTip1.SetToolTip(this.ExplicitCapture, "Groups without a name or number will not be captured");
			this.ExplicitCapture.CheckedChanged += new System.EventHandler(this.Name1_TextChanged);
			// 
			// Singleline
			// 
			this.Singleline.Location = new System.Drawing.Point(16, 72);
			this.Singleline.Name = "Singleline";
			this.Singleline.Size = new System.Drawing.Size(112, 24);
			this.Singleline.TabIndex = 2;
			this.Singleline.Text = "Single Line s";
			this.toolTip1.SetToolTip(this.Singleline, "Period (.) matches every character including new line");
			this.Singleline.CheckedChanged += new System.EventHandler(this.Name1_TextChanged);
			// 
			// Multiline
			// 
			this.Multiline.Location = new System.Drawing.Point(16, 48);
			this.Multiline.Name = "Multiline";
			this.Multiline.Size = new System.Drawing.Size(112, 24);
			this.Multiline.TabIndex = 1;
			this.Multiline.Text = "Multiline m";
			this.toolTip1.SetToolTip(this.Multiline, "^ and $ match at beginning and end of each line");
			this.Multiline.CheckedChanged += new System.EventHandler(this.Name1_TextChanged);
			// 
			// IgnoreCase
			// 
			this.IgnoreCase.Location = new System.Drawing.Point(16, 24);
			this.IgnoreCase.Name = "IgnoreCase";
			this.IgnoreCase.Size = new System.Drawing.Size(112, 24);
			this.IgnoreCase.TabIndex = 0;
			this.IgnoreCase.Text = "Ignore Case i";
			this.toolTip1.SetToolTip(this.IgnoreCase, "Don\'t distinguish betwen upper and lower case");
			this.IgnoreCase.CheckedChanged += new System.EventHandler(this.Name1_TextChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						this.Until,
																																						this.Within});
			this.groupBox2.Location = new System.Drawing.Point(294, 30);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(260, 88);
			this.groupBox2.TabIndex = 12;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Change Options";
			// 
			// Until
			// 
			this.Until.Location = new System.Drawing.Point(16, 47);
			this.Until.Name = "Until";
			this.Until.Size = new System.Drawing.Size(221, 24);
			this.Until.TabIndex = 6;
			this.Until.Text = "Until end of enclosing group (?imsnx)";
			this.toolTip1.SetToolTip(this.Until, "Changes the options for the rest of the current group");
			this.Until.CheckedChanged += new System.EventHandler(this.Until_CheckedChanged);
			// 
			// Within
			// 
			this.Within.Location = new System.Drawing.Point(15, 21);
			this.Within.Name = "Within";
			this.Within.Size = new System.Drawing.Size(240, 24);
			this.Within.TabIndex = 5;
			this.Within.Text = "Within a new noncapturing group (?imsnx:)";
			this.toolTip1.SetToolTip(this.Within, "Creates a group within which new options apply");
			this.Within.CheckedChanged += new System.EventHandler(this.Within_CheckedChanged);
			// 
			// HideBtn
			// 
			this.HideBtn.Location = new System.Drawing.Point(480, 4);
			this.HideBtn.Name = "HideBtn";
			this.HideBtn.TabIndex = 13;
			this.HideBtn.Text = "&Hide";
			this.toolTip1.SetToolTip(this.HideBtn, "Hide the Regex Builder");
			this.HideBtn.Click += new System.EventHandler(this.HideBtn_Click);
			// 
			// Groups
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.HideBtn,
																																	this.groupBox2,
																																	this.OptionBox,
																																	this.MatchBox,
																																	this.label3,
																																	this.InsertBtn,
																																	this.Results});
			this.Name = "Groups";
			this.Size = new System.Drawing.Size(570, 315);
			this.MatchBox.ResumeLayout(false);
			this.OptionBox.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Set an enum when a radio button is clicked, then format the results
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GroupChanged(object sender, System.EventArgs e)
		{
			RadioButton radio = (RadioButton)sender;
			if(!radio.Checked)return;
			switch(radio.Name)
			{
				case "Numbered" : type=GroupTypes.Numbered; break;
				case "Noncapturing" : type=GroupTypes.Noncapturing;	break;
				case "Named" : type=GroupTypes.Named; break;
				case "Balancing" : type=GroupTypes.Balancing; break;
				case "MatchPrefix" : type=GroupTypes.MatchPrefix; break;
				case "MatchSuffix" : type=GroupTypes.MatchSuffix; break;
				case "NotMatchSuffix" : type=GroupTypes.NotMatchSuffix; break;
				case "NotMatchPrefix" : type=GroupTypes.NotMatchPrefix; break;
				case "Greedy" : type=GroupTypes.Greedy; break;
				case "Comment" : type=GroupTypes.Comment; break;
				default : 
					type=GroupTypes.Numbered;
					MessageBox.Show("Oops!");
					break;
			}
			Within.Checked=false;
			Until.Checked=false;
			FormatResult();
		}

		/// <summary>
		/// Format the results depending on what type of group is selected
		/// </summary>
		private void FormatResult()
		{
			string GroupTag;
			switch(type)
			{
				case GroupTypes.Balancing: 
					GroupTag="(?<"+Name1.Text+"-"+Name2.Text+">)";
					break;
				case GroupTypes.MatchPrefix: 
					GroupTag="(?<=)";
					break;
				case GroupTypes.MatchSuffix: 
					GroupTag="(?=)";
					break;
				case GroupTypes.Named: 
					GroupTag="(?<"+Name1.Text+">)";
					break;
				case GroupTypes.Noncapturing: 
					GroupTag="(?:)";
					break;
				case GroupTypes.NotMatchPrefix: 
					GroupTag="(?!)";
					break;
				case GroupTypes.NotMatchSuffix: 
					GroupTag="(?<!)";
					break;
				case GroupTypes.Numbered: 
					GroupTag="()";
					break;
				case GroupTypes.Greedy: 
					GroupTag="(?>)";
					break;
				case GroupTypes.Comment: 
					GroupTag="(?#"+CommentBox.Text+")";
					break;
				default:
					GroupTag="";
					break;
			}

			// Now figure out Options
			string PosOptions,NegOptions;
			if(Within.Checked || Until.Checked)
			{
				PosOptions="";
				NegOptions="";
				if(IgnoreCase.Checked)PosOptions+="i";
				else NegOptions+="i";
				if(Multiline.Checked)PosOptions+="m";
				else NegOptions+="m";
				if(Singleline.Checked)PosOptions+="s";
				else NegOptions+="s";
				if(ExplicitCapture.Checked)PosOptions+="n";
				else NegOptions+="n";
				if(IgnorePattern.Checked)PosOptions+="x";
				else NegOptions+="x";

				// Now form the expression
				string Options=PosOptions+"-"+NegOptions;
				if(Within.Checked)
					GroupTag="(?"+Options+":)";
				else
					GroupTag="(?"+Options+")";
			}
			Results.Text=GroupTag;
		}

		private void Name1_TextChanged(object sender, System.EventArgs e)
		{
			FormatResult();
		}

		private void Within_CheckedChanged(object sender, System.EventArgs e)
		{
			if(Within.Checked)
			{
				Until.Checked=false;
				OptionBox.Enabled=true;
				SetButtons(false);
			}
			else if(!Until.Checked)
			{
				OptionBox.Enabled=false;
			}
			FormatResult();
		}

		private void Until_CheckedChanged(object sender, System.EventArgs e)
		{
			if(Until.Checked)
			{
				Within.Checked=false;
				OptionBox.Enabled=true;
				SetButtons(false);
			}
			else if(!Within.Checked)
			{
				OptionBox.Enabled=false;
			}
			FormatResult();
		}

		private void SetButtons(bool Value)
		{
			Control radio=(Control)Numbered;
			do
			{
				if(radio is RadioButton)((RadioButton)radio).Checked=Value;
				radio=MatchBox.GetNextControl(radio,true);
			}
			while (radio != Numbered);
		}

		/// <summary>
		/// Insert text into the regular expression
		/// </summary>
		private void InsertRegex()
		{
			Builder.InsertRegex(Results.Text,1);
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
