using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CefSharp;
using CefSharp.WinForms;
using FarsiLibrary.Win;
using ServiceStack.Text;
using Tor;
using Tor.Config;

namespace TorBrowser {
    public partial class MainForm:Form {
        private const int PROGRESS_DISABLED = -1;
        private const int PROGRESS_INDETERMINATE = -2;
        private RouterCollection allRouters;
        private Client client;
        private CircuitCollection circuits;
        private volatile bool closing;
        private ORConnectionCollection connections;
        private StreamCollection streams;
        private DownloadHandler dHandler;
        private MenuHandler mHandler;
        private LifeSpanHandler lHandler;
        private KeyboardHandler kHandler;
        private FATabStripItem newStrip;
        private FATabStripItem downloadsStrip;
        private string appPath = Path.GetDirectoryName(Application.ExecutablePath) + @"\";
        private Dictionary<int,DownloadItem> downloads;
        private Dictionary<int,string> downloadNames;
        private List<int> downloadCancelRequests;
        private Host host;


        private string startURL = "about:blank";
        private string torProjectURL = "https://check.torproject.org";
        private string downloadsURL = "chrome://storage/downloads.htm";

        public class Host {
            MainForm myForm;

            public Host(MainForm form) {
                myForm = form;
            }
            public void addNewBrowserTab(string url,bool focusNewTab = true) {
                myForm.AddNewBrowserTab(url,focusNewTab);
            }
            public string getDownloads() {
                lock(myForm.downloads) {
                    string x = JsonSerializer.SerializeToString(myForm.downloads);
                    return x;
                }
            }

            public bool cancelDownload(int downloadId) {
                lock(myForm.downloadCancelRequests) {
                    if(!myForm.downloadCancelRequests.Contains(downloadId)) {
                        myForm.downloadCancelRequests.Add(downloadId);
                    }
                }
                return true;
            }
        }

        public MainForm() {
            InitializeComponent();

            CefSettings settings = new CefSettings();

            //Set proxy for Tor
            //settings.CefCommandLineArgs.Add("proxy-server", "127.0.0.1:8182");

            //Load pepper flash player 
            //settings.CefCommandLineArgs.Add("ppapi-flash-path", appPath + @"PepperFlash\pepflashplayer.dll");

            settings.RegisterScheme(new CefCustomScheme {
                SchemeName = SchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new SchemeHandlerFactory()
            });

            settings.RegisterScheme(new CefCustomScheme {
                SchemeName = SchemeHandlerFactory.SchemeNameTest,
                SchemeHandlerFactory = new SchemeHandlerFactory()
            });

            Cef.Initialize(settings);

            dHandler = new DownloadHandler(this);
            lHandler = new LifeSpanHandler(this);
            mHandler = new MenuHandler(this);
            kHandler = new KeyboardHandler(this);
            downloads = new Dictionary<int,DownloadItem>();
            downloadNames = new Dictionary<int,string>();
            downloadCancelRequests = new List<int>();
            host = new Host(this);

            txtUrl.Text = startURL;
            AddNewBrowser(tabStrip1,startURL);

        }

        private ChromiumWebBrowser AddNewBrowser(FATabStripItem tabStrip,String url) {
            if(url == "") url = startURL;
            ChromiumWebBrowser browser = new ChromiumWebBrowser(url);
            browser.Dock = DockStyle.Fill;
            tabStrip.Controls.Add(browser);
            browser.BringToFront();
            browser.StatusMessage += Browser_StatusMessage;
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.TitleChanged += Browser_TitleChanged;
            browser.LoadError += Browser_LoadError;
            browser.AddressChanged += Browser_AddressChanged;
            browser.DownloadHandler = dHandler;
            browser.MenuHandler = mHandler;
            browser.LifeSpanHandler = lHandler;
            browser.KeyboardHandler = kHandler;
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
            if(url.StartsWith("chrome:")) {
                browser.RegisterAsyncJsObject("host",host,true);
            }
            return browser;
        }

        private void Browser_FrameLoadEnd(object sender,CefSharp.FrameLoadEndEventArgs e) {
             if(e.Url.ToLower().StartsWith("https://www.nseindia.com/products/content")) {
                string script = "$('#dataType').val(\"priceVolume\");";
                script += "$('#series').val(\"EQ\");";
                script += "$('#rdPeriod')[0].checked=true;";
                script += "$('#dateRange').val(\"week\");";
                script += "$('#symbol').val(\"RAIN\");";
                script += "$('.getdata-button').click();";
                script += "setTimeout(function(){";
                script += " try { ";
                script += " var fileName = $('#csvFileName').val(); var csv=$('#csvContentDiv').html();";
                script += " var csvFile;";
                script += " var downloadLink;";
                script += " csvFile = new Blob([csv], {type: \"text/csv\"});";
                script += " downloadLink = document.createElement(\"a\");";
                script += " downloadLink.download = fileName;";
                script += " downloadLink.href = window.URL.createObjectURL(csvFile);";
                script += " downloadLink.style.display = \"none\";";
                script += " document.body.appendChild(downloadLink);";
                script += " downloadLink.click();";
                script += " alert(fileName); } catch(e) { alert(e); } },5000);";
                Browser.ExecuteScriptAsync(script);
            }
        }

        public ChromiumWebBrowser Browser {
            get {
                if(tabPages.SelectedItem.Controls.Count > 0)
                    return (ChromiumWebBrowser)tabPages.SelectedItem.Controls[0];
                else
                    return null;
            }
        }

        private void Browser_AddressChanged(object sender,AddressChangedEventArgs e) {
            InvokeOnUiThreadIfRequired(() => {
                if(sender == Browser) txtUrl.Text = e.Address;
            });
        }

        private void Browser_LoadError(object sender,LoadErrorEventArgs e) {
            SetErrorText("Load Error:" + e.ErrorCode + ";" + e.ErrorText);
        }

        private void Browser_TitleChanged(object sender,TitleChangedEventArgs e) {
            InvokeOnUiThreadIfRequired(() => {
                FATabStripItem tabStrip = (FATabStripItem)((ChromiumWebBrowser)sender).Parent;
                tabStrip.Title = e.Title;
            });
        }

        private void Browser_LoadingStateChanged(object sender,LoadingStateChangedEventArgs e) {
            if(sender == Browser) {
                SetCanGoBack(e.CanGoBack);
                SetCanGoForward(e.CanGoForward);
                if(e.IsLoading) {
                    SetStatusProgress(PROGRESS_INDETERMINATE);
                } else {
                    SetStatusProgress(PROGRESS_DISABLED);
                }
                SetErrorText("");
            }
        }

        public void InvokeOnUiThreadIfRequired(Action action) {
            if(this.InvokeRequired) {
                this.BeginInvoke(action);
            } else {
                action.Invoke();
            }
        }

        private void SetStatusText(string txt) {
            InvokeOnUiThreadIfRequired(() => lblStatus.Text = txt);
        }

        private void SetErrorText(string txt) {
            InvokeOnUiThreadIfRequired(() => lblError.Text = txt);
        }

        private void Browser_StatusMessage(object sender,StatusMessageEventArgs e) {
            SetStatusText(e.Value);
        }

        public void WaitForBrowserToInitialize(ChromiumWebBrowser browser) {
            while(!browser.IsBrowserInitialized) {
                Thread.Sleep(100);
            }
        }

        private void btnBack_Click(object sender,EventArgs e) {
            Browser.Back();
        }

        private void btnForward_Click(object sender,EventArgs e) {
            Browser.Forward();
        }


        private void LoadUrl(string url) {
            Uri outUri;
            string newUrl = url;
            string urlToLower = url.Trim().ToLower();

            Uri.TryCreate(url,UriKind.Absolute,out outUri);

            if(!(urlToLower.StartsWith("http") || urlToLower.StartsWith(SchemeHandlerFactory.SchemeName) || urlToLower.StartsWith(SchemeHandlerFactory.SchemeNameTest))) {
                if(outUri == null || outUri.Scheme != Uri.UriSchemeFile) newUrl = "http://" + url;
            }

            //if (urlToLower.StartsWith(SchemeHandlerFactory.SchemeName + ":") || urlToLower.StartsWith(SchemeHandlerFactory.SchemeNameTest + ":") ||
            //    (Uri.TryCreate(newUrl, UriKind.Absolute, out outUri)
            //     && ((outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps) && newUrl.Contains(".") || outUri.Scheme == Uri.UriSchemeFile)))
            //{
            Browser.Load(newUrl);
            //}
            //else
            //{
            //    string searchURL = "https://www.google.com.tr/webhp?#q=";
            //    Browser.Load(searchURL + HttpUtility.UrlEncode(url));
            //}
        }

        private void btnRefresh_Click(object sender,EventArgs e) {
            Browser.Refresh();
        }

        private void btnGo_Click(object sender,EventArgs e) {
            LoadUrl(txtUrl.Text);
        }

        private void SetCanGoBack(bool canGoBack) {
            InvokeOnUiThreadIfRequired(() => btnBack.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward) {
            InvokeOnUiThreadIfRequired(() => btnForward.Enabled = canGoForward);
        }

        private void SetStatusProgress(int value) {
            if(closing)
                return;

            Invoke((Action)delegate {
                if(value == PROGRESS_DISABLED) {
                    statusProgress.Visible = false;
                    btnLoading.Image = global::TorBrowser.Properties.Resources.loader2;
                } else if(value == PROGRESS_INDETERMINATE) {
                    statusProgress.Visible = true;
                    statusProgress.Style = ProgressBarStyle.Marquee;
                    btnLoading.Image = global::TorBrowser.Properties.Resources.loader;
                } else {
                    statusProgress.Visible = true;
                    statusProgress.Value = Math.Min(100,Math.Max(0,value));
                }
            });
        }


        private void InitializeTor() {
            Process[] previous = Process.GetProcessesByName("tor");

            SetStatusProgress(PROGRESS_INDETERMINATE);

            if(previous != null && previous.Length > 0) {
                SetStatusText("Killing previous tor instances..");

                foreach(Process process in previous)
                    process.Kill();
            }

            SetStatusText("Creating the tor client..");

            ClientCreateParams createParameters = new ClientCreateParams();
            createParameters.ConfigurationFile = ConfigurationManager.AppSettings["torConfigurationFile"];
            createParameters.ControlPassword = ConfigurationManager.AppSettings["torControlPassword"];
            createParameters.ControlPort = Convert.ToInt32(ConfigurationManager.AppSettings["torControlPort"]);
            createParameters.DefaultConfigurationFile = ConfigurationManager.AppSettings["torDefaultConfigurationFile"];
            createParameters.Path = ConfigurationManager.AppSettings["torPath"];

            createParameters.SetConfig(ConfigurationNames.AvoidDiskWrites,true);
            createParameters.SetConfig(ConfigurationNames.GeoIPFile,Path.Combine(Environment.CurrentDirectory,@"Tor\Data\Tor\geoip"));
            createParameters.SetConfig(ConfigurationNames.GeoIPv6File,Path.Combine(Environment.CurrentDirectory,@"Tor\Data\Tor\geoip6"));

            client = Client.Create(createParameters);

            if(!client.IsRunning) {
                SetStatusProgress(PROGRESS_DISABLED);
                SetStatusText("The tor client could not be created");
                return;
            }

            client.Status.BandwidthChanged += OnClientBandwidthChanged;
            client.Status.CircuitsChanged += OnClientCircuitsChanged;
            client.Status.ORConnectionsChanged += OnClientConnectionsChanged;
            client.Status.StreamsChanged += OnClientStreamsChanged;
            client.Configuration.PropertyChanged += (s,e) => { Invoke((Action)delegate { configGrid.Refresh(); }); };
            client.Shutdown += new EventHandler(OnClientShutdown);

            SetStatusProgress(PROGRESS_DISABLED);
            SetStatusText("Ready");

            configGrid.SelectedObject = client.Configuration;

            SetStatusText("Downloading routers");
            SetStatusProgress(PROGRESS_INDETERMINATE);

            ThreadPool.QueueUserWorkItem(state => {
                allRouters = client.Status.GetAllRouters();

                if(allRouters == null) {
                    SetStatusText("Could not download routers");
                    SetStatusProgress(PROGRESS_DISABLED);
                } else {
                    Invoke((Action)delegate {
                        routerList.BeginUpdate();

                        foreach(Router router in allRouters)
                            routerList.Items.Add(string.Format("{0} [{1}] ({2}/s)",router.Nickname,router.IPAddress,router.Bandwidth));

                        routerList.EndUpdate();
                    });

                    SetStatusText("Ready");
                    SetStatusProgress(PROGRESS_DISABLED);
                    ShowTorReady();
                }
            });
        }

        private void ShowTorReady() {
            InvokeOnUiThreadIfRequired(() => {
                btnShowTor.Image = Properties.Resources.Onion_icon_16x16g;
                txtUrl.BackColor = System.Drawing.Color.FromArgb(237,255,196);
                //LoadUrl(myIpURL);
            });
        }

        private void MainForm_FormClosing(object sender,FormClosingEventArgs e) {
            if(client != null && client.IsRunning) {
                closing = true;
                client.Status.BandwidthChanged -= OnClientBandwidthChanged;
                client.Status.CircuitsChanged -= OnClientCircuitsChanged;
                client.Dispose();

                e.Cancel = true;
            }

        }

        private void OnClientBandwidthChanged(object sender,BandwidthEventArgs e) {
            if(closing)
                return;

            Invoke((Action)delegate {
                if(e.Downloaded.Value == 0 && e.Uploaded.Value == 0)
                    lblBandwidth.Text = "";
                else
                    lblBandwidth.Text = string.Format("Down: {0}/s, Up: {1}/s",e.Downloaded,e.Uploaded);
            });
        }

        private void OnClientCircuitsChanged(object sender,EventArgs e) {
            if(closing)
                return;

            circuits = client.Status.Circuits;

            Invoke((Action)delegate {
                circuitTree.BeginUpdate();

                List<TreeNode> removals = new List<TreeNode>();

                foreach(TreeNode n in circuitTree.Nodes)
                    removals.Add(n);

                foreach(Circuit circuit in circuits) {
                    bool added = false;
                    TreeNode node = null;

                    if(!showClosedCheckBox.Checked)
                        if(circuit.Status == CircuitStatus.Closed || circuit.Status == CircuitStatus.Failed)
                            continue;

                    foreach(TreeNode existingNode in circuitTree.Nodes)
                        if(((Circuit)existingNode.Tag).ID == circuit.ID) {
                            node = existingNode;
                            break;
                        }

                    string text = string.Format("Circuit #{0} [{1}] ({2})",circuit.ID,circuit.Status,circuit.Routers.Count);
                    string tooltip = string.Format("Created: {0}\nBuild Flags: {1}",circuit.TimeCreated,circuit.BuildFlags);

                    if(node == null) {
                        node = new TreeNode(text);
                        //AUZ node.ContextMenuStrip = circuitMenuStrip;
                        node.Tag = circuit;
                        node.ToolTipText = tooltip;
                        added = true;
                    } else {
                        node.Text = text;
                        node.ToolTipText = tooltip;
                        node.Nodes.Clear();

                        removals.Remove(node);
                    }

                    foreach(Router router in circuit.Routers)
                        node.Nodes.Add(string.Format("{0} [{1}] ({2}/s)",router.Nickname,router.IPAddress,router.Bandwidth));

                    if(added)
                        circuitTree.Nodes.Add(node);
                }

                foreach(TreeNode remove in removals)
                    circuitTree.Nodes.Remove(remove);

                circuitTree.EndUpdate();
            });
        }

        private void OnClientConnectionsChanged(object sender,EventArgs e) {
            if(closing)
                return;

            connections = client.Status.ORConnections;

            Invoke((Action)delegate {
                connectionTree.BeginUpdate();

                List<TreeNode> removals = new List<TreeNode>();

                foreach(TreeNode n in connectionTree.Nodes)
                    removals.Add(n);

                foreach(ORConnection connection in connections) {
                    bool added = false;
                    TreeNode node = null;

                    if(!showClosedCheckBox.Checked)
                        if(connection.Status == ORStatus.Closed || connection.Status == ORStatus.Failed)
                            continue;

                    foreach(TreeNode existingNode in connectionTree.Nodes) {
                        ORConnection existing = (ORConnection)existingNode.Tag;

                        if(connection.ID != 0 && connection.ID == existing.ID) {
                            node = existingNode;
                            break;
                        }
                        if(connection.Target.Equals(existing.Target,StringComparison.CurrentCultureIgnoreCase)) {
                            node = existingNode;
                            break;
                        }
                    }

                    string text = string.Format("Connection #{0} [{1}] ({2})",connection.ID,connection.Status,connection.Target);

                    if(node == null) {
                        node = new TreeNode(text);
                        node.Tag = connection;
                        added = true;
                    } else {
                        node.Text = text;
                        node.Nodes.Clear();

                        removals.Remove(node);
                    }

                    if(added)
                        connectionTree.Nodes.Add(node);
                }

                foreach(TreeNode remove in removals)
                    connectionTree.Nodes.Remove(remove);

                connectionTree.EndUpdate();
            });
        }

        private void OnClientShutdown(object sender,EventArgs e) {
            if(!closing) {
                MessageBox.Show("The tor client has been terminated without warning","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            client = null;

            Invoke((Action)delegate { Close(); });
        }

        private void OnClientStreamsChanged(object sender,EventArgs e) {
            if(closing)
                return;

            streams = client.Status.Streams;

            Invoke((Action)delegate {
                streamsTree.BeginUpdate();

                List<TreeNode> removals = new List<TreeNode>();

                foreach(TreeNode n in streamsTree.Nodes)
                    removals.Add(n);

                foreach(Tor.Stream stream in streams) {
                    bool added = false;
                    TreeNode node = null;

                    if(!showClosedCheckBox.Checked)
                        if(stream.Status == StreamStatus.Failed || stream.Status == StreamStatus.Closed)
                            continue;

                    foreach(TreeNode existingNode in streamsTree.Nodes)
                        if(((Tor.Stream)existingNode.Tag).ID == stream.ID) {
                            node = existingNode;
                            break;
                        }

                    Circuit circuit = null;

                    if(stream.CircuitID > 0)
                        circuit = circuits.Where(c => c.ID == stream.CircuitID).FirstOrDefault();

                    string text = string.Format("Stream #{0} [{1}] ({2}, {3})",stream.ID,stream.Status,stream.Target,circuit == null ? "detached" : "circuit #" + circuit.ID);
                    string tooltip = string.Format("Purpose: {0}",stream.Purpose);

                    if(node == null) {
                        node = new TreeNode(text);
                        //AUZ node.ContextMenuStrip = streamMenuStrip;
                        node.Tag = stream;
                        node.ToolTipText = tooltip;
                        added = true;
                    } else {
                        node.Text = text;
                        node.ToolTipText = tooltip;
                        node.Nodes.Clear();

                        removals.Remove(node);
                    }

                    if(added)
                        streamsTree.Nodes.Add(node);
                }

                foreach(TreeNode remove in removals)
                    streamsTree.Nodes.Remove(remove);

                streamsTree.EndUpdate();
            });
        }

        private void MainForm_Load(object sender,EventArgs e) {
            InitializeTor();
        }

        private void newCircuitButton_Click(object sender,EventArgs e) {
            if(client.IsRunning)
                client.Controller.CreateCircuit();
        }

        private void showClosedCheckBox_CheckedChanged(object sender,EventArgs e) {
            OnClientCircuitsChanged(client,EventArgs.Empty);
            OnClientConnectionsChanged(client,EventArgs.Empty);
            OnClientStreamsChanged(client,EventArgs.Empty);
        }


        private void txtUrl_KeyDown(object sender,KeyEventArgs e) {
            if(e.KeyCode == Keys.Enter) {
                LoadUrl(txtUrl.Text);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtUrl_Enter(object sender,EventArgs e) {
            BeginInvoke((Action)delegate {
                txtUrl.SelectAll();
            });
        }


        private void btnDuckDuckGo_Click(object sender,EventArgs e) {
            LoadUrl("http://3g2upl4pq6kufc4m.onion/");
        }

        public ChromiumWebBrowser AddNewBrowserTab(String url,bool focusNewTab = true) {
            return (ChromiumWebBrowser)this.Invoke((Func<ChromiumWebBrowser>)delegate {
                FATabStripItem tabStrip = new FATabStripItem();
                tabStrip.Title = "about:blank";
                tabPages.Items.Insert(tabPages.Items.Count - 1,tabStrip);
                newStrip = tabStrip;
                ChromiumWebBrowser browser = AddNewBrowser(newStrip,url);
                if(focusNewTab) timer1.Enabled = true;
                return browser;
            });
        }

        private void tabPages_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e) {
            if(e.ChangeType == FATabStripItemChangeTypes.SelectionChanged) {
                if(tabPages.SelectedItem == tabStripAdd) {
                    AddNewBrowserTab("");
                } else {
                    txtUrl.Text = Browser.Address;
                    if(Browser.IsLoading) SetStatusProgress(PROGRESS_INDETERMINATE);
                    else SetStatusProgress(PROGRESS_DISABLED);
                    SetCanGoBack(Browser.CanGoBack);
                    SetCanGoForward(Browser.CanGoForward);
                }
            }
            if(e.ChangeType == FATabStripItemChangeTypes.Removed) {
                if(e.Item == downloadsStrip) downloadsStrip = null;
                if(e.Item.Controls.Count > 0) {
                    ((ChromiumWebBrowser)e.Item.Controls[0]).Dispose();
                }
            }
            if(e.ChangeType == FATabStripItemChangeTypes.Changed) {
                if(e.Item.Controls.Count > 0) {
                    ((ChromiumWebBrowser)e.Item.Controls[0]).Focus();
                }
            }
        }

        private void timer1_Tick(object sender,EventArgs e) {
            tabPages.SelectedItem = newStrip;
            timer1.Enabled = false;
        }

        public void CloseActiveTab() {
            FATabStripItem activeStrip = tabPages.SelectedItem;
            if(activeStrip.Controls.Count > 0) {
                if(activeStrip.Controls[0] is ChromiumWebBrowser) {
                    InvokeOnUiThreadIfRequired(() => {
                        tabPages.RemoveTab(activeStrip);
                    });
                }
            }
        }

        private void menuCheckTor_Click(object sender,EventArgs e) {
            LoadUrl(torProjectURL);
        }

        private void menuDevTools_Click(object sender,EventArgs e) {
            Browser.ShowDevTools();
        }

        private void btnShowTor_Click(object sender,EventArgs e) {
            tabControl.Visible = btnShowTor.Checked;
        }

        public void UpdateDownloadItem(DownloadItem item) {
            lock(downloads) {
                //SuggestedFileName comes full only in the first attempt so keep it somewhere
                if(item.SuggestedFileName != "") downloadNames[item.Id] = item.SuggestedFileName;

                //Set it back if it is empty
                if(item.SuggestedFileName == "" && downloadNames.ContainsKey(item.Id)) item.SuggestedFileName = downloadNames[item.Id];

                downloads[item.Id] = item;
            }
        }

        private void btnDownloads_Click(object sender,EventArgs e) {
            if(downloadsStrip != null && ((ChromiumWebBrowser)downloadsStrip.Controls[0]).Address == downloadsURL) {
                tabPages.SelectedItem = downloadsStrip;
            } else {
                ChromiumWebBrowser brw = AddNewBrowserTab(downloadsURL);
                downloadsStrip = (FATabStripItem)brw.Parent;
            }
        }

        private void menuCloseTab_Click(object sender,EventArgs e) {
            CloseActiveTab();
        }

        private void menuCloseOtherTabs_Click(object sender,EventArgs e) {
            List<FATabStripItem> listToClose = new List<FATabStripItem>();
            foreach(FATabStripItem tab in tabPages.Items) {
                if(tab != tabStripAdd && tab != tabPages.SelectedItem) listToClose.Add(tab);
            }
            foreach(FATabStripItem tab in listToClose) {
                tabPages.RemoveTab(tab);
            }

        }

        public Dictionary<int,DownloadItem> Downloads {
            get {
                return downloads;
            }
        }

        public List<int> CancelRequests {
            get {
                return downloadCancelRequests;
            }
        }

    }
}
