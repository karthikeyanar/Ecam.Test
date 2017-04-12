using Ecam.Framework;
using Ecam.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadHTMLContent
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<tra_company> _companies;
        private tra_company _lastCompany = null;
        private int _index = -1;
        private List<int> _Years = new List<int> { 2017, 2016, 2015 };
        private int _YearIndex = -1;
        private int _MonthIndex = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            using (EcamContext context = new EcamContext())
            {
                List<string> filterSymbols = new List<string> { "ICICIPRULI", "WIPRO", "STCINDIA" };
                _companies = (from q in context.tra_company
                              where filterSymbols.Contains(q.symbol) == true
                              select q).ToList();
            }
            //DownloadHTMLCompanies(); 
            DownloadHTMLCompanies_YearWise();
        }

        private void ProcessHTMLFiles()
        {
            string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];
            string backUpDirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadBackUpHTMLPath"];
            if (System.IO.Directory.Exists(dirPath) == false)
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            if (System.IO.Directory.Exists(backUpDirPath) == false)
            {
                System.IO.Directory.CreateDirectory(backUpDirPath);
            }
            string[] directories = System.IO.Directory.GetDirectories(dirPath);
            foreach (string dirctoryPath in directories)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirctoryPath);
                string[] files = System.IO.Directory.GetFiles(dirctoryPath);
                foreach (string fullFileName in files)
                {
                    string backUpDirectory = backUpDirPath + "\\" + dirInfo.Name;
                    if (System.IO.Directory.Exists(backUpDirectory) == false)
                    {
                        System.IO.Directory.CreateDirectory(backUpDirectory);
                    }
                    string fileName = System.IO.Path.GetFileName(fullFileName);
                    string backupFileName = backUpDirectory + "\\" + fileName;
                    string symbol = fileName.Replace(".html", "");
                    string html = System.IO.File.ReadAllText(fullFileName);
                    System.IO.File.Copy(fullFileName, backupFileName, true);
                    lblError.Text = "Prcess html symbol=" + symbol;
                    NSEIndiaImport(symbol, html);
                    System.IO.File.Delete(fullFileName);
                }
            }
        }

        //private void DownloadHTMLCompanies()
        //{
        //    _index += 1;
        //    if (_companies != null)
        //    {
        //        try
        //        {
        //            var company = _companies[_index];
        //            _lastCompany = company;

        //            string datePeriod = System.Configuration.ConfigurationManager.AppSettings["date_period"];
        //            string url = string.Format("https://www.nseindia.com/live_market/dynaContent/live_watch/get_quote/getHistoricalData.jsp?symbol={0}&series=EQ&fromDate=undefined&toDate=undefined&datePeriod={1}"
        //                                                                                              , company.symbol.Replace("&", "%26")
        //                                                                                              , datePeriod);
        //            webBrowser1.Navigate(url);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Ex=" + ex.Message);
        //        }
        //    }
        //}

        private void DownloadHTMLCompanies_YearWise()
        {
            _YearIndex = -1;
            _MonthIndex = 0;
            _index += 1;
            if (_companies != null)
            {
                if (_index < _companies.Count)
                {
                    var company = _companies[_index];
                    _lastCompany = company;
                    lblCompanyStatus.Text = "Total Companies " + (_index + 1) + " Of " + _companies.Count();
                    Helper.Log(lblCompanyStatus.Text);
                    lblSymbol.Text = _lastCompany.symbol;
                    lblCompany.Text = _lastCompany.company_name;
                    DownloadHTMLCompanies_NexYear();
                }
                else
                {
                    Helper.Log("Completed");
                    MessageBox.Show("Completed");
                }
            }
        }

        private void DownloadHTMLCompanies_NexYear()
        {
            _YearIndex += 1;
            if (_YearIndex < _Years.Count())
            {
                int year = _Years[_YearIndex];
                DownloadHTMLCompanies_MonthWise();
            }
            else
            {
                lblError.Text = "All years completed symbol=" + _lastCompany.symbol;
                DownloadHTMLCompanies_YearWise();
            }
        }

        private DateTime _STARTDATE;
        private DateTime _ENDDATE;
        private void DownloadHTMLCompanies_MonthWise()
        {

            _MonthIndex += 1;
            if (_YearIndex <= _Years.Count())
            {
                int year = _Years[_YearIndex];
                if (_MonthIndex <= 12)
                {
                    DateTime dt = Convert.ToDateTime(_MonthIndex + "/01/" + year);
                    DateTime startDate = DataTypeHelper.GetFirstDayOfMonth(dt);
                    DateTime endDate = DataTypeHelper.GetLastDayOfMonth(dt);
                    _STARTDATE = startDate;
                    _ENDDATE = endDate;
                    lblStartDate.Text = startDate.ToString("dd/MMM/yyyy");
                    lblEndDate.Text = endDate.ToString("dd/MMM/yyyy");
                    //Helper.Log(lblStartDate.Text + " TO " + lblEndDate.Text);
                    DownloadHTMLCompanies_MonthWise(startDate, endDate);
                }
                else
                {
                    _MonthIndex = 0;
                    DownloadHTMLCompanies_NexYear();
                }
            }

        }


        private string _lastURL = string.Empty;
        private void DownloadHTMLCompanies_MonthWise(DateTime startDate, DateTime endDate)
        {
            if (_lastCompany != null)
            {
                try
                {
                    string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];
                    dirPath += "\\" + _STARTDATE.ToString("dd_MMM_yyyy");
                    if (System.IO.Directory.Exists(dirPath) == false)
                    {
                        System.IO.Directory.CreateDirectory(dirPath);
                    }
                    string fullFileName = dirPath + "\\" + _lastCompany.symbol + ".html";
                    //Helper.Log(fullFileName);
                    if (System.IO.File.Exists(fullFileName) == true)
                    {
                        string html = System.IO.File.ReadAllText(fullFileName);
                        if(html.Contains("Make sure the web address") == true)
                        {
                            Helper.Log(fullFileName, "MakeSure");
                            System.IO.File.Delete(fullFileName);
                        }
                        if (html.Contains("Service Unavailable") == true)
                        {
                            Helper.Log(fullFileName, "ServiceUnavailable");
                            System.IO.File.Delete(fullFileName);
                        }
                        if (html.Contains("What you can try") == true)
                        {
                            Helper.Log(fullFileName, "Whatyoucantry");
                            System.IO.File.Delete(fullFileName);
                        }
                    }
                    if (System.IO.File.Exists(fullFileName) == false)
                    {
                        string url = string.Format("https://www.nseindia.com/products/dynaContent/common/productsSymbolMapping.jsp?symbol={0}&segmentLink=3&symbolCount=1&series=ALL&dateRange=+&fromDate={1}&toDate={2}&dataType=PRICEVOLUME"
                                                                                                          , _lastCompany.symbol.Replace("&", "%26")
                                                                                                          , startDate.ToString("dd-MM-yyyy")
                                                                                                          , endDate.ToString("dd-MM-yyyy")
                                                                                                          );

                        _lastURL = url;
                        Helper.Log(url, "NAVIGATE");
                        webBrowser1.Navigate(url);
                    }
                    else
                    {
                        DownloadHTMLCompanies_MonthWise();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ex=" + ex.Message);
                }
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string html = webBrowser1.DocumentText;
            string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];
            dirPath += "\\" + _STARTDATE.ToString("dd_MMM_yyyy");
            if (System.IO.Directory.Exists(dirPath) == false)
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            bool isIgnore = false;
            if (html.Contains("Make sure the web address") == true)
            {
                isIgnore = true;
            }
            if (html.Contains("Service Unavailable") == true)
            {
                isIgnore = true;
            }
            if (html.Contains("What you can try") == true)
            {
                isIgnore = true;
            }
            if (isIgnore == false)
            {
                string fullFileName = dirPath + "\\" + _lastCompany.symbol + ".html";
                if (System.IO.File.Exists(fullFileName) == true)
                {
                    System.IO.File.Delete(fullFileName);
                }
                System.IO.File.WriteAllText(fullFileName, html);
                Helper.Log("New Symbol=" + _lastCompany.symbol + ",fullFileName=" + fullFileName, "NEWCOMPANY");
                //string html = webBrowser1.DocumentText;
                //NSEIndiaImport(_lastCompany.symbol, html);
            }
            DownloadHTMLCompanies_MonthWise();
        }

        private string ReplaceTagAttributes(string html, string tagName)
        {
            Regex regex = new Regex(
   @"<" + tagName + "(.*?)>",
   RegexOptions.IgnoreCase
   | RegexOptions.Multiline
   | RegexOptions.IgnorePatternWhitespace
   | RegexOptions.Compiled
   );
            html = regex.Replace(html, "<" + tagName + ">");
            return html;
        }

        private string ReplaceAttributes(string html, string attrName, string replaceContent)
        {
            string exp = attrName + "\\s*=\\s*\"(.*?)\"";
            Regex regex = new Regex(
    exp,
   RegexOptions.IgnoreCase
   | RegexOptions.Multiline
   | RegexOptions.IgnorePatternWhitespace
   | RegexOptions.Compiled
   );
            html = regex.Replace(html, replaceContent);
            return html;
        }

        private void NSEIndiaImport(string symbol, string html)
        {
            //try
            //{
            html = ReplaceTagAttributes(html, "table");
            html = ReplaceTagAttributes(html, "tbody");
            html = ReplaceTagAttributes(html, "tr");
            html = ReplaceTagAttributes(html, "td");
            html = ReplaceTagAttributes(html, "th");
            html = ReplaceTagAttributes(html, "img");
            html = ReplaceTagAttributes(html, "div");
            html = ReplaceAttributes(html, "style", "");
            html = ReplaceAttributes(html, "class", "");
            html = ReplaceAttributes(html, "nowrap", "");
            html = ReplaceAttributes(html, "title", "");
            html = html.Replace("<TH nowrap=\"\">", "<th>");
            html = html.Replace("<TD class=\"normalText\" nowrap=\"\">", "<td>");
            html = html.Replace("<TD class=\"date\" nowrap=\"\">", "<td>");
            html = html.Replace("<TD class=\"number\" nowrap=\"\">", "<td>");
            html = html.Replace("\n", "").Replace("\r", "").Replace("\r\n", "").Replace("TABLE", "table").Replace("TR", "tr").Replace("TD", "td").Replace("TH", "th").Replace("TBODY", "tbody");
            int startIndex = html.IndexOf("<tbody>");
            int endIndex = html.IndexOf("</tbody>");
            int length = endIndex - startIndex + 8;
            string tblHTML = html.Substring(startIndex, length);
            tblHTML = tblHTML.Replace(" ", "");
            List<TempClass> tempList = new List<TempClass>();
            Regex regex = new Regex(
 @"<tr>(.*?)</tr>",
 RegexOptions.IgnoreCase
 | RegexOptions.Multiline
 | RegexOptions.IgnorePatternWhitespace
 | RegexOptions.Compiled
 );

            MatchCollection trCollections = regex.Matches(tblHTML);
            int i = 0;
            foreach (Match trMatch in trCollections)
            {
                i += 1;
                string tr = trMatch.Value;
                string tagName = "td";
                if (i == 1)
                {
                    tagName = "th";
                }
                regex = new Regex(
                            @"<" + tagName + ">(.+?)</" + tagName + ">",
                            RegexOptions.IgnoreCase
                            | RegexOptions.Multiline
                            | RegexOptions.IgnorePatternWhitespace
                            | RegexOptions.Compiled
                            );
                MatchCollection rowMatches = regex.Matches(tr);
                string date = "";
                string originalSymbol = "";
                string series = "";
                string prev = "";
                string open = "";
                string high = "";
                string low = "";
                string lastTrade = "";
                string close = "";
                string tradeType = "NSE";
                int colIndex = -1;
                foreach (Match colMatch in rowMatches)
                {
                    colIndex += 1;
                    if (i > 1)
                    {
                        string value = string.Empty;
                        if (colMatch.Groups.Count >= 2)
                        {
                            value = colMatch.Groups[1].Value;
                        }
                        if (string.IsNullOrEmpty(value) == false)
                        {
                            value = value.Trim();
                        }
                        switch (colIndex)
                        {
                            case 0: originalSymbol = value; break;
                            case 1: series = value; break;
                            case 2: date = value; break;
                            case 3: prev = value; break;
                            case 4: open = value; break;
                            case 5: high = value; break;
                            case 6: low = value; break;
                            case 7: lastTrade = value; break;
                            case 8: close = value; break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(date) == false
                    && string.IsNullOrEmpty(originalSymbol) == false
                    )
                {
                    DateTime dt = DataTypeHelper.ToDateTime(date);
                    if (dt.Year < 2015)
                    {
                        continue;
                    }
                    tempList.Add(new TempClass
                    {
                        symbol = symbol,
                        trade_type = tradeType,
                        trade_date = dt,
                        close_price = DataTypeHelper.ToDecimal(close),
                        high_price = DataTypeHelper.ToDecimal(high),
                        low_price = DataTypeHelper.ToDecimal(low),
                        open_price = DataTypeHelper.ToDecimal(open),
                        ltp_price = DataTypeHelper.ToDecimal(lastTrade),
                    });
                }
            }
            if (tempList.Count() <= 0)
            {
                lblError.Text = "NSEIndiaImport Temp List No Records Found Symbol =" + symbol;
                Helper.Log(_lastURL, "NORECORDSURL");
                Helper.Log("NSEIndiaImport Temp List No Records Found Symbol =" + symbol, "ERROR");
            }
            int rowIndex = 0;
            foreach (var row in tempList)
            {
                rowIndex += 1;
                lblTotalRecords.Text = " Rows " + rowIndex + " Of " + tempList.Count();
                ImportPrice(row);
            }
            //}
            //catch (Exception ex)
            //{
            //    lblError.Text = "NSEIndiaImport symbol=" + symbol + ",ex=" + ex.Message;
            //    Helper.Log("NSEIndiaImport symbol=" + symbol + ",ex =" + ex.Message, "ERROR");
            //}
        }

        private void ImportPrice(TempClass import)
        {
            if (import.trade_date.Year < 2015)
            {
                return;
            }
            using (EcamContext context = new EcamContext())
            {
                var row = (from q in context.tra_market
                           where q.symbol == import.symbol
                           && q.trade_date == import.trade_date
                           && q.trade_type == import.trade_type
                           select q).FirstOrDefault();
                bool isNew = false;
                if (row == null)
                {
                    row = new tra_market();
                    isNew = true;
                }
                row.symbol = import.symbol;
                row.trade_type = import.trade_type;
                row.trade_date = import.trade_date;
                row.open_price = import.open_price;
                row.high_price = import.high_price;
                row.low_price = import.low_price;
                row.close_price = import.close_price;
                row.ltp_price = import.ltp_price;
                if (isNew == true)
                {
                    context.tra_market.Add(row);
                }
                else
                {
                    context.Entry(row).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
                lblError.Text = "ImportPrice Price Index symbol=" + row.symbol + ",Date=" + row.trade_date;
                Console.WriteLine("ImportPrice Price Index symbol=" + row.symbol + ",Date=" + row.trade_date);
            }
        }
    }

    public class TempClass
    {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public string trade_type { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> high_price { get; set; }
        public Nullable<decimal> low_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> ltp_price { get; set; }
    }
}
