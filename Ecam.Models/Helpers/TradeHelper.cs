using Ecam.Contracts;
using Ecam.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Mail;
using System.Linq;
using Ecam.Framework;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Data.Entity.Validation;
using Ecam.Models.Helpers;
using Ecam.Contracts.Enums;
using Ecam.Framework.ExcelHelper;
using CsvHelper;

namespace Ecam.Models {

    public class TempClass {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> high_price { get; set; }
        public Nullable<decimal> low_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> ltp_price { get; set; }
        public Nullable<decimal> prev_price { get; set; }
        public Nullable<decimal> turn_over { get; set; }
        public bool is_prev_price_exist { get; set; }
    }

    public class TradeHelper {

        private static string ReplaceTagAttributes(string html,string tagName) {
            Regex regex = new Regex(
        @"<" + tagName + "(.*?)>",
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
            html = regex.Replace(html,"<" + tagName + ">");
            return html;
        }

        private static string ReplaceAttributes(string html,string attrName,string replaceContent) {
            string exp = attrName + "\\s*=\\s*\"(.*?)\"";
            Regex regex = new Regex(
        exp,
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
            html = regex.Replace(html,replaceContent);
            return html;
        }

        public static void GoogleFinance52WeekImport(string symbol,string html) {
            decimal low = 0;
            decimal high = 0;
            html = html.Replace("\n","").Replace("\r","").Replace("\r\n","");
            //int startIndex = html.IndexOf("<table class=\"snap-data\">");
            //int endIndex = html.IndexOf("</table>");
            //int length = endIndex - startIndex + 8;
            //string tblHTML = html.Substring(startIndex, length);
            //tblHTML = tblHTML.Replace(" ", "");
            Regex regex = new Regex(
    @"<table\s*class=""snap-data"">(.*?)</table>",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );

            MatchCollection tblCollections = regex.Matches(html);
            int i = 0;
            foreach(Match tblMatch in tblCollections) {
                if(high > 0) {
                    break;
                }
                i += 1;
                if(i == 1) {
                    string tbl = tblMatch.Value;
                    regex = new Regex(
        @"<tr>(.*?)</tr>",
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
                    MatchCollection trCollections = regex.Matches(tbl);
                    foreach(Match trMatch in trCollections) {
                        if(high > 0) {
                            break;
                        }
                        string tr = trMatch.Value;
                        regex = new Regex(
    @"<td(.*?)>(.*?)</td>",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );
                        MatchCollection tdCollections = regex.Matches(tr);
                        bool is52WeekStart = false;
                        foreach(Match tdMatch in tdCollections) {
                            string cell1 = string.Empty;
                            string cell2 = string.Empty;
                            string cell3 = string.Empty;
                            if(tdMatch.Groups.Count >= 1) {
                                cell1 = tdMatch.Groups[0].Value;
                            }
                            if(tdMatch.Groups.Count >= 2) {
                                cell2 = tdMatch.Groups[1].Value;
                            }
                            if(tdMatch.Groups.Count >= 3) {
                                cell3 = tdMatch.Groups[2].Value;
                            }
                            if(is52WeekStart == true) {
                                is52WeekStart = false;
                                string[] arr = cell3.Split((" - ").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                                if(arr.Length >= 1) {
                                    low = DataTypeHelper.ToDecimal(arr[0].Trim());
                                }
                                if(arr.Length >= 2) {
                                    high = DataTypeHelper.ToDecimal(arr[1].Trim());
                                }
                                break;
                            }
                            if(cell2.Contains("data-snapfield=\"range_52week\"") == true) {
                                is52WeekStart = true;
                            }
                        }
                    }
                }
            }

            string sql = string.Format(" update tra_company set week_52_low={0},week_52_high={1} where symbol='{2}'",low,high,symbol);
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
            if(high <= 0 || low <= 0) {
                Helper.Log("Not update google finace value symbol=" + symbol,"NOT_UPDATE_GOOGLEFINANCE");
            }
        }

        public static void NSEIndia52WeekImport(string html) {
            decimal week_52_low = 0;
            decimal week_52_high = 0;
            string low = "";
            string high = "";
            string close = "";
            string open = "";
            string lastTrade = "";
            string prev = "";
            string symbol = "";
            string series = "";
            string sql = "";
            string lastUpdateTime = "";
            List<TempClass> tempList = new List<TempClass>();
            html = html.Replace("\n","").Replace("\r","").Replace("\r\n","").Replace("DIV","div");
            if(html.Contains("futLink") == true) {
                int startIndex = html.IndexOf("{\"futLink\"");
                int endIndex = html.IndexOf("],\"optLink\"");
                int length = endIndex - startIndex + 8;

                html = html.Substring(startIndex,length);

                //int startIndex = html.IndexOf("<table class=\"snap-data\">");
                //int endIndex = html.IndexOf("</table>");
                //int length = endIndex - startIndex + 8;
                //string tblHTML = html.Substring(startIndex, length);
                //tblHTML = tblHTML.Replace(" ", "");
                Regex regex = new Regex(
        @"\""high52\""\:\""(?<v>.*)\""\,\""purpose",
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
                MatchCollection collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        week_52_high = DataTypeHelper.ToDecimal(col.Groups["v"].Value);
                    }
                }

                regex = new Regex(
         @"\""low52\""\:\""(?<v>.*)\""\,\""securityVar",
         RegexOptions.IgnoreCase
         | RegexOptions.Multiline
         | RegexOptions.IgnorePatternWhitespace
         | RegexOptions.Compiled
         );
                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        week_52_low = DataTypeHelper.ToDecimal(col.Groups["v"].Value);
                    }
                }

                regex = new Regex(
        @"\""symbol\""\:\""(?<v>.*)\""\,\""varMargin",
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        symbol = col.Groups["v"].Value;
                        symbol = symbol.Replace("&amp;","&");
                    }
                }

                regex = new Regex(
        @"\""series\""\:\""(?<v>.*)\""\,\""isinCode",
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );

                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        series = col.Groups["v"].Value;
                    }
                }

                regex = new Regex(
       @"\""lastUpdateTime\""\:\""(?<v>.*)\""\,\""tradedDate",
       RegexOptions.IgnoreCase
       | RegexOptions.Multiline
       | RegexOptions.IgnorePatternWhitespace
       | RegexOptions.Compiled
       );

                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        lastUpdateTime = col.Groups["v"].Value;
                    }
                }

                regex = new Regex(
     @"\""dayLow\""\:\""(?<v>.*)\""\,\""deliveryToTradedQuantity",
     RegexOptions.IgnoreCase
     | RegexOptions.Multiline
     | RegexOptions.IgnorePatternWhitespace
     | RegexOptions.Compiled
     );

                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        low = col.Groups["v"].Value;
                    }
                }

                regex = new Regex(
    @"\""dayHigh\""\:\""(?<v>.*)\""\,\""exDate",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );

                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        high = col.Groups["v"].Value;
                    }
                }

                regex = new Regex(
    @"\""closePrice\""\:\""(?<v>.*)\""\,\""isExDateFlag",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );

                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        close = col.Groups["v"].Value;
                    }
                }

                regex = new Regex(
    @"\""open\""\:\""(?<v>.*)\""\,\""low52",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );

                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        open = col.Groups["v"].Value;
                    }
                }

                regex = new Regex(
    @"\""lastPrice\""\:\""(?<v>.*)\""\,\""pChange",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );

                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        lastTrade = col.Groups["v"].Value;
                    }
                }

                regex = new Regex(
    @"\""previousClose\""\:\""(?<v>.*)\""\,\""symbol",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );

                collections = regex.Matches(html);
                foreach(Match col in collections) {
                    if(col.Groups.Count > 0) {
                        prev = col.Groups["v"].Value;
                    }
                }

                if(string.IsNullOrEmpty(symbol) == false
                    && string.IsNullOrEmpty(series) == false) {
                    if(series != "EQ") {
                        Helper.Log("Not EQ Series symbol=" + symbol,"NOT_EQ_SERIES_NSEINDIA_Week_52");
                        week_52_low = 0; week_52_high = 0;
                    }
                    DateTime dt = DataTypeHelper.ToDateTime(lastUpdateTime);
                    tempList.Add(new TempClass {
                        symbol = symbol,
                        trade_date = dt,
                        close_price = DataTypeHelper.ToDecimal(close),
                        high_price = DataTypeHelper.ToDecimal(high),
                        low_price = DataTypeHelper.ToDecimal(low),
                        open_price = DataTypeHelper.ToDecimal(open),
                        ltp_price = DataTypeHelper.ToDecimal(lastTrade),
                        prev_price = DataTypeHelper.ToDecimal(prev),
                    });
                    int rowIndex = 0;
                    foreach(var row in tempList) {
                        rowIndex += 1;
                        //Console.WriteLine(" Rows " + rowIndex + " Of " + tempList.Count());
                        //lblTotalRecords.Text = " Import Price Rows " + rowIndex + " Of " + tempList.Count();
                        ImportPrice(row);
                    }
                    TradeHelper.UpdateCompanyPrice(symbol);
                    sql = string.Format(" update tra_company set week_52_low={0},week_52_high={1} where symbol='{2}'",week_52_low,week_52_high,symbol);
                    MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
                    if(week_52_high <= 0 || week_52_low <= 0) {
                        Helper.Log("Not update nse india symbol=" + symbol,"NOT_Update_Week_52_SERIES_NSEINDIA_Week_52");
                    }
                }
            } else {
                Helper.Log("NO CONTENT in futLink","futLink");
            }
        }

        public static void GoogleIndiaImport(string html,string symbol) {
            //try
            //{
            html = ReplaceTagAttributes(html,"table");
            html = ReplaceTagAttributes(html,"tbody");
            html = ReplaceTagAttributes(html,"tr");
            html = ReplaceTagAttributes(html,"td");
            html = ReplaceTagAttributes(html,"th");
            html = ReplaceTagAttributes(html,"img");
            html = ReplaceTagAttributes(html,"div");
            html = ReplaceAttributes(html,"style","");
            html = ReplaceAttributes(html,"class","");
            html = ReplaceAttributes(html,"nowrap","");
            html = ReplaceAttributes(html,"title","");
            html = html.Replace("<TH nowrap=\"\">","<th>");
            html = html.Replace("<TD class=\"normalText\" nowrap=\"\">","<td>");
            html = html.Replace("<TD class=\"date\" nowrap=\"\">","<td>");
            html = html.Replace("<TD class=\"number\" nowrap=\"\">","<td>");
            html = html.Replace("\n","").Replace("\r","").Replace("\r\n","").Replace("TABLE","table").Replace("TR","tr").Replace("TD","td").Replace("TH","th").Replace("TBODY","tbody");
            int startIndex = html.IndexOf("<tbody>");
            int endIndex = html.IndexOf("</tbody>");
            int length = endIndex - startIndex + 8;
            string tblHTML = html;//.Substring(startIndex, length);
            tblHTML = tblHTML.Replace(" ","");
            List<TempClass> tempList = new List<TempClass>();
            tblHTML = tblHTML.Replace("<tr>","||");
            tblHTML = tblHTML.Replace("<td>","~");
            tblHTML = tblHTML.Replace("<table>","");
            tblHTML = tblHTML.Replace("</table>","");
            string date = "";
            //string prev = "";
            string open = "";
            string high = "";
            string low = "";
            string lastTrade = "";
            string close = "";
            string tradeType = "NSE";
            string[] rows = tblHTML.Split(("||").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
            foreach(string row in rows) {

                string[] cells = row.Split(("~").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int index = 0;
                for(index = 0;index < cells.Length;index++) {
                    string value = cells[index].Trim();
                    switch(index) {
                        case 0: date = value; break;
                        case 1: open = value; break;
                        case 2: high = value; break;
                        case 3: low = value; break;
                        case 4: close = value; break;
                    }
                }
                lastTrade = close;
                if(string.IsNullOrEmpty(date) == false
                  && string.IsNullOrEmpty(symbol) == false
                  ) {
                    DateTime dt = DataTypeHelper.ToDateTime(date);
                    if(dt.Year > 1900) {
                        tempList.Add(new TempClass {
                            symbol = symbol,
                            trade_date = dt,
                            close_price = DataTypeHelper.ToDecimal(close),
                            high_price = DataTypeHelper.ToDecimal(high),
                            low_price = DataTypeHelper.ToDecimal(low),
                            open_price = DataTypeHelper.ToDecimal(open),
                            ltp_price = DataTypeHelper.ToDecimal(lastTrade),
                            //prev_price = DataTypeHelper.ToDecimal(prev),
                        });
                    }
                }
            }
            tempList = (from q in tempList orderby q.trade_date ascending select q).ToList();
            int rowIndex = 0;
            foreach(var temprow in tempList) {
                rowIndex += 1;
                ImportPrice(temprow);
                Console.WriteLine("Symbol=" + symbol + ",Date=" + temprow.trade_date.ToString("dd/MM/yyyy") + " Completed");
            }
            /*
            using (EcamContext context = new EcamContext())
            {
                tra_company company = (from q in context.tra_company
                                       where q.symbol == symbol
                                       select q).FirstOrDefault();
                if (company != null)
                {
                    DateTime startDate = DateTime.Now.AddDays(-120);
                    DateTime endDate = DateTime.Now.Date;
                    List<tra_market> markets = (from q in context.tra_market
                                                where q.trade_date >= startDate
                                                && q.trade_date <= endDate
                                                && q.symbol == symbol
                                                orderby q.trade_date descending
                                                select q).ToList();
                     
                    foreach(var market in markets)
                    {
                        tra_market prevRow = (from q in markets
                                       where q.trade_date < market.trade_date
                                       && q.symbol == market.symbol
                                       orderby q.trade_date descending
                                       select q).FirstOrDefault();
                        if (prevRow != null)
                        {
                            market.prev_price = prevRow.close_price;
                            context.Entry(market).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            Console.WriteLine("Symbol=" + symbol + ",Date=" + market.trade_date.ToString("dd/MM/yyyy") + " Update prev date=" + prevRow.trade_date.ToString("dd/MMM/yyyy") + ",preprice=" + prevRow.close_price + " completed");
                        }
                    }
                }
            }
            */
            /*
            Regex regex = new Regex(
        @"<tr>(.*?)<tr>",
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

                regex = new Regex(
                            @"<" + tagName + ">(.+?)</" + tagName + ">",
                            RegexOptions.IgnoreCase
                            | RegexOptions.Multiline
                            | RegexOptions.IgnorePatternWhitespace
                            | RegexOptions.Compiled
                            );
                MatchCollection rowMatches = regex.Matches(tr);
                string date = "";
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
                        case 0: symbol = value; break;
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
                if (string.IsNullOrEmpty(series) == false)
                {
                    if (series != "EQ")
                    {
                        Helper.Log("NOTSERIES Symbol =" + symbol, "NOTSERIES");
                    }
                }
                if (string.IsNullOrEmpty(date) == false
                    && string.IsNullOrEmpty(symbol) == false
                    && series == "EQ"
                    )
                {
                    DateTime dt = DataTypeHelper.ToDateTime(date);
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
                        prev_price = DataTypeHelper.ToDecimal(prev),
                    });
                }
            }
            
            if (tempList.Count() <= 0)
            {
                Helper.Log("NSEIndiaImport Temp List No Records Found " + Environment.NewLine + html, "ERROR");
            }
            int rowIndex = 0;
            foreach (var row in tempList)
            {
                rowIndex += 1;
                ImportPrice(row);
            } 
            */
        }

        public static void NSEIndiaImport(string html) {
            //try
            //{
            html = ReplaceTagAttributes(html,"table");
            html = ReplaceTagAttributes(html,"tbody");
            html = ReplaceTagAttributes(html,"tr");
            html = ReplaceTagAttributes(html,"td");
            html = ReplaceTagAttributes(html,"th");
            html = ReplaceTagAttributes(html,"img");
            html = ReplaceTagAttributes(html,"div");
            html = ReplaceAttributes(html,"style","");
            html = ReplaceAttributes(html,"class","");
            html = ReplaceAttributes(html,"nowrap","");
            html = ReplaceAttributes(html,"title","");
            html = html.Replace("<TH nowrap=\"\">","<th>");
            html = html.Replace("<TD class=\"normalText\" nowrap=\"\">","<td>");
            html = html.Replace("<TD class=\"date\" nowrap=\"\">","<td>");
            html = html.Replace("<TD class=\"number\" nowrap=\"\">","<td>");
            html = html.Replace("\n","").Replace("\r","").Replace("\r\n","").Replace("TABLE","table").Replace("TR","tr").Replace("TD","td").Replace("TH","th").Replace("TBODY","tbody");
            int startIndex = html.IndexOf("<tbody>");
            int endIndex = html.IndexOf("</tbody>");
            int length = endIndex - startIndex + 8;
            string tblHTML = html.Substring(startIndex,length);
            tblHTML = tblHTML.Replace(" ","");
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
            foreach(Match trMatch in trCollections) {
                i += 1;
                string tr = trMatch.Value;
                string tagName = "td";
                if(i == 1) {
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
                string symbol = "";
                string series = "";
                string prev = "";
                string open = "";
                string high = "";
                string low = "";
                string lastTrade = "";
                string close = "";
                string tradeType = "NSE";
                int colIndex = -1;
                foreach(Match colMatch in rowMatches) {
                    colIndex += 1;
                    if(i > 1) {
                        string value = string.Empty;
                        if(colMatch.Groups.Count >= 2) {
                            value = colMatch.Groups[1].Value;
                        }
                        if(string.IsNullOrEmpty(value) == false) {
                            value = value.Trim();
                        }
                        switch(colIndex) {
                            case 0: symbol = value; break;
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
                if(string.IsNullOrEmpty(series) == false) {
                    if(series != "EQ") {
                        Helper.Log("NOTSERIES Symbol =" + symbol,"NOTSERIES");
                    }
                }
                if(string.IsNullOrEmpty(date) == false
                    && string.IsNullOrEmpty(symbol) == false
                    && series == "EQ"
                    ) {
                    DateTime dt = DataTypeHelper.ToDateTime(date);
                    tempList.Add(new TempClass {
                        symbol = symbol,
                        trade_date = dt,
                        close_price = DataTypeHelper.ToDecimal(close),
                        high_price = DataTypeHelper.ToDecimal(high),
                        low_price = DataTypeHelper.ToDecimal(low),
                        open_price = DataTypeHelper.ToDecimal(open),
                        ltp_price = DataTypeHelper.ToDecimal(lastTrade),
                        prev_price = DataTypeHelper.ToDecimal(prev),
                    });
                }
            }
            if(tempList.Count() <= 0) {
                Helper.Log("NSEIndiaImport Temp List No Records Found " + Environment.NewLine + html,"ERROR");
            }
            int rowIndex = 0;
            foreach(var row in tempList) {
                rowIndex += 1;
                //Console.WriteLine(" Rows " + rowIndex + " Of " + tempList.Count());
                //lblTotalRecords.Text = " Import Price Rows " + rowIndex + " Of " + tempList.Count();
                ImportPrice(row);
            }
            //rowIndex = 0;
            //foreach (var row in tempList)
            //{
            //rowIndex += 1;
            //Console.WriteLine(" Rows " + rowIndex + " Of " + tempList.Count());
            //lblTotalRecords.Text = " Update Price Rows " + rowIndex + " Of " + tempList.Count();
            //CalculatedPrice(row);
            //}
            //UpdateCompanyPrice(symbol);
            //}
            //catch (Exception ex)
            //{
            //    lblError.Text = "NSEIndiaImport symbol=" + symbol + ",ex=" + ex.Message;
            //    Helper.Log("NSEIndiaImport symbol=" + symbol + ",ex =" + ex.Message, "ERROR");
            //}
        }

        public static void UpdateCompanyPrice(string symbol) {
            #region Update Company Price
            using(EcamContext context = new EcamContext()) {
                tra_company company = (from q in context.tra_company
                                       where q.symbol == symbol
                                       select q).FirstOrDefault();
                if(company != null) {
                    company.open_price = 0;
                    company.high_price = 0;
                    company.low_price = 0;
                    company.close_price = 0;
                    company.prev_price = 0;
                    company.ltp_price = 0;
                    company.monthly_avg = 0;
                    company.weekly_avg = 0;
                    tra_market lastMarket = (from q in context.tra_market
                                             where q.symbol == symbol
                                             orderby q.trade_date descending
                                             select q).FirstOrDefault();
                    if(lastMarket != null) {
                        company.open_price = lastMarket.open_price;
                        company.high_price = lastMarket.high_price;
                        company.low_price = lastMarket.low_price;
                        company.close_price = lastMarket.close_price;
                        company.prev_price = lastMarket.prev_price;
                        company.ltp_price = lastMarket.ltp_price;
                    }
                    List<tra_market_avg> avgs = (from q in context.tra_market_avg
                                                 where q.symbol == symbol
                                                 select q).ToList();
                    decimal total = 0;

                    var monthly = (from q in avgs
                                   where q.symbol == symbol && q.avg_type == "M"
                                   select q).ToList();
                    total = (from q in monthly select q.percentage).Sum();
                    company.monthly_avg = DataTypeHelper.SafeDivision(total,monthly.Count());

                    var weekly = (from q in avgs
                                  where q.symbol == symbol && q.avg_type == "W"
                                  select q).ToList();
                    total = (from q in monthly select q.percentage).Sum();
                    company.weekly_avg = DataTypeHelper.SafeDivision(total,weekly.Count());

                    context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    //Console.WriteLine("CalculatedPrice Update Company=" + company.company_name);
                }
            }
            #endregion
        }

        //        public static void CalculateMovingAVG(string symbol)
        //        {
        //            using (EcamContext context = new EcamContext())
        //            {
        //                tra_company company = (from q in context.tra_company
        //                                       where q.symbol == symbol
        //                                       select q).FirstOrDefault();
        //                if (company != null)
        //                {
        //                    DateTime startDate = DateTime.Now.AddDays(-120);
        //                    DateTime endDate = DateTime.Now.Date;
        //                    List<tra_market> markets = (from q in context.tra_market
        //                                                where q.trade_date >= startDate
        //                                                && q.trade_date < endDate
        //                                                && q.symbol == symbol
        //                                                orderby q.trade_date descending
        //                                                select q).ToList();

        //                    company.day_1 = CalculateAVG(markets, 1);
        //                    company.day_2 = CalculateAVG(markets, 2);
        //                    company.day_3 = CalculateAVG(markets, 3);
        //                    company.day_4 = CalculateAVG(markets, 4);

        //                    company.day_5 = CalculateAVG(markets, 5);
        //                    company.day_10 = CalculateAVG(markets, 10);
        //                    company.day_15 = CalculateAVG(markets, 15);
        //                    company.day_20 = CalculateAVG(markets, 20);
        //                    company.day_25 = CalculateAVG(markets, 25);
        //                    company.day_30 = CalculateAVG(markets, 30);
        //                    company.day_35 = CalculateAVG(markets, 35);
        //                    company.day_40 = CalculateAVG(markets, 40);
        //                    company.day_45 = CalculateAVG(markets, 45);
        //                    company.day_50 = CalculateAVG(markets, 50);
        //                    company.day_55 = CalculateAVG(markets, 55);
        //                    company.day_60 = CalculateAVG(markets, 60);
        //                    company.day_65 = CalculateAVG(markets, 65);
        //                    company.day_70 = CalculateAVG(markets, 70);
        //                    company.day_75 = CalculateAVG(markets, 75);
        //                    company.day_80 = CalculateAVG(markets, 80);
        //                    company.day_85 = CalculateAVG(markets, 85);
        //                    company.day_90 = CalculateAVG(markets, 90);

        //                    bool isDay1High = (company.day_1 ?? 0) <= (company.ltp_price ?? 0);
        //                    bool isDay2High = (
        //                            (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //                            && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //                            );
        //                    bool isDay3High = (
        //(company.day_3 ?? 0) < (company.day_2 ?? 0)
        //&& (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //&& (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //);
        //                    bool isDay4High = (
        //                        (company.day_4 ?? 0) < (company.day_3 ?? 0)
        //&& (company.day_3 ?? 0) < (company.day_2 ?? 0)
        //&& (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //&& (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //);

        //                    bool isDay5High = (
        //                        (company.day_5 ?? 0) < (company.day_4 ?? 0)
        //                       && (company.day_4 ?? 0) < (company.day_3 ?? 0)
        //&& (company.day_3 ?? 0) < (company.day_2 ?? 0)
        //&& (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //&& (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //);

        //                    bool isDay1Low = (company.day_1 ?? 0) >= (company.ltp_price ?? 0);
        //                    bool isDay2Low = (
        //                            (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //                            && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //                            );
        //                    bool isDay3Low = (
        //(company.day_3 ?? 0) > (company.day_2 ?? 0)
        //&& (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //&& (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //);
        //                    bool isDay4Low = (
        //                        (company.day_4 ?? 0) > (company.day_3 ?? 0)
        //&& (company.day_3 ?? 0) > (company.day_2 ?? 0)
        //&& (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //&& (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //);
        //                    bool isDay5Low = (
        //                        (company.day_5 ?? 0) > (company.day_4 ?? 0)
        //                        && (company.day_4 ?? 0) > (company.day_3 ?? 0)
        //&& (company.day_3 ?? 0) > (company.day_2 ?? 0)
        //&& (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //&& (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //);

        //                    int highCnt = 0;
        //                    int lowCnt = 0;
        //                    if (isDay1High == true && isDay2High == true
        //                        && isDay3High == true && isDay4High == true
        //                        && isDay5High == true)
        //                    {
        //                        highCnt = 5;
        //                    }
        //                    else if (isDay1High == true && isDay2High == true
        //                        && isDay3High == true && isDay4High == true
        //                        )
        //                    {
        //                        highCnt = 4;
        //                    }
        //                    else if (isDay1High == true && isDay2High == true
        //                        && isDay3High == true
        //                        )
        //                    {
        //                        highCnt = 3;
        //                    }
        //                    else if (isDay1High == true && isDay2High == true
        //                        )
        //                    {
        //                        highCnt = 2;
        //                    }
        //                    else if (isDay1High == true)
        //                    {
        //                        highCnt = 1;
        //                    }

        //                    if (isDay1Low == true && isDay2Low == true
        //                        && isDay3Low == true && isDay4Low == true
        //                        && isDay5Low == true)
        //                    {
        //                        lowCnt = 5;
        //                    }
        //                    else if (isDay1Low == true && isDay2Low == true
        //                        && isDay3Low == true && isDay4Low == true
        //                        )
        //                    {
        //                        lowCnt = 4;
        //                    }
        //                    else if (isDay1Low == true && isDay2Low == true
        //                        && isDay3Low == true
        //                        )
        //                    {
        //                        lowCnt = 3;
        //                    }
        //                    else if (isDay1Low == true && isDay2Low == true
        //                        )
        //                    {
        //                        lowCnt = 2;
        //                    }
        //                    else if (isDay1Low == true)
        //                    {
        //                        lowCnt = 1;
        //                    }
        //                    company.high_count = highCnt;
        //                    company.low_count = lowCnt;
        //                    context.Entry(company).State = System.Data.Entity.EntityState.Modified;
        //                    context.SaveChanges();
        //                }
        //            }
        //        }

        private static decimal CalculateAVG(List<tra_market> markets,int dayCount) {
            int i = 0;
            //decimal total = 0;
            List<tra_market> temp = new List<tra_market>();
            foreach(var market in markets) {
                if(dayCount > i) {
                    //      total += (market.close_price ?? 0);
                    temp.Add(market);
                    i += 1;
                } else {
                    break;
                }
            }
            //decimal avg = DataTypeHelper.SafeDivision(total, dayCount);
            decimal value = 0;
            if(temp.Count > 0) {
                var tempRow = (from q in temp
                               orderby q.trade_date ascending
                               select q).FirstOrDefault();
                value = (tempRow.ltp_price ?? 0);
            }
            return value; // DataTypeHelper.SafeDivision((currentPrice - avg), avg) * 100;
        }

        public static void ImportPrice(TempClass import) {
            using(EcamContext context = new EcamContext()) {
                import.symbol = import.symbol.Replace("&amp;","&");
                var row = (from q in context.tra_market
                           where q.symbol == import.symbol
                           && q.trade_date == import.trade_date.Date
                           select q).FirstOrDefault();
                bool isNew = false;
                if(row == null) {
                    row = new tra_market();
                    isNew = true;
                }
                //if(isNew == true) {
                row.symbol = import.symbol;
                row.trade_date = import.trade_date.Date;
                row.open_price = import.open_price;
                row.high_price = import.high_price;
                row.low_price = import.low_price;
                row.close_price = import.close_price;
                row.ltp_price = import.ltp_price;
                if(import.is_prev_price_exist == false) {
                    tra_market market = (from q in context.tra_market
                                         where q.symbol == import.symbol
                                         && q.trade_date < row.trade_date
                                         orderby q.trade_date descending
                                         select q).FirstOrDefault();
                    if(market != null) {
                        row.prev_price = market.close_price;
                    }
                } else {
                    row.prev_price = import.prev_price;
                }
                row.percentage = DataTypeHelper.SafeDivision(((row.ltp_price ?? 0) - (row.prev_price ?? 0)),(row.prev_price ?? 0)) * 100;
                if(isNew == true) {
                    context.tra_market.Add(row);
                } else {
                    context.Entry(row).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
                //}
                CreateAVG(row.symbol,row.trade_date);
                CreateYearLog(row.symbol,row.trade_date);
                //lblError.Text = "ImportPrice Price Index symbol=" + row.symbol + ",Date=" + row.trade_date;
                Console.WriteLine("ImportPrice Price Index symbol=" + row.symbol + ",Date=" + row.trade_date);
            }
            UpdateCompanyPrice(import.symbol);
        }

        public static void CreateDailyLog() {
            DateTime startDate = Convert.ToDateTime("01/01/2014");
            int total = (365 * 5);
            int i;
            for(i = 0;i < total;i++) {
                DateTime tradeDate = startDate.AddDays(i);
                CreateDailyLog(tradeDate,"true");
            }
        }

        public static void CreateDailyLog(DateTime tradeDate,string isBookMark) {
            tradeDate = tradeDate.Date;
            string strDate = tradeDate.ToString("yyyy-MM-dd");
            string sql = string.Format(" select ((m.close_price-m.prev_price)/m.prev_price) * 100 as profit,m.symbol,m.trade_date  " + Environment.NewLine +
                         " from tra_market m " + Environment.NewLine +
                         " join tra_company c on c.symbol = m.symbol " + Environment.NewLine +
                         " join tra_company_category cc on c.symbol = cc.symbol " + Environment.NewLine +
                         " join tra_category cat on cat.category_name = cc.category_name " + Environment.NewLine +
                         " where m.trade_date = '{0}' and ifnull(m.prev_price,0) > 0 ",strDate);
            if(string.IsNullOrEmpty(isBookMark) == false) {
                if(isBookMark == "true") {
                    sql += " and ifnull(cat.is_book_mark,0)=1";
                }
            }
            sql += " group by m.symbol,m.trade_date ";
            using(MySqlDataReader dr = MySqlHelper.ExecuteReader(Ecam.Framework.Helper.ConnectionString,sql)) {
                int positive = 0;
                int negative = 0;
                while(dr.Read()) {
                    decimal profit = DataTypeHelper.ToDecimal(dr["profit"].ToString());
                    if(profit > 0) {
                        positive += 1;
                    } else {
                        negative += 1;
                    }
                }
                if(positive > 0 || negative > 0) {
                    using(EcamContext context = new EcamContext()) {
                        try {
                            tra_daily_log log = (from q in context.tra_daily_log
                                                 where q.trade_date == tradeDate
                                                 && q.is_book_mark == (isBookMark == "true" ? true : false)
                                                 select q).FirstOrDefault();
                            if(log == null) {
                                log = new tra_daily_log();
                            }
                            log.trade_date = tradeDate;
                            log.positive = positive;
                            log.negative = negative;
                            log.is_book_mark = (isBookMark == "true" ? true : false);
                            if(log.id > 0) {
                                context.Entry(log).State = System.Data.Entity.EntityState.Modified;
                            } else {
                                context.tra_daily_log.Add(log);
                            }
                            context.SaveChanges();
                            Console.WriteLine("Log Completed Date=" + log.trade_date);
                        } catch { }
                    }
                }
            }
        }

        public static void CreateYearLog(string symbol,DateTime date) {
            DateTime startDate = DataTypeHelper.ToDateTime(date.Year + "/04/01");
            DateTime endDate = DataTypeHelper.ToDateTime((date.Year + 1) + "/03/31");
            int year = date.Year;
            using(EcamContext context = new EcamContext()) {
                tra_market openMarket = (from q in context.tra_market
                                         where q.symbol == symbol
                                         && q.trade_date >= startDate
                                         && q.trade_date <= endDate
                                         orderby q.trade_date ascending
                                         select q).FirstOrDefault();
                tra_market closeMarket = (from q in context.tra_market
                                          where q.symbol == symbol
                                          && q.trade_date >= startDate
                                          && q.trade_date <= endDate
                                          orderby q.trade_date descending
                                          select q).FirstOrDefault();
                if(openMarket != null && closeMarket != null) {
                    tra_year_log log = (from q in context.tra_year_log
                                        where q.symbol == symbol
                                        && q.year == year
                                        select q).FirstOrDefault();
                    bool isNew = false;
                    if(log == null) {
                        log = new tra_year_log();
                        isNew = true;
                    }
                    log.symbol = symbol;
                    log.year = year;
                    log.open_price = openMarket.open_price;
                    log.close_price = closeMarket.close_price;
                    log.percentage = DataTypeHelper.SafeDivision((log.close_price ?? 0) - (log.open_price ?? 0),(log.open_price ?? 0)) * 100;
                    if(isNew == true) {
                        context.tra_year_log.Add(log);
                    } else {
                        context.Entry(log).State = System.Data.Entity.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
        }

        public static void CreateAVG(string symbol,DateTime date) {
            using(EcamContext context = new EcamContext()) {
                IQueryable<tra_market> markets = context.tra_market.Where(q => q.symbol == symbol);
                if(date.Year > 1900) {
                    markets = markets.Where(q => q.trade_date == date);
                }
                tra_market market = (from q in markets orderby q.trade_date descending select q).FirstOrDefault();
                if(market != null) {
                    DateTime monthStartDate = DataTypeHelper.GetFirstDayOfMonth(market.trade_date);
                    DateTime monthEndDate = DataTypeHelper.GetLastDayOfMonth(market.trade_date);
                    DateTime weekFirstDate = DataTypeHelper.GetFirstDayOfWeek(market.trade_date);
                    DateTime weekLastDate = DataTypeHelper.GetLastDayOfWeek(market.trade_date);

                    var monthFirstTrade = (from q in context.tra_market where q.symbol == market.symbol && q.trade_date >= monthStartDate orderby q.trade_date ascending select q).FirstOrDefault();
                    var monthLastTrade = (from q in context.tra_market where q.symbol == market.symbol && q.trade_date <= monthEndDate orderby q.trade_date descending select q).FirstOrDefault();
                    if(monthFirstTrade != null && monthLastTrade != null) {
                        //if (monthLastTrade.trade_date.Day >= 15)
                        //{
                        decimal openPrice = (monthFirstTrade.open_price ?? 0);
                        decimal lastPrice = (monthLastTrade.close_price ?? 0);
                        CreateAVGRecord(market.symbol,"M",openPrice,lastPrice,monthStartDate);
                        //}
                    }

                    var weekFirstTrade = (from q in context.tra_market where q.symbol == market.symbol && q.trade_date >= weekFirstDate orderby q.trade_date ascending select q).FirstOrDefault();
                    var weekLastTrade = (from q in context.tra_market where q.symbol == market.symbol && q.trade_date <= weekLastDate orderby q.trade_date descending select q).FirstOrDefault();
                    if(weekFirstTrade != null && weekLastTrade != null) {
                        decimal openPrice = (weekFirstTrade.open_price ?? 0);
                        decimal lastPrice = (weekLastTrade.close_price ?? 0);
                        CreateAVGRecord(market.symbol,"W",openPrice,lastPrice,weekFirstDate);
                    }
                }
            }
        }

        public static void _CreateAVG(string symbol) {
            List<tra_market> markets;
            using(EcamContext context = new EcamContext()) {
                markets = (from q in context.tra_market where q.symbol == symbol orderby q.trade_date select q).ToList();
            }
            foreach(var market in markets) {
                DateTime monthStartDate = DataTypeHelper.GetFirstDayOfMonth(market.trade_date);
                DateTime monthEndDate = DataTypeHelper.GetLastDayOfMonth(market.trade_date);
                DateTime weekFirstDate = DataTypeHelper.GetFirstDayOfWeek(market.trade_date);
                DateTime weekLastDate = DataTypeHelper.GetLastDayOfWeek(market.trade_date);

                var monthFirstTrade = (from q in markets where q.trade_date >= monthStartDate orderby q.trade_date ascending select q).FirstOrDefault();
                var monthLastTrade = (from q in markets where q.trade_date <= monthEndDate orderby q.trade_date descending select q).FirstOrDefault();
                if(monthFirstTrade != null && monthLastTrade != null) {
                    //if (monthLastTrade.trade_date.Day >= 15)
                    //{
                    decimal openPrice = (monthFirstTrade.open_price ?? 0);
                    decimal lastPrice = (monthLastTrade.close_price ?? 0);
                    CreateAVGRecord(symbol,"M",openPrice,lastPrice,monthStartDate);
                    //}
                }

                var weekFirstTrade = (from q in markets where q.trade_date >= weekFirstDate orderby q.trade_date ascending select q).FirstOrDefault();
                var weekLastTrade = (from q in markets where q.trade_date <= weekLastDate orderby q.trade_date descending select q).FirstOrDefault();
                if(weekFirstTrade != null && weekLastTrade != null) {
                    decimal openPrice = (weekFirstTrade.open_price ?? 0);
                    decimal lastPrice = (weekLastTrade.close_price ?? 0);
                    CreateAVGRecord(symbol,"W",openPrice,lastPrice,weekFirstDate);
                }
            }
        }

        private static void CreateAVGRecord(string symbol,string type,decimal openPrice,decimal lastPrice,DateTime date) {
            decimal profit = 0;
            try { profit = ((lastPrice - openPrice) / openPrice) * 100; } catch { }
            using(EcamContext context = new EcamContext()) {
                tra_market_avg avg = (from q in context.tra_market_avg
                                      where q.symbol == symbol
                                      && q.avg_type == type
                                      && q.avg_date == date
                                      select q).FirstOrDefault();
                if(avg == null) {
                    avg = new tra_market_avg();
                }
                avg.symbol = symbol;
                avg.avg_type = type;
                avg.avg_date = date;
                avg.percentage = profit;
                if(avg.id > 0) {
                    context.Entry(avg).State = System.Data.Entity.EntityState.Modified;
                } else {
                    context.tra_market_avg.Add(avg);
                }
                context.SaveChanges();
            }
        }

        public static string RemoveHTMLTag(string html) {
            Regex regex = new Regex(
@"<[^>]*>",
RegexOptions.IgnoreCase
| RegexOptions.Multiline
| RegexOptions.IgnorePatternWhitespace
| RegexOptions.Compiled
);
            string result = regex.Replace(html,"");
            result = result.Replace("&nbsp;","");
            return result;
        }

        public static void GetUpdatePriceUsingGoogle(string symbol) {
            //string type = "NSE";
            //string url = string.Format("http://finance.google.com/finance/info?client=ig&q={0}:{1}", type, symbol.Replace("&", "%26"));
            //WebClient client = new WebClient();
            //string html = client.DownloadString(url);
            //GoogleDownloadData gd = new GoogleDownloadData();
            //return gd.GetPrice(html);
            GoogleDownloadData gd = new GoogleDownloadData();
            gd.GoogleDataDownload(symbol);
        }

        //private static void CalculatedPrice(TempClass import)
        //{
        //    #region UpdatePrice2
        //    using (EcamContext context = new EcamContext())
        //    {
        //        var row = (from q in context.tra_market
        //                   where q.symbol == import.symbol
        //                   && q.trade_date == import.trade_date
        //                   && q.trade_type == import.trade_type
        //                   select q).FirstOrDefault();
        //        if (row != null)
        //        {
        //            var market = (from q in _Markets
        //                          where q.trade_date < row.trade_date
        //                          && q.symbol == import.symbol
        //                          orderby q.trade_date descending
        //                          select q).FirstOrDefault();
        //            if (market != null)
        //            {
        //                row.prev_price = market.close_price;
        //            }
        //            // Uses the default calendar of the InvariantCulture.
        //            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
        //            DateTime tempStartDate = myCal.AddWeeks(row.trade_date, -52);
        //            market = (from q in _Markets
        //                      where q.trade_date >= tempStartDate //row.trade_date.AddDays(-261)
        //                      && q.trade_date <= row.trade_date
        //                      && q.symbol == import.symbol
        //                      orderby q.high_price descending, q.trade_date descending
        //                      select q).FirstOrDefault();
        //            if (market != null)
        //            {
        //                row.week_52_high = market.high_price;
        //            }
        //            market = (from q in _Markets
        //                      where q.trade_date >= row.trade_date.AddDays(-80)
        //                      && q.trade_date <= row.trade_date
        //                      && q.symbol == import.symbol
        //                      orderby q.high_price descending, q.trade_date descending
        //                      select q).FirstOrDefault();
        //            if (market != null)
        //            {
        //                row.months_3_high = market.high_price;
        //            }
        //            market = (from q in _Markets
        //                      where q.trade_date >= row.trade_date.AddDays(-28)
        //                      && q.trade_date <= row.trade_date
        //                      && q.symbol == import.symbol
        //                      orderby q.high_price descending, q.trade_date descending
        //                      select q).FirstOrDefault();
        //            if (market != null)
        //            {
        //                row.months_1_high = market.high_price;
        //            }
        //            market = (from q in _Markets
        //                      where q.trade_date >= row.trade_date.AddDays(-7)
        //                      && q.trade_date <= row.trade_date
        //                      && q.symbol == import.symbol
        //                      orderby q.high_price descending, q.trade_date descending
        //                      select q).FirstOrDefault();
        //            if (market != null)
        //            {
        //                row.day_5_high = market.high_price;
        //            }
        //            context.Entry(row).State = System.Data.Entity.EntityState.Modified;
        //            context.SaveChanges();
        //        }
        //        //Console.WriteLine("Update calculated price Index=" + i);
        //    }
        //    #endregion
        //}
    }

    public class TempRSI {
        public string symbol { get; set; }
        public decimal close { get; set; }
        public decimal prev { get; set; }
        public decimal change {
            get {
                return this.close - this.prev;
            }
        }
        public decimal upward {
            get {
                return (this.change > 0 ? this.change : 0);
            }
        }
        public decimal downward {
            get {
                return (this.change < 0 ? this.change * -1 : 0);
            }
        }
        public decimal avg_upward { get; set; }
        public decimal avg_downward { get; set; }
        public decimal rs {
            get {
                return DataTypeHelper.SafeDivision(this.avg_upward,this.avg_downward);
            }
        }
        public decimal rsi {
            get {
                if(this.avg_downward == 0)
                    return 100;
                else
                    return (100 - (100 / (1 + this.rs)));
            }
        }
        public DateTime date { get; set; }
    }

    public class PriceDetailJSON {
        public string id { get; set; }
        public string t { get; set; }
        public string e { get; set; }
        public string l { get; set; }
        public string l_fix { get; set; }
        public string l_cur { get; set; }
        public string lt_dts { get; set; }
        public string c { get; set; }
        public string c_fix { get; set; }
        public string cp { get; set; }
        public string cp_fix { get; set; }
    }

    public class RowCollections {
        public RowCollections() {
            this.cells = new List<string>();
        }
        public int index { get; set; }
        public string name { get; set; }
        public List<string> cells { get; set; }
    }

}