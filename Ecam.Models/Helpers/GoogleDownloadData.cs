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
            if (string.IsNullOrEmpty(_Symbol) == false)
            {
                GoogleDataDownload(_Symbol);
            }
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
            url = string.Format("https://finance.google.com/finance?q=NSE:{0}"
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
            //if ((now >= eveningStart && now <= eveningEnd))
            //{
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
                    try
                    {
                        File.WriteAllText(fileName, html);
                    }
                    catch { }
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
            //}
            //else
            //{
            //    html = client.DownloadString(url);
            //    Console.WriteLine("Download google data symbol morning=" + symbol);
            //    if (File.Exists(fileName) == true)
            //    {
            //        File.Delete(fileName);
            //    }
            //    //if (File.Exists(fileName) == false)
            //    //{
            //    //    try
            //    //    {
            //    //        html = client.DownloadString(url);
            //    //        //File.WriteAllText(fileName, html);
            //    //        Console.WriteLine("Download google data symbol morning=" + symbol);
            //    //    }
            //    //    catch
            //    //    {
            //    //        //Helper.Log("DownloadErrorOnGoogleData symbol=" + symbol, "ErrorOnGoogleData_" + rnd.Next(1000, 10000));
            //    //    }
            //    //}
            //    //else
            //    //{
            //    //    html = File.ReadAllText(fileName);
            //    //}
            //}
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
                        // Helper.Log("ErrorOnGoogleData symbol=" + symbol, "DownloadErrorOnGoogleData_" + rnd.Next(1000, 10000));
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
                    decimal marketCapital = 0;
                    string marketCapitalValue = "";
                    decimal volume = 0;
                    decimal pe = 0;
                    decimal eps = 0;
                    decimal lowPrice = 0;
                    decimal highPrice = 0;
                    decimal week52Low = 0;
                    decimal week52High = 0;

                    bool isBilion = false;
                    bool isTrillion = false;
                    bool isCrore = false;
                    bool isMillion = false;
                    decimal crore = 10000000;
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
                    foreach (Match tblMatch in collections)
                    {
                        string tableContent = tblMatch.Value;// collections[0].Groups[2].Value;

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
                                    case "Mkt cap":
                                        marketCapitalValue = secondCell;
                                        isBilion = false;
                                        isTrillion = false;
                                        isCrore = false;
                                        isMillion = false;
                                        if (secondCell.Contains("B") == true)
                                        {
                                            isBilion = true;
                                        }
                                        else if (secondCell.Contains("T") == true)
                                        {
                                            isTrillion = true;
                                        }
                                        else if (secondCell.Contains("C") == true)
                                        {
                                            isCrore = true;
                                        }
                                        else if (secondCell.Contains("M") == true)
                                        {
                                            isMillion = true;
                                        }
                                        secondCell = secondCell.Replace("B", "").Replace("T", "").Replace("C", "").Replace("M", "");
                                        marketCapital = DataTypeHelper.ToDecimal(secondCell);
                                        if (isBilion == true)
                                        {
                                            marketCapital = marketCapital * (100 * crore);
                                        }
                                        else if (isTrillion == true)
                                        {
                                            marketCapital = marketCapital * (100000 * crore);
                                        }
                                        else if (isCrore == true)
                                        {
                                            marketCapital = marketCapital * crore;
                                        }
                                        marketCapital = marketCapital / crore;
                                        break;
                                    case "P/E":
                                        pe = DataTypeHelper.ToDecimal(secondCell);
                                        break;
                                    case "Vol.":
                                        isBilion = false;
                                        isTrillion = false;
                                        isCrore = false;
                                        isMillion = false;
                                        if (secondCell.Contains("B") == true)
                                        {
                                            isBilion = true;
                                        }
                                        else if (secondCell.Contains("T") == true)
                                        {
                                            isTrillion = true;
                                        }
                                        else if (secondCell.Contains("C") == true)
                                        {
                                            isCrore = true;
                                        }
                                        else if (secondCell.Contains("M") == true)
                                        {
                                            isMillion = true;
                                        }
                                        secondCell = secondCell.Replace("B", "").Replace("T", "").Replace("C", "").Replace("M", "");
                                        volume = DataTypeHelper.ToDecimal(secondCell);
                                        if (isBilion == true)
                                        {
                                            volume = volume * (100 * crore);
                                        }
                                        else if (isTrillion == true)
                                        {
                                            volume = volume * (100000 * crore);
                                        }
                                        else if (isCrore == true)
                                        {
                                            volume = volume * crore;
                                        }
                                        volume = volume / crore;
                                        break;
                                    case "EPS":
                                        eps = DataTypeHelper.ToDecimal(secondCell);
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
                        });
                        string sql = string.Format(" update tra_company set week_52_low={0},week_52_high={1},mcstr='{2}',mc={3},pe={4},volume={5},eps={6} where symbol='{7}'"
                            , week52Low
                            , week52High
                            , marketCapitalValue
                            , marketCapital
                            , pe
                            , volume
                            , eps
                            , symbol);
                        MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
                        //if (week52High <= 0 || week52Low <= 0)
                        //{
                            //Helper.Log("GoogleException symbol 1=" + symbol, "GoogleException_" + rnd.Next(1000, 10000));
                        //}
                        Console.WriteLine("Completed symbol=" + symbol);

                        /*
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
                        */
                    }
                    else
                    {
                        //Helper.Log("GoogleException symbol 2=" + symbol, "GoogleException_" + rnd.Next(1000, 10000));
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                    Helper.Log("GoogleException symbol 3=" + symbol + ",EX=" + ex.Message, "GoogleException_" + rnd.Next(1000, 10000));
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
     
}