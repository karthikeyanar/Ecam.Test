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
	enum CharacterClasses {Anything,Alphanumeric,Digit,Whitespace,Specific, Named,Specified};
	enum Repetitions {One,Any,OneOrMore,ZeroOrOne,N,AtLeastN,FromNToM};
	
	/// <summary>
	/// This form is used to build a selection for a group of characters
	/// </summary>
	public class Characters : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton Alphanumeric;
		private System.Windows.Forms.RadioButton Digit;
		private System.Windows.Forms.RadioButton Whitespace;
		private System.Windows.Forms.RadioButton NamedClass;
		private System.Windows.Forms.RadioButton SpecifiedSet;
		private System.Windows.Forms.RadioButton Once;
		private System.Windows.Forms.RadioButton Any;
		private System.Windows.Forms.CheckBox Absent;
		private System.Windows.Forms.CheckBox Lazy;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton OneOrMore;
		private System.Windows.Forms.RadioButton ZeroOrOne;
		private System.Windows.Forms.ComboBox NamedBox;
		private System.Windows.Forms.ComboBox SpecifiedBox;
		private System.Windows.Forms.RadioButton ExactlyN;
		private System.Windows.Forms.NumericUpDown NBox;
		private System.Windows.Forms.NumericUpDown MBox;
		private System.Windows.Forms.RadioButton AtLeastN;
		private System.Windows.Forms.RadioButton FromNtoM;
		public System.Windows.Forms.TextBox Results;
		private System.ComponentModel.IContainer components;
		public string Value;
		private CharacterClasses CharacterClass;
		private System.Windows.Forms.Label label3;
		private Repetitions Repetition;
		private System.Windows.Forms.Button InsertBtn;
		private System.Windows.Forms.Button HideBtn;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.RadioButton Anything;
		private System.Windows.Forms.RadioButton Specific;
		private System.Windows.Forms.TextBox TheCharacter;
		private RegBuilder Builder;

		public Characters(RegBuilder builder)
		{
			InitializeComponent();

			NamedBox.SelectedIndex=0;
			SpecifiedBox.SelectedIndex=0;
			Alphanumeric.Checked=true;
			Any.Checked=true;
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.SpecifiedBox = new System.Windows.Forms.ComboBox();
			this.NamedBox = new System.Windows.Forms.ComboBox();
			this.SpecifiedSet = new System.Windows.Forms.RadioButton();
			this.NamedClass = new System.Windows.Forms.RadioButton();
			this.Whitespace = new System.Windows.Forms.RadioButton();
			this.Digit = new System.Windows.Forms.RadioButton();
			this.Alphanumeric = new System.Windows.Forms.RadioButton();
			this.Absent = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.FromNtoM = new System.Windows.Forms.RadioButton();
			this.AtLeastN = new System.Windows.Forms.RadioButton();
			this.MBox = new System.Windows.Forms.NumericUpDown();
			this.NBox = new System.Windows.Forms.NumericUpDown();
			this.ExactlyN = new System.Windows.Forms.RadioButton();
			this.ZeroOrOne = new System.Windows.Forms.RadioButton();
			this.OneOrMore = new System.Windows.Forms.RadioButton();
			this.Any = new System.Windows.Forms.RadioButton();
			this.Once = new System.Windows.Forms.RadioButton();
			this.Lazy = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.Results = new System.Windows.Forms.TextBox();
			this.InsertBtn = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.HideBtn = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.Anything = new System.Windows.Forms.RadioButton();
			this.Specific = new System.Windows.Forms.RadioButton();
			this.TheCharacter = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NBox)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						this.TheCharacter,
																																						this.Specific,
																																						this.SpecifiedBox,
																																						this.NamedBox,
																																						this.SpecifiedSet,
																																						this.NamedClass,
																																						this.Whitespace,
																																						this.Digit,
																																						this.Alphanumeric,
																																						this.Absent,
																																						this.Anything});
			this.groupBox1.Location = new System.Drawing.Point(29, 30);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 280);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Character class";
			this.toolTip1.SetToolTip(this.groupBox1, "Match any member of a class of characters");
			// 
			// SpecifiedBox
			// 
			this.SpecifiedBox.Enabled = false;
			this.SpecifiedBox.Items.AddRange(new object[] {
																											"a-zA-Z",
																											"a-z",
																											"A-Z",
																											"0-9",
																											"aeiou"});
			this.SpecifiedBox.Location = new System.Drawing.Point(59, 238);
			this.SpecifiedBox.Name = "SpecifiedBox";
			this.SpecifiedBox.Size = new System.Drawing.Size(121, 21);
			this.SpecifiedBox.TabIndex = 7;
			this.toolTip1.SetToolTip(this.SpecifiedBox, "List the desired characters here");
			this.SpecifiedBox.TextChanged += new System.EventHandler(this.CheckChanged);
			this.SpecifiedBox.SelectedIndexChanged += new System.EventHandler(this.CheckChanged);
			// 
			// NamedBox
			// 
			this.NamedBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.NamedBox.Enabled = false;
			this.NamedBox.Items.AddRange(new object[] {
																									"Ll",
																									"Nd",
																									"Z",
																									"IsGreek",
																									"IsBoxDrawing"});
			this.NamedBox.Location = new System.Drawing.Point(59, 193);
			this.NamedBox.Name = "NamedBox";
			this.NamedBox.Size = new System.Drawing.Size(121, 21);
			this.NamedBox.TabIndex = 6;
			this.toolTip1.SetToolTip(this.NamedBox, "Enter class name here");
			this.NamedBox.SelectedIndexChanged += new System.EventHandler(this.CheckChanged);
			// 
			// SpecifiedSet
			// 
			this.SpecifiedSet.Location = new System.Drawing.Point(32, 213);
			this.SpecifiedSet.Name = "SpecifiedSet";
			this.SpecifiedSet.Size = new System.Drawing.Size(140, 24);
			this.SpecifiedSet.TabIndex = 4;
			this.SpecifiedSet.Text = "Specified Set [a-zA-Z]";
			this.toolTip1.SetToolTip(this.SpecifiedSet, "Specify a class explicity using ranges if desired");
			this.SpecifiedSet.CheckedChanged += new System.EventHandler(this.ClassChanged);
			// 
			// NamedClass
			// 
			this.NamedClass.Location = new System.Drawing.Point(32, 168);
			this.NamedClass.Name = "NamedClass";
			this.NamedClass.Size = new System.Drawing.Size(149, 24);
			this.NamedClass.TabIndex = 3;
			this.NamedClass.Text = "Named Class \\p{Class}";
			this.toolTip1.SetToolTip(this.NamedClass, "Characters from classes defined for Unicode characters");
			this.NamedClass.CheckedChanged += new System.EventHandler(this.ClassChanged);
			// 
			// Whitespace
			// 
			this.Whitespace.Location = new System.Drawing.Point(32, 120);
			this.Whitespace.Name = "Whitespace";
			this.Whitespace.TabIndex = 2;
			this.Whitespace.Text = "Whitespace \\s";
			this.toolTip1.SetToolTip(this.Whitespace, "Includes spaces, tabs, form feed, new line and carriage return");
			this.Whitespace.CheckedChanged += new System.EventHandler(this.ClassChanged);
			// 
			// Digit
			// 
			this.Digit.Location = new System.Drawing.Point(32, 96);
			this.Digit.Name = "Digit";
			this.Digit.TabIndex = 1;
			this.Digit.Text = "Digit \\d";
			this.toolTip1.SetToolTip(this.Digit, "Numerals 0 to 9");
			this.Digit.CheckedChanged += new System.EventHandler(this.ClassChanged);
			// 
			// Alphanumeric
			// 
			this.Alphanumeric.Location = new System.Drawing.Point(32, 72);
			this.Alphanumeric.Name = "Alphanumeric";
			this.Alphanumeric.Size = new System.Drawing.Size(110, 24);
			this.Alphanumeric.TabIndex = 0;
			this.Alphanumeric.Text = "Alphanumeric \\w";
			this.toolTip1.SetToolTip(this.Alphanumeric, "All alphabetic and numeric characters plus underscore");
			this.Alphanumeric.CheckedChanged += new System.EventHandler(this.ClassChanged);
			// 
			// Absent
			// 
			this.Absent.Location = new System.Drawing.Point(16, 24);
			this.Absent.Name = "Absent";
			this.Absent.Size = new System.Drawing.Size(136, 24);
			this.Absent.TabIndex = 5;
			this.Absent.Text = "Match only if absent";
			this.toolTip1.SetToolTip(this.Absent, "Match is valid only for characters that are not members of the class");
			this.Absent.CheckedChanged += new System.EventHandler(this.CheckChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						this.label2,
																																						this.label1,
																																						this.FromNtoM,
																																						this.AtLeastN,
																																						this.MBox,
																																						this.NBox,
																																						this.ExactlyN,
																																						this.ZeroOrOne,
																																						this.OneOrMore,
																																						this.Any,
																																						this.Once,
																																						this.Lazy,
																																						this.panel1});
			this.groupBox2.Location = new System.Drawing.Point(237, 30);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(304, 280);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Repetitions";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(176, 208);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 23);
			this.label2.TabIndex = 14;
			this.label2.Text = "m";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(176, 176);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(16, 23);
			this.label1.TabIndex = 13;
			this.label1.Text = "n";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FromNtoM
			// 
			this.FromNtoM.Location = new System.Drawing.Point(32, 216);
			this.FromNtoM.Name = "FromNtoM";
			this.FromNtoM.Size = new System.Drawing.Size(144, 24);
			this.FromNtoM.TabIndex = 12;
			this.FromNtoM.Text = "Between n and m {n,m}";
			this.toolTip1.SetToolTip(this.FromNtoM, "Matches any number of repetitions between n and m inclusive");
			this.FromNtoM.CheckedChanged += new System.EventHandler(this.RepetitionChanged);
			// 
			// AtLeastN
			// 
			this.AtLeastN.Location = new System.Drawing.Point(32, 192);
			this.AtLeastN.Name = "AtLeastN";
			this.AtLeastN.TabIndex = 11;
			this.AtLeastN.Text = "At least n {n,}";
			this.toolTip1.SetToolTip(this.AtLeastN, "At least n must be present");
			this.AtLeastN.CheckedChanged += new System.EventHandler(this.RepetitionChanged);
			// 
			// MBox
			// 
			this.MBox.Enabled = false;
			this.MBox.Location = new System.Drawing.Point(200, 208);
			this.MBox.Minimum = new System.Decimal(new int[] {
																												 1,
																												 0,
																												 0,
																												 0});
			this.MBox.Name = "MBox";
			this.MBox.Size = new System.Drawing.Size(56, 20);
			this.MBox.TabIndex = 10;
			this.MBox.Value = new System.Decimal(new int[] {
																											 2,
																											 0,
																											 0,
																											 0});
			this.MBox.ValueChanged += new System.EventHandler(this.MBox_ValueChanged);
			// 
			// NBox
			// 
			this.NBox.Enabled = false;
			this.NBox.Location = new System.Drawing.Point(200, 176);
			this.NBox.Name = "NBox";
			this.NBox.Size = new System.Drawing.Size(56, 20);
			this.NBox.TabIndex = 9;
			this.NBox.Value = new System.Decimal(new int[] {
																											 1,
																											 0,
																											 0,
																											 0});
			this.NBox.ValueChanged += new System.EventHandler(this.NBox_ValueChanged);
			// 
			// ExactlyN
			// 
			this.ExactlyN.Location = new System.Drawing.Point(32, 168);
			this.ExactlyN.Name = "ExactlyN";
			this.ExactlyN.TabIndex = 5;
			this.ExactlyN.Text = "Exactly n {n}";
			this.toolTip1.SetToolTip(this.ExactlyN, "Matches a string of n repetitions of the character");
			this.ExactlyN.CheckedChanged += new System.EventHandler(this.RepetitionChanged);
			// 
			// ZeroOrOne
			// 
			this.ZeroOrOne.Location = new System.Drawing.Point(32, 134);
			this.ZeroOrOne.Name = "ZeroOrOne";
			this.ZeroOrOne.TabIndex = 4;
			this.ZeroOrOne.Text = "Zero or one ?";
			this.toolTip1.SetToolTip(this.ZeroOrOne, "Matches zero or one occurence");
			this.ZeroOrOne.CheckedChanged += new System.EventHandler(this.RepetitionChanged);
			// 
			// OneOrMore
			// 
			this.OneOrMore.Location = new System.Drawing.Point(32, 108);
			this.OneOrMore.Name = "OneOrMore";
			this.OneOrMore.TabIndex = 3;
			this.OneOrMore.Text = "One or more +";
			this.toolTip1.SetToolTip(this.OneOrMore, "Matches if at least one character is present");
			this.OneOrMore.CheckedChanged += new System.EventHandler(this.RepetitionChanged);
			// 
			// Any
			// 
			this.Any.Location = new System.Drawing.Point(32, 82);
			this.Any.Name = "Any";
			this.Any.TabIndex = 2;
			this.Any.Text = "Any number *";
			this.toolTip1.SetToolTip(this.Any, "Any number including zero");
			this.Any.CheckedChanged += new System.EventHandler(this.RepetitionChanged);
			// 
			// Once
			// 
			this.Once.Location = new System.Drawing.Point(32, 56);
			this.Once.Name = "Once";
			this.Once.TabIndex = 1;
			this.Once.Text = "Just once";
			this.toolTip1.SetToolTip(this.Once, "Matches only a single character");
			this.Once.CheckedChanged += new System.EventHandler(this.RepetitionChanged);
			// 
			// Lazy
			// 
			this.Lazy.Location = new System.Drawing.Point(16, 24);
			this.Lazy.Name = "Lazy";
			this.Lazy.Size = new System.Drawing.Size(152, 24);
			this.Lazy.TabIndex = 8;
			this.Lazy.Text = "As few as possible ?";
			this.toolTip1.SetToolTip(this.Lazy, "Lazy match");
			this.Lazy.CheckedChanged += new System.EventHandler(this.CheckChanged);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Location = new System.Drawing.Point(16, 160);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(264, 88);
			this.panel1.TabIndex = 15;
			// 
			// Results
			// 
			this.Results.Location = new System.Drawing.Point(114, 5);
			this.Results.Name = "Results";
			this.Results.Size = new System.Drawing.Size(273, 20);
			this.Results.TabIndex = 2;
			this.Results.Text = "";
			// 
			// InsertBtn
			// 
			this.InsertBtn.Location = new System.Drawing.Point(400, 4);
			this.InsertBtn.Name = "InsertBtn";
			this.InsertBtn.TabIndex = 3;
			this.InsertBtn.Text = "&Insert";
			this.toolTip1.SetToolTip(this.InsertBtn, "Insert the subexpression");
			this.InsertBtn.Click += new System.EventHandler(this.InsertBtn_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 23);
			this.label3.TabIndex = 5;
			this.label3.Text = "Regular Expression";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// HideBtn
			// 
			this.HideBtn.Location = new System.Drawing.Point(480, 4);
			this.HideBtn.Name = "HideBtn";
			this.HideBtn.TabIndex = 14;
			this.HideBtn.Text = "&Hide";
			this.toolTip1.SetToolTip(this.HideBtn, "Hide the Regex Builder");
			this.HideBtn.Click += new System.EventHandler(this.HideBtn_Click);
			// 
			// Anything
			// 
			this.Anything.Location = new System.Drawing.Point(32, 48);
			this.Anything.Name = "Anything";
			this.Anything.Size = new System.Drawing.Size(140, 24);
			this.Anything.TabIndex = 8;
			this.Anything.Text = "Any character .";
			this.toolTip1.SetToolTip(this.Anything, "Any character except \\n");
			this.Anything.CheckedChanged += new System.EventHandler(this.ClassChanged);
			// 
			// Specific
			// 
			this.Specific.Location = new System.Drawing.Point(32, 144);
			this.Specific.Name = "Specific";
			this.Specific.Size = new System.Drawing.Size(121, 24);
			this.Specific.TabIndex = 9;
			this.Specific.Text = "Specific character";
			this.toolTip1.SetToolTip(this.Specific, "Match a specific character");
			this.Specific.CheckedChanged += new System.EventHandler(this.ClassChanged);
			// 
			// TheCharacter
			// 
			this.TheCharacter.Location = new System.Drawing.Point(165, 146);
			this.TheCharacter.Name = "TheCharacter";
			this.TheCharacter.Size = new System.Drawing.Size(24, 20);
			this.TheCharacter.TabIndex = 10;
			this.TheCharacter.Text = "X";
			this.TheCharacter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.toolTip1.SetToolTip(this.TheCharacter, "A single character to match");
			this.TheCharacter.TextChanged += new System.EventHandler(this.TheCharacter_TextChanged);
			// 
			// Characters
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.HideBtn,
																																	this.label3,
																																	this.InsertBtn,
																																	this.Results,
																																	this.groupBox2,
																																	this.groupBox1});
			this.Name = "Characters";
			this.Size = new System.Drawing.Size(570, 315);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.MBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NBox)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Respond to radio button changes for the character class, set the appropriate CharacterClass
		/// enumeration, and reformat the results.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClassChanged(object sender, System.EventArgs e)
		{
			RadioButton radio = (RadioButton)sender;
			if(!radio.Checked)return;
			NamedBox.Enabled=NamedClass.Checked;
			SpecifiedBox.Enabled=SpecifiedSet.Checked;
			switch (radio.Name)
			{
				case "Anything" : CharacterClass=CharacterClasses.Anything; break;
				case "Alphanumeric" : CharacterClass=CharacterClasses.Alphanumeric; break;
				case "Digit" : CharacterClass=CharacterClasses.Digit; break;
				case "Whitespace" : CharacterClass=CharacterClasses.Whitespace; break;
				case "NamedClass" : 
					CharacterClass=CharacterClasses.Named; break;
				case "SpecifiedSet" :CharacterClass=CharacterClasses.Specified;break;
				case "Specific" : CharacterClass=CharacterClasses.Specific;break;
				default: MessageBox.Show("Oops!"); break;
			}
			FormatResult();
		}

		/// <summary>
		/// Respond to radio button changes for the repetitions, set the appropriate Repetitions
		/// enumeration, and reformat the results.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RepetitionChanged(object sender, System.EventArgs e)
		{
			RadioButton radio = (RadioButton)sender;
			NBox.Enabled=ExactlyN.Checked | AtLeastN.Checked | FromNtoM.Checked;
			MBox.Enabled=NBox.Enabled;
			if(!radio.Checked)return;
			switch (radio.Name)
			{
				case "Once" : Repetition = Repetitions.One; break;
				case "Any" : Repetition = Repetitions.Any; break;
				case "OneOrMore" : Repetition = Repetitions.OneOrMore; break;
				case "ZeroOrOne" : Repetition = Repetitions.ZeroOrOne; break;
				case "ExactlyN" : Repetition = Repetitions.N; break;
				case "AtLeastN" : Repetition = Repetitions.AtLeastN; break;
				case "FromNtoM" : Repetition = Repetitions.FromNToM; break;
				default: MessageBox.Show("Oops!"); break;
			}
			FormatResult();
		}


		/// <summary>
		/// Format the regular expression based on the dialog box settings
		/// </summary>
		private void FormatResult()
		{
			string CharacterTag, RepetitionTag;
			
			// First format the character class
			if(!Absent.Checked)
			{
				switch (CharacterClass)
				{
					case CharacterClasses.Anything: 
						CharacterTag= "."; break;
					case CharacterClasses.Alphanumeric: 
						CharacterTag= "\\w"; break;
					case CharacterClasses.Digit : 
						CharacterTag= "\\d"; break;
					case CharacterClasses.Whitespace : 
						CharacterTag= "\\s"; break;
					case CharacterClasses.Named : 
						CharacterTag="\\p{"+NamedBox.Text+"}";break;
					case CharacterClasses.Specific :
						CharacterTag=TheCharacter.Text;break;
					case CharacterClasses.Specified :
						CharacterTag="["+SpecifiedBox.Text+"]";break;
					default:
						CharacterTag= ""; MessageBox.Show("Oops!"); break;
				}
			}
			else
			{
				switch (CharacterClass)
				{
					case CharacterClasses.Anything :
						CharacterTag="";break;
					case CharacterClasses.Alphanumeric: 
						CharacterTag= "\\W"; break;
					case CharacterClasses.Digit : 
						CharacterTag= "\\D"; break;
					case CharacterClasses.Whitespace : 
						CharacterTag= "\\S"; break;
					case CharacterClasses.Named : 
						CharacterTag="\\P{"+NamedBox.Text+"}";break;
					case CharacterClasses.Specific :
						CharacterTag="[^"+TheCharacter.Text+"]";break;
					case CharacterClasses.Specified :
						CharacterTag="[^"+SpecifiedBox.Text+"]";break;
					default:
						CharacterTag= ""; MessageBox.Show("Oops!"); break;
				}
			}

			// Next format the repetition
			string N=NBox.Value.ToString();
			string M=MBox.Value.ToString();
			if(!Lazy.Checked)
			{
				switch(Repetition)
				{
					case Repetitions.Any:
						RepetitionTag= "*"; break;
					case Repetitions.AtLeastN:
						RepetitionTag= "{"+N+",}"; break;
					case Repetitions.FromNToM:
						RepetitionTag= "{"+N+","+M+"}"; break;
					case Repetitions.N:
						RepetitionTag= "{"+N+"}"; break;
					case Repetitions.One:
						RepetitionTag= ""; break;
					case Repetitions.OneOrMore:
						RepetitionTag= "+"; break;
					case Repetitions.ZeroOrOne:
						RepetitionTag= "?"; break;
					default: 
						RepetitionTag= ""; MessageBox.Show("Oops!"); break;
				}
			}
			else
			{
				switch(Repetition)
				{
					case Repetitions.Any:
						RepetitionTag= "*?"; break;
					case Repetitions.AtLeastN:
						RepetitionTag= "{"+N+",}?"; break;
					case Repetitions.FromNToM:
						RepetitionTag= "{"+N+","+M+"}?"; break;
					case Repetitions.N:
						RepetitionTag= "{"+N+"}?"; break;
					case Repetitions.One:
						RepetitionTag= ""; break;
					case Repetitions.OneOrMore:
						RepetitionTag= "+?"; break;
					case Repetitions.ZeroOrOne:
						RepetitionTag= "??"; break;
					default: 
						RepetitionTag=""; MessageBox.Show("Oops!"); break;
				}
			}
			Results.Text=CharacterTag+RepetitionTag;
		}

		private void CheckChanged(object sender, System.EventArgs e)
		{
			FormatResult();
		}

		private void NBox_ValueChanged(object sender, System.EventArgs e)
		{
			if(NBox.Value>MBox.Value)MBox.Value=NBox.Value;
			FormatResult();
		}

		private void MBox_ValueChanged(object sender, System.EventArgs e)
		{
			if(MBox.Value<NBox.Value)NBox.Value=MBox.Value;
			FormatResult();
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

		private void TheCharacter_TextChanged(object sender, System.EventArgs e)
		{
			TheCharacter.Text=(TheCharacter.Text).Substring(0,1);
			this.FormatResult();
		}

	}
}
