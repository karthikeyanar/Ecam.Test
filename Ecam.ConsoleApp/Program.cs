﻿using Ecam.Framework;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Ecam.ConsoleApp
{
    class Program
    {
        private static string IS_DOWNLOAD_HISTORY = "";
        static void Main(string[] args)
        {
            IS_DOWNLOAD_HISTORY = System.Configuration.ConfigurationManager.AppSettings["IS_DOWNLOAD_HISTORY"];
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                companies = (from q in context.tra_company orderby q.symbol ascending select q).ToList();
            }
            if (IS_DOWNLOAD_HISTORY == "true")
            {
                GoogleHistoryData();
            }
            else
            {
                GoogleData();
            }
            Console.WriteLine("Completed");
            Console.ReadLine();
        }

        private static void GoogleData()
        {
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                companies = (from q in context.tra_company orderby q.symbol ascending select q).ToList();
            }
            string url = string.Empty;
            string html = string.Empty;
            string GOOGLE_DATA = System.Configuration.ConfigurationManager.AppSettings["GOOGLE_DATA"];
            WebClient client = new WebClient();
            foreach (var company in companies)
            {
                url = string.Format("https://www.google.com/finance?q=NSE:{0}"
                                                                    , company.symbol.Replace("&", "%26")
                                                                    );
                string fileName = GOOGLE_DATA + "\\" + company.symbol + ".html";
                if (File.Exists(fileName) == false)
                {
                    html = client.DownloadString(url);
                    File.WriteAllText(fileName, html);
                    Console.WriteLine("Download google data symbol=" + company.symbol);
                }
                else
                {
                    html = File.ReadAllText(fileName);
                }
                if (string.IsNullOrEmpty(html) == false)
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
                        Helper.Log("ErrorOnGoogleData symbol=" + company.symbol, "ErrorOnGoogleData");
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
                    decimal lowPrice = 0;
                    decimal highPrice = 0;

                    if (collections.Count > 0)
                    {
                        currentPrice = DataTypeHelper.ToDecimal(RemoveHTMLTag(collections[0].Groups[2].Value));
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
                        change = DataTypeHelper.ToDecimal(RemoveHTMLTag(collections[0].Groups[2].Value));
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
                            string dt = RemoveHTMLTag(collections[0].Groups[2].Value).Replace("- Close", "").Trim();
                            string[] arr = dt.Split((" ").ToCharArray());
                            string month = arr[0];
                            string date = arr[1];
                            tradeDate = DataTypeHelper.ToDateTime(date + "/" + month + "/" + DateTime.Now.Year);
                        }
                        catch (Exception ex)
                        {
                            Helper.Log("Symbol=" + company.symbol + ",Google date parse=" + ex.Message, "GoogleDateParse");
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
                                string firstCell = RemoveHTMLTag(tdCollections[0].Groups[2].Value).Trim();
                                string secondCell = RemoveHTMLTag(tdCollections[1].Groups[2].Value).Trim();
                                string[] arr;
                                switch (firstCell)
                                {
                                    case "Range":
                                        arr = secondCell.Split(("-").ToCharArray());
                                        lowPrice = DataTypeHelper.ToDecimal(arr[0].Trim());
                                        highPrice = DataTypeHelper.ToDecimal(arr[1].Trim());
                                        break;
                                }
                            }

                        }
                    }
                    //startWord = "<table class=\"gf-table historical_price\">";
                    //endWord = "</table>";
                    //startIndex = html.IndexOf(startWord);
                    //endIndex = html.IndexOf(endWord);
                    //length = endIndex - startIndex + endWord.Length;
                    //if (startIndex >= 0 && endIndex > 0)
                    //{

                    //    //TradeHelper.GoogleIndiaImport(parseContent, company.symbol);
                    //}
                    //else
                    //{
                    //    Helper.Log("ErrorOnGoogleData symbol=" + company.symbol, "ErrorOnGoogleData");
                    //}
                }
            }
        }

        private static string RemoveHTMLTag(string html)
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

        private static void GoogleHistoryData()
        {
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                companies = (from q in context.tra_company orderby q.symbol ascending select q).ToList();
            }
            string url = string.Empty;
            string html = string.Empty;
            string GOOGLE_HISTORY_DATA = System.Configuration.ConfigurationManager.AppSettings["GOOGLE_HISTORY_DATA"];
            WebClient client = new WebClient();
            foreach (var company in companies)
            {
                url = string.Format("https://www.google.com/finance/historical?q=NSE:{0}&num=10"
                                                                    , company.symbol.Replace("&", "%26")
                                                                    );
                string fileName = GOOGLE_HISTORY_DATA + "\\" + company.symbol + ".html";
                if (File.Exists(fileName) == false)
                {
                    html = client.DownloadString(url);
                    File.WriteAllText(fileName, html);
                    Console.WriteLine("Download google data symbol=" + company.symbol);
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
                        Helper.Log("ErrorOnGoogleData symbol=" + company.symbol, "ErrorOnGoogleData");
                    }
                    startWord = "<table class=\"gf-table historical_price\">";
                    endWord = "</table>";
                    startIndex = html.IndexOf(startWord);
                    endIndex = html.IndexOf(endWord);
                    length = endIndex - startIndex + endWord.Length;
                    if (startIndex >= 0 && endIndex > 0)
                    {
                        string parseContent = html.Substring(startIndex, length);
                        TradeHelper.GoogleIndiaImport(parseContent, company.symbol);
                    }
                    else
                    {
                        Helper.Log("ErrorOnGoogleData symbol=" + company.symbol, "ErrorOnGoogleData");
                    }
                }
            }
        }

        private static void MutualFunds()
        {
            string linkFileName = System.Configuration.ConfigurationManager.AppSettings["LINK_FILE_NAME"];
            if (string.IsNullOrEmpty(linkFileName) == false)
            {
                string content = System.IO.File.ReadAllText(linkFileName);
                string[] arr = content.Split(("\r\n").ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string url in arr)
                {
                    if (string.IsNullOrEmpty(url) == false)
                    {
                        ParseMainHTML(url);
                    }
                }
            }
            using (EcamContext context = new EcamContext())
            {
                var companies = (from q in context.tra_company select q).ToList();
                var funds = (from q in context.tra_mutual_fund_pf select q).ToList();
                foreach (var company in companies)
                {
                    company.mf_cnt = (from q in funds
                                      where q.symbol == company.symbol
                                      select q.fund_id).Count();
                    company.mf_qty = (from q in funds
                                      where q.symbol == company.symbol
                                      select q.quantity).Sum();
                    context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
            }
        }

        private static void ParseMainHTML(string url)
        {
            //string fileName = "C:\\Users\\kart\\Desktop\\MF\\1.html";
            //string content = System.IO.File.ReadAllText(fileName);
            string MF_HTML_PATH = System.Configuration.ConfigurationManager.AppSettings["MF_HTML_PATH"];
            string fundCode = url.Replace("http://www.moneycontrol.com/india/mutualfunds/mfinfo/portfolio_holdings/", "");
            string fileName = MF_HTML_PATH + "\\" + fundCode + ".html";
            string content = "";
            if (File.Exists(fileName) == true)
            {
                content = System.IO.File.ReadAllText(fileName);
            }
            else
            {
                WebClient client = new WebClient();
                content = client.DownloadString(url);
                System.IO.File.WriteAllText(fileName, content);
            }
            if (string.IsNullOrEmpty(content) == false)
            {
                Console.WriteLine("ParseMainHTML url=" + url);
                string startWord = "<table width=\"100%\" class=\"tblporhd\">";
                string endWord = "<table width=\"100%\" class=\"tblporhd MT25\">";
                int startIndex = content.IndexOf(startWord);
                int endIndex = content.IndexOf(endWord);
                int length = endIndex - startIndex + endWord.Length;
                if (startIndex > 0 && endIndex > 0)
                {
                    string parseContent = content.Substring(startIndex, length);
                    endWord = "</table>";
                    startIndex = parseContent.IndexOf(startWord);
                    endIndex = parseContent.IndexOf(endWord);
                    length = endIndex - startIndex + endWord.Length;
                    parseContent = parseContent.Substring(startIndex, length);
                    Regex regex = null;

                    string fundName = string.Empty;
                    tra_mutual_fund fund = null;
                    regex = new Regex(
            @"<h1\s*style\=\""font-size:30px;\"">(?<fund_name>.*)</h1>",
            RegexOptions.IgnoreCase
            | RegexOptions.Multiline
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
            );
                    MatchCollection collections = regex.Matches(content);
                    if (collections.Count > 0)
                    {
                        fundName = collections[0].Groups["fund_name"].Value;
                    }
                    if (string.IsNullOrEmpty(fundName) == false)
                    {
                        using (EcamContext context = new EcamContext())
                        {
                            fund = (from q in context.tra_mutual_fund
                                    where q.fund_name == fundName
                                    select q).FirstOrDefault();
                            if (fund == null)
                            {
                                fund = new tra_mutual_fund();
                                fund.fund_name = fundName;
                                context.tra_mutual_fund.Add(fund);
                                context.SaveChanges();
                            }
                            var rows = (from q in context.tra_mutual_fund_pf
                                        where q.fund_id == fund.id
                                        select q).ToList();
                            foreach (var row in rows)
                            {
                                context.tra_mutual_fund_pf.Remove(row);
                            }
                            context.SaveChanges();
                        }
                    }
                    regex = new Regex(
         @"<tr>(.*?)</tr>",
         RegexOptions.IgnoreCase
         | RegexOptions.Multiline
         | RegexOptions.IgnorePatternWhitespace
         | RegexOptions.Compiled
         );

                    MatchCollection trCollections = regex.Matches(parseContent);
                    int i = 0;
                    foreach (Match trMatch in trCollections)
                    {
                        i += 1;
                        string tr = trMatch.Value;
                        string tagName = "td";

                        regex = new Regex(
                                    @"<" + tagName + "[^>]*>(.+?)</" + tagName + ">",
                                    RegexOptions.IgnoreCase
                                    | RegexOptions.Multiline
                                    | RegexOptions.IgnorePatternWhitespace
                                    | RegexOptions.Compiled
                                    );
                        MatchCollection rowMatches = regex.Matches(tr);
                        string equity = "";
                        string sector = "";
                        string qty = "";
                        string totalValue = "";
                        string percentage = "";

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
                                case 0: equity = value; break;
                                case 1: sector = value; break;
                                case 2: qty = value; break;
                                case 3: totalValue = value; break;
                                case 4: percentage = value; break;
                            }

                        }
                        string symbol = "";
                        string symbolName = "";
                        if (string.IsNullOrEmpty(equity) == false)
                        {
                            regex = new Regex(
                                    @"<[^>].+?>",
                                    RegexOptions.IgnoreCase
                                    | RegexOptions.Multiline
                                    | RegexOptions.IgnorePatternWhitespace
                                    | RegexOptions.Compiled
                                    );
                            symbolName = regex.Replace(equity, "").Trim();
                            if (string.IsNullOrEmpty(symbolName) == false)
                            {
                                using (EcamContext context = new EcamContext())
                                {
                                    tra_mutual_fund_pf temp = (from q in context.tra_mutual_fund_pf
                                                               where q.stock_name == symbolName
                                                               select q).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        symbol = temp.symbol;
                                    }
                                    if (string.IsNullOrEmpty(symbol) == false)
                                    {
                                        tra_company company = (from q in context.tra_company
                                                               where q.symbol == symbol
                                                               select q).FirstOrDefault();
                                        if (company == null)
                                        {
                                            symbol = string.Empty;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Symbol already get symbol=" + symbol);
                                        }
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(symbol) == true)
                            {
                                symbol = GetSymbol(equity, symbolName);
                            }
                        }
                        if (string.IsNullOrEmpty(symbol) == false
                            && fund != null)
                        {
                            using (EcamContext context = new EcamContext())
                            {
                                tra_company company = (from q in context.tra_company
                                                       where q.symbol == symbol
                                                       select q).FirstOrDefault();
                                if (company != null)
                                {
                                    tra_mutual_fund_pf pf = (from q in context.tra_mutual_fund_pf
                                                             where q.fund_id == fund.id
                                                             && q.symbol == symbol
                                                             select q).FirstOrDefault();
                                    bool isNew = false;
                                    if (pf == null)
                                    {
                                        pf = new tra_mutual_fund_pf();
                                        isNew = true;
                                    }
                                    pf.symbol = symbol;
                                    pf.fund_id = fund.id;
                                    pf.quantity = DataTypeHelper.ToDecimal(qty);
                                    pf.stock_value = DataTypeHelper.ToDecimal(totalValue);
                                    pf.stock_percentage = DataTypeHelper.ToDecimal(percentage);
                                    pf.stock_name = symbolName;
                                    if (isNew == true)
                                    {
                                        context.tra_mutual_fund_pf.Add(pf);
                                    }
                                    else
                                    {
                                        context.Entry(pf).State = System.Data.Entity.EntityState.Modified;
                                    }
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Helper.Log("equity=" + equity + ",symbol=" + symbol, "FundDoesNotExist");
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string GetSymbol(string equity, string symbolName)
        {
            string symbol = "";
            try
            {
                Regex regex = new Regex(
        @"href=[""|'](.*?)[""|']",
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
                MatchCollection collections = regex.Matches(equity);
                if (collections.Count > 0)
                {
                    string content = string.Empty;
                    string HTML_PATH = System.Configuration.ConfigurationManager.AppSettings["HTML_PATH"];
                    string fileName = HTML_PATH + "\\" + symbolName + ".html";
                    if (File.Exists(fileName) == true)
                    {
                        content = System.IO.File.ReadAllText(fileName);
                        Console.WriteLine("Process File Text=" + symbolName);
                    }
                    else
                    {
                        string href = "http://www.moneycontrol.com/" + collections[0].Groups[1].Value;
                        WebClient client = new WebClient();
                        content = client.DownloadString(href);
                        System.IO.File.WriteAllText(fileName, content);
                        Console.WriteLine("Download equity=" + href);
                    }
                    //string fileName = "C:\\Users\\kart\\Desktop\\MF\\relianceindustries_RI.html";
                    //content = System.IO.File.ReadAllText(fileName);
                    regex = new Regex(
@"NSE:(?<symbol>\s*\w*&*\w*)",
RegexOptions.IgnoreCase
| RegexOptions.Multiline
| RegexOptions.IgnorePatternWhitespace
| RegexOptions.Compiled
);
                    collections = regex.Matches(content);
                    if (collections.Count > 0)
                    {
                        symbol = collections[0].Groups[1].Value.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += ex.InnerException.Message;
                }
                Console.WriteLine("GetSymbol_Error=" + msg);
                Helper.Log(msg, "GetSymbol_Error");
            }
            return symbol;
        }
    }
}
