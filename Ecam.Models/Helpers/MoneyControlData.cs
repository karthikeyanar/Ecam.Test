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

namespace Ecam.Models
{
    public class MoneyControlData
    {
        public MoneyControlData(string url, ManualResetEvent doneEvent)
        {
            _URL = url;
            _doneEvent = doneEvent;
        }

        public MoneyControlData()
        {
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...", threadIndex);
            if (string.IsNullOrEmpty(_URL) == false)
            {
                MoneyControlDataDownload(_URL);
            }
            Console.WriteLine("thread {0} result calculated...", threadIndex);
            _doneEvent.Set();
        }

        public void MoneyControlDataDownload(string url)
        {
            string MONEY_CONTROL = System.Configuration.ConfigurationManager.AppSettings["MONEY_CONTROL"];
            Regex regex = null;
            string[] arr;
            WebClient webClient = new WebClient();
            string companyName = "";
            string symbol = "";
            string categoryName = "";

            arr = url.Split(("//").ToCharArray());
            string name = arr[arr.Length - 1];
            string dir = System.IO.Path.Combine(MONEY_CONTROL, "equities");
            if (System.IO.Directory.Exists(dir) == false)
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            string fileName = dir + "\\" + name + ".html";
            string rootHTML = string.Empty;
            if (System.IO.File.Exists(fileName) == false)
            {
                try
                {
                    rootHTML = webClient.DownloadString(url);
                    System.IO.File.WriteAllText(fileName, rootHTML);
                }
                catch (Exception ex)
                {
                    //Helper.Log(url + Environment.NewLine, "MoneyControlDataDownload_Error");
                    //Helper.Log(ex.Message, "MoneyControlDataDownload_Error");
                }
            }
            else
            {
                rootHTML = System.IO.File.ReadAllText(fileName);
            }

            if (string.IsNullOrEmpty(rootHTML) == false)
            {
                string html = string.Empty;

                regex = new Regex(
@"<h1(.*?)>(.*?)</h1>",
RegexOptions.IgnoreCase
| RegexOptions.Multiline
| RegexOptions.IgnorePatternWhitespace
| RegexOptions.Compiled
);

                MatchCollection collections = regex.Matches(rootHTML);
                if (collections.Count >= 1)
                {
                    companyName = TradeHelper.RemoveHTMLTag(collections[0].Value).Trim();
                }

                string startWord = "<div class=\"FL gry10\">";
                string endWord = "<div id=\"mcpcp_addportfolio\"";
                int startIndex = rootHTML.IndexOf(startWord);
                int endIndex = rootHTML.IndexOf(endWord);
                int length = endIndex - startIndex + endWord.Length;
                if (startIndex > 0 && endIndex > 0)
                {
                    html = rootHTML.Substring(startIndex, length);

                    string value = TradeHelper.RemoveHTMLTag(html);
                    arr = value.Split(("|").ToArray());
                    foreach (string row in arr)
                    {
                        string[] cells = row.Split((":").ToCharArray());
                        if (cells.Length >= 2)
                        {
                            if (string.IsNullOrEmpty(cells[0]) == false)
                            {
                                cells[0] = cells[0].Trim();
                            }
                            if (string.IsNullOrEmpty(cells[1]) == false)
                            {
                                cells[1] = cells[1].Trim();
                            }
                            if (cells[0] == "NSE")
                            {
                                symbol = cells[1];
                            }
                            else if (cells[0] == "SECTOR")
                            {
                                categoryName = cells[1];
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(companyName) == false
                    && string.IsNullOrEmpty(symbol) == false
                    && string.IsNullOrEmpty(categoryName) == false)
                {
                    using (EcamContext context = new EcamContext())
                    {
                        tra_category category = (from q in context.tra_category
                                                 where q.category_name == categoryName
                                                 select q).FirstOrDefault();
                        if (category == null)
                        {
                            context.tra_category.Add(new tra_category
                            {
                                category_name = categoryName
                            });
                            context.SaveChanges();
                        }
                        tra_company company = (from q in context.tra_company
                                               where q.symbol == symbol
                                               select q).FirstOrDefault();
                        if (company == null)
                        {
                            company = new tra_company
                            {
                                symbol = symbol,
                                company_name = companyName,
                            };
                            context.tra_company.Add(company);
                            context.SaveChanges();
                        }
                        else
                        {
                            company.money_control_url = url;
                            context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        tra_company_category companyCategory = (from q in context.tra_company_category
                                                                where q.symbol == symbol
                                                                && q.category_name == categoryName
                                                                select q).FirstOrDefault();
                        if (companyCategory == null)
                        {
                            context.tra_company_category.Add(new tra_company_category
                            {
                                category_name = categoryName,
                                symbol = symbol
                            });
                            context.SaveChanges();
                        }
                    }
                    Console.WriteLine("Company=" + companyName + ",Symbol=" + symbol + ",Category=" + categoryName);
                }
            }
        }

        public string URL { get { return _URL; } }
        private string _URL;

        private ManualResetEvent _doneEvent;
    }
}