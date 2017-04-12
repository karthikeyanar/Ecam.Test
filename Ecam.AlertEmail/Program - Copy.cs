using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;
using Ecam.Framework;
using Ecam.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using CsvHelper;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Ecam.AlertEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            string is_import_companies = System.Configuration.ConfigurationManager.AppSettings["is_import_companies"];
            if (is_import_companies == "true")
            {
                ImportCompanies();
            }
            string import_type = System.Configuration.ConfigurationManager.AppSettings["import_type"];
            if (import_type == "nseindia")
            {
                NSEIndiaImport();
            }
            else if (import_type == "yahoo")
            {
                string is_ignore_download = System.Configuration.ConfigurationManager.AppSettings["is_ignore_download"];
                if (is_ignore_download != "true")
                {
                    DownloadPrice();
                }
                string is_ignore_import_price = System.Configuration.ConfigurationManager.AppSettings["is_ignore_import_price"];
                if (is_ignore_import_price != "true")
                {
                    ReadDownloadCSV_ImportPrice();
                }
                string is_ignore_calculated_price = System.Configuration.ConfigurationManager.AppSettings["is_ignore_calculated_price"];
                if (is_ignore_calculated_price != "true")
                {
                    ReadDownloadCSV_CalculatedPrice();
                }
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        #region NSEINDIA Import
        private static void NSEIndiaImport()
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string dirPath = appPath + "\\DownloadHTML";
                if (System.IO.Directory.Exists(dirPath) == true)
                {
                    string[] files = System.IO.Directory.GetFiles(dirPath);
                    Console.WriteLine("NSEIndiaImport Total=" + files.Count());
                    int i = 0;
                    foreach (string fullFileName in files)
                    {
                        i += 1;
                        string fileName = System.IO.Path.GetFileName(fullFileName);
                        string html = System.IO.File.ReadAllText(fullFileName);
                        string symbol = fileName.Replace(".html", "");
                        NSEIndiaImport(symbol, html);
                        System.IO.File.Delete(fullFileName);
                        Console.WriteLine("NSEIndiaImport Completed symbol=" + symbol + ",i=" + i);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("NSEIndiaImport 1 ex=" + ex.Message);
                Helper.Log("NSEIndiaImport 1 ex =" + ex.Message, "ERROR");
            }
        }

        private static void NSEIndiaImport(string symbol, string html)
        {
            try
            {
                html = html.Replace("\n", "").Replace("\r", "").Replace("\r\n", "").Replace("TABLE", "table").Replace("TR", "tr").Replace("TD", "td").Replace("TH", "th").Replace("TBODY", "tbody");
                int startIndex = html.IndexOf("<table>");
                int endIndex = html.IndexOf("</table>");
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
                                case 0: date = value; break;
                                case 1: originalSymbol = value; break;
                                case 2: series = value; break;
                                case 3: open = value; break;
                                case 4: high = value; break;
                                case 5: low = value; break;
                                case 6: lastTrade = value; break;
                                case 7: close = value; break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(date) == false
                        && string.IsNullOrEmpty(originalSymbol) == false
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
                        });
                    }
                }
                if (tempList.Count() <= 0)
                {
                    Helper.Log("NSEIndiaImport Temp List No Records Found Symbol =" + symbol, "ERROR");
                }
                foreach (var row in tempList)
                {
                    ImportPrice(row);
                }
                DateTime startDate = DateTime.Now.AddYears(-2);
                DateTime endDate = DateTime.Now.Date;
                using (EcamContext context = new EcamContext())
                {
                    _Markets = (from q in context.tra_market
                                where q.trade_date >= startDate
                                && q.trade_date <= endDate
                                && q.symbol == symbol
                                select q).ToList();
                }
                foreach (var row in tempList)
                {
                    CalculatedPrice(row);
                }
                UpdateCompanyPrice(symbol);
            }
            catch (Exception ex)
            {
                Console.WriteLine("NSEIndiaImport symbol=" + symbol +",ex=" + ex.Message);
                Helper.Log("NSEIndiaImport symbol=" + symbol + ",ex =" + ex.Message, "ERROR");
            }
        }
        #endregion

        private static void DownloadPrice()
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string dirPath = appPath + "\\DownloadCSV";
                int noOfDays = DataTypeHelper.ToInt32(System.Configuration.ConfigurationManager.AppSettings["no_of_days"]);
                Console.WriteLine("NoOfDays=" + noOfDays);
                DateTime startDate = DateTime.Now.Date.AddDays(-noOfDays);
                DateTime endDate = DateTime.Now.Date;
                Console.WriteLine("StartDate=" + startDate.ToString());
                Console.WriteLine("EndDate=" + endDate.ToString());
                if (System.IO.Directory.Exists(dirPath) == false)
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                if (System.IO.Directory.Exists(dirPath) == true)
                {
                    string[] files = System.IO.Directory.GetFiles(dirPath);
                    foreach (string fullFileName in files)
                    {
                        System.IO.File.Delete(fullFileName);
                    }
                }
                if (System.IO.Directory.Exists(dirPath) == true)
                {
                    List<tra_company> companies;
                    using (EcamContext context = new EcamContext())
                    {
                        companies = (from q in context.tra_company
                                     select q).ToList();
                    }
                    foreach (var company in companies)
                    {
                        string fileName = dirPath + "\\" + company.symbol + ".csv";
                        string url = string.Format("http://chart.finance.yahoo.com/table.csv?s={0}.NS&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}&g=d&ignore=.csv"
                                                                                                , company.symbol
                                                                                                , startDate.Month - 1
                                                                                                , startDate.Day
                                                                                                , startDate.Year
                                                                                                , endDate.Month - 1
                                                                                                , endDate.Day
                                                                                                , endDate.Year);
                        Console.WriteLine("Company Name=" + company.company_name);
                        Console.WriteLine("url=" + url);
                        if (System.IO.File.Exists(fileName) == true)
                        {
                            System.IO.File.Delete(fileName);
                        }
                        try
                        {
                            WebClient webClient = new WebClient();
                            byte[] bytes = webClient.DownloadData(url);
                            System.IO.File.WriteAllBytes(fileName, bytes);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("WebClient Download ex=" + ex.Message);
                            Helper.Log("WebClient Download symbol=" + company.symbol + ",ex =" + ex.Message, "ERROR");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DownloadPrice ex=" + ex.Message);
                Helper.Log("DownloadPrice ex =" + ex.Message, "ERROR");
            }
        }

        private static List<tra_market> _Markets = null;

        private static void ReadDownloadCSV_ImportPrice()
        {
            string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string dirPath = appPath + "\\DownloadCSV";
            if (System.IO.Directory.Exists(dirPath) == true)
            {
                string[] files = System.IO.Directory.GetFiles(dirPath);
                Console.WriteLine("ReadDownloadCSV_ImportPrice Total=" + files.Count());
                int i = 0;
                foreach (string fullFileName in files)
                {
                    i += 1;
                    string fileName = System.IO.Path.GetFileName(fullFileName);
                    string symbol = fileName.Replace(".csv", "");
                    ImportPrice(symbol);
                    Console.WriteLine("ImportPrice Completed symbol=" + symbol + ",i=" + i);
                }
            }
        }

        private static void ReadDownloadCSV_CalculatedPrice()
        {
            DateTime startDate = DateTime.Now.AddYears(-2);
            DateTime endDate = DateTime.Now.Date;
            using (EcamContext context = new EcamContext())
            {
                _Markets = (from q in context.tra_market
                            where q.trade_date >= startDate
                            && q.trade_date <= endDate
                            select q).ToList();
            }
            string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string dirPath = appPath + "\\DownloadCSV";
            if (System.IO.Directory.Exists(dirPath) == true)
            {
                string[] files = System.IO.Directory.GetFiles(dirPath);
                Console.WriteLine("ReadDownloadCSV_ImportPrice Total=" + files.Count());
                int i = 0;
                foreach (string fullFileName in files)
                {
                    i += 1;
                    string fileName = System.IO.Path.GetFileName(fullFileName);
                    string symbol = fileName.Replace(".csv", "");
                    CalculatedPrice(symbol);
                    System.IO.File.Delete(fullFileName);
                    Console.WriteLine("CalculatedPrice Completed symbol=" + symbol + ",i=" + i);
                }
            }
        }

        private static void ImportPrice(string symbol)
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string sFile = appPath + "\\DownloadCSV\\" + symbol + ".csv";
                CsvReader csv = null;
                int i = 0;
                #region UpdatePrice
                using (TextReader reader = File.OpenText(sFile))
                {
                    csv = new CsvReader(reader);
                    i = 0;
                    while (csv.Read())
                    {
                        i += 1;
                        string date = csv.GetField<string>("Date");
                        string open = csv.GetField<string>("Open");
                        string high = csv.GetField<string>("High");
                        string low = csv.GetField<string>("Low");
                        string close = csv.GetField<string>("Close");
                        string tradeType = "NSE";
                        DateTime dt = DataTypeHelper.ToDateTime(date);

                        if (dt.Year < 2016)
                        {
                            continue;
                        }
                        ImportPrice(new TempClass
                        {
                            symbol = symbol,
                            trade_type = tradeType,
                            trade_date = dt,
                            close_price = DataTypeHelper.ToDecimal(close),
                            high_price = DataTypeHelper.ToDecimal(high),
                            low_price = DataTypeHelper.ToDecimal(low),
                            open_price = DataTypeHelper.ToDecimal(open),
                        });
                    }
                }
                #endregion
                //Console.WriteLine("ImportPrice Completed symbol=" + symbol);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ImportPrice ex=" + ex.Message);
                Helper.Log("ImportPrice symbol=" + symbol + ",ex =" + ex.Message, "ERROR");
            }
        }

        private static void CalculatedPrice(string symbol)
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string sFile = appPath + "\\DownloadCSV\\" + symbol + ".csv";
                CsvReader csv = null;
                int i = 0;
                #region UpdatePrice2
                i = 0;
                using (TextReader reader = File.OpenText(sFile))
                {
                    csv = new CsvReader(reader);
                    while (csv.Read())
                    {
                        i += 1;
                        string date = csv.GetField<string>("Date");
                        string open = csv.GetField<string>("Open");
                        string high = csv.GetField<string>("High");
                        string low = csv.GetField<string>("Low");
                        string close = csv.GetField<string>("Close");
                        string tradeType = "NSE";
                        DateTime dt = DataTypeHelper.ToDateTime(date);
                        CalculatedPrice(new TempClass
                        {
                            symbol = symbol,
                            trade_type = tradeType,
                            trade_date = dt,
                            close_price = DataTypeHelper.ToDecimal(close),
                            high_price = DataTypeHelper.ToDecimal(high),
                            low_price = DataTypeHelper.ToDecimal(low),
                            open_price = DataTypeHelper.ToDecimal(open),
                        });
                    }
                }
                #endregion
                UpdateCompanyPrice(symbol);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CalculatedPrice ex=" + ex.Message);
                Helper.Log("CalculatedPrice symbol=" + symbol + ",ex =" + ex.Message, "ERROR");
            }
        }

        private static void ImportPrice(TempClass import)
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
                if (isNew == true)
                {
                    context.tra_market.Add(row);
                }
                else
                {
                    context.Entry(row).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
                Console.WriteLine("ImportPrice Price Index symbol=" + row.symbol + ",Date=" + row.trade_date);
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
        private static void UpdateCompanyPrice(string symbol)
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

        private static void ImportCompanies()
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string import_company_csv_file_name = System.Configuration.ConfigurationManager.AppSettings["import_company_csv_file_name"];
                string sFile = appPath + "\\" + import_company_csv_file_name;
                using (TextReader reader = File.OpenText(sFile))
                {
                    var csv = new CsvReader(reader);
                    int i = 0;
                    while (csv.Read())
                    {
                        i += 1;
                        string companyName = csv.GetField<string>("Company Name");
                        string industry = csv.GetField<string>("Industry");
                        string symbol = csv.GetField<string>("Symbol");
                        using (EcamContext context = new EcamContext())
                        {
                            tra_category category = (from q in context.tra_category
                                                     where q.category_name == industry
                                                     select q).FirstOrDefault();
                            if (category == null)
                            {
                                category = new tra_category { category_name = industry };
                                context.tra_category.Add(category);
                                context.SaveChanges();
                            }
                            tra_company company = (from q in context.tra_company
                                                   where q.symbol == symbol
                                                   select q).FirstOrDefault();
                            bool isNew = false;
                            if (company == null)
                            {
                                company = new tra_company();
                                isNew = true;
                            }
                            company.company_name = companyName;
                            company.symbol = symbol;
                            if (isNew == true)
                            {
                                context.tra_company.Add(company);
                            }
                            else
                            {
                                context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                            }
                            //string saveError = string.Empty;
                            //try
                            //{
                            context.SaveChanges();
                            //}
                            //catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                            //{
                            //    foreach (var err in ex.EntityValidationErrors)
                            //    {
                            //        foreach (var ve in err.ValidationErrors)
                            //        {
                            //            saveError += ve.PropertyName + ":" + ve.ErrorMessage;
                            //        }
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    saveError += ex.Message;
                            //}
                            tra_company_category companyCategory = (from q in context.tra_company_category
                                                                    where q.category_name == category.category_name
                                                                    && q.symbol == company.symbol
                                                                    select q).FirstOrDefault();
                            if (companyCategory == null)
                            {
                                context.tra_company_category.Add(new tra_company_category
                                {
                                    category_name = category.category_name,
                                    symbol = company.symbol
                                });
                                context.SaveChanges();
                            }
                            Console.WriteLine("Completed Index=" + i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ImportCompanies ex=" + ex.Message);
                Helper.Log("ImportCompanies ex=" + ex.Message, "ERROR");
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
    }
}

