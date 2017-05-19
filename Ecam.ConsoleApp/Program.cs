using Ecam.Framework;
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
        static void Main(string[] args)
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
                foreach(var company in companies)
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
            Console.WriteLine("Completed");
            Console.ReadLine();
        }

        private static void ParseMainHTML(string url)
        {
            //string fileName = "C:\\Users\\kart\\Desktop\\MF\\1.html";
            //string content = System.IO.File.ReadAllText(fileName);
            string content = "";
            WebClient client = new WebClient();
            content = client.DownloadString(url);
            if (string.IsNullOrEmpty(content) == false)
            {
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
                        if (i == 1)
                        {
                            tagName = "th";
                        }
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
                                    case 0: equity = value; break;
                                    case 1: sector = value; break;
                                    case 2: qty = value; break;
                                    case 3: totalValue = value; break;
                                    case 4: percentage = value; break;
                                }
                            }
                        }
                        string symbol = "";
                        if (string.IsNullOrEmpty(equity) == false)
                        {
                            symbol = GetSymbol(equity);
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
                                    Helper.Log("equity=" + equity + ",symbol=" + symbol, "Fund Does Not Exist");
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string GetSymbol(string equity)
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
                    string href = "http://www.moneycontrol.com/" + collections[0].Groups[1].Value;
                    WebClient client = new WebClient();
                    string content = string.Empty;
                    //string fileName = "C:\\Users\\kart\\Desktop\\MF\\relianceindustries_RI.html";
                    content = client.DownloadString(href);
                    Console.WriteLine("Download equity=" + href);
                    //content = System.IO.File.ReadAllText(fileName);
                    regex = new Regex(
        @"NSE:(?<symbol>\s*\w*)",
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
