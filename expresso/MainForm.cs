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
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using SaveAndRestore;

namespace Expresso
{
	/// <summary>
	/// This is the main application form for Expresso
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button RunBtn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox IgnoreCase;
		private System.Windows.Forms.CheckBox Multiline;
		private System.Windows.Forms.CheckBox Explicit;
		private System.Windows.Forms.CheckBox Singleline;
		private System.Windows.Forms.CheckBox IgnorePattern;
		private System.Windows.Forms.CheckBox ECMA;
		private System.Windows.Forms.Button ReplaceBtn;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox Replace;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mnOpen;
		private System.Windows.Forms.OpenFileDialog dlgOpen;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem mnExit;
		private System.Windows.Forms.Button ExitBtn;
		private System.Windows.Forms.TreeView Tree;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem mnAbout;
		private System.Windows.Forms.TextBox ResultsBox;
		private AboutBox about;
		public System.Windows.Forms.Button mBuildBtn;
		private System.Windows.Forms.ComboBox RegexList;
		private System.Windows.Forms.TextBox RegexBox;
		private RegBuilder builder;
		private System.Windows.Forms.TextBox InputBox;
		private bool Dirty;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mnWebLink;
		private System.Windows.Forms.SaveFileDialog dlgSave;
		public System.Windows.Forms.Button BuildBtn;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem mnMakeC;
		private System.Windows.Forms.CheckBox RightToLeftBox;
		private System.Windows.Forms.MenuItem mnMakeVB;
		private System.Windows.Forms.CheckBox Compiled;
		private System.Windows.Forms.MenuItem mnMakeAssembly;   // Indicates that the InputBox has been edited since the last match or replace
		private bool Skip=false; // Used to skip the actions when a tree node is selected
		private MakeAssemblyForm MakeForm;
		private System.Windows.Forms.MenuItem mnNewProject;
		private System.Windows.Forms.MenuItem mnOpenProject;
		private System.Windows.Forms.MenuItem mnSaveProject;
		private System.Windows.Forms.MenuItem mnSaveProjectAs;
		private RegexOptions TheOptions;
		private Settings settings;        // This stores all the project and application settings
		private RegistrySettings regSettings;  // These are the settings in the registry
		private string ProjectFileName;
		private System.Windows.Forms.Panel LeftPanel;
		private System.Windows.Forms.MenuItem mnFile;
		private System.Windows.Forms.MenuItem mnMRUStart;
		private System.Windows.Forms.MenuItem mnMRUEnd;
		private MostRecentFiles MRUList;
		private System.Windows.Forms.Panel TopPanel;
		private System.Windows.Forms.Panel RightPanel;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.StatusBarPanel statusBarPanel3;
		private System.Windows.Forms.Splitter splitter2;  // The list of most recently used file names
		private string[] Arguments; // The command line arguments
		private string ExpressoKey = "Software\\Ultrapico\\Expresso"; // Registry key for application data
		private int SaveTreeHeight;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnSaveCCode;
		private System.Windows.Forms.MenuItem mnSaveVBCode;
		private System.Windows.Forms.MenuItem mnSampleProject;
		private System.Windows.Forms.MenuItem menuItem6;
		private bool IsVisible=false;
		private System.Windows.Forms.ToolBar ToolBar;
		private System.Windows.Forms.ToolBarButton NewToolBtn;
		private System.Windows.Forms.ToolBarButton SaveToolBtn;
		private System.Windows.Forms.ToolBarButton OpenToolBtn;
		private System.Windows.Forms.ToolBarButton MatchToolBtn;
		private System.Windows.Forms.ToolBarButton ReplaceToolBtn;
		private System.Windows.Forms.ToolBarButton BuildToolBtn;
		private System.Windows.Forms.ImageList ToolImages;
		private System.Windows.Forms.ToolBarButton SeparatorToolBtn1;
		private System.Windows.Forms.ToolBarButton SeparatorToolBtn2;
		private string SampleProject;

		public MainForm(string[] args)
		{
			InitializeComponent();
			SampleProject=Path.GetDirectoryName(Application.ExecutablePath)+"\\Sample.xso";
			Arguments=args;
			MakeForm = new MakeAssemblyForm();
			settings = new Settings();
			regSettings = new RegistrySettings();
			MRUList = new MostRecentFiles();
			MRUList.Capacity=5;

			ReadRegistryData();
			builder = new RegBuilder(this);
			builder.Visible=false;
			builder.Location=new Point(0,RightPanel.Height);
			this.Controls.Add(this.builder);
			builder.BringToFront();
			builder.Dock=DockStyle.Fill;
			RegexList.SelectedIndex=3;
			Dirty=false;
			ShowTree(true);
			if(Arguments!=null && Arguments.Length>0 && Arguments[0]!="")
				OpenProject(Arguments[0]);
			else if(ProjectFileName!="")OpenProject(ProjectFileName);
			EnableSave(false);
		}

		/// <summary>
		/// Enable or disable the project save menu
		/// </summary>
		/// <param name="Enable"></param>
		private void EnableSave(bool Enable)
		{
			if(ProjectFileName!=SampleProject)
				this.mnSaveProject.Enabled=Enable;
			else this.mnSaveProject.Enabled=false;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.RunBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.IgnoreCase = new System.Windows.Forms.CheckBox();
            this.Multiline = new System.Windows.Forms.CheckBox();
            this.Explicit = new System.Windows.Forms.CheckBox();
            this.Singleline = new System.Windows.Forms.CheckBox();
            this.IgnorePattern = new System.Windows.Forms.CheckBox();
            this.ECMA = new System.Windows.Forms.CheckBox();
            this.RightToLeftBox = new System.Windows.Forms.CheckBox();
            this.RegexList = new System.Windows.Forms.ComboBox();
            this.Tree = new System.Windows.Forms.TreeView();
            this.ResultsBox = new System.Windows.Forms.TextBox();
            this.InputBox = new System.Windows.Forms.TextBox();
            this.TopPanel = new System.Windows.Forms.Panel();
            this.Compiled = new System.Windows.Forms.CheckBox();
            this.BuildBtn = new System.Windows.Forms.Button();
            this.RegexBox = new System.Windows.Forms.TextBox();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.Replace = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ReplaceBtn = new System.Windows.Forms.Button();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.mnFile = new System.Windows.Forms.MenuItem();
            this.mnNewProject = new System.Windows.Forms.MenuItem();
            this.mnOpenProject = new System.Windows.Forms.MenuItem();
            this.mnSaveProject = new System.Windows.Forms.MenuItem();
            this.mnSaveProjectAs = new System.Windows.Forms.MenuItem();
            this.mnOpen = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.mnSampleProject = new System.Windows.Forms.MenuItem();
            this.mnMRUStart = new System.Windows.Forms.MenuItem();
            this.mnMRUEnd = new System.Windows.Forms.MenuItem();
            this.mnExit = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.mnMakeC = new System.Windows.Forms.MenuItem();
            this.mnMakeVB = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnSaveCCode = new System.Windows.Forms.MenuItem();
            this.mnSaveVBCode = new System.Windows.Forms.MenuItem();
            this.mnMakeAssembly = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.mnWebLink = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.mnAbout = new System.Windows.Forms.MenuItem();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.LeftPanel = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.RightPanel = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
            this.ToolBar = new System.Windows.Forms.ToolBar();
            this.NewToolBtn = new System.Windows.Forms.ToolBarButton();
            this.OpenToolBtn = new System.Windows.Forms.ToolBarButton();
            this.SaveToolBtn = new System.Windows.Forms.ToolBarButton();
            this.SeparatorToolBtn1 = new System.Windows.Forms.ToolBarButton();
            this.MatchToolBtn = new System.Windows.Forms.ToolBarButton();
            this.ReplaceToolBtn = new System.Windows.Forms.ToolBarButton();
            this.SeparatorToolBtn2 = new System.Windows.Forms.ToolBarButton();
            this.BuildToolBtn = new System.Windows.Forms.ToolBarButton();
            this.ToolImages = new System.Windows.Forms.ImageList(this.components);
            this.TopPanel.SuspendLayout();
            this.LeftPanel.SuspendLayout();
            this.RightPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
            this.SuspendLayout();
            // 
            // RunBtn
            // 
            this.RunBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RunBtn.Location = new System.Drawing.Point(623, 7);
            this.RunBtn.Name = "RunBtn";
            this.RunBtn.Size = new System.Drawing.Size(87, 23);
            this.RunBtn.TabIndex = 5;
            this.RunBtn.Text = "Find &Matches";
            this.toolTip1.SetToolTip(this.RunBtn, "Look for matches between the regular expression and the input data");
            this.RunBtn.Click += new System.EventHandler(this.RunBtn_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Regular &Expression";
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(424, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Res&ults of Search and Replace";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(421, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Sample &Input Data";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // IgnoreCase
            // 
            this.IgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IgnoreCase.Checked = true;
            this.IgnoreCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IgnoreCase.Location = new System.Drawing.Point(726, 2);
            this.IgnoreCase.Name = "IgnoreCase";
            this.IgnoreCase.Size = new System.Drawing.Size(88, 24);
            this.IgnoreCase.TabIndex = 10;
            this.IgnoreCase.Text = "Ignore Case";
            this.toolTip1.SetToolTip(this.IgnoreCase, "Don\'t distinguish between upper and lower case");
            this.IgnoreCase.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // Multiline
            // 
            this.Multiline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Multiline.Checked = true;
            this.Multiline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Multiline.Location = new System.Drawing.Point(726, 23);
            this.Multiline.Name = "Multiline";
            this.Multiline.Size = new System.Drawing.Size(88, 24);
            this.Multiline.TabIndex = 11;
            this.Multiline.Text = "Multiline";
            this.toolTip1.SetToolTip(this.Multiline, "Should ^ and $ match lines or entire string?");
            this.Multiline.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // Explicit
            // 
            this.Explicit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Explicit.Location = new System.Drawing.Point(726, 65);
            this.Explicit.Name = "Explicit";
            this.Explicit.Size = new System.Drawing.Size(104, 24);
            this.Explicit.TabIndex = 13;
            this.Explicit.Text = "Explicit Capture";
            this.toolTip1.SetToolTip(this.Explicit, "Capture groups only if explicitly numbered or named");
            this.Explicit.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // Singleline
            // 
            this.Singleline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Singleline.Location = new System.Drawing.Point(726, 44);
            this.Singleline.Name = "Singleline";
            this.Singleline.Size = new System.Drawing.Size(88, 24);
            this.Singleline.TabIndex = 12;
            this.Singleline.Text = "Singleline";
            this.toolTip1.SetToolTip(this.Singleline, "Make period match \\n as well as any other character");
            this.Singleline.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // IgnorePattern
            // 
            this.IgnorePattern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IgnorePattern.Checked = true;
            this.IgnorePattern.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IgnorePattern.Location = new System.Drawing.Point(726, 128);
            this.IgnorePattern.Name = "IgnorePattern";
            this.IgnorePattern.Size = new System.Drawing.Size(117, 24);
            this.IgnorePattern.TabIndex = 16;
            this.IgnorePattern.Text = "Ignore Pattern WS";
            this.toolTip1.SetToolTip(this.IgnorePattern, "Ignore unescaped whitespace, allow comments after # ");
            this.IgnorePattern.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // ECMA
            // 
            this.ECMA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ECMA.Location = new System.Drawing.Point(726, 86);
            this.ECMA.Name = "ECMA";
            this.ECMA.Size = new System.Drawing.Size(97, 24);
            this.ECMA.TabIndex = 14;
            this.ECMA.Text = "ECMA Script";
            this.toolTip1.SetToolTip(this.ECMA, "ECMA script compliant, can only use with ignore case and multiline ");
            this.ECMA.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // RightToLeftBox
            // 
            this.RightToLeftBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RightToLeftBox.Location = new System.Drawing.Point(726, 107);
            this.RightToLeftBox.Name = "RightToLeftBox";
            this.RightToLeftBox.Size = new System.Drawing.Size(84, 24);
            this.RightToLeftBox.TabIndex = 15;
            this.RightToLeftBox.Text = "Right to Left";
            this.toolTip1.SetToolTip(this.RightToLeftBox, "Search from Right to Left");
            this.RightToLeftBox.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // RegexList
            // 
            this.RegexList.AllowDrop = true;
            this.RegexList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RegexList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RegexList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegexList.Items.AddRange(new object[] {
            "\\w*(?x)        # All words",
            "(?<=(?:\\s|\\G|\\A))\\w{5,6}(?=(?:\\s|\\Z|\\.|\\?|\\!))(?x)      # 5 and 6 letter words",
            "(?<Zip>\\d{5})-(?<Sub>\\d{4})(?x)      # Zip Codes",
            "(?<Month>\\d{1,2})/(?<Day>\\d{1,2})/(?<Year>(?:\\d{4}|\\d{2}))(?x)     # Dates",
            "\\((?<AreaCode>\\d{3})\\)\\s*(?<Number>\\d{3}(?:-|\\s*)\\d{4})(?x)        # Phone number" +
                "s",
            "(?sx-m)[^\\r\\n].*?(?:(?:\\.|\\?|!)\\s)     # Sentences",
            "(?<First>[01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.(?<Second>[01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.(?" +
                "<Third>[01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.(?<Fourth>[01]?\\d\\d?|2[0-4]\\d|25[0-5])(?x) " +
                "      #IP Addresses",
            "(?<Protocol>\\w+):\\/\\/(?<Domain>[\\w.]+\\/?)\\S*(?x)        # URL",
            "(a(b)c)+(?x)   # Shows multiple captures in each group"});
            this.RegexList.Location = new System.Drawing.Point(124, 10);
            this.RegexList.Name = "RegexList";
            this.RegexList.Size = new System.Drawing.Size(486, 21);
            this.RegexList.TabIndex = 4;
            this.toolTip1.SetToolTip(this.RegexList, "You may select a regular expression from this list");
            this.RegexList.SelectedIndexChanged += new System.EventHandler(this.RegexList_SelectedIndexChanged);
            // 
            // Tree
            // 
            this.Tree.Dock = System.Windows.Forms.DockStyle.Top;
            this.Tree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tree.Location = new System.Drawing.Point(0, 17);
            this.Tree.Name = "Tree";
            this.Tree.Size = new System.Drawing.Size(424, 210);
            this.Tree.TabIndex = 2;
            this.toolTip1.SetToolTip(this.Tree, "Displays the matches and matched groups.");
            this.Tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Tree_AfterSelect);
            // 
            // ResultsBox
            // 
            this.ResultsBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultsBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultsBox.Location = new System.Drawing.Point(0, 230);
            this.ResultsBox.Multiline = true;
            this.ResultsBox.Name = "ResultsBox";
            this.ResultsBox.ReadOnly = true;
            this.ResultsBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ResultsBox.Size = new System.Drawing.Size(424, 185);
            this.ResultsBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.ResultsBox, "Results are shown here.");
            this.ResultsBox.WordWrap = false;
            // 
            // InputBox
            // 
            this.InputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InputBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputBox.HideSelection = false;
            this.InputBox.Location = new System.Drawing.Point(0, 17);
            this.InputBox.MaxLength = 1000000000;
            this.InputBox.Multiline = true;
            this.InputBox.Name = "InputBox";
            this.InputBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.InputBox.Size = new System.Drawing.Size(421, 420);
            this.InputBox.TabIndex = 1;
            this.InputBox.Text = resources.GetString("InputBox.Text");
            this.toolTip1.SetToolTip(this.InputBox, "Place sample input data here");
            this.InputBox.WordWrap = false;
            this.InputBox.TextChanged += new System.EventHandler(this.InputBox_TextChanged);
            // 
            // TopPanel
            // 
            this.TopPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TopPanel.Controls.Add(this.Compiled);
            this.TopPanel.Controls.Add(this.BuildBtn);
            this.TopPanel.Controls.Add(this.RegexBox);
            this.TopPanel.Controls.Add(this.ExitBtn);
            this.TopPanel.Controls.Add(this.Replace);
            this.TopPanel.Controls.Add(this.label4);
            this.TopPanel.Controls.Add(this.ReplaceBtn);
            this.TopPanel.Controls.Add(this.IgnoreCase);
            this.TopPanel.Controls.Add(this.RegexList);
            this.TopPanel.Controls.Add(this.RightToLeftBox);
            this.TopPanel.Controls.Add(this.label1);
            this.TopPanel.Controls.Add(this.Multiline);
            this.TopPanel.Controls.Add(this.Explicit);
            this.TopPanel.Controls.Add(this.Singleline);
            this.TopPanel.Controls.Add(this.IgnorePattern);
            this.TopPanel.Controls.Add(this.RunBtn);
            this.TopPanel.Controls.Add(this.ECMA);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 28);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(848, 160);
            this.TopPanel.TabIndex = 0;
            // 
            // Compiled
            // 
            this.Compiled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Compiled.Checked = true;
            this.Compiled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Compiled.Location = new System.Drawing.Point(629, 129);
            this.Compiled.Name = "Compiled";
            this.Compiled.Size = new System.Drawing.Size(84, 24);
            this.Compiled.TabIndex = 9;
            this.Compiled.Text = "Compiled";
            this.toolTip1.SetToolTip(this.Compiled, "Compile to assembly, slower start up, faster search");
            this.Compiled.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // BuildBtn
            // 
            this.BuildBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BuildBtn.Location = new System.Drawing.Point(623, 68);
            this.BuildBtn.Name = "BuildBtn";
            this.BuildBtn.Size = new System.Drawing.Size(87, 23);
            this.BuildBtn.TabIndex = 7;
            this.BuildBtn.Text = "Show &Builder";
            this.toolTip1.SetToolTip(this.BuildBtn, "Show the regular expression design tool");
            this.BuildBtn.Click += new System.EventHandler(this.BuildBtn_Click);
            // 
            // RegexBox
            // 
            this.RegexBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RegexBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegexBox.HideSelection = false;
            this.RegexBox.Location = new System.Drawing.Point(7, 37);
            this.RegexBox.Multiline = true;
            this.RegexBox.Name = "RegexBox";
            this.RegexBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.RegexBox.Size = new System.Drawing.Size(605, 61);
            this.RegexBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.RegexBox, "Enter a regular expression here");
            this.RegexBox.TextChanged += new System.EventHandler(this.RegexBox_TextChanged);
            // 
            // ExitBtn
            // 
            this.ExitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExitBtn.Location = new System.Drawing.Point(623, 97);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(87, 23);
            this.ExitBtn.TabIndex = 8;
            this.ExitBtn.Text = "E&xit";
            this.toolTip1.SetToolTip(this.ExitBtn, "Exit Expresso");
            this.ExitBtn.Click += new System.EventHandler(this.mnExit_Click);
            // 
            // Replace
            // 
            this.Replace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Replace.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Replace.HideSelection = false;
            this.Replace.Location = new System.Drawing.Point(8, 124);
            this.Replace.Name = "Replace";
            this.Replace.Size = new System.Drawing.Size(605, 23);
            this.Replace.TabIndex = 3;
            this.Replace.Text = "$& [${Day}-${Month}-${Year}]";
            this.toolTip1.SetToolTip(this.Replace, "Enter a replacement string in this box");
            this.Replace.TextChanged += new System.EventHandler(this.RegexBox_TextChanged);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(192, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "Replacement &String";
            // 
            // ReplaceBtn
            // 
            this.ReplaceBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceBtn.Location = new System.Drawing.Point(623, 37);
            this.ReplaceBtn.Name = "ReplaceBtn";
            this.ReplaceBtn.Size = new System.Drawing.Size(87, 23);
            this.ReplaceBtn.TabIndex = 6;
            this.ReplaceBtn.Text = "&Replace";
            this.toolTip1.SetToolTip(this.ReplaceBtn, "Find matches and then replace with the text in the replacement string");
            this.ReplaceBtn.Click += new System.EventHandler(this.ReplaceBtn_Click);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnFile,
            this.menuItem5,
            this.menuItem2});
            // 
            // mnFile
            // 
            this.mnFile.Index = 0;
            this.mnFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnNewProject,
            this.mnOpenProject,
            this.mnSaveProject,
            this.mnSaveProjectAs,
            this.mnOpen,
            this.menuItem6,
            this.mnSampleProject,
            this.mnMRUStart,
            this.mnMRUEnd,
            this.mnExit});
            this.mnFile.Text = "&File";
            // 
            // mnNewProject
            // 
            this.mnNewProject.Index = 0;
            this.mnNewProject.Text = "&New Project";
            this.mnNewProject.Click += new System.EventHandler(this.mnNewProject_Click);
            // 
            // mnOpenProject
            // 
            this.mnOpenProject.Index = 1;
            this.mnOpenProject.Text = "&Open Project ...";
            this.mnOpenProject.Click += new System.EventHandler(this.mnOpenProject_Click);
            // 
            // mnSaveProject
            // 
            this.mnSaveProject.Index = 2;
            this.mnSaveProject.Text = "&Save Project";
            this.mnSaveProject.Click += new System.EventHandler(this.mnSaveProject_Click);
            // 
            // mnSaveProjectAs
            // 
            this.mnSaveProjectAs.Index = 3;
            this.mnSaveProjectAs.Text = "Save Project &As ...";
            this.mnSaveProjectAs.Click += new System.EventHandler(this.mnSaveProjectAs_Click);
            // 
            // mnOpen
            // 
            this.mnOpen.Index = 4;
            this.mnOpen.Text = "&Read Input Data ...";
            this.mnOpen.Click += new System.EventHandler(this.mnOpen_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 5;
            this.menuItem6.Text = "-";
            // 
            // mnSampleProject
            // 
            this.mnSampleProject.Index = 6;
            this.mnSampleProject.Text = "Open Sample &Project";
            this.mnSampleProject.Click += new System.EventHandler(this.mnSampleProject_Click);
            // 
            // mnMRUStart
            // 
            this.mnMRUStart.Index = 7;
            this.mnMRUStart.Text = "-";
            // 
            // mnMRUEnd
            // 
            this.mnMRUEnd.Index = 8;
            this.mnMRUEnd.Text = "-";
            this.mnMRUEnd.Visible = false;
            // 
            // mnExit
            // 
            this.mnExit.Index = 9;
            this.mnExit.Text = "E&xit";
            this.mnExit.Click += new System.EventHandler(this.mnExit_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnMakeC,
            this.mnMakeVB,
            this.menuItem1,
            this.mnMakeAssembly});
            this.menuItem5.Text = "&Code";
            // 
            // mnMakeC
            // 
            this.mnMakeC.Index = 0;
            this.mnMakeC.Text = "View &C# Code";
            this.mnMakeC.Click += new System.EventHandler(this.mnMakeC_Click);
            // 
            // mnMakeVB
            // 
            this.mnMakeVB.Index = 1;
            this.mnMakeVB.Text = "View &Visual Basic Code";
            this.mnMakeVB.Click += new System.EventHandler(this.mnMakeVB_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnSaveCCode,
            this.mnSaveVBCode});
            this.menuItem1.Text = "&Save Code to a File";
            // 
            // mnSaveCCode
            // 
            this.mnSaveCCode.Index = 0;
            this.mnSaveCCode.Text = "&C# ...";
            this.mnSaveCCode.Click += new System.EventHandler(this.mnSaveCCode_Click);
            // 
            // mnSaveVBCode
            // 
            this.mnSaveVBCode.Index = 1;
            this.mnSaveVBCode.Text = "&Visual Basic ...";
            this.mnSaveVBCode.Click += new System.EventHandler(this.mnSaveCCode_Click);
            // 
            // mnMakeAssembly
            // 
            this.mnMakeAssembly.Index = 3;
            this.mnMakeAssembly.Text = "Compile to &Assembly ...";
            this.mnMakeAssembly.Click += new System.EventHandler(this.mnMakeAssembly_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 2;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnWebLink,
            this.menuItem4,
            this.mnAbout});
            this.menuItem2.Text = "&Help";
            // 
            // mnWebLink
            // 
            this.mnWebLink.Index = 0;
            this.mnWebLink.Text = "&Regular Expressions ...";
            this.mnWebLink.Click += new System.EventHandler(this.mnWebLink_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 1;
            this.menuItem4.Text = "-";
            // 
            // mnAbout
            // 
            this.mnAbout.Index = 2;
            this.mnAbout.Text = "&About Expresso ...";
            this.mnAbout.Click += new System.EventHandler(this.mnAbout_Click);
            // 
            // dlgOpen
            // 
            this.dlgOpen.AddExtension = false;
            this.dlgOpen.DefaultExt = "xso";
            this.dlgOpen.Filter = "Expresso Project Files|*.xso|All files|*.*";
            this.dlgOpen.Title = "Open a file";
            // 
            // LeftPanel
            // 
            this.LeftPanel.Controls.Add(this.InputBox);
            this.LeftPanel.Controls.Add(this.label3);
            this.LeftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftPanel.Location = new System.Drawing.Point(0, 188);
            this.LeftPanel.Name = "LeftPanel";
            this.LeftPanel.Size = new System.Drawing.Size(421, 437);
            this.LeftPanel.TabIndex = 1;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(421, 188);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 437);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // dlgSave
            // 
            this.dlgSave.DefaultExt = "txt";
            this.dlgSave.FileName = "expresso";
            this.dlgSave.Filter = "Text Files|*.txt";
            // 
            // RightPanel
            // 
            this.RightPanel.Controls.Add(this.ResultsBox);
            this.RightPanel.Controls.Add(this.splitter2);
            this.RightPanel.Controls.Add(this.Tree);
            this.RightPanel.Controls.Add(this.statusBar);
            this.RightPanel.Controls.Add(this.label2);
            this.RightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightPanel.Location = new System.Drawing.Point(424, 188);
            this.RightPanel.Name = "RightPanel";
            this.RightPanel.Size = new System.Drawing.Size(424, 437);
            this.RightPanel.TabIndex = 11;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 227);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(424, 3);
            this.splitter2.TabIndex = 9;
            this.splitter2.TabStop = false;
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 415);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2,
            this.statusBarPanel3});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(424, 22);
            this.statusBar.TabIndex = 6;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Width = 150;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.Name = "statusBarPanel2";
            // 
            // statusBarPanel3
            // 
            this.statusBarPanel3.Name = "statusBarPanel3";
            // 
            // ToolBar
            // 
            this.ToolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.ToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.NewToolBtn,
            this.OpenToolBtn,
            this.SaveToolBtn,
            this.SeparatorToolBtn1,
            this.MatchToolBtn,
            this.ReplaceToolBtn,
            this.SeparatorToolBtn2,
            this.BuildToolBtn});
            this.ToolBar.DropDownArrows = true;
            this.ToolBar.ImageList = this.ToolImages;
            this.ToolBar.Location = new System.Drawing.Point(0, 0);
            this.ToolBar.Name = "ToolBar";
            this.ToolBar.ShowToolTips = true;
            this.ToolBar.Size = new System.Drawing.Size(848, 28);
            this.ToolBar.TabIndex = 17;
            this.ToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.ToolBar_ButtonClick);
            // 
            // NewToolBtn
            // 
            this.NewToolBtn.ImageIndex = 0;
            this.NewToolBtn.Name = "NewToolBtn";
            this.NewToolBtn.ToolTipText = "New Project";
            // 
            // OpenToolBtn
            // 
            this.OpenToolBtn.ImageIndex = 1;
            this.OpenToolBtn.Name = "OpenToolBtn";
            this.OpenToolBtn.ToolTipText = "Open Project";
            // 
            // SaveToolBtn
            // 
            this.SaveToolBtn.ImageIndex = 2;
            this.SaveToolBtn.Name = "SaveToolBtn";
            this.SaveToolBtn.ToolTipText = "Save Project";
            // 
            // SeparatorToolBtn1
            // 
            this.SeparatorToolBtn1.ImageIndex = 3;
            this.SeparatorToolBtn1.Name = "SeparatorToolBtn1";
            this.SeparatorToolBtn1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // MatchToolBtn
            // 
            this.MatchToolBtn.ImageIndex = 3;
            this.MatchToolBtn.Name = "MatchToolBtn";
            this.MatchToolBtn.ToolTipText = "Find Matches";
            // 
            // ReplaceToolBtn
            // 
            this.ReplaceToolBtn.ImageIndex = 4;
            this.ReplaceToolBtn.Name = "ReplaceToolBtn";
            this.ReplaceToolBtn.ToolTipText = "Replace";
            // 
            // SeparatorToolBtn2
            // 
            this.SeparatorToolBtn2.Name = "SeparatorToolBtn2";
            this.SeparatorToolBtn2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // BuildToolBtn
            // 
            this.BuildToolBtn.ImageIndex = 5;
            this.BuildToolBtn.Name = "BuildToolBtn";
            this.BuildToolBtn.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.BuildToolBtn.ToolTipText = "Show Builder";
            // 
            // ToolImages
            // 
            this.ToolImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ToolImages.ImageStream")));
            this.ToolImages.TransparentColor = System.Drawing.Color.Transparent;
            this.ToolImages.Images.SetKeyName(0, "");
            this.ToolImages.Images.SetKeyName(1, "");
            this.ToolImages.Images.SetKeyName(2, "");
            this.ToolImages.Images.SetKeyName(3, "");
            this.ToolImages.Images.SetKeyName(4, "");
            this.ToolImages.Images.SetKeyName(5, "");
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(848, 625);
            this.Controls.Add(this.RightPanel);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.LeftPanel);
            this.Controls.Add(this.TopPanel);
            this.Controls.Add(this.ToolBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Expresso - For Building and Testing Regular Expressions";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.TopPanel.ResumeLayout(false);
            this.TopPanel.PerformLayout();
            this.LeftPanel.ResumeLayout(false);
            this.LeftPanel.PerformLayout();
            this.RightPanel.ResumeLayout(false);
            this.RightPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			Application.Run(new MainForm(args));
		}

		/// <summary>
		/// This makes the regular expression and traps exceptions in case the regular
		/// expression is ill-formed
		/// </summary>
		/// <returns></returns>
		private Regex MakeRegex()
		{
			Regex r;
			// First set the Regex options based on the check boxes
			TheOptions=RegexOptions.None;
			if(IgnoreCase.Checked)TheOptions|=RegexOptions.IgnoreCase;
			if(ECMA.Checked)TheOptions|=RegexOptions.ECMAScript;
			if(Explicit.Checked)TheOptions|=RegexOptions.ExplicitCapture;
			if(IgnorePattern.Checked)TheOptions|=RegexOptions.IgnorePatternWhitespace;
			if(Multiline.Checked)TheOptions|=RegexOptions.Multiline;
			if(RightToLeftBox.Checked)TheOptions|=RegexOptions.RightToLeft;
			if(Singleline.Checked)TheOptions|=RegexOptions.Singleline;
			
			try
			{
				r = new Regex(RegexBox.Text,TheOptions);
				// Store the regular expression in the combobox list
				string NewText=RegexBox.Text;
				if(!RegexList.Items.Contains(NewText))RegexList.Items.Add(RegexBox.Text);
				return r;
			}
			catch (Exception ex)
			{
				MessageBox.Show("There was an error in the regular expression!\n\n"
					+ex.Message+"\n","Expresso Error",
					MessageBoxButtons.OK,MessageBoxIcon.Error);
				return null;
			}
		}

		/// <summary>
		/// Handle the Run Match button click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RunBtn_Click(object sender, System.EventArgs e)
		{
			RunMatch();
		}

		/// <summary>
		/// Look for matches for the regular expression
		/// </summary>
		private void RunMatch()
		{
			Regex r;
			Match m;

			statusBar.Panels[0].Text="";
			statusBar.Panels[1].Text="";
			statusBar.Panels[2].Text="";

			InputBox.Select(0,0);   // Unselect all the text
			Dirty=false;
			Skip=true;

			this.Cursor=Cursors.WaitCursor;
			if((r=MakeRegex())==null)
			{
				this.Cursor=Cursors.Default;
				return;
			}

			Tree.Nodes.Clear();
			ResultsBox.Text="";
			ShowBuilder(false);
			ShowTree(true);
			this.Cursor=Cursors.Default;

			// Store the results in the text box
			for (m = r.Match(InputBox.Text); m.Success; m = m.NextMatch()) 
			{
				if(m.Value.Length>0)
				{
					Tree.Nodes.Add("["+m.Value + "]");
					int ThisNode=Tree.Nodes.Count-1;
					Tree.Nodes[ThisNode].Tag=m;
					if(m.Groups.Count>1)
					{
						for (int i=1;i<m.Groups.Count;i++)
						{
							Tree.Nodes[ThisNode].Nodes.Add(r.GroupNameFromNumber(i)+": ["+m.Groups[i].Value+"]");
							Tree.Nodes[ThisNode].Nodes[i-1].Tag=m.Groups[i];
              //This bit of code puts in another level of nodes showing the captures for each group
							int Number=m.Groups[i].Captures.Count;
							if(Number>1)
								for(int j=0;j<Number;j++)
								{
									Tree.Nodes[ThisNode].Nodes[i-1].Nodes.Add(m.Groups[i].Captures[j].Value);
									Tree.Nodes[ThisNode].Nodes[i-1].Nodes[j].Tag=m.Groups[i].Captures[j];
								}
						}
					}
				}
			}
			statusBar.Panels[0].Text=Tree.Nodes.Count.ToString()+" Matches";
			Skip=false;
		}

		/// <summary>
		/// Handle the "Replace" button click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReplaceBtn_Click(object sender, System.EventArgs e)
		{
			RunReplace();
		}
		
		
		/// <summary>
		/// Make the regular expression and execute a "Replace" operation
		/// </summary>
		private void RunReplace()
		{
			Regex r;
			this.Cursor=Cursors.WaitCursor;
			statusBar.Panels[0].Text="";
			statusBar.Panels[1].Text="";
			statusBar.Panels[2].Text="";

			InputBox.Select(0,0);   // Unselect all the text
			Dirty=false;
			Skip=true;

			if((r=MakeRegex())==null)
			{
				this.Cursor=Cursors.Default;
				return;
			}

			Tree.Nodes.Clear();
			ShowBuilder(false);
			ShowTree(false);
			this.Cursor=Cursors.Default;
			ResultsBox.Text=r.Replace(InputBox.Text,Replace.Text);
			Skip=false;
			statusBar.Panels[0].Text="";
		}

		/// <summary>
		/// Read application settings that are stored in the registry
		/// </summary>
		private void ReadRegistryData()
		{
			Savior.Read(regSettings,ExpressoKey);
			this.Location=regSettings.Location;
			this.MRUList.Clear();
			this.MRUList.AddRange(regSettings.MRUList);
			this.ProjectFileName=regSettings.ProjectFile;
			string OpenPath=regSettings.OpenPathName;
			string SavePath=regSettings.SavePathName;
			string ApplicationPath=Path.GetDirectoryName(Application.ExecutablePath);
			if(OpenPath=="")OpenPath=ApplicationPath;
			if(SavePath=="")SavePath=ApplicationPath;
			this.dlgOpen.InitialDirectory=OpenPath;
			this.dlgSave.InitialDirectory=SavePath;

			if(ProjectFileName!="")
			{
				FileInfo fi = new FileInfo(ProjectFileName);
				if(fi.Exists)this.OpenProject(ProjectFileName);
				else ProjectFileName="";
			}
			this.UpdateMRU();
		}

		/// <summary>
		/// Read data from the Project File
		/// </summary>
		private bool ReadFileData()
		{
			settings = (Settings)Savior.ReadFromFile(settings,ProjectFileName);
			if(settings==null)return false;
			InputBox.Text=settings.InputData;
			Replace.Text=settings.ReplacementString;
			RegexBox.Text=settings.RegularExpression;
			MakeForm.FileName=settings.FileName;
			MakeForm.IsPublic=settings.IsPublic;
			MakeForm.Namespace=settings.Namespace;
			MakeForm.ClassName=settings.ClassName;
			IgnoreCase.Checked=settings.IgnoreCase;
			Multiline.Checked=settings.Multiline;
			Singleline.Checked=settings.Singleline;
			Explicit.Checked=settings.ExplicitCapture;
			ECMA.Checked=settings.ECMAScript;
			RightToLeftBox.Checked=settings.RightToLeft;
			IgnorePattern.Checked=settings.IgnorePatternWS;
			Compiled.Checked=settings.Compiled;
			LeftPanel.Width=settings.LeftPanelWidth;
			Tree.Height=settings.TreeHeight;
			SaveTreeHeight=Tree.Height;
			this.Size=settings.Size;
			return true;
		}

		/// <summary>
		/// Save application settings that are stored in the registry
		/// </summary>
		private void SaveRegistryData()
		{
			regSettings.Location=this.Location;
			regSettings.MRUList=this.MRUList.GetFileNames();
			if(ProjectFileName=="NewProject.xso")ProjectFileName="";
			regSettings.ProjectFile=ProjectFileName;
			if(dlgOpen.FileName!="")regSettings.OpenPathName=Path.GetDirectoryName(dlgOpen.FileName);
			if(dlgSave.FileName!="")regSettings.SavePathName=Path.GetDirectoryName(dlgSave.FileName);
			
			Savior.Save(regSettings,ExpressoKey);
		}

		/// <summary>
		/// Save data to the Project File
		/// </summary>
		private bool SaveFileData()
		{
			settings.InputData=InputBox.Text;
			settings.IsPublic=MakeForm.IsPublic;
			settings.Namespace=MakeForm.Namespace;
			settings.ClassName=MakeForm.ClassName;
			settings.InputData=InputBox.Text;
			settings.ReplacementString=Replace.Text;
			settings.RegularExpression=RegexBox.Text;
			settings.IgnoreCase=IgnoreCase.Checked;
			settings.Multiline=Multiline.Checked;
			settings.Singleline=Singleline.Checked;
			settings.ExplicitCapture=Explicit.Checked;
			settings.ECMAScript=ECMA.Checked;
			settings.RightToLeft=RightToLeftBox.Checked;
			settings.IgnorePatternWS=IgnorePattern.Checked;
			settings.Compiled=Compiled.Checked;
			settings.LeftPanelWidth=LeftPanel.Width;
			if(IsVisible)settings.TreeHeight=Tree.Height;
			else settings.TreeHeight=SaveTreeHeight;
			settings.Size=this.Size;
			if(Savior.SaveToFile(settings, ProjectFileName))return true;
			else return false;
		}


		/// <summary>
		/// Respond to selections in the output tree by displaying the appropriate information
		/// in the results area and the status bar. Also highlights the appropriate text in the
		/// input data.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if(Skip)return;
			if(Tree.SelectedNode.Parent==null) // Must be the top level node
			{
				Match m=(Match)Tree.SelectedNode.Tag;
				ResultsBox.Text=m.Value;
				statusBar.Panels[1].Text="Position: "+m.Index;
				statusBar.Panels[2].Text="Length: "+m.Length;
				if(m!=null && !Dirty)
				{
					InputBox.Select(m.Index,m.Length);
					InputBox.ScrollToCaret();
				}
			}
			else if(Tree.SelectedNode.Parent.Parent==null) // Must be a group
			{
				Group g=(Group)Tree.SelectedNode.Tag;
				ResultsBox.Text=g.Value;
				statusBar.Panels[1].Text="Position: "+g.Index;
				statusBar.Panels[2].Text="Length: "+g.Length;
				if(g!=null && !Dirty)
				{
					InputBox.Select(g.Index,g.Length);
					InputBox.ScrollToCaret();
				}
			}
			else // Must be a capture
			{
				Capture c=(Capture)Tree.SelectedNode.Tag;
				ResultsBox.Text=c.Value;
				statusBar.Panels[1].Text="Position: "+c.Index;
				statusBar.Panels[2].Text="Length: "+c.Length;
				if(c!=null && !Dirty)
				{
					InputBox.Select(c.Index,c.Length);
					InputBox.ScrollToCaret();
				}
			}
		}

		/// <summary>
		/// Toggle the visibility of the Regular Expression Builder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BuildBtn_Click(object sender, System.EventArgs e)
		{
			ShowBuilder(!builder.Visible);   // Toggle the visibility of builder
		}

		/// <summary>
		/// This either shows or hides the Builder, depending on the ShowIt parameter
		/// </summary>
		/// <param name="ShowIt">If true, show the builder, otherwise hide it.</param>
		public void ShowBuilder(bool ShowIt)
		{
			if(ShowIt)
			{
				BuildBtn.Text="Hide &Builder";
				ResultsBox.Visible=false;
				LeftPanel.Visible=false;
				statusBar.Visible=false;
				Tree.Visible=false;
				splitter1.Enabled=false;
				splitter2.Enabled=false;
				label2.Visible=false;
				builder.Visible=true;
				this.BuildToolBtn.Pushed=true;
				this.BuildToolBtn.ToolTipText="Hide Builder";
			}
			else 
			{
				builder.Visible=false;
				ResultsBox.Visible=true;
				LeftPanel.Visible=true;
				statusBar.Visible=true;
				Tree.Visible=true;
				BuildBtn.Text="Show &Builder";
				splitter1.Enabled=true;
				splitter2.Enabled=true;
				label2.Visible=true;
				this.BuildToolBtn.Pushed=false;
				this.BuildToolBtn.ToolTipText="Show Builder";
			}
		}

		/// <summary>
		/// Shows or hides the TreeList which displays the results of a search
		/// </summary>
		/// <param name="ShowIt"></param>
		public void ShowTree(bool ShowIt)
		{
			if(ShowIt)
			{
				//if(IsVisible)return;
				IsVisible=true;

				statusBar.Visible=true;
				if(SaveTreeHeight!=0)Tree.Height=SaveTreeHeight;
			}
			else
			{
				//if(!IsVisible)return;
				IsVisible=false;

				statusBar.Visible=false;
				if(Tree.Height!=0)SaveTreeHeight=Tree.Height;
				Tree.Height=0;
			}
		}

		/// <summary>
		/// Insert text into the regular expression
		/// </summary>
		/// <param name="input"></param>
		/// <param name="CursorIndex">How many steps back should the cursor be placed?</param>
		public void InsertRegex(string input, int CursorIndex)
		{
			int Start=RegexBox.SelectionStart;
			int Length=RegexBox.SelectionLength;
			string NewText=RegexBox.Text.Remove(Start,Length);
			RegexBox.Text=NewText.Insert(Start,input);
			RegexBox.Select(Start+input.Length-CursorIndex,0);
		}

		/// <summary>
		/// Insert text into the replacement expression
		/// </summary>
		/// <param name="input"></param>
		public void InsertReplaceText(string input, int CursorIndex)
		{
			int Start=Replace.SelectionStart;
			int Length=Replace.SelectionLength;
			string NewText=Replace.Text.Remove(Start,Length);
			Replace.Text=NewText.Insert(Start,input);
			Replace.Select(Start+input.Length-CursorIndex,0);
		}

		private void RegexList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RegexBox.Text=(string)RegexList.SelectedItem;
		}

		private void InputBox_TextChanged(object sender, System.EventArgs e)
		{
			Dirty=true;
			EnableSave(true);
		}

		private void mnWebLink_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconregularexpressionslanguageelements.asp");
		}

		
		private void mnExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void mnAbout_Click(object sender, System.EventArgs e)
		{
			if(about==null)about = new AboutBox(this);
			about.Show();
		}

		/// <summary>
		/// Read a file and put the text into the Input box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnOpen_Click(object sender, System.EventArgs e)
		{
			this.ShowBuilder(false);
			dlgOpen.Title="Read sample text from a file";
			ReadFile((Control)InputBox);
		}


		/// <summary>
		/// Read a file and put the test into the Regular Expression box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnOpenRegex_Click(object sender, System.EventArgs e)
		{
			dlgOpen.Title="Read a regular expression from a file";
			ReadFile((Control)RegexBox);
		}

		/// <summary>
		/// Read the text of a control from a file
		/// </summary>
		/// <param name="TextBox"></param>
		private void ReadFile(Control TextBox)
		{
			dlgOpen.Filter="Text Files|*.txt|All files|*.*";
			if(dlgOpen.ShowDialog()==DialogResult.OK)
			{
				try
				{
					StreamReader reader = new StreamReader(dlgOpen.FileName);
					TextBox.Text=reader.ReadToEnd();
				}
				catch
				{
					MessageBox.Show("Problem reading the file!\n\n"+dlgOpen.FileName,
						"Expresso Error",
						MessageBoxButtons.OK,MessageBoxIcon.Error);
				}
			}
		}

		/// <summary>
		/// Save the text of a control into a file
		/// </summary>
		/// <param name="TextBox"></param>
		private void SaveFile(Control TextBox)
		{
			if(dlgSave.ShowDialog()==DialogResult.OK)
			{
				try
				{
					StreamWriter writer = new StreamWriter(dlgSave.FileName);
					writer.Write(TextBox.Text);
					writer.Close();
				}
				catch
				{
					MessageBox.Show("Problem reading the file!");
				}
			}
		}

		/// <summary>
		/// Save the contents of the results box to a file
		/// </summary>
		private void SaveResults()
		{
			ShowBuilder(false);
			dlgSave.Title="Save the results to a file";
			SaveFile((Control)ResultsBox);
		}

		/// <summary>
		/// Save the contents of the input box to a file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnSaveInput_Click(object sender, System.EventArgs e)
		{
			ShowBuilder(false);
			dlgSave.Title="Save the sample text to a file";
			SaveFile((Control)InputBox);
		}

		/// <summary>
		/// Save the regular expression to a file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnSaveRegex_Click(object sender, System.EventArgs e)
		{
			dlgSave.Title="Save the regular expression to a file";
			SaveFile((Control)RegexBox);
		}

		/// <summary>
		/// Save code to a file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnSaveCCode_Click(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			if(item==this.mnSaveCCode)
				MakeCode("C");
			else if(item==this.mnSaveVBCode)
				MakeCode("VB");
			dlgSave.Filter="Program Files (*.cs;*.cpp;*.vb;*.txt)|*.cs;*.cpp;*.vb;*.txt|All Files (*.*)|*.*";
			this.SaveResults();
		}

		private void mnMakeC_Click(object sender, System.EventArgs e)
		{
			MakeCode("C");
		}

		private void mnMakeVB_Click(object sender, System.EventArgs e)
		{
			MakeCode("VB");
		}
		
		/// <summary>
		/// Generate code for a regular expression in either C# or VB
		/// </summary>
		/// <param name="language">If "C" generates C# code, if "VB" generates Visual Basic .NET code.</param>
		private void MakeCode(string language)
		{
			ResultsBox.Text="";
			if(MakeRegex()==null)return;            // Check to be sure Regex is syntactically correct
			ShowBuilder(false);
			ShowTree(false);
			string Start,Or,LineEnd,Terminate,Body,LiteralBreak;
      
			if(language=="C")
			{
				LineEnd="\r\n    ";
				LiteralBreak="\""+LineEnd+"+ @\"";
				Start="using System.Text.RegularExpressions;\r\n\r\n"
					+"Regex regex = new Regex(\r\n    @\"";
				Or="| ";
				Terminate=";";
			}
			else if(language=="VB")
			{
				LineEnd=" _\r\n    ";
				LiteralBreak="\""+LineEnd+"+ \"";
				Start="Imports System.Text.RegularExpressions\r\n\r\n"
					+"Dim regex = New Regex( _\r\n    \"";
				Or="Or ";
				Terminate="";
			}
			else return;

			// Format the regular expression
			Body = RegexBox.Text;			
			// Double any quotes
			Body = Body.Replace("\"","\"\"");
			// Shorten the regular expression so it fits on a reasonable line of code
			for(int i=60;i<Body.Length;i+=60)
			{
				if(Body.Substring(i,1)=="\"")
				{  // Keep stepping forward one character at a time to find a spot that does not directly
					// follow a quote character in which to put a line break
					i-=59;  // Increment by one
					continue;
				}
				Body = Body.Insert(i,LiteralBreak);
				i+=LiteralBreak.Length;
			}

			string options="";
			if(IgnoreCase.Checked)options+=Or+"RegexOptions.IgnoreCase"+LineEnd;
			if(Multiline.Checked)options+=Or+"RegexOptions.Multiline"+LineEnd;
			if(Singleline.Checked)options+=Or+"RegexOptions.Singleline"+LineEnd;
			if(Explicit.Checked)options+=Or+"RegexOptions.ExplicitCapture"+LineEnd;
			if(ECMA.Checked)options+=Or+"RegexOptions.ECMAScript"+LineEnd;
			if(RightToLeftBox.Checked)options+=Or+"RegexOptions.RightToLeft"+LineEnd;
			if(IgnorePattern.Checked)options+=Or+"RegexOptions.IgnorePatternWhitespace"+LineEnd;
			if(Compiled.Checked)options+=Or+"RegexOptions.Compiled"+LineEnd;
			if(options=="")options="RegexOptions.None"+LineEnd;
			else options=options.Substring(Or.Length);
			ResultsBox.Text=Start+Body+"\","+LineEnd+options+")"+Terminate;
		}

		/// <summary>
		/// Process a change in the option check boxes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionCheckChanged(object sender, System.EventArgs e)
		{
			CheckBox chk = (CheckBox)sender;
			string name = chk.Name;

			if(name=="ECMA")
			{
				if(chk.Checked)
				{
					Singleline.Checked=false;
					Explicit.Checked=false;
					RightToLeftBox.Checked=false;
					IgnorePattern.Checked=false;
				}
			}
			else if(name=="Singleline" || name=="Explicit" || name=="RightToLeftBox" || name=="IgnorePattern")
			{
				if(chk.Checked)
				{
					ECMA.Checked=false;
				}
			}
			EnableSave(true); // The project has changed, so enable the Save menu item			
		}

		private void mnMakeAssembly_Click(object sender, System.EventArgs e)
		{
			if(MakeForm.ShowDialog()==DialogResult.OK)
			{
				if(MakeRegex()==null)return;
				if(Compiled.Checked)TheOptions|=RegexOptions.Compiled;
				RegexCompilationInfo info =new RegexCompilationInfo(RegexBox.Text,TheOptions,MakeForm.ClassName,
					MakeForm.Namespace,MakeForm.IsPublic);
				RegexCompilationInfo[] infos = {info};
				AssemblyName assembly = new AssemblyName();
				assembly.Name=Path.GetFileNameWithoutExtension(MakeForm.FileName);
				Regex.CompileToAssembly(infos,assembly);
				EnableSave(true);
			}
		}

		private void mnNewProject_Click(object sender, System.EventArgs e)
		{
			NewProject();
		}
		
		/// <summary>
		/// Create a new project
		/// </summary>
		private void NewProject()
		{
			Tree.Nodes.Clear();
			InputBox.Text="";
			Replace.Text="";
			RegexBox.Text="";
			ResultsBox.Text="";
			statusBar.Panels[0].Text="";
			statusBar.Panels[1].Text="";
			statusBar.Panels[2].Text="";
			ProjectFileName="NewProject.xso";
			MakeForm.FileName="TheRegexAssembly.dll";
			MakeForm.IsPublic=true;
			MakeForm.Namespace="TheRegex";
			MakeForm.ClassName="TheRegexClass";
			EnableSave(false);
			this.Text="Expresso - Regular Expression Editor - "+ProjectFileName;
		}

		private void mnSampleProject_Click(object sender, System.EventArgs e)
		{
			this.OpenProject(SampleProject);
			this.Text="Expresso - Regular Expression Editor - "+"Sample Project";
		}

		private void mnOpenProject_Click(object sender, System.EventArgs e)
		{
			OpenProject();
		}

		/// <summary>
		/// Select a project file using the Open File Dialog and then open it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenProject()
		{
			dlgOpen.Filter="Expresso Project Files|*.xso|All files|*.*";
			dlgOpen.DefaultExt="xso";
			if(dlgOpen.ShowDialog()==DialogResult.OK)
				OpenProject(dlgOpen.FileName);
		}

		/// <summary>
		/// Open a project file with the given name
		/// </summary>
		/// <param name="FileName">The full path name of the file to open</param>
		private void OpenProject(string FileName)
		{
			ProjectFileName=FileName;

			// Clear all the display areas
			Tree.Nodes.Clear();
			statusBar.Panels[0].Text="";
			statusBar.Panels[1].Text="";
			statusBar.Panels[2].Text="";
			ResultsBox.Text="";

			// Read the data and add the file name to the MRUList only if no error
			if(ReadFileData())
			{
				MRUList.Add(ProjectFileName);
				UpdateMRU();
				this.Text="Expresso - Regular Expression Editor - "+Path.GetFileName(ProjectFileName);
			}
			else 
			{
				MRUList.Remove(ProjectFileName);
				UpdateMRU();
			}
			EnableSave(false);
		}

		/// <summary>
		/// Update the list of most recently used files in the menu
		/// </summary>
		private void UpdateMRU()
		{
			MenuItem mnNewFile;
			int Number= MRUList.Count;
			int StartIndex = mnMRUStart.Index+1;
			int EndIndex = mnMRUEnd.Index-1;

			// First remove any menu items between the start and end separators, starting from the bottom
			for(int i=EndIndex;i>=StartIndex;i--)
				mnFile.MenuItems.RemoveAt(i);

			// Now add menu items for each file name in the MRU list
			for(int i=0;i<Number;i++)
			{
				int ShortCut=(i+1)%10;   // Use the least significant digit as the shortcut
				mnNewFile = new MenuItem("&"+ShortCut.ToString()+" "+MRUList[i], new EventHandler(MRU_OnClick));
				mnFile.MenuItems.Add(StartIndex+i,mnNewFile);
			}
			if(Number==0)mnMRUEnd.Visible=false;
			else mnMRUEnd.Visible=true;
		}

		private void MRU_OnClick(object sender, System.EventArgs e)
		{
			string MenuText=((MenuItem)sender).Text;
			string FileName=MenuText.Remove(0,3);
			OpenProject(FileName);
		}

		/// <summary>
		/// Handle the "Save As" menu click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnSaveProjectAs_Click(object sender, System.EventArgs e)
		{
			SaveProjectAs();
		}

		/// <summary>
		/// Save all project settings to a new file
		/// </summary>
		private void SaveProjectAs()
		{
			dlgSave.Filter="Expresso Project Files|*.xso|All files|*.*";
			dlgSave.DefaultExt="xso";
			dlgSave.FileName=ProjectFileName;
			if(dlgSave.ShowDialog()==DialogResult.OK)
			{
				ProjectFileName=dlgSave.FileName;
				if(SaveFileData())
				{
					MRUList.Add(ProjectFileName);
					this.UpdateMRU();
				}
			}
			EnableSave(false);
			this.Text="Expresso - Regular Expression Editor - "+Path.GetFileName(ProjectFileName);
		}

		/// <summary>
		/// Save project settings to the current project file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnSaveProject_Click(object sender, System.EventArgs e)
		{
			SaveFileData();
			EnableSave(false);
		}

		private void RegexBox_TextChanged(object sender, System.EventArgs e)
		{
			EnableSave(true);
		}

		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(this.mnSaveProject.Enabled)
			{
				if(MessageBox.Show("Would you like to save changes to the project file?",
					"Expresso Project - "+Path.GetFileName(ProjectFileName),
					MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.OK)
				{
					SaveFileData();
				}
			}
			SaveRegistryData();		
		}

		/// <summary>
		/// Handle the tool bar button clicks
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			ToolBarButton btn = e.Button;
			if(btn==this.NewToolBtn)this.NewProject();
			else if(btn==this.OpenToolBtn)this.OpenProject();
			else if(btn==this.SaveToolBtn)this.SaveProjectAs();
			else if(btn==this.MatchToolBtn)this.RunMatch();
			else if(btn==this.ReplaceToolBtn)this.RunReplace();
			else if(btn==this.BuildToolBtn)this.ShowBuilder(!this.builder.Visible);
		}
	}
}
