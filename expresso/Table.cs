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
	/// This is a simple ASCII table tab page. It simply displays the table, with no user
	/// interaction.
	/// </summary>
	public class Table : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button HideBtn;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolTip toolTip1;

		private RegBuilder Builder;

		public Table(RegBuilder builder)
		{
			InitializeComponent();
			textBox1.SelectionLength=0;
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.HideBtn = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.textBox1.Location = new System.Drawing.Point(16, 81);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(538, 152);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = @"    0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F
0  NUL SOH STX ETX EOT ENQ ACK BEL BS  HT  LF  VT  FF  CR  SO  SI
1  DLE DC1 DC2 DC3 DC4 NAK SYN ETB CAN EM  SUB ESC FS  GS  RS  US
2   SP  !   ""   #   $   %   &   '   (   )   *   +   ,   -   .   /
3   0   1   2   3   4   5   6   7   8   9   :   ;   <   =   >   ?
4   @   A   B   C   D   E   F   G   H   I   J   K   L   M   N   O
5   P   Q   R   S   T   U   V   W   X   Y   Z   [   \   ]   ^   _
6   `   a   b   c   d   e   f   g   h   i   j   k   l   m   n   o
7   p   q   r   s   t   u   v   w   x   y   z   {   |   }   ~ DEL";
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
			// Table
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.HideBtn,
																																	this.textBox1});
			this.Name = "Table";
			this.Size = new System.Drawing.Size(570, 315);
			this.ResumeLayout(false);

		}
		#endregion

		private void HideBtn_Click(object sender, System.EventArgs e)
		{
			Builder.ShowBuilder(false);
		}
	}
}
