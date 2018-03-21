namespace TorBrowser
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnBack = new System.Windows.Forms.ToolStripButton();
            this.btnForward = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnLoading = new System.Windows.Forms.ToolStripButton();
            this.txtUrl = new System.Windows.Forms.ToolStripTextBox();
            this.btnGo = new System.Windows.Forms.ToolStripButton();
            this.btnShowTor = new System.Windows.Forms.ToolStripButton();
            this.btnDownloads = new System.Windows.Forms.ToolStripButton();
            this.btnOther = new System.Windows.Forms.ToolStripDropDownButton();
            this.menuCheckTor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDevTools = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDuckDuckGo = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lblBandwidth = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblError = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.showClosedCheckBox = new System.Windows.Forms.CheckBox();
            this.connectionTree = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.circuitTree = new System.Windows.Forms.TreeView();
            this.streamsTree = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.newCircuitButton = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.configGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.routerList = new System.Windows.Forms.ListBox();
            this.tabPages = new FarsiLibrary.Win.FATabStrip();
            this.menuStripTab = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuCloseTab = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCloseOtherTabs = new System.Windows.Forms.ToolStripMenuItem();
            this.tabStrip1 = new FarsiLibrary.Win.FATabStripItem();
            this.tabStripAdd = new FarsiLibrary.Win.FATabStripItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabPages)).BeginInit();
            this.tabPages.SuspendLayout();
            this.menuStripTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBack,
            this.btnForward,
            this.btnRefresh,
            this.btnLoading,
            this.txtUrl,
            this.btnGo,
            this.btnShowTor,
            this.btnDownloads,
            this.btnOther,
            this.btnDuckDuckGo});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1131, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnBack
            // 
            this.btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBack.Image = global::TorBrowser.Properties.Resources.left_round_16;
            this.btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(23, 22);
            this.btnBack.Text = "Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnForward
            // 
            this.btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnForward.Image = global::TorBrowser.Properties.Resources.right_round_16;
            this.btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(23, 22);
            this.btnForward.Text = "Forward";
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::TorBrowser.Properties.Resources.refresh_16;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(23, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnLoading
            // 
            this.btnLoading.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLoading.Image = global::TorBrowser.Properties.Resources.loader2;
            this.btnLoading.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoading.Name = "btnLoading";
            this.btnLoading.Size = new System.Drawing.Size(23, 22);
            // 
            // txtUrl
            // 
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(600, 25);
            this.txtUrl.Enter += new System.EventHandler(this.txtUrl_Enter);
            this.txtUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUrl_KeyDown);
            // 
            // btnGo
            // 
            this.btnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGo.Image = ((System.Drawing.Image)(resources.GetObject("btnGo.Image")));
            this.btnGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(26, 22);
            this.btnGo.Text = "Go";
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnShowTor
            // 
            this.btnShowTor.CheckOnClick = true;
            this.btnShowTor.Image = global::TorBrowser.Properties.Resources.Onion_icon_16x16;
            this.btnShowTor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowTor.Name = "btnShowTor";
            this.btnShowTor.Size = new System.Drawing.Size(77, 22);
            this.btnShowTor.Text = "Show Tor";
            this.btnShowTor.Click += new System.EventHandler(this.btnShowTor_Click);
            // 
            // btnDownloads
            // 
            this.btnDownloads.Image = global::TorBrowser.Properties.Resources.down_round_16;
            this.btnDownloads.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDownloads.Name = "btnDownloads";
            this.btnDownloads.Size = new System.Drawing.Size(86, 22);
            this.btnDownloads.Text = "Downloads";
            this.btnDownloads.Click += new System.EventHandler(this.btnDownloads_Click);
            // 
            // btnOther
            // 
            this.btnOther.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnOther.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOther.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCheckTor,
            this.menuDevTools});
            this.btnOther.Image = ((System.Drawing.Image)(resources.GetObject("btnOther.Image")));
            this.btnOther.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOther.Name = "btnOther";
            this.btnOther.Size = new System.Drawing.Size(29, 22);
            // 
            // menuCheckTor
            // 
            this.menuCheckTor.Image = global::TorBrowser.Properties.Resources.Onion_icon_16x16;
            this.menuCheckTor.Name = "menuCheckTor";
            this.menuCheckTor.Size = new System.Drawing.Size(183, 22);
            this.menuCheckTor.Text = "Check if Tor is active";
            this.menuCheckTor.Click += new System.EventHandler(this.menuCheckTor_Click);
            // 
            // menuDevTools
            // 
            this.menuDevTools.Image = global::TorBrowser.Properties.Resources.gear_16x16;
            this.menuDevTools.Name = "menuDevTools";
            this.menuDevTools.Size = new System.Drawing.Size(183, 22);
            this.menuDevTools.Text = "Dev.Tools";
            this.menuDevTools.Click += new System.EventHandler(this.menuDevTools_Click);
            // 
            // btnDuckDuckGo
            // 
            this.btnDuckDuckGo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnDuckDuckGo.Image = global::TorBrowser.Properties.Resources.download;
            this.btnDuckDuckGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDuckDuckGo.Name = "btnDuckDuckGo";
            this.btnDuckDuckGo.Size = new System.Drawing.Size(96, 22);
            this.btnDuckDuckGo.Text = "DuckDuckGo";
            this.btnDuckDuckGo.ToolTipText = "DuckDuckGo Search Engine";
            this.btnDuckDuckGo.Click += new System.EventHandler(this.btnDuckDuckGo_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusProgress,
            this.lblBandwidth,
            this.lblStatus,
            this.lblError});
            this.statusStrip1.Location = new System.Drawing.Point(0, 650);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1131, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusProgress
            // 
            this.statusProgress.Name = "statusProgress";
            this.statusProgress.Size = new System.Drawing.Size(50, 16);
            // 
            // lblBandwidth
            // 
            this.lblBandwidth.AutoSize = false;
            this.lblBandwidth.Name = "lblBandwidth";
            this.lblBandwidth.Size = new System.Drawing.Size(200, 17);
            this.lblBandwidth.Text = "-";
            this.lblBandwidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = false;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(200, 17);
            this.lblStatus.Text = "-";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblError
            // 
            this.lblError.AutoSize = false;
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(200, 17);
            this.lblError.Text = "-";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl.Location = new System.Drawing.Point(0, 25);
            this.tabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(267, 625);
            this.tabControl.TabIndex = 3;
            this.tabControl.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.showClosedCheckBox);
            this.tabPage1.Controls.Add(this.connectionTree);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.circuitTree);
            this.tabPage1.Controls.Add(this.streamsTree);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.newCircuitButton);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(10);
            this.tabPage1.Size = new System.Drawing.Size(259, 599);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Status";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // showClosedCheckBox
            // 
            this.showClosedCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.showClosedCheckBox.AutoSize = true;
            this.showClosedCheckBox.Location = new System.Drawing.Point(13, 572);
            this.showClosedCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.showClosedCheckBox.Name = "showClosedCheckBox";
            this.showClosedCheckBox.Size = new System.Drawing.Size(117, 17);
            this.showClosedCheckBox.TabIndex = 13;
            this.showClosedCheckBox.Text = "Show closed/failed";
            this.showClosedCheckBox.UseVisualStyleBackColor = true;
            this.showClosedCheckBox.CheckedChanged += new System.EventHandler(this.showClosedCheckBox_CheckedChanged);
            // 
            // connectionTree
            // 
            this.connectionTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionTree.FullRowSelect = true;
            this.connectionTree.ItemHeight = 20;
            this.connectionTree.Location = new System.Drawing.Point(13, 220);
            this.connectionTree.Margin = new System.Windows.Forms.Padding(0, 0, 10, 13);
            this.connectionTree.Name = "connectionTree";
            this.connectionTree.Size = new System.Drawing.Size(239, 150);
            this.connectionTree.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 202);
            this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 10, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Current Connections";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 10, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Circuits";
            // 
            // circuitTree
            // 
            this.circuitTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.circuitTree.FullRowSelect = true;
            this.circuitTree.ItemHeight = 20;
            this.circuitTree.Location = new System.Drawing.Point(10, 39);
            this.circuitTree.Margin = new System.Windows.Forms.Padding(0, 0, 10, 13);
            this.circuitTree.Name = "circuitTree";
            this.circuitTree.Size = new System.Drawing.Size(239, 150);
            this.circuitTree.TabIndex = 1;
            // 
            // streamsTree
            // 
            this.streamsTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.streamsTree.FullRowSelect = true;
            this.streamsTree.ItemHeight = 20;
            this.streamsTree.Location = new System.Drawing.Point(13, 400);
            this.streamsTree.Margin = new System.Windows.Forms.Padding(0, 0, 10, 13);
            this.streamsTree.Name = "streamsTree";
            this.streamsTree.Size = new System.Drawing.Size(239, 160);
            this.streamsTree.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 382);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 10, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Current Streams";
            // 
            // newCircuitButton
            // 
            this.newCircuitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newCircuitButton.Image = global::TorBrowser.Properties.Resources.plus_16;
            this.newCircuitButton.Location = new System.Drawing.Point(225, 10);
            this.newCircuitButton.Margin = new System.Windows.Forms.Padding(0, 0, 7, 6);
            this.newCircuitButton.Name = "newCircuitButton";
            this.newCircuitButton.Size = new System.Drawing.Size(23, 23);
            this.newCircuitButton.TabIndex = 3;
            this.newCircuitButton.UseVisualStyleBackColor = true;
            this.newCircuitButton.Click += new System.EventHandler(this.newCircuitButton_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.configGrid);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(10);
            this.tabPage2.Size = new System.Drawing.Size(259, 599);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Configuration";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // configGrid
            // 
            this.configGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.configGrid.Location = new System.Drawing.Point(10, 10);
            this.configGrid.Margin = new System.Windows.Forms.Padding(0);
            this.configGrid.Name = "configGrid";
            this.configGrid.Size = new System.Drawing.Size(209, 620);
            this.configGrid.TabIndex = 12;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.routerList);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(10);
            this.tabPage3.Size = new System.Drawing.Size(259, 599);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Routers";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // routerList
            // 
            this.routerList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.routerList.FormattingEnabled = true;
            this.routerList.Location = new System.Drawing.Point(13, 13);
            this.routerList.Name = "routerList";
            this.routerList.Size = new System.Drawing.Size(203, 615);
            this.routerList.TabIndex = 12;
            // 
            // tabPages
            // 
            this.tabPages.ContextMenuStrip = this.menuStripTab;
            this.tabPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPages.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tabPages.Items.AddRange(new FarsiLibrary.Win.FATabStripItem[] {
            this.tabStrip1,
            this.tabStripAdd});
            this.tabPages.Location = new System.Drawing.Point(267, 25);
            this.tabPages.Name = "tabPages";
            this.tabPages.SelectedItem = this.tabStrip1;
            this.tabPages.Size = new System.Drawing.Size(864, 625);
            this.tabPages.TabIndex = 4;
            this.tabPages.Text = "faTabStrip1";
            this.tabPages.TabStripItemSelectionChanged += new FarsiLibrary.Win.TabStripItemChangedHandler(this.tabPages_TabStripItemSelectionChanged);
            // 
            // menuStripTab
            // 
            this.menuStripTab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCloseTab,
            this.menuCloseOtherTabs});
            this.menuStripTab.Name = "menuStripTab";
            this.menuStripTab.Size = new System.Drawing.Size(170, 48);
            // 
            // menuCloseTab
            // 
            this.menuCloseTab.Name = "menuCloseTab";
            this.menuCloseTab.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.menuCloseTab.Size = new System.Drawing.Size(169, 22);
            this.menuCloseTab.Text = "Close tab";
            this.menuCloseTab.Click += new System.EventHandler(this.menuCloseTab_Click);
            // 
            // menuCloseOtherTabs
            // 
            this.menuCloseOtherTabs.Name = "menuCloseOtherTabs";
            this.menuCloseOtherTabs.Size = new System.Drawing.Size(169, 22);
            this.menuCloseOtherTabs.Text = "Close other tabs";
            this.menuCloseOtherTabs.Click += new System.EventHandler(this.menuCloseOtherTabs_Click);
            // 
            // tabStrip1
            // 
            this.tabStrip1.IsDrawn = true;
            this.tabStrip1.Name = "tabStrip1";
            this.tabStrip1.Selected = true;
            this.tabStrip1.Size = new System.Drawing.Size(862, 604);
            this.tabStrip1.TabIndex = 0;
            this.tabStrip1.Title = "Tab1";
            // 
            // tabStripAdd
            // 
            this.tabStripAdd.IsDrawn = true;
            this.tabStripAdd.Name = "tabStripAdd";
            this.tabStripAdd.Size = new System.Drawing.Size(862, 604);
            this.tabStripAdd.TabIndex = 1;
            this.tabStripAdd.Title = "+";
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1131, 672);
            this.Controls.Add(this.tabPages);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tor Browser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabPages)).EndInit();
            this.tabPages.ResumeLayout(false);
            this.menuStripTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripTextBox txtUrl;
        private System.Windows.Forms.ToolStripButton btnGo;
        private System.Windows.Forms.ToolStripButton btnBack;
        private System.Windows.Forms.ToolStripButton btnForward;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblError;
        private System.Windows.Forms.ToolStripProgressBar statusProgress;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox showClosedCheckBox;
        private System.Windows.Forms.TreeView connectionTree;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView circuitTree;
        private System.Windows.Forms.TreeView streamsTree;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button newCircuitButton;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PropertyGrid configGrid;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox routerList;
        private System.Windows.Forms.ToolStripStatusLabel lblBandwidth;
        private System.Windows.Forms.ToolStripButton btnDuckDuckGo;
        private FarsiLibrary.Win.FATabStrip tabPages;
        private FarsiLibrary.Win.FATabStripItem tabStrip1;
        private FarsiLibrary.Win.FATabStripItem tabStripAdd;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripButton btnLoading;
        private System.Windows.Forms.ToolStripDropDownButton btnOther;
        private System.Windows.Forms.ToolStripMenuItem menuCheckTor;
        private System.Windows.Forms.ToolStripMenuItem menuDevTools;
        private System.Windows.Forms.ToolStripButton btnShowTor;
        private System.Windows.Forms.ToolStripButton btnDownloads;
        private System.Windows.Forms.ContextMenuStrip menuStripTab;
        private System.Windows.Forms.ToolStripMenuItem menuCloseTab;
        private System.Windows.Forms.ToolStripMenuItem menuCloseOtherTabs;
    }
}

