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
        private static List<tra_market> _Markets = null;

        static void Main(string[] args)
        {
            ImportCompanies();
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
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

 //       private static string _DIRNAME = "";
 //       private static void ProcessHTMLFiles()
 //       {
 //           string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];
 //           string backUpDirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadBackUpHTMLPath"];
 //           if (System.IO.Directory.Exists(dirPath) == false)
 //           {
 //               System.IO.Directory.CreateDirectory(dirPath);
 //           }
 //           if (System.IO.Directory.Exists(backUpDirPath) == false)
 //           {
 //               System.IO.Directory.CreateDirectory(backUpDirPath);
 //           }
 //           string[] directories = System.IO.Directory.GetDirectories(dirPath);
 //           foreach (string dirctoryPath in directories)
 //           {
 //               DirectoryInfo dirInfo = new DirectoryInfo(dirctoryPath);
 //               string[] files = System.IO.Directory.GetFiles(dirctoryPath);
 //               foreach (string fullFileName in files)
 //               {
 //                   string backUpDirectory = backUpDirPath + "\\" + dirInfo.Name;
 //                   _DIRNAME = dirInfo.Name;
 //                   if (System.IO.Directory.Exists(backUpDirectory) == false)
 //                   {
 //                       System.IO.Directory.CreateDirectory(backUpDirectory);
 //                   }
 //                   string fileName = System.IO.Path.GetFileName(fullFileName);
 //                   string backupFileName = backUpDirectory + "\\" + fileName;
 //                   string symbol = fileName.Replace(".html", "");
 //                   string html = System.IO.File.ReadAllText(fullFileName);
 //                   System.IO.File.Copy(fullFileName, backupFileName, true);
 //                   if (html.Contains("Service Unavailable") == true)
 //                   {
 //                       Helper.Log("Service Unavailable=" + fileName + ",DIR Name=" + dirInfo.Name, "ServiceUnavailable");
 //                   }
 //                   else if (html.Contains("Make sure the web address") == true)
 //                   {
 //                       Helper.Log("Make sure the web address FullFileName=" + fileName + ",DIR Name=" + dirInfo.Name, "MAKESURE");
 //                   }
 //                   else if (html.Contains("What you can try") == true)
 //                   {
 //                       Helper.Log("What you can try FullFileName=" + fileName + ",DIR Name=" + dirInfo.Name, "WHATYOUCANTRY");
 //                   }
 //                   else if (html.Contains("No Records") == true)
 //                   {
 //                       Helper.Log("What you can try FullFileName=" + fileName + ",DIR Name=" + dirInfo.Name, "NoRecords");
 //                   }
 //                   else
 //                   {
 //                       Console.WriteLine("Prcess html symbol=" + symbol + ",DIR Name=" + dirInfo.Name);
 //                       NSEIndiaImport(html);
 //                       System.IO.File.Delete(fullFileName);
 //                   }
 //               }
 //           }
 //       }

 //       private static void CalculatedPrice()
 //       {
 //           string symbolsAppSetting = System.Configuration.ConfigurationManager.AppSettings["symbols"];
 //           List<string> symbols = null;
 //           #region UpdatePrice2
 //           int i = 0;
 //           List<tra_market> rows;
 //           using (EcamContext context = new EcamContext())
 //           {
 //               IQueryable<tra_market> query = context.tra_market;
 //               if (string.IsNullOrEmpty(symbolsAppSetting) == false)
 //               {
 //                   symbols = Helper.ConvertStringList(symbolsAppSetting);
 //                   query = (from q in query where symbols.Contains(q.symbol) == true select q);
 //               }
 //               rows = (from q in query
 //                       orderby q.trade_date descending, q.symbol ascending
 //                       select q).ToList();
 //           }
 //           int total = rows.Count();
 //           foreach (var row in rows)
 //           {
 //               i += 1;

 //               var market = (from q in _Markets
 //                             where q.trade_date < row.trade_date
 //                             && q.symbol == row.symbol
 //                             orderby q.trade_date descending
 //                             select q).FirstOrDefault();
 //               if (market != null)
 //               {
 //                   row.prev_price = market.close_price;
 //               }
 //               market = (from q in _Markets
 //                         where q.trade_date >= row.trade_date.AddDays(-261)
 //                         && q.trade_date <= row.trade_date
 //                         && q.symbol == row.symbol
 //                         orderby q.high_price descending, q.trade_date descending
 //                         select q).FirstOrDefault();
 //               if (market != null)
 //               {
 //                   row.week_52_high = market.high_price;
 //               }
 //               market = (from q in _Markets
 //                         where q.trade_date >= row.trade_date.AddDays(-80)
 //                         && q.trade_date <= row.trade_date
 //                         && q.symbol == row.symbol
 //                         orderby q.high_price descending, q.trade_date descending
 //                         select q).FirstOrDefault();
 //               if (market != null)
 //               {
 //                   row.months_3_high = market.high_price;
 //               }
 //               market = (from q in _Markets
 //                         where q.trade_date >= row.trade_date.AddDays(-28)
 //                         && q.trade_date <= row.trade_date
 //                         && q.symbol == row.symbol
 //                         orderby q.high_price descending, q.trade_date descending
 //                         select q).FirstOrDefault();
 //               if (market != null)
 //               {
 //                   row.months_1_high = market.high_price;
 //               }
 //               market = (from q in _Markets
 //                         where q.trade_date >= row.trade_date.AddDays(-7)
 //                         && q.trade_date <= row.trade_date
 //                         && q.symbol == row.symbol
 //                         orderby q.high_price descending, q.trade_date descending
 //                         select q).FirstOrDefault();
 //               if (market != null)
 //               {
 //                   row.day_5_high = market.high_price;
 //               }
 //               //context.Entry(row).State = System.Data.Entity.EntityState.Modified;
 //               //context.SaveChanges();
 //               string sql = string.Format("update tra_market set prev_price={0},week_52_high={1},months_3_high={2},months_1_high={3},day_5_high={4} where market_id={5};"
 //                   , (row.prev_price ?? 0)
 //                   , (row.week_52_high ?? 0)
 //                   , (row.months_3_high ?? 0)
 //                   , (row.months_1_high ?? 0)
 //                   , (row.day_5_high ?? 0)
 //                   , row.id);
 //               try
 //               {
 //                   MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
 //               }
 //               catch (Exception ex)
 //               {
 //                   Helper.Log(sql, "ERRORSQL");
 //               }
 //               Console.WriteLine("Update calculated price Rows " + i + " Of " + total);

 //           }
 //           if (symbols != null)
 //           {
 //               using (EcamContext context = new EcamContext())
 //               {
 //                   symbols = (from q in context.tra_company
 //                              select q.symbol).ToList();
 //               }
 //               foreach (string symbol in symbols)
 //               {
 //                   UpdateCompanyPrice(symbol);
 //               }
 //           }
 //           #endregion
 //       }

 //       private static string ReplaceTagAttributes(string html, string tagName)
 //       {
 //           Regex regex = new Regex(
 //  @"<" + tagName + "(.*?)>",
 //  RegexOptions.IgnoreCase
 //  | RegexOptions.Multiline
 //  | RegexOptions.IgnorePatternWhitespace
 //  | RegexOptions.Compiled
 //  );
 //           html = regex.Replace(html, "<" + tagName + ">");
 //           return html;
 //       }

 //       private static string ReplaceAttributes(string html, string attrName, string replaceContent)
 //       {
 //           string exp = attrName + "\\s*=\\s*\"(.*?)\"";
 //           Regex regex = new Regex(
 //   exp,
 //  RegexOptions.IgnoreCase
 //  | RegexOptions.Multiline
 //  | RegexOptions.IgnorePatternWhitespace
 //  | RegexOptions.Compiled
 //  );
 //           html = regex.Replace(html, replaceContent);
 //           return html;
 //       }

 //       private static void NSEIndiaImport(string html)
 //       {
 //           //try
 //           //{
 //           html = ReplaceTagAttributes(html, "table");
 //           html = ReplaceTagAttributes(html, "tbody");
 //           html = ReplaceTagAttributes(html, "tr");
 //           html = ReplaceTagAttributes(html, "td");
 //           html = ReplaceTagAttributes(html, "th");
 //           html = ReplaceTagAttributes(html, "img");
 //           html = ReplaceTagAttributes(html, "div");
 //           html = ReplaceAttributes(html, "style", "");
 //           html = ReplaceAttributes(html, "class", "");
 //           html = ReplaceAttributes(html, "nowrap", "");
 //           html = ReplaceAttributes(html, "title", "");
 //           html = html.Replace("<TH nowrap=\"\">", "<th>");
 //           html = html.Replace("<TD class=\"normalText\" nowrap=\"\">", "<td>");
 //           html = html.Replace("<TD class=\"date\" nowrap=\"\">", "<td>");
 //           html = html.Replace("<TD class=\"number\" nowrap=\"\">", "<td>");
 //           html = html.Replace("\n", "").Replace("\r", "").Replace("\r\n", "").Replace("TABLE", "table").Replace("TR", "tr").Replace("TD", "td").Replace("TH", "th").Replace("TBODY", "tbody");
 //           int startIndex = html.IndexOf("<tbody>");
 //           int endIndex = html.IndexOf("</tbody>");
 //           int length = endIndex - startIndex + 8;
 //           string tblHTML = html.Substring(startIndex, length);
 //           tblHTML = tblHTML.Replace(" ", "");
 //           List<TempClass> tempList = new List<TempClass>();
 //           Regex regex = new Regex(
 //@"<tr>(.*?)</tr>",
 //RegexOptions.IgnoreCase
 //| RegexOptions.Multiline
 //| RegexOptions.IgnorePatternWhitespace
 //| RegexOptions.Compiled
 //);

 //           MatchCollection trCollections = regex.Matches(tblHTML);
 //           int i = 0;
 //           foreach (Match trMatch in trCollections)
 //           {
 //               i += 1;
 //               string tr = trMatch.Value;
 //               string tagName = "td";
 //               if (i == 1)
 //               {
 //                   tagName = "th";
 //               }
 //               regex = new Regex(
 //                           @"<" + tagName + ">(.+?)</" + tagName + ">",
 //                           RegexOptions.IgnoreCase
 //                           | RegexOptions.Multiline
 //                           | RegexOptions.IgnorePatternWhitespace
 //                           | RegexOptions.Compiled
 //                           );
 //               MatchCollection rowMatches = regex.Matches(tr);
 //               string date = "";
 //               string symbol = "";
 //               string series = "";
 //               string prev = "";
 //               string open = "";
 //               string high = "";
 //               string low = "";
 //               string lastTrade = "";
 //               string close = "";
 //               string tradeType = "NSE";
 //               int colIndex = -1;
 //               foreach (Match colMatch in rowMatches)
 //               {
 //                   colIndex += 1;
 //                   if (i > 1)
 //                   {
 //                       string value = string.Empty;
 //                       if (colMatch.Groups.Count >= 2)
 //                       {
 //                           value = colMatch.Groups[1].Value;
 //                       }
 //                       if (string.IsNullOrEmpty(value) == false)
 //                       {
 //                           value = value.Trim();
 //                       }
 //                       switch (colIndex)
 //                       {
 //                           case 0: symbol = value; break;
 //                           case 1: series = value; break;
 //                           case 2: date = value; break;
 //                           case 3: prev = value; break;
 //                           case 4: open = value; break;
 //                           case 5: high = value; break;
 //                           case 6: low = value; break;
 //                           case 7: lastTrade = value; break;
 //                           case 8: close = value; break;
 //                       }
 //                   }
 //               }
 //               if (string.IsNullOrEmpty(series) == false)
 //               {
 //                   if (series != "EQ")
 //                   {
 //                       Helper.Log("NOTSERIES Symbol =" + symbol + ",_DIRNAME=" + _DIRNAME, "NOTSERIES");
 //                   }
 //               }
 //               if (string.IsNullOrEmpty(date) == false
 //                   && string.IsNullOrEmpty(symbol) == false
 //                   && series == "EQ"
 //                   )
 //               {
 //                   DateTime dt = DataTypeHelper.ToDateTime(date);
 //                   if (dt.Year < 2015)
 //                   {
 //                       continue;
 //                   }
 //                   tempList.Add(new TempClass
 //                   {
 //                       symbol = symbol,
 //                       trade_type = tradeType,
 //                       trade_date = dt,
 //                       close_price = DataTypeHelper.ToDecimal(close),
 //                       high_price = DataTypeHelper.ToDecimal(high),
 //                       low_price = DataTypeHelper.ToDecimal(low),
 //                       open_price = DataTypeHelper.ToDecimal(open),
 //                       ltp_price = DataTypeHelper.ToDecimal(lastTrade),
 //                   });
 //               }
 //           }
 //           if (tempList.Count() <= 0)
 //           {
 //               //Console.WriteLine("NSEIndiaImport Temp List No Records Found Symbol =" + symbol + ",_DIRNAME=" + _DIRNAME);
 //               Helper.Log("NSEIndiaImport Temp List No Records Found _DIRNAME=" + _DIRNAME, "ERROR");
 //           }
 //           int rowIndex = 0;
 //           foreach (var row in tempList)
 //           {
 //               rowIndex += 1;
 //               Console.WriteLine(" Rows " + rowIndex + " Of " + tempList.Count());
 //               ImportPrice(row);
 //           }
 //           //}
 //           //catch (Exception ex)
 //           //{
 //           //    lblError.Text = "NSEIndiaImport symbol=" + symbol + ",ex=" + ex.Message;
 //           //    Helper.Log("NSEIndiaImport symbol=" + symbol + ",ex =" + ex.Message, "ERROR");
 //           //}
 //       }

 //       private static void ImportPrice(TempClass import)
 //       {
 //           if (import.trade_date.Year < 2015)
 //           {
 //               return;
 //           }
 //           using (EcamContext context = new EcamContext())
 //           {
 //               var row = (from q in context.tra_market
 //                          where q.symbol == import.symbol
 //                          && q.trade_date == import.trade_date
 //                          && q.trade_type == import.trade_type
 //                          select q).FirstOrDefault();
 //               bool isNew = false;
 //               if (row == null)
 //               {
 //                   row = new tra_market();
 //                   isNew = true;
 //               }
 //               row.symbol = import.symbol;
 //               row.trade_type = import.trade_type;
 //               row.trade_date = import.trade_date;
 //               row.open_price = import.open_price;
 //               row.high_price = import.high_price;
 //               row.low_price = import.low_price;
 //               row.close_price = import.close_price;
 //               row.ltp_price = import.ltp_price;
 //               if (isNew == true)
 //               {
 //                   context.tra_market.Add(row);
 //               }
 //               else
 //               {
 //                   context.Entry(row).State = System.Data.Entity.EntityState.Modified;
 //               }
 //               context.SaveChanges();
 //               //lblError.Text = "ImportPrice Price Index symbol=" + row.symbol + ",Date=" + row.trade_date;
 //               //Console.WriteLine("ImportPrice Price Index symbol=" + row.symbol + ",Date=" + row.trade_date);
 //           }
 //       }

 //       private static void UpdateCompanyPrice(string symbol)
 //       {
 //           #region Update Company Price
 //           using (EcamContext context = new EcamContext())
 //           {
 //               tra_market lastMarket = (from q in context.tra_market
 //                                        where q.symbol == symbol
 //                                        orderby q.trade_date descending
 //                                        select q).FirstOrDefault();
 //               if (lastMarket != null)
 //               {
 //                   tra_company company = (from q in context.tra_company
 //                                          where q.symbol == symbol
 //                                          select q).FirstOrDefault();

 //                   if (company != null)
 //                   {
 //                       company.open_price = lastMarket.open_price;
 //                       company.high_price = lastMarket.high_price;
 //                       company.low_price = lastMarket.low_price;
 //                       company.close_price = lastMarket.close_price;
 //                       company.prev_price = lastMarket.prev_price;

 //                       company.ltp_price = lastMarket.ltp_price;
 //                       company.week_52_high = lastMarket.week_52_high;
 //                       company.months_1_high = lastMarket.months_1_high;
 //                       company.months_3_high = lastMarket.months_3_high;
 //                       company.day_5_high = lastMarket.day_5_high;

 //                       context.Entry(company).State = System.Data.Entity.EntityState.Modified;
 //                       context.SaveChanges();
 //                       //Console.WriteLine("CalculatedPrice Update Company=" + company.company_name);
 //                   }
 //               }
 //           }
 //           #endregion
 //       }
    }


    //public class TempClass
    //{
    //    public string symbol { get; set; }
    //    public System.DateTime trade_date { get; set; }
    //    public string trade_type { get; set; }
    //    public Nullable<decimal> open_price { get; set; }
    //    public Nullable<decimal> high_price { get; set; }
    //    public Nullable<decimal> low_price { get; set; }
    //    public Nullable<decimal> close_price { get; set; }
    //    public Nullable<decimal> ltp_price { get; set; }
    //}
}

