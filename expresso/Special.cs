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
using System.Text.RegularExpressions;

namespace Expresso
{
	/// <summary>
	/// This is the "Special" tab page for the RegBuilder
	/// </summary>
	public class Special : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.GroupBox MatchBox;
		private System.Windows.Forms.RadioButton ASCIIOctal;
		private System.Windows.Forms.RadioButton NewLine;
		private System.Windows.Forms.RadioButton FormFeed;
		private System.Windows.Forms.RadioButton Return;
		private System.Windows.Forms.RadioButton Vertical;
		private System.Windows.Forms.RadioButton Tab;
		private System.Windows.Forms.RadioButton Backspace;
		private System.Windows.Forms.RadioButton Bell;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox Results;
		private System.Windows.Forms.RadioButton Unicode;
		private System.Windows.Forms.RadioButton ControlChar;
		private System.Windows.Forms.TextBox ASCIIOctalBox;
		private System.Windows.Forms.TextBox ASCIIHexBox;
		private System.Windows.Forms.TextBox UnicodeBox;
		private System.Windows.Forms.TextBox ControlCharBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton Begin;
		private System.Windows.Forms.RadioButton End;
		private System.Windows.Forms.RadioButton BeginAll;
		private System.Windows.Forms.RadioButton EndAll;
		private System.Windows.Forms.RadioButton EndString;
		private System.Windows.Forms.RadioButton BeginSearch;
		private System.Windows.Forms.RadioButton Word;
		private System.Windows.Forms.RadioButton NotWord;
		private System.ComponentModel.IContainer components;
		private static Regex rTestOctal = new Regex("[0-7]{0,3}",RegexOptions.Compiled);
		private static Regex rTestHex = new Regex("[0-9A-Fa-f]{0,2}",RegexOptions.Compiled);
		private System.Windows.Forms.RadioButton ASCIIHex;
		private System.Windows.Forms.RadioButton EscapeBtn;
		private static Regex rTestUnicode = new Regex("[0-9A-Fa-f]{0,4}",RegexOptions.Compiled);
		private static Regex rTestControl = new Regex("[A-_a-z]{0,1}",RegexOptions.Compiled);
		private bool skip;
		private System.Windows.Forms.Button InsertBtn;
		private System.Windows.Forms.Button HideBtn;
		private System.Windows.Forms.ToolTip toolTip1;
		private RegBuilder Builder;

		public Special(RegBuilder builder)
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
			this.MatchBox = new System.Windows.Forms.GroupBox();
			this.NotWord = new System.Windows.Forms.RadioButton();
			this.Word = new System.Windows.Forms.RadioButton();
			this.BeginSearch = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.EndString = new System.Windows.Forms.RadioButton();
			this.EndAll = new System.Windows.Forms.RadioButton();
			this.BeginAll = new System.Windows.Forms.RadioButton();
			this.End = new System.Windows.Forms.RadioButton();
			this.Begin = new System.Windows.Forms.RadioButton();
			this.ControlCharBox = new System.Windows.Forms.TextBox();
			this.UnicodeBox = new System.Windows.Forms.TextBox();
			this.ASCIIHexBox = new System.Windows.Forms.TextBox();
			this.ASCIIOctalBox = new System.Windows.Forms.TextBox();
			this.ControlChar = new System.Windows.Forms.RadioButton();
			this.Unicode = new System.Windows.Forms.RadioButton();
			this.ASCIIHex = new System.Windows.Forms.RadioButton();
			this.ASCIIOctal = new System.Windows.Forms.RadioButton();
			this.EscapeBtn = new System.Windows.Forms.RadioButton();
			this.NewLine = new System.Windows.Forms.RadioButton();
			this.FormFeed = new System.Windows.Forms.RadioButton();
			this.Return = new System.Windows.Forms.RadioButton();
			this.Vertical = new System.Windows.Forms.RadioButton();
			this.Tab = new System.Windows.Forms.RadioButton();
			this.Backspace = new System.Windows.Forms.RadioButton();
			this.Bell = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.InsertBtn = new System.Windows.Forms.Button();
			this.Results = new System.Windows.Forms.TextBox();
			this.HideBtn = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.MatchBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// MatchBox
			// 
			this.MatchBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																					 this.NotWord,
																																					 this.Word,
																																					 this.BeginSearch,
																																					 this.label2,
																																					 this.label1,
																																					 this.EndString,
																																					 this.EndAll,
																																					 this.BeginAll,
																																					 this.End,
																																					 this.Begin,
																																					 this.ControlCharBox,
																																					 this.UnicodeBox,
																																					 this.ASCIIHexBox,
																																					 this.ASCIIOctalBox,
																																					 this.ControlChar,
																																					 this.Unicode,
																																					 this.ASCIIHex,
																																					 this.ASCIIOctal,
																																					 this.EscapeBtn,
																																					 this.NewLine,
																																					 this.FormFeed,
																																					 this.Return,
																																					 this.Vertical,
																																					 this.Tab,
																																					 this.Backspace,
																																					 this.Bell});
			this.MatchBox.Location = new System.Drawing.Point(10, 30);
			this.MatchBox.Name = "MatchBox";
			this.MatchBox.Size = new System.Drawing.Size(550, 280);
			this.MatchBox.TabIndex = 15;
			this.MatchBox.TabStop = false;
			this.MatchBox.Text = "Special Characters";
			// 
			// NotWord
			// 
			this.NotWord.Location = new System.Drawing.Point(312, 240);
			this.NotWord.Name = "NotWord";
			this.NotWord.Size = new System.Drawing.Size(201, 24);
			this.NotWord.TabIndex = 28;
			this.NotWord.Text = "Not first or last character in word \\B";
			this.NotWord.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// Word
			// 
			this.Word.Location = new System.Drawing.Point(312, 216);
			this.Word.Name = "Word";
			this.Word.Size = new System.Drawing.Size(188, 24);
			this.Word.TabIndex = 27;
			this.Word.Text = "First or last character in word \\b";
			this.toolTip1.SetToolTip(this.Word, "\\b means backspace only within character class [] or replacement string");
			this.Word.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// BeginSearch
			// 
			this.BeginSearch.Location = new System.Drawing.Point(312, 192);
			this.BeginSearch.Name = "BeginSearch";
			this.BeginSearch.Size = new System.Drawing.Size(178, 24);
			this.BeginSearch.TabIndex = 26;
			this.BeginSearch.Text = "Beginning of current search \\G";
			this.BeginSearch.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(304, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(168, 23);
			this.label2.TabIndex = 25;
			this.label2.Text = "Depends on multiline option:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(304, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(168, 23);
			this.label1.TabIndex = 24;
			this.label1.Text = "Ignores the multiline option:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// EndString
			// 
			this.EndString.Location = new System.Drawing.Point(312, 168);
			this.EndString.Name = "EndString";
			this.EndString.Size = new System.Drawing.Size(120, 24);
			this.EndString.TabIndex = 23;
			this.EndString.Text = "End of string \\z";
			this.EndString.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// EndAll
			// 
			this.EndAll.Location = new System.Drawing.Point(312, 144);
			this.EndAll.Name = "EndAll";
			this.EndAll.Size = new System.Drawing.Size(225, 24);
			this.EndAll.TabIndex = 22;
			this.EndAll.Text = "End of string or before newline at end \\Z";
			this.EndAll.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// BeginAll
			// 
			this.BeginAll.Location = new System.Drawing.Point(312, 120);
			this.BeginAll.Name = "BeginAll";
			this.BeginAll.Size = new System.Drawing.Size(208, 24);
			this.BeginAll.TabIndex = 21;
			this.BeginAll.Text = "Beginning of string \\A";
			this.BeginAll.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// End
			// 
			this.End.Location = new System.Drawing.Point(312, 64);
			this.End.Name = "End";
			this.End.Size = new System.Drawing.Size(160, 24);
			this.End.TabIndex = 20;
			this.End.Text = "End of string or line $";
			this.toolTip1.SetToolTip(this.End, "End of line if multiline is set");
			this.End.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// Begin
			// 
			this.Begin.Location = new System.Drawing.Point(312, 40);
			this.Begin.Name = "Begin";
			this.Begin.Size = new System.Drawing.Size(160, 24);
			this.Begin.TabIndex = 19;
			this.Begin.Text = "Beginning of string or line ^";
			this.toolTip1.SetToolTip(this.Begin, "Beginning of line if multiline is set");
			this.Begin.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// ControlCharBox
			// 
			this.ControlCharBox.Location = new System.Drawing.Point(238, 144);
			this.ControlCharBox.Name = "ControlCharBox";
			this.ControlCharBox.Size = new System.Drawing.Size(46, 20);
			this.ControlCharBox.TabIndex = 18;
			this.ControlCharBox.Text = "A";
			this.ControlCharBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlCharBox_KeyPress);
			this.ControlCharBox.TextChanged += new System.EventHandler(this.ControlCharBox_TextChanged);
			// 
			// UnicodeBox
			// 
			this.UnicodeBox.Location = new System.Drawing.Point(238, 120);
			this.UnicodeBox.Name = "UnicodeBox";
			this.UnicodeBox.Size = new System.Drawing.Size(46, 20);
			this.UnicodeBox.TabIndex = 17;
			this.UnicodeBox.Text = "0041";
			this.UnicodeBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UnicodeBox_KeyPress);
			this.UnicodeBox.TextChanged += new System.EventHandler(this.UnicodeBox_TextChanged);
			// 
			// ASCIIHexBox
			// 
			this.ASCIIHexBox.Location = new System.Drawing.Point(238, 96);
			this.ASCIIHexBox.Name = "ASCIIHexBox";
			this.ASCIIHexBox.Size = new System.Drawing.Size(46, 20);
			this.ASCIIHexBox.TabIndex = 16;
			this.ASCIIHexBox.Text = "41";
			this.ASCIIHexBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ASCIIHexBox_KeyPress);
			this.ASCIIHexBox.TextChanged += new System.EventHandler(this.ASCIIHexBox_TextChanged);
			// 
			// ASCIIOctalBox
			// 
			this.ASCIIOctalBox.Location = new System.Drawing.Point(238, 72);
			this.ASCIIOctalBox.Name = "ASCIIOctalBox";
			this.ASCIIOctalBox.Size = new System.Drawing.Size(46, 20);
			this.ASCIIOctalBox.TabIndex = 15;
			this.ASCIIOctalBox.Text = "101";
			this.ASCIIOctalBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ASCIIOctalBox_KeyPress);
			this.ASCIIOctalBox.TextChanged += new System.EventHandler(this.ASCIIOctalBox_TextChanged);
			// 
			// ControlChar
			// 
			this.ControlChar.Location = new System.Drawing.Point(125, 143);
			this.ControlChar.Name = "ControlChar";
			this.ControlChar.Size = new System.Drawing.Size(106, 24);
			this.ControlChar.TabIndex = 14;
			this.ControlChar.Text = "Control Char \\cA";
			this.toolTip1.SetToolTip(this.ControlChar, "Any valid ASCII control character");
			this.ControlChar.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// Unicode
			// 
			this.Unicode.Location = new System.Drawing.Point(125, 119);
			this.Unicode.Name = "Unicode";
			this.Unicode.Size = new System.Drawing.Size(106, 24);
			this.Unicode.TabIndex = 13;
			this.Unicode.Text = "Unicode \\u0041";
			this.toolTip1.SetToolTip(this.Unicode, "Hex code for any unicode character");
			this.Unicode.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// ASCIIHex
			// 
			this.ASCIIHex.Location = new System.Drawing.Point(125, 95);
			this.ASCIIHex.Name = "ASCIIHex";
			this.ASCIIHex.Size = new System.Drawing.Size(99, 24);
			this.ASCIIHex.TabIndex = 12;
			this.ASCIIHex.Text = "ASCII Hex \\x41";
			this.toolTip1.SetToolTip(this.ASCIIHex, "Hex code for any ASCII character");
			this.ASCIIHex.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// ASCIIOctal
			// 
			this.ASCIIOctal.Location = new System.Drawing.Point(125, 71);
			this.ASCIIOctal.Name = "ASCIIOctal";
			this.ASCIIOctal.Size = new System.Drawing.Size(106, 24);
			this.ASCIIOctal.TabIndex = 11;
			this.ASCIIOctal.Text = "ASCII Octal \\101";
			this.toolTip1.SetToolTip(this.ASCIIOctal, "Octal code for any ASCII character");
			this.ASCIIOctal.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// EscapeBtn
			// 
			this.EscapeBtn.Location = new System.Drawing.Point(125, 47);
			this.EscapeBtn.Name = "EscapeBtn";
			this.EscapeBtn.Size = new System.Drawing.Size(120, 24);
			this.EscapeBtn.TabIndex = 10;
			this.EscapeBtn.Text = "Escape \\e";
			this.EscapeBtn.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// NewLine
			// 
			this.NewLine.Location = new System.Drawing.Point(125, 23);
			this.NewLine.Name = "NewLine";
			this.NewLine.Size = new System.Drawing.Size(120, 24);
			this.NewLine.TabIndex = 9;
			this.NewLine.Text = "New line \\n";
			this.NewLine.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// FormFeed
			// 
			this.FormFeed.Location = new System.Drawing.Point(8, 144);
			this.FormFeed.Name = "FormFeed";
			this.FormFeed.Size = new System.Drawing.Size(110, 24);
			this.FormFeed.TabIndex = 8;
			this.FormFeed.Text = "Form feed \\f";
			this.FormFeed.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// Return
			// 
			this.Return.Location = new System.Drawing.Point(8, 96);
			this.Return.Name = "Return";
			this.Return.Size = new System.Drawing.Size(110, 24);
			this.Return.TabIndex = 6;
			this.Return.Text = "Carriage return \\r";
			this.Return.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// Vertical
			// 
			this.Vertical.Location = new System.Drawing.Point(8, 120);
			this.Vertical.Name = "Vertical";
			this.Vertical.Size = new System.Drawing.Size(110, 24);
			this.Vertical.TabIndex = 4;
			this.Vertical.Text = "Vertical tab \\v";
			this.Vertical.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// Tab
			// 
			this.Tab.Location = new System.Drawing.Point(8, 72);
			this.Tab.Name = "Tab";
			this.Tab.Size = new System.Drawing.Size(110, 24);
			this.Tab.TabIndex = 3;
			this.Tab.Text = "Tab \\t";
			this.Tab.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// Backspace
			// 
			this.Backspace.Location = new System.Drawing.Point(8, 48);
			this.Backspace.Name = "Backspace";
			this.Backspace.Size = new System.Drawing.Size(110, 24);
			this.Backspace.TabIndex = 2;
			this.Backspace.Text = "Backspace \\b";
			this.toolTip1.SetToolTip(this.Backspace, "Use only within character class [] or replacement pattern");
			this.Backspace.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// Bell
			// 
			this.Bell.Location = new System.Drawing.Point(8, 24);
			this.Bell.Name = "Bell";
			this.Bell.Size = new System.Drawing.Size(110, 24);
			this.Bell.TabIndex = 0;
			this.Bell.Text = "Bell \\a";
			this.Bell.CheckedChanged += new System.EventHandler(this.Tab_CheckedChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 23);
			this.label3.TabIndex = 14;
			this.label3.Text = "Regular Expression";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// InsertBtn
			// 
			this.InsertBtn.Location = new System.Drawing.Point(400, 4);
			this.InsertBtn.Name = "InsertBtn";
			this.InsertBtn.TabIndex = 12;
			this.InsertBtn.Text = "&Insert";
			this.toolTip1.SetToolTip(this.InsertBtn, "Insert the subexpression");
			this.InsertBtn.Click += new System.EventHandler(this.InsertBtn_Click);
			// 
			// Results
			// 
			this.Results.Location = new System.Drawing.Point(114, 5);
			this.Results.Name = "Results";
			this.Results.Size = new System.Drawing.Size(273, 20);
			this.Results.TabIndex = 11;
			this.Results.Text = "";
			// 
			// HideBtn
			// 
			this.HideBtn.Location = new System.Drawing.Point(480, 4);
			this.HideBtn.Name = "HideBtn";
			this.HideBtn.TabIndex = 16;
			this.HideBtn.Text = "&Hide";
			this.toolTip1.SetToolTip(this.HideBtn, "Hide the Regex Builder");
			this.HideBtn.Click += new System.EventHandler(this.HideBtn_Click);
			// 
			// Special
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.HideBtn,
																																	this.MatchBox,
																																	this.label3,
																																	this.InsertBtn,
																																	this.Results});
			this.Name = "Special";
			this.Size = new System.Drawing.Size(570, 315);
			this.MatchBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// This is the event handler that resonds to changes in any of the checkboxes.
		/// It figures out which check box was changes and generates the appropriate output
		/// characters.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tab_CheckedChanged(object sender, System.EventArgs e)
		{
			if(skip)return;
			string Output="";
			RadioButton radio = (RadioButton)sender;
			if(!radio.Checked)return;
			switch(radio.Name)
			{
				case "Tab" : Output="\\t"; break;
				case "Bell" : Output="\\a"; break;
				case "Backspace" : Output="\\b"; break;
				case "Return" : Output="\\r"; break;
				case "Vertical" : Output="\\v"; break;
				case "FormFeed" : Output="\\f"; break;
				case "NewLine" : Output="\\n"; break;
				case "EscapeBtn" : Output="\\e"; break;
				case "ASCIIOctal" : 
					Output=UpdateOctal(); 
					break;
				case "ASCIIHex" : 
					Output=UpdateHex();
					break;
				case "Unicode" : 
					Output=UpdateUnicode();
					break;
				case "ControlChar" : 
					Output=UpdateControlChar();
					break;
				case "Begin" : Output="^"; break;
				case "End" : Output="$"; break;
				case "BeginAll" : Output="\\A"; break;
				case "EndAll" : Output="\\Z"; break;
				case "EndString" : Output="\\z"; break;
				case "BeginSearch" : Output="\\G"; break;
				case "Word" : Output="\\b"; break;
				case "NotWord" : Output="\\B"; break;
				default: MessageBox.Show("Oops!"); break;
			}
			Results.Text=Output;
		}

		private void ASCIIOctalBox_TextChanged(object sender, System.EventArgs e)
		{
			UpdateOctal();
		}

		private string UpdateOctal()
		{
			string num = ASCIIOctalBox.Text;
			Match m=rTestOctal.Match(num);
			if(!m.Success || m.Value.Length!=num.Length || int.Parse(num)>377)
			{
				MessageBox.Show("Must be an octal constant with a maximum of 377",
					"Expresso Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				ASCIIOctalBox.Undo();
				return "";
			}
			switch(num.Length)
			{
				case 0: num="000"; break;
				case 1: num="00"+num; break;
				case 2: num="0"+num; break;
			}
			skip=true;
			ASCIIOctal.Checked=true;
			skip=false;
			return Results.Text="\\"+num;
		}

		private void ASCIIHexBox_TextChanged(object sender, System.EventArgs e)
		{
			UpdateHex();
		}

		private string UpdateHex()
		{
			string num = ASCIIHexBox.Text;
			Match m=rTestHex.Match(num);
			if(!m.Success || m.Value.Length!=num.Length)
			{
				MessageBox.Show("Must be a hexadecimal constant with a maximum of three digits",
					"Expresso Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				ASCIIHexBox.Undo();
				return "";
			}		
			switch(num.Length)
			{
				case 0: num="00"; break;
				case 1: num="0"+num; break;
			}
			skip=true;
			ASCIIHex.Checked=true;
			skip=false;
			return Results.Text="\\x"+num;
		}

		private void UnicodeBox_TextChanged(object sender, System.EventArgs e)
		{
			UpdateUnicode();
		}

		private string UpdateUnicode()
		{
			string num = UnicodeBox.Text;
			Match m=rTestUnicode.Match(num);
			if(!m.Success || m.Value.Length!=num.Length)
			{
				MessageBox.Show("Must be a hexadecimal constant with a maximum of four digits",
					"Expresso Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				UnicodeBox.Undo();
				return "";
			}		
			switch(num.Length)
			{
				case 0: num="0000"; break;
				case 1: num="000"+num; break;
				case 2: num="00"+num; break;
				case 3: num="0"+num; break;
			}
			skip=true;
			Unicode.Checked=true;
			skip=false;
			return Results.Text="\\u"+num;
		}

		private void ControlCharBox_TextChanged(object sender, System.EventArgs e)
		{
			UpdateControlChar();
		}

		private string UpdateControlChar()
		{
			string num = ControlCharBox.Text;
			Match m=rTestControl.Match(num);
			if(!m.Success || num.Length>1)
			{
				MessageBox.Show("Must be a valid control character",
					"Expresso Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				ControlCharBox.Undo();
				return "";
			}		
			skip=true;
			ControlChar.Checked=true;
			skip=false;
			return Results.Text="\\c"+num;
		}

		private void ASCIIOctalBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			char c = e.KeyChar;
			int Number = ASCIIOctalBox.Text.Length-ASCIIOctalBox.SelectedText.Length;;
			if(Char.IsDigit(c) && c!='9' && c!='8' && Number<3)return;
			else if(Char.IsControl(c))return;
			else e.Handled=true;
		}

		private void ASCIIHexBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			char c = e.KeyChar;
			int Number = ASCIIHexBox.Text.Length-ASCIIHexBox.SelectedText.Length;
			if(Char.IsNumber(c) && Number<2)return;
			else if(Char.IsControl(c))return;
			else e.Handled=true;
		}

		private void UnicodeBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			char c = e.KeyChar;
			int Number = UnicodeBox.Text.Length-UnicodeBox.SelectedText.Length;
			if(Char.IsNumber(c) && Number<4)return;
			else if(Char.IsControl(c))return;
			else e.Handled=true;		
		}

		private void ControlCharBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			char c = e.KeyChar;
			int Number = ControlCharBox.Text.Length-ControlCharBox.SelectedText.Length;
			Match m=rTestControl.Match(c.ToString());
			if(m.Success && Number==0)return;
			else if(Char.IsControl(c))return;
			else e.Handled=true;		
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
