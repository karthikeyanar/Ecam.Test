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
        private List<int> _Years = new List<int> { 2017, 2016 };
        private int _YearIndex = -1;
        private int _MonthIndex = 0;
        private static List<tra_market> _Markets = null;
        private bool _IsYearWise = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            string IsYearWise = System.Configuration.ConfigurationManager.AppSettings["IsYearWise"];
            if (IsYearWise == "true")
            {
                _IsYearWise = true;
            }
            using (EcamContext context = new EcamContext())
            {
                string symbolsAppSetting = System.Configuration.ConfigurationManager.AppSettings["symbols"];
                IQueryable<tra_company> query = context.tra_company;
                if (string.IsNullOrEmpty(symbolsAppSetting) == false)
                {
                    List<string> symbols = Helper.ConvertStringList(symbolsAppSetting);
                    query = (from q in query where symbols.Contains(q.symbol) == true select q);
                }
                _companies = (from q in query
                              select q).ToList();
            }
            if (_IsYearWise)
            {
                DownloadHTMLCompanies_YearWise();
            }
            else
            {
                DownloadHTMLCompanies();
            }
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

        private void DownloadHTMLCompanies()
        {
            _index += 1;
            if (_companies != null)
            {
                if (_index < _companies.Count)
                {
                    var company = _companies[_index];
                    _lastCompany = company;

                    lblCompanyStatus.Text = "Companies " + _index + " Of " + _companies.Count();
                    if (_lastCompany != null)
                    {

                        string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];
                        if (System.IO.Directory.Exists(dirPath) == false)
                        {
                            System.IO.Directory.CreateDirectory(dirPath);
                        }
                        string fullFileName = dirPath + "\\" + _lastCompany.symbol + ".html";
                        string html = string.Empty;
                        //Helper.Log(fullFileName);
                        if (System.IO.File.Exists(fullFileName) == true)
                        {
                            html = System.IO.File.ReadAllText(fullFileName);
                            if (html.Contains("Make sure the web address") == true)
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
                            lblCompany.Text = _lastCompany.company_name;
                            lblSymbol.Text = _lastCompany.symbol;
                            DateTime startDate = DateTime.Now.Date.AddDays(-DataTypeHelper.ToInt32(System.Configuration.ConfigurationManager.AppSettings["no_of_days"].ToString()));
                            DateTime endDate = DateTime.Now.Date;
                            string url = string.Format("https://nseindia.com/products/dynaContent/common/productsSymbolMapping.jsp?symbol={0}&segmentLink=3&symbolCount=2&series=EQ&dateRange=+&fromDate={1}&toDate={2}&dataType=PRICEVOLUME"
                                                                                                              , _lastCompany.symbol.Replace("&", "%26")
                                                                                                              , startDate.ToString("dd-MM-yyyy")
                                                                                                              , endDate.ToString("dd-MM-yyyy")
                                                                                                              );
                            lblStartDate.Text = startDate.ToString("dd/MMM/yyyy");
                            lblEndDate.Text = endDate.ToString("dd/MMM/yyyy");
                            //_lastURL = url;
                            //Helper.Log(url, "NAVIGATE");
                            webBrowser1.Navigate(url);
                        }
                        else
                        {
                            NSEIndiaImport(_lastCompany.symbol, html);
                            DownloadHTMLCompanies();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Completed");
                }
            }
        }

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
                    if (html.Contains("Make sure the web address") == true)
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
                    string url = string.Format("https://nseindia.com/products/dynaContent/common/productsSymbolMapping.jsp?symbol={0}&segmentLink=3&symbolCount=2&series=EQ&dateRange=+&fromDate={1}&toDate={2}&dataType=PRICEVOLUME"
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
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string html = webBrowser1.DocumentText;
            string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];
            if (_IsYearWise)
            {
                dirPath += "\\" + _STARTDATE.ToString("dd_MMM_yyyy");
            }
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
            if (html.Contains("No Records") == true)
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
                //Helper.Log("New Symbol=" + _lastCompany.symbol + ",fullFileName=" + fullFileName, "NEWCOMPANY");
                if (_IsYearWise == false)
                {
                    NSEIndiaImport(_lastCompany.symbol, html);
                }
            }
            //
            if (_IsYearWise)
            {
                DownloadHTMLCompanies_MonthWise();
            }
            else
            {
                DownloadHTMLCompanies();
            }
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
                if (string.IsNullOrEmpty(series) == false)
                {
                    if (series != "EQ")
                    {
                        Helper.Log("NOTSERIES Symbol =" + symbol, "NOTSERIES");
                    }
                }
                if (string.IsNullOrEmpty(date) == false
                    && string.IsNullOrEmpty(originalSymbol) == false
                    && series == "EQ"
                    )
                {
                    DateTime dt = DataTypeHelper.ToDateTime(date);
                    if (dt.Year < 2016)
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
                Console.WriteLine("NSEIndiaImport Temp List No Records Found Symbol =" + symbol);
                Helper.Log("NSEIndiaImport Temp List No Records Found Symbol =" + symbol, "ERROR");
            }
            int rowIndex = 0;
            foreach (var row in tempList)
            {
                rowIndex += 1;
                Console.WriteLine(" Rows " + rowIndex + " Of " + tempList.Count());
                lblTotalRecords.Text = " Import Price Rows " + rowIndex + " Of " + tempList.Count();
                ImportPrice(row);
            }
            using (EcamContext context = new EcamContext())
            {
                DateTime tempStartDate = DateTime.Now.Date.AddDays(-365);
                DateTime tempEndDate = DateTime.Now.Date;
                _Markets = (from q in context.tra_market
                            where q.trade_date >= tempStartDate
                            && q.trade_date <= tempEndDate
                            && q.symbol == symbol
                            select q).ToList();
            }
            rowIndex = 0;
            foreach (var row in tempList)
            {
                rowIndex += 1;
                Console.WriteLine(" Rows " + rowIndex + " Of " + tempList.Count());
                lblTotalRecords.Text = " Update Price Rows " + rowIndex + " Of " + tempList.Count();
                CalculatedPrice(row);
            }
            UpdateCompanyPrice(symbol);
            //}
            //catch (Exception ex)
            //{
            //    lblError.Text = "NSEIndiaImport symbol=" + symbol + ",ex=" + ex.Message;
            //    Helper.Log("NSEIndiaImport symbol=" + symbol + ",ex =" + ex.Message, "ERROR");
            //}
        }

        private void ImportPrice(TempClass import)
        {
            if (import.trade_date.Year < 2016)
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
                //lblError.Text = "ImportPrice Price Index symbol=" + row.symbol + ",Date=" + row.trade_date;
                //Console.WriteLine("ImportPrice Price Index symbol=" + row.symbol + ",Date=" + row.trade_date);
            }
        }

        private static void CalculatedPrice(TempClass import)
        {
            #region UpdatePrice2
            using (EcamContext context = new EcamContext())
            {
                var row = (from q in context.tra_market
                           where q.symbol == import.symbol
                           && q.trade_date == import.trade_date
                           && q.trade_type == import.trade_type
                           select q).FirstOrDefault();
                if (row != null)
                {
                    var market = (from q in _Markets
                                  where q.trade_date < row.trade_date
                                  && q.symbol == import.symbol
                                  orderby q.trade_date descending
                                  select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.prev_price = market.close_price;
                    }
                    market = (from q in _Markets
                              where q.trade_date >= row.trade_date.AddDays(-261)
                              && q.trade_date <= row.trade_date
                              && q.symbol == import.symbol
                              orderby q.high_price descending, q.trade_date descending
                              select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.week_52_high = market.high_price;
                    }
                    market = (from q in _Markets
                              where q.trade_date >= row.trade_date.AddDays(-80)
                              && q.trade_date <= row.trade_date
                              && q.symbol == import.symbol
                              orderby q.high_price descending, q.trade_date descending
                              select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.months_3_high = market.high_price;
                    }
                    market = (from q in _Markets
                              where q.trade_date >= row.trade_date.AddDays(-28)
                              && q.trade_date <= row.trade_date
                              && q.symbol == import.symbol
                              orderby q.high_price descending, q.trade_date descending
                              select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.months_1_high = market.high_price;
                    }
                    market = (from q in _Markets
                              where q.trade_date >= row.trade_date.AddDays(-7)
                              && q.trade_date <= row.trade_date
                              && q.symbol == import.symbol
                              orderby q.high_price descending, q.trade_date descending
                              select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.day_5_high = market.high_price;
                    }
                    context.Entry(row).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                //Console.WriteLine("Update calculated price Index=" + i);
            }
            #endregion
        }

        private void UpdateCompanyPrice(string symbol)
        {
            #region Update Company Price
            using (EcamContext context = new EcamContext())
            {
                tra_market lastMarket = (from q in context.tra_market
                                         where q.symbol == symbol
                                         orderby q.trade_date descending
                                         select q).FirstOrDefault();
                if (lastMarket != null)
                {
                    tra_company company = (from q in context.tra_company
                                           where q.symbol == symbol
                                           select q).FirstOrDefault();

                    if (company != null)
                    {
                        company.open_price = lastMarket.open_price;
                        company.high_price = lastMarket.high_price;
                        company.low_price = lastMarket.low_price;
                        company.close_price = lastMarket.close_price;
                        company.prev_price = lastMarket.prev_price;

                        company.ltp_price = lastMarket.ltp_price;
                        company.week_52_high = lastMarket.week_52_high;
                        company.months_1_high = lastMarket.months_1_high;
                        company.months_3_high = lastMarket.months_3_high;
                        company.day_5_high = lastMarket.day_5_high;

                        context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        //Console.WriteLine("CalculatedPrice Update Company=" + company.company_name);
                    }
                }
            }
            #endregion
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
