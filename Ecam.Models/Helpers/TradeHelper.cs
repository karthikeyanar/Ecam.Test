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

namespace Ecam.Models
{

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
        public Nullable<decimal> prev_price { get; set; }
    }

    public class TradeHelper
    {

        private static string ReplaceTagAttributes(string html, string tagName)
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

        private static string ReplaceAttributes(string html, string attrName, string replaceContent)
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

        public static void GoogleFinance52WeekImport(string symbol, string html)
        {
            decimal low = 0;
            decimal high = 0;
            html = html.Replace("\n", "").Replace("\r", "").Replace("\r\n", "");
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
            foreach (Match tblMatch in tblCollections)
            {
                if (high > 0)
                {
                    break;
                }
                i += 1;
                if (i == 1)
                {
                    string tbl = tblMatch.Value;
                    regex = new Regex(
        @"<tr>(.*?)</tr>",
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
                    MatchCollection trCollections = regex.Matches(tbl);
                    foreach (Match trMatch in trCollections)
                    {
                        if (high > 0)
                        {
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
                        foreach (Match tdMatch in tdCollections)
                        {
                            string cell1 = string.Empty;
                            string cell2 = string.Empty;
                            string cell3 = string.Empty;
                            if (tdMatch.Groups.Count >= 1)
                            {
                                cell1 = tdMatch.Groups[0].Value;
                            }
                            if (tdMatch.Groups.Count >= 2)
                            {
                                cell2 = tdMatch.Groups[1].Value;
                            }
                            if (tdMatch.Groups.Count >= 3)
                            {
                                cell3 = tdMatch.Groups[2].Value;
                            }
                            if (is52WeekStart == true)
                            {
                                is52WeekStart = false;
                                string[] arr = cell3.Split((" - ").ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                if (arr.Length >= 1)
                                {
                                    low = DataTypeHelper.ToDecimal(arr[0].Trim());
                                }
                                if (arr.Length >= 2)
                                {
                                    high = DataTypeHelper.ToDecimal(arr[1].Trim());
                                }
                                break;
                            }
                            if (cell2.Contains("data-snapfield=\"range_52week\"") == true)
                            {
                                is52WeekStart = true;
                            }
                        }
                    }
                }
            }

            string sql = string.Format(" update tra_company set week_52_low={0},week_52_high={1} where symbol='{2}'", low, high, symbol);
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
            if (high <= 0 || low <= 0)
            {
                Helper.Log("Not update google finace value symbol=" + symbol, "NOT_UPDATE_GOOGLEFINANCE");
            }
        }

        public static void NSEIndia52WeekImport(string html)
        {
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
            html = html.Replace("\n", "").Replace("\r", "").Replace("\r\n", "").Replace("DIV", "div");
            if (html.Contains("futLink") == true)
            {
                int startIndex = html.IndexOf("{\"futLink\"");
                int endIndex = html.IndexOf("],\"optLink\"");
                int length = endIndex - startIndex + 8;

                html = html.Substring(startIndex, length);

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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
                        symbol = col.Groups["v"].Value;
                        symbol = symbol.Replace("&amp;", "&");
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
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
                foreach (Match col in collections)
                {
                    if (col.Groups.Count > 0)
                    {
                        prev = col.Groups["v"].Value;
                    }
                }

                if (string.IsNullOrEmpty(symbol) == false
                    && string.IsNullOrEmpty(series) == false)
                {
                    if (series != "EQ")
                    {
                        Helper.Log("Not EQ Series symbol=" + symbol, "NOT_EQ_SERIES_NSEINDIA_Week_52");
                        week_52_low = 0; week_52_high = 0;
                    }
                    DateTime dt = DataTypeHelper.ToDateTime(lastUpdateTime);
                    tempList.Add(new TempClass
                    {
                        symbol = symbol,
                        trade_type = "NSE",
                        trade_date = dt,
                        close_price = DataTypeHelper.ToDecimal(close),
                        high_price = DataTypeHelper.ToDecimal(high),
                        low_price = DataTypeHelper.ToDecimal(low),
                        open_price = DataTypeHelper.ToDecimal(open),
                        ltp_price = DataTypeHelper.ToDecimal(lastTrade),
                        prev_price = DataTypeHelper.ToDecimal(prev),
                    });
                    int rowIndex = 0;
                    foreach (var row in tempList)
                    {
                        rowIndex += 1;
                        //Console.WriteLine(" Rows " + rowIndex + " Of " + tempList.Count());
                        //lblTotalRecords.Text = " Import Price Rows " + rowIndex + " Of " + tempList.Count();
                        ImportPrice(row);
                    }
                    TradeHelper.UpdateCompanyPrice(symbol);
                    sql = string.Format(" update tra_company set week_52_low={0},week_52_high={1} where symbol='{2}'", week_52_low, week_52_high, symbol);
                    MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
                    if (week_52_high <= 0 || week_52_low <= 0)
                    {
                        Helper.Log("Not update nse india symbol=" + symbol, "NOT_Update_Week_52_SERIES_NSEINDIA_Week_52");
                    }
                }
            }
            else
            {
                Helper.Log("NO CONTENT in futLink", "futLink");
            }
        }

        public static void GoogleIndiaImport(string html, string symbol)
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
            string tblHTML = html;//.Substring(startIndex, length);
            tblHTML = tblHTML.Replace(" ", "");
            List<TempClass> tempList = new List<TempClass>();
            tblHTML = tblHTML.Replace("<tr>", "||");
            tblHTML = tblHTML.Replace("<td>", "~");
            tblHTML = tblHTML.Replace("<table>", "");
            tblHTML = tblHTML.Replace("</table>", "");
            string date = "";
            //string prev = "";
            string open = "";
            string high = "";
            string low = "";
            string lastTrade = "";
            string close = "";
            string tradeType = "NSE";
            string[] rows = tblHTML.Split(("||").ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {

                string[] cells = row.Split(("~").ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int index = 0;
                for (index = 0; index < cells.Length; index++)
                {
                    string value = cells[index].Trim();
                    switch (index)
                    {
                        case 0: date = value; break;
                        case 1: open = value; break;
                        case 2: high = value; break;
                        case 3: low = value; break;
                        case 4: close = value; break;
                    }
                }
                lastTrade = close;
                if (string.IsNullOrEmpty(date) == false
                  && string.IsNullOrEmpty(symbol) == false
                  )
                {
                    DateTime dt = DataTypeHelper.ToDateTime(date);
                    if (dt.Year > 1900)
                    {
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
                            //prev_price = DataTypeHelper.ToDecimal(prev),
                        });
                    }
                }
            }
            int rowIndex = 0;
            foreach (var temprow in tempList)
            {
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

        public static void NSEIndiaImport(string html)
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

        public static void UpdateCompanyPrice(string symbol)
        {
            #region Update Company Price
            using (EcamContext context = new EcamContext())
            {
                tra_company company = (from q in context.tra_company
                                       where q.symbol == symbol
                                       select q).FirstOrDefault();
                if (company != null)
                {
                    company.open_price = 0;
                    company.high_price = 0;
                    company.low_price = 0;
                    company.close_price = 0;
                    company.prev_price = 0;
                    company.ltp_price = 0;
                    tra_market lastMarket = (from q in context.tra_market
                                             where q.symbol == symbol
                                             orderby q.trade_date descending
                                             select q).FirstOrDefault();
                    if (lastMarket != null)
                    {
                        company.open_price = lastMarket.open_price;
                        company.high_price = lastMarket.high_price;
                        company.low_price = lastMarket.low_price;
                        company.close_price = lastMarket.close_price;
                        company.prev_price = lastMarket.prev_price;
                        company.ltp_price = lastMarket.ltp_price;
                    }
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

        private static decimal CalculateAVG(List<tra_market> markets, int dayCount)
        {
            int i = 0;
            //decimal total = 0;
            List<tra_market> temp = new List<tra_market>();
            foreach (var market in markets)
            {
                if (dayCount > i)
                {
                    //      total += (market.close_price ?? 0);
                    temp.Add(market);
                    i += 1;
                }
                else
                {
                    break;
                }
            }
            //decimal avg = DataTypeHelper.SafeDivision(total, dayCount);
            decimal value = 0;
            if (temp.Count > 0)
            {
                var tempRow = (from q in temp
                               orderby q.trade_date ascending
                               select q).FirstOrDefault();
                value = (tempRow.ltp_price ?? 0);
            }
            return value; // DataTypeHelper.SafeDivision((currentPrice - avg), avg) * 100;
        }

        public static void ImportPrice(TempClass import)
        {
            using (EcamContext context = new EcamContext())
            {
                import.symbol = import.symbol.Replace("&amp;", "&");
                var row = (from q in context.tra_market
                           where q.symbol == import.symbol
                           && q.trade_date == import.trade_date.Date
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
                row.trade_date = import.trade_date.Date;
                row.open_price = import.open_price;
                row.high_price = import.high_price;
                row.low_price = import.low_price;
                row.close_price = import.close_price;
                row.ltp_price = import.ltp_price;
                //row.prev_price = import.prev_price;
                //if ((row.prev_price ?? 0) <= 0)
                //{
                tra_market market = (from q in context.tra_market
                                     where q.symbol == import.symbol
                                     && q.trade_date < row.trade_date
                                     orderby q.trade_date descending
                                     select q).FirstOrDefault();
                if (market != null)
                {
                    row.prev_price = market.close_price;
                }
                //}

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
            UpdateCompanyPrice(import.symbol);
        }

        public static string RemoveHTMLTag(string html)
        {
            Regex regex = new Regex(
@"<[^>]*>",
RegexOptions.IgnoreCase
| RegexOptions.Multiline
| RegexOptions.IgnorePatternWhitespace
| RegexOptions.Compiled
);
            return regex.Replace(html, "");
        }

        public static void GetUpdatePriceUsingGoogle(string symbol)
        {
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

    public class GoogleDownloadData
    {
        public GoogleDownloadData(string symbol, ManualResetEvent doneEvent)
        {
            _Symbol = symbol;
            _doneEvent = doneEvent;
        }

        public GoogleDownloadData()
        {
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...", threadIndex);
            GoogleDataDownload(_Symbol);
            Console.WriteLine("thread {0} result calculated...", threadIndex);
            _doneEvent.Set();
        }

        public void GoogleDataDownload(string symbol)
        {
            if (string.IsNullOrEmpty(symbol) == true) { return; }
            Random rnd = new Random();
            string url = string.Empty;
            string html = string.Empty;
            string type = "NSE";
            string GOOGLE_DATA = System.Configuration.ConfigurationManager.AppSettings["GOOGLE_DATA"];
            WebClient client = new WebClient();

            DateTime morningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 9:15AM");
            DateTime morningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 10:15AM");
            DateTime eveningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:31PM");
            DateTime eveningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 11:59PM");
            DateTime now = DateTime.Now;
            url = string.Format("https://www.google.com/finance?q=NSE:{0}"
                                                                   , symbol.Replace("&", "%26")
                                                                   );
            string fileName = GOOGLE_DATA + "\\" + symbol + ".html";
            //bool isJSONAPI = false;
            //if ((now >= morningStart && now <= morningEnd))
            //{
            //    if (File.Exists(fileName) == true)
            //    {
            //        FileInfo fileInfo = new FileInfo(fileName);
            //        if (fileInfo.CreationTime < morningStart)
            //        {
            //            File.Delete(fileName);
            //        }
            //    }
            //    //if (File.Exists(fileName) == true)
            //    //{
            //    //    url = string.Format("http://finance.google.com/finance/info?client=ig&q={0}:{1}", type, symbol.Replace("&", "%26"));
            //    //    isJSONAPI = true;
            //    //}
            //}
            if ((now >= eveningStart && now <= eveningEnd))
            {
                if (File.Exists(fileName) == true)
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    if (fileInfo.CreationTime < eveningStart)
                    {
                        File.Delete(fileName);
                    }
                }
                if (File.Exists(fileName) == false)
                {
                    try
                    {
                        html = client.DownloadString(url);
                        File.WriteAllText(fileName, html);
                        Console.WriteLine("Download google data symbol evening=" + symbol);
                    }
                    catch
                    {
                        //Helper.Log("DownloadErrorOnGoogleData symbol=" + symbol, "ErrorOnGoogleData_" + rnd.Next(1000, 10000));
                    }
                }
                else
                {
                    html = File.ReadAllText(fileName);
                }
            }
            else
            {
                html = client.DownloadString(url);
                Console.WriteLine("Download google data symbol morning=" + symbol);
                if (File.Exists(fileName) == true)
                {
                    File.Delete(fileName);
                }
                //if (File.Exists(fileName) == false)
                //{
                //    try
                //    {
                //        html = client.DownloadString(url);
                //        //File.WriteAllText(fileName, html);
                //        Console.WriteLine("Download google data symbol morning=" + symbol);
                //    }
                //    catch
                //    {
                //        //Helper.Log("DownloadErrorOnGoogleData symbol=" + symbol, "ErrorOnGoogleData_" + rnd.Next(1000, 10000));
                //    }
                //}
                //else
                //{
                //    html = File.ReadAllText(fileName);
                //}
            }
            //else if (isJSONAPI == true)
            //{
            //    try
            //    {
            //        html = client.DownloadString(url);
            //        Console.WriteLine("Download google json api data symbol morning=" + symbol);
            //    }
            //    catch
            //    {
            //        //Helper.Log("DownloadErrorOnGoogleData symbol=" + symbol, "ErrorOnGoogleData_" + rnd.Next(1000, 10000));
            //    }
            //}
            //if (isJSONAPI == true)
            //{
            //    if (string.IsNullOrEmpty(html) == false)
            //    {
            //        string jsonTEXT = html;
            //        //string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //        //string sFile = appPath + "\\test.json";
            //        //string jsonTEXT = System.IO.File.ReadAllText(sFile);
            //        GetPrice(jsonTEXT);
            //    }
            //}
            //else
            //{
            if (string.IsNullOrEmpty(html) == false)
            {
                try
                {
                    string startWord = "<div id=market-data-div class=\"id-market-data-div nwp g-floatfix\">";
                    string endWord = "<div id=\"sharebox-data\"";
                    int startIndex = html.IndexOf(startWord);
                    int endIndex = html.IndexOf(endWord);
                    int length = endIndex - startIndex + endWord.Length;
                    if (startIndex > 0 && endIndex > 0)
                    {
                        html = html.Substring(startIndex, length);
                    }
                    else
                    {
                        Helper.Log("ErrorOnGoogleData symbol=" + symbol, "DownloadErrorOnGoogleData_" + rnd.Next(1000, 10000));
                    }
                    html = html.Replace("\n", "").Replace("\r", "").Replace("\r\n", "");

                    Regex regex = new Regex(
                        @"<span\s*class=\""pr\""(.*?)>(.*?)</span>",
                        RegexOptions.IgnoreCase
                        | RegexOptions.Multiline
                        | RegexOptions.IgnorePatternWhitespace
                        | RegexOptions.Compiled
                        );
                    MatchCollection collections = regex.Matches(html);

                    decimal currentPrice = 0;
                    decimal change = 0;
                    decimal prevPrice = 0;
                    DateTime tradeDate = DateTime.MinValue;
                    decimal openPrice = 0;
                    decimal lowPrice = 0;
                    decimal highPrice = 0;
                    decimal week52Low = 0;
                    decimal week52High = 0;
                    if (collections.Count > 0)
                    {
                        currentPrice = DataTypeHelper.ToDecimal(TradeHelper.RemoveHTMLTag(collections[0].Groups[2].Value));
                    }
                    regex = new Regex(
    @"<span\s*class=\""ch\s+bld\""(.*?)>(.*?)</span>",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );

                    collections = regex.Matches(html);
                    if (collections.Count > 0)
                    {
                        change = DataTypeHelper.ToDecimal(TradeHelper.RemoveHTMLTag(collections[0].Groups[2].Value));
                    }
                    prevPrice = currentPrice - change;

                    regex = new Regex(
    @"<span\s*class=nwp(.*?)>(.*?)</span>",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );
                    collections = regex.Matches(html);
                    if (collections.Count > 0)
                    {
                        try
                        {
                            string dt = TradeHelper.RemoveHTMLTag(collections[0].Groups[2].Value).Replace("- Close", "").Trim();
                            if (dt.Contains("Real-time") == false)
                            {
                                string[] arr = dt.Split((" ").ToCharArray());
                                string month = arr[0];
                                string date = arr[1];
                                tradeDate = DataTypeHelper.ToDateTime(date + "/" + month + "/" + DateTime.Now.Year);
                            }
                            else
                            {
                                tradeDate = DateTime.Now.Date;
                            }
                        }
                        catch (Exception ex)
                        {
                            //Helper.Log("Symbol=" + symbol + ",Google date parse=" + ex.Message, "GoogleDateParse_" + rnd.Next(1000, 10000));
                        }
                    }

                    regex = new Regex(
   @"<table(.*?)>(.*?)</table>",
   RegexOptions.IgnoreCase
   | RegexOptions.Multiline
   | RegexOptions.IgnorePatternWhitespace
   | RegexOptions.Compiled
   );
                    collections = regex.Matches(html);
                    if (collections.Count > 0)
                    {
                        string tableContent = collections[0].Groups[2].Value;

                        regex = new Regex(
    @"<tr(.*?)>(.*?)</tr>",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );
                        MatchCollection trCollections = regex.Matches(tableContent);
                        foreach (Match trMatch in trCollections)
                        {
                            string tr = trMatch.Value;
                            regex = new Regex(
   @"<td(.*?)>(.*?)</td>",
   RegexOptions.IgnoreCase
   | RegexOptions.Multiline
   | RegexOptions.IgnorePatternWhitespace
   | RegexOptions.Compiled
   );
                            MatchCollection tdCollections = regex.Matches(tr);

                            if (tdCollections.Count > 0)
                            {
                                string firstCell = TradeHelper.RemoveHTMLTag(tdCollections[0].Groups[2].Value).Trim();
                                string secondCell = TradeHelper.RemoveHTMLTag(tdCollections[1].Groups[2].Value).Trim();
                                string[] arr;
                                switch (firstCell)
                                {
                                    case "Range":
                                        arr = secondCell.Split(("-").ToCharArray());
                                        lowPrice = DataTypeHelper.ToDecimal(arr[0].Trim());
                                        highPrice = DataTypeHelper.ToDecimal(arr[1].Trim());
                                        break;
                                    case "52 week":
                                        arr = secondCell.Split(("-").ToCharArray());
                                        week52Low = DataTypeHelper.ToDecimal(arr[0].Trim());
                                        week52High = DataTypeHelper.ToDecimal(arr[1].Trim());
                                        break;
                                    case "Open":
                                        openPrice = DataTypeHelper.ToDecimal(secondCell);
                                        break;
                                }
                            }
                        }
                    }
                    if (currentPrice > 0 && tradeDate.Year > 1900)
                    {
                        if (tradeDate.Date == now.Date && (now >= morningStart && now <= morningEnd))
                        {
                            try
                            {
                                using (EcamContext context = new EcamContext())
                                {
                                    TempRSI value = new TempRSI
                                    {
                                        symbol = symbol,
                                        close = currentPrice,
                                        prev = 0,
                                        date = tradeDate.Date
                                    };
                                    var prev = (from q in context.tra_market
                                                where q.symbol == symbol
                                                && q.trade_date < tradeDate.Date
                                                orderby q.trade_date descending
                                                select q).FirstOrDefault();
                                    if (prev != null)
                                    {
                                        value.prev = (prev.close_price ?? 0);
                                        value.avg_downward = (((prev.avg_downward ?? 0) * (14 - 1) + value.downward) / 14);
                                        value.avg_upward = (((prev.avg_upward ?? 0) * (14 - 1) + value.upward) / 14);
                                    }
                                    context.tra_market_intra_day.Add(new tra_market_intra_day
                                    {
                                        symbol = symbol,
                                        ltp_price = currentPrice,
                                        trade_date = DateTime.Now,
                                        rsi = value.rsi,
                                    });
                                    context.SaveChanges();
                                }
                            }
                            catch { }
                        }
                        TradeHelper.ImportPrice(new TempClass
                        {
                            symbol = symbol,
                            high_price = highPrice,
                            low_price = lowPrice,
                            ltp_price = currentPrice,
                            close_price = currentPrice,
                            open_price = openPrice,
                            prev_price = prevPrice,
                            trade_date = tradeDate,
                            trade_type = "NSE"
                        });
                        string sql = string.Format(" update tra_company set week_52_low={0},week_52_high={1} where symbol='{2}'", week52Low, week52High, symbol);
                        MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
                        if (week52High <= 0 || week52Low <= 0)
                        {
                            //Helper.Log("GoogleException symbol 1=" + symbol, "GoogleException_" + rnd.Next(1000, 10000));
                        }
                        Console.WriteLine("Completed symbol=" + symbol);
                        //sql = string.Format(" update tra_company set is_book_mark=1 where ifnull(open_price,0)>=ifnull(high_price,0) and ifnull(open_price,0)>=ifnull(low_price,0)");
                        //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
                        //sql = string.Format(" update tra_company set is_book_mark=1 where ifnull(open_price,0)<=ifnull(high_price,0) and ifnull(open_price,0)<=ifnull(low_price,0)");
                        //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);

                        using (EcamContext context = new EcamContext())
                        {
                            var market = (from q in context.tra_market where q.symbol == symbol orderby q.trade_date descending select q).FirstOrDefault();
                            if (market != null)
                            {
                                var prev = (from q in context.tra_market
                                            where q.symbol == symbol
                                            && q.trade_date < market.trade_date
                                            orderby q.trade_date descending
                                            select q).FirstOrDefault();
                                TempRSI value = new TempRSI
                                {
                                    symbol = market.symbol,
                                    close = (market.close_price ?? 0),
                                    prev = (prev.close_price ?? 0),
                                    date = market.trade_date,
                                    id = market.id,
                                };
                                value.avg_downward = (((prev.avg_downward ?? 0) * (14 - 1) + value.downward) / 14);
                                value.avg_upward = (((prev.avg_upward ?? 0) * (14 - 1) + value.upward) / 14);
                                market.avg_upward = value.avg_upward;
                                market.avg_downward = value.avg_downward;
                                market.prev_rsi = prev.rsi;
                                market.rsi = value.rsi;
                                context.Entry(market).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                var company = (from q in context.tra_company where q.symbol == symbol select q).FirstOrDefault();
                                if (company != null)
                                {
                                    company.rsi = market.rsi;
                                    company.prev_rsi = market.prev_rsi;
                                    context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        //Helper.Log("GoogleException symbol 2=" + symbol, "GoogleException_" + rnd.Next(1000, 10000));
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                    //Helper.Log("GoogleException symbol 3=" + symbol, "GoogleException_" + rnd.Next(1000, 10000));
                }
                //  }
            }
        }

        public decimal GetPrice(string jsonTEXT)
        {
            decimal currentPrice = 0;
            try
            {
                jsonTEXT = jsonTEXT.Replace("\n", "").Replace(" ", "").Replace("//", "").Replace("[{", "{").Replace("}]", "}");
                //string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string sFile = appPath + "\\test.json";
                //string jsonTEXT = System.IO.File.ReadAllText(sFile);
                PriceDetailJSON json = JsonConvert.DeserializeObject<PriceDetailJSON>(jsonTEXT);
                DateTime dt = Convert.ToDateTime(json.lt_dts);
                string symbol = json.t;
                currentPrice = DataTypeHelper.ToDecimal(json.l_cur);
                //decimal prevPrice = 0;
                //DateTime tradeDate = dt.Date;
                //decimal openPrice = 0;
                //decimal lowPrice = 0;
                //decimal highPrice = 0;

                string sql = string.Format("update tra_company set ltp_price={0} where symbol='{1}'", currentPrice, symbol);
                MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
                /*
                tra_company company = null;
                using (EcamContext context = new EcamContext())
                {
                    company = (from q in context.tra_company
                               where q.symbol == symbol
                               select q).FirstOrDefault();
                }
                if (company != null)
                {
                    lowPrice = (company.low_price ?? 0);
                    highPrice = (company.high_price ?? 0);
                    prevPrice = (company.prev_price ?? 0); // currentPrice - (DataTypeHelper.ToDecimal(json.c) * -1);
                    if (lowPrice >= currentPrice)
                    {
                        lowPrice = currentPrice;
                    }
                    if (highPrice <= currentPrice)
                    {
                        highPrice = currentPrice;
                    }
                    TradeHelper.ImportPrice(new TempClass
                    {
                        symbol = symbol,
                        high_price = highPrice,
                        low_price = lowPrice,
                        ltp_price = currentPrice,
                        close_price = currentPrice,
                        open_price = openPrice,
                        prev_price = prevPrice,
                        trade_date = tradeDate,
                        trade_type = "NSE"
                    });
                }
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdatePrice ex=" + ex.Message);
            }
            return currentPrice;
        }

        public string SYMBOL { get { return _Symbol; } }
        private string _Symbol;

        private ManualResetEvent _doneEvent;
    }

    public class TempRSI
    {
        public int id { get; set; }
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
                return this.avg_upward / this.avg_downward;
            }
        }
        public decimal rsi {
            get {
                if (this.avg_downward == 0)
                    return 100;
                else
                    return (100 - (100 / (1 + this.rs)));
            }
        }
        public DateTime date { get; set; }
    }

    public class GoogleHistoryDownloadData
    {
        public GoogleHistoryDownloadData(string symbol, ManualResetEvent doneEvent)
        {
            _Symbol = symbol;
            _doneEvent = doneEvent;
        }

        public GoogleHistoryDownloadData()
        {

        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...", threadIndex);
            GoogleHistoryDataDownload(_Symbol);
            CalculateRSI(_Symbol);
            Console.WriteLine("thread {0} result calculated...", threadIndex);
            _doneEvent.Set();
        }

        private void GoogleHistoryDataDownload(string symbol)
        {
            string url = string.Empty;
            string html = string.Empty;
            string GOOGLE_HISTORY_DATA = System.Configuration.ConfigurationManager.AppSettings["GOOGLE_HISTORY_DATA"];
            WebClient client = new WebClient();
            int numberOfRows = DataTypeHelper.ToInt32(System.Configuration.ConfigurationManager.AppSettings["NUMBER_OF_ROWS"]);
            url = string.Format("https://www.google.com/finance/historical?q=NSE:{0}&num=" + numberOfRows
                                                                , symbol.Replace("&", "%26")
                                                                );
            string fileName = GOOGLE_HISTORY_DATA + "\\" + symbol + ".html";
            if (File.Exists(fileName) == false)
            {
                html = client.DownloadString(url);
                File.WriteAllText(fileName, html);
                Console.WriteLine("Download google data symbol=" + symbol);
            }
            else
            {
                html = File.ReadAllText(fileName);
            }
            if (string.IsNullOrEmpty(html) == false)
            {
                string startWord = "<table class=\"gf-table historical_price\">";
                string endWord = "google.finance.gce";
                int startIndex = html.IndexOf(startWord);
                int endIndex = html.IndexOf(endWord);
                int length = endIndex - startIndex + endWord.Length;
                if (startIndex > 0 && endIndex > 0)
                {
                    html = html.Substring(startIndex, length);
                }
                else
                {
                    Helper.Log("ErrorOnGoogleData symbol=" + symbol, "ErrorOnGoogleData");
                }
                startWord = "<table class=\"gf-table historical_price\">";
                endWord = "</table>";
                startIndex = html.IndexOf(startWord);
                endIndex = html.IndexOf(endWord);
                length = endIndex - startIndex + endWord.Length;
                if (startIndex >= 0 && endIndex > 0)
                {
                    string parseContent = html.Substring(startIndex, length);
                    TradeHelper.GoogleIndiaImport(parseContent, symbol);
                }
                else
                {
                    Helper.Log("ErrorOnGoogleData symbol=" + symbol, "ErrorOnGoogleData");
                }
            }
        }


        public static void CalculateRSI(string symbol)
        {
            using (EcamContext context = new EcamContext())
            {
                List<tra_company> companies = (from q in context.tra_company
                                               where q.symbol == symbol
                                               orderby q.company_name ascending
                                               select q).ToList();
                foreach (var company in companies)
                {
                    DateTime today = DateTime.Now.Date;
                    DateTime endDate = DateTime.Now.Date.AddDays(-120);
                    var markets = (from q in context.tra_market
                                   where q.symbol == company.symbol
                                   && q.trade_date <= today
                                   && q.trade_date >= endDate
                                   orderby q.trade_date ascending
                                   select q).ToList();
                    foreach (var market in markets)
                    {
                        CalculateRSI(markets, market.trade_date, market.id);
                    }
                    var updateMarket = (from q in context.tra_market
                                        where q.symbol == company.symbol
                                        orderby q.trade_date descending
                                        select q).FirstOrDefault();
                    if (updateMarket != null)
                    {
                        var prevMarket = (from q in context.tra_market
                                          where q.symbol == company.symbol
                                          && q.trade_date < updateMarket.trade_date
                                          orderby q.trade_date descending
                                          select q).FirstOrDefault();
                        if (prevMarket != null)
                        {
                            company.prev_rsi = prevMarket.rsi;
                        }
                        company.rsi = updateMarket.rsi;
                        context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
        }

        public static void CalculateRSI(List<tra_market> fullMarkets, DateTime date, int id)
        {
            List<tra_market> markets = (from q in fullMarkets
                                        where q.trade_date.Date <= date
                                        orderby q.trade_date descending
                                        select q).ToList();
            int total = 15;
            int i;
            List<TempRSI> values = new List<TempRSI>();
            if (markets.Count == 15)
            {
                string s = string.Empty;
            }
            if (markets.Count < total)
            {
                string s = string.Empty;
            }
            else
            {
                for (i = 0; i < total; i++)
                {
                    var market = markets[i];
                    values.Add(new TempRSI
                    {
                        id = market.id,
                        symbol = market.symbol,
                        date = market.trade_date,
                        avg_upward = (market.avg_upward ?? 0),
                        avg_downward = (market.avg_downward ?? 0),
                        prev = 0, // (market.prev_price ?? 0),
                        close = (market.close_price ?? 0)
                    });
                }
                values = (from q in values orderby q.date ascending select q).ToList();
                for (i = 0; i < total; i++)
                {
                    var value = values[i];
                    TempRSI prev = null;
                    if (i > 0)
                    {
                        prev = values[i - 1];
                    }
                    if (prev != null)
                    {
                        value.prev = prev.close;
                    }
                }
                values = (from q in values orderby q.date ascending select q).ToList();
                for (i = 0; i < total; i++)
                {
                    if (i == 14)
                    {
                        TempRSI value = values[i];
                        TempRSI prev = values[i - 1];

                        //var getPreviousMarket = (from q in markets
                        //                         where q.trade_date.Date < value.date
                        //                         select q).FirstOrDefault();
                        //value.change = value.close - (getPreviousMarket.close_price ?? 0);
                        int j;
                        int avgTotal = 15;

                        List<decimal> tempValues = null;
                        tempValues = new List<decimal>();
                        decimal calcTotal = 0;

                        if (prev.avg_upward <= 0)
                        {
                            for (j = 0; j < avgTotal; j++)
                            {
                                if (j > 0)
                                {
                                    tempValues.Add(values[j].upward);
                                    calcTotal += values[j].upward;
                                }
                            }
                            value.avg_upward = (calcTotal / tempValues.Count);
                        }
                        else
                        {
                            //= ((G17 * ($G$2 - 1)+E18)/$G$2)
                            value.avg_upward = ((prev.avg_upward * (14 - 1) + value.upward) / 14);
                        }

                        tempValues = new List<decimal>();
                        calcTotal = 0;

                        if (prev.avg_downward <= 0)
                        {
                            for (j = 0; j < avgTotal; j++)
                            {
                                if (j > 0)
                                {
                                    tempValues.Add(values[j].downward);
                                    calcTotal += values[j].downward;
                                }
                            }
                            value.avg_downward = (calcTotal / tempValues.Count);
                        }
                        else
                        {
                            //= ((H17 * ($H$2 - 1)+F18)/$H$2)
                            value.avg_downward = ((prev.avg_downward * (14 - 1) + value.downward) / 14);
                        }
                    }
                }

                TempRSI existValue = (from q in values
                                      where q.id == id
                                      select q).FirstOrDefault();
                if (existValue != null)
                {
                    using (EcamContext context = new EcamContext())
                    {
                        var market = (from q in context.tra_market where q.id == existValue.id && q.symbol == existValue.symbol select q).FirstOrDefault();
                        if (market != null)
                        {
                            var prevRecord = (from q in context.tra_market where q.symbol == existValue.symbol && q.trade_date < existValue.date orderby q.trade_date descending select q).FirstOrDefault();
                            if (prevRecord != null)
                            {
                                market.prev_rsi = prevRecord.rsi;
                            }
                            market.rsi = existValue.rsi;
                            market.upward = existValue.upward;
                            market.downward = existValue.downward;
                            market.avg_downward = existValue.avg_downward;
                            market.avg_upward = existValue.avg_upward;
                            market.rs = existValue.rs;
                            context.Entry(market).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            var preMarket = (from q in markets where q.symbol == existValue.symbol && q.id == existValue.id select q).FirstOrDefault();
                            preMarket.avg_downward = existValue.avg_downward;
                            preMarket.avg_upward = existValue.avg_upward;
                            preMarket.upward = existValue.upward;
                            preMarket.downward = existValue.downward;
                            preMarket.rs = existValue.rs;
                            preMarket.rsi = existValue.rsi;
                            Console.WriteLine("Update RSI Symbol=" + market.symbol + ",Date=" + market.trade_date.ToString("dd-MMM-yyy") + ",id=" + market.id + ",rsi=" + existValue.rsi);
                        }
                    }
                }
                //for (i = 0; i < total; i++)
                //{
                //    if (i > 12)
                //    {
                //        Console.WriteLine("i=" + i + ",Date=" + values[i].date.ToString("dd/MMM/yyyy") + ",RS=" + FormatHelper.NumberFormat(values[i].rs, 2) + ",RSI=" + FormatHelper.NumberFormat(values[i].rsi, 2));
                //    }
                //}
            }
        }

        public string SYMBOL { get { return _Symbol; } }
        private string _Symbol;

        private ManualResetEvent _doneEvent;
    }

    public class PriceDetailJSON
    {
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
}