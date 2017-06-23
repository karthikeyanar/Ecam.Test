using Ecam.Framework;
using Ecam.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Ecam.ConsoleApp
{
    class Program
    {
        public static string IS_DOWNLOAD_HISTORY = "";
        public static string GOOGLE_DATA = "";
        static void Main(string[] args)
        {
            IS_DOWNLOAD_HISTORY = System.Configuration.ConfigurationManager.AppSettings["IS_DOWNLOAD_HISTORY"];
            GOOGLE_DATA = System.Configuration.ConfigurationManager.AppSettings["GOOGLE_DATA"];
            string sql = "delete from tra_market_intra_day where DATE_FORMAT(trade_date, '%Y-%m-%d') < DATE_FORMAT(curdate(), '%Y-%m-%d')";
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
            //CaculateIntraydayProfit();
            DownloadStart();
            //CalculateRSI();
        }

        private static void DownloadStart()
        {
            //Helper.Log("DownloadStart=" + DateTime.Now.ToString(), "DOWNLOAD");
            DateTime morningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 9:15AM");
            DateTime morningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 10:15AM");
            DateTime eveningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:31PM");
            DateTime eveningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 11:59PM");
            DateTime now = DateTime.Now;
            if ((now >= morningStart && now <= morningEnd) || (now >= eveningStart && now <= eveningEnd))
            {
                if (IS_DOWNLOAD_HISTORY == "true")
                {
                    GoogleHistoryData();
                }
                else
                {
                    GoogleData();
                }
                if ((now >= eveningStart && now <= eveningEnd))
                {
                    CalculateRSI();
                }
                Console.WriteLine("Completed");
                //Helper.Log("DownloadEnd=" + DateTime.Now.ToString(), "DOWNLOAD");
                //if ((now >= morningStart && now <= morningEnd))
                //{
                //    int minute1 = (1000 * 60);
                //    Console.WriteLine("Wait till=" + DateTime.Now.AddMinutes(5).ToString());
                //    System.Threading.Thread.Sleep((minute1 * 5));
                //    DownloadStart();
                //}
            }
        }

        private static void CalculateRSI()
        {
            string sql = "update tra_market set upward = 0, downward = 0, avg_downward = 0, avg_upward = 0, rsi = 0, prev_rsi = 0;";
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
            DateTime today = DateTime.Now.Date;
            using (EcamContext context = new EcamContext())
            {
                List<tra_company> companies = (from q in context.tra_company
                                               where q.is_nifty_50 == true
                                               //&& q.symbol == "ADANIPORTS"
                                               orderby q.company_name ascending
                                               select q).ToList();
                foreach (var company in companies)
                {
                    var markets = (from q in context.tra_market
                                   where q.symbol == company.symbol
                                   && q.trade_date < today
                                   orderby q.trade_date ascending
                                   select q).ToList();
                    foreach (var market in markets)
                    {
                        CalculateRSI(markets, market.trade_date, market.id);
                    }
                    var updateMarket = (from q in context.tra_market
                                        where q.symbol == company.symbol
                                        && q.trade_date < today 
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

        private static void CalculateRSI(List<tra_market> fullMarkets, DateTime date, int id)
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
                        var market = (from q in context.tra_market where q.id == existValue.id select q).FirstOrDefault();
                        if (market != null)
                        {
                            var prevRecord = (from q in context.tra_market where q.trade_date < existValue.date orderby q.trade_date descending select q).FirstOrDefault();
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
                            var preMarket = (from q in markets where q.id == existValue.id select q).FirstOrDefault();
                            preMarket.avg_downward = existValue.avg_downward;
                            preMarket.avg_upward = existValue.avg_upward;
                            preMarket.upward = existValue.upward;
                            preMarket.downward = existValue.downward;
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

        private static void CaculateIntraydayProfit()
        {
            List<tra_market_intra_day> rows;
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                rows = (from q in context.tra_market_intra_day orderby q.symbol, q.trade_date ascending select q).ToList();
                companies = (from q in context.tra_company select q).ToList();
            }
            if (rows.Count > 0)
            {
                List<string> symbols = (from q in rows select q.symbol).Distinct().ToList();
                DateTime firstDate = (from q in rows orderby q.trade_date descending select q).FirstOrDefault().trade_date.Date;
                DateTime startTime = Convert.ToDateTime(firstDate.ToString("dd/MMM/yyyy") + " 9:25AM");
                DateTime endTime = Convert.ToDateTime(firstDate.ToString("dd/MMM/yyyy") + " 10:00AM");
                foreach (string symbol in symbols)
                {
                    var company = (from q in companies where q.symbol == symbol select q).FirstOrDefault();
                    var firstLTP = (from q in rows where q.symbol == symbol && q.trade_date >= firstDate && q.trade_date <= startTime orderby q.trade_date descending select q).FirstOrDefault();
                    var lastLTP = (from q in rows where q.symbol == symbol && q.trade_date >= firstDate && q.trade_date <= endTime orderby q.trade_date descending select q).FirstOrDefault();
                    if (firstLTP != null && lastLTP != null && company != null)
                    {
                        try
                        {
                            decimal? firstPrice = firstLTP.ltp_price;
                            decimal? lastPrice = lastLTP.ltp_price;
                            decimal? finalPrice = company.ltp_price;
                            decimal? openPrice = company.open_price;

                            List<decimal> percentageList = new List<decimal>();

                            var nextList = (from q in rows
                                            where q.symbol == symbol && q.trade_date > firstLTP.trade_date
                                            orderby q.trade_date ascending
                                            select q).ToList();
                            foreach (var nextRow in nextList)
                            {
                                decimal? p = ((nextRow.ltp_price - (firstPrice ?? 0)) / (firstPrice ?? 0)) * 100;
                                percentageList.Add((p ?? 0));
                            }

                            decimal? percentage = 0;
                            if (percentageList.Count > 0)
                            {
                                percentage = (from q in percentageList orderby q descending select q).FirstOrDefault();
                            }
                            else
                            {
                                percentage = (((lastPrice ?? 0) - (firstPrice ?? 0)) / (firstPrice ?? 0)) * 100;
                            }

                            percentageList = new List<decimal>();
                            foreach (var nextRow in nextList)
                            {
                                decimal? p = (((firstPrice ?? 0) - nextRow.ltp_price) / nextRow.ltp_price) * 100;
                                percentageList.Add((p ?? 0));
                            }

                            decimal? reversePercentage = 0;
                            if (percentageList.Count > 0)
                            {
                                reversePercentage = (from q in percentageList orderby q descending select q).FirstOrDefault();
                            }
                            else
                            {
                                reversePercentage = (((firstPrice ?? 0) - (lastPrice ?? 0)) / (lastPrice ?? 0)) * 100;
                            }

                            decimal? finalPercentage = (((finalPrice ?? 0) - (firstPrice ?? 0)) / (firstPrice ?? 0)) * 100;

                            decimal? firstPercentage = (((firstPrice ?? 0) - (openPrice ?? 0)) / (openPrice ?? 0)) * 100;

                            decimal? lastPercentage = (((lastPrice ?? 0) - (openPrice ?? 0)) / (openPrice ?? 0)) * 100;

                            bool isDay1High = (company.day_1 ?? 0) <= (company.ltp_price ?? 0);
                            bool isDay2High = (
                                    (company.day_2 ?? 0) < (company.day_1 ?? 0)
                                    && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
                                    );
                            bool isDay3High = (
        (company.day_3 ?? 0) < (company.day_2 ?? 0)
        && (company.day_2 ?? 0) < (company.day_1 ?? 0)
        && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        );
                            bool isDay4High = (
                                (company.day_4 ?? 0) < (company.day_3 ?? 0)
       && (company.day_3 ?? 0) < (company.day_2 ?? 0)
       && (company.day_2 ?? 0) < (company.day_1 ?? 0)
       && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
       );

                            bool isDay5High = (
                                (company.day_5 ?? 0) < (company.day_4 ?? 0)
                               && (company.day_4 ?? 0) < (company.day_3 ?? 0)
      && (company.day_3 ?? 0) < (company.day_2 ?? 0)
      && (company.day_2 ?? 0) < (company.day_1 ?? 0)
      && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
      );

                            bool isDay1Low = (company.day_1 ?? 0) >= (company.ltp_price ?? 0);
                            bool isDay2Low = (
                                    (company.day_2 ?? 0) > (company.day_1 ?? 0)
                                    && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
                                    );
                            bool isDay3Low = (
        (company.day_3 ?? 0) > (company.day_2 ?? 0)
        && (company.day_2 ?? 0) > (company.day_1 ?? 0)
        && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        );
                            bool isDay4Low = (
                                (company.day_4 ?? 0) > (company.day_3 ?? 0)
    && (company.day_3 ?? 0) > (company.day_2 ?? 0)
    && (company.day_2 ?? 0) > (company.day_1 ?? 0)
    && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
    );
                            bool isDay5Low = (
                                (company.day_5 ?? 0) > (company.day_4 ?? 0)
                                && (company.day_4 ?? 0) > (company.day_3 ?? 0)
    && (company.day_3 ?? 0) > (company.day_2 ?? 0)
    && (company.day_2 ?? 0) > (company.day_1 ?? 0)
    && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
    );
                            using (EcamContext context = new EcamContext())
                            {
                                tra_intra_day_profit profit = (from q in context.tra_intra_day_profit
                                                               where q.symbol == symbol
                                                               && q.trade_date == firstDate
                                                               select q).FirstOrDefault();
                                bool isNew = false;
                                if (profit == null)
                                {
                                    profit = new tra_intra_day_profit();
                                    isNew = true;
                                }
                                profit.symbol = symbol;
                                profit.trade_date = firstDate;
                                profit.profit_percentage = (percentage ?? 0);
                                profit.reverse_percentage = (reversePercentage ?? 0);
                                profit.last_percentage = (lastPercentage ?? 0);
                                profit.first_percentage = (firstPercentage ?? 0);
                                profit.final_percentage = (finalPercentage ?? 0);
                                int highCnt = 0;
                                int lowCnt = 0;
                                if (isDay1High == true && isDay2High == true
                                    && isDay3High == true && isDay4High == true
                                    && isDay5High == true)
                                {
                                    highCnt = 5;
                                }
                                else if (isDay1High == true && isDay2High == true
                                    && isDay3High == true && isDay4High == true
                                    )
                                {
                                    highCnt = 4;
                                }
                                else if (isDay1High == true && isDay2High == true
                                    && isDay3High == true
                                    )
                                {
                                    highCnt = 3;
                                }
                                else if (isDay1High == true && isDay2High == true
                                    )
                                {
                                    highCnt = 2;
                                }
                                else if (isDay1High == true)
                                {
                                    highCnt = 1;
                                }

                                if (isDay1Low == true && isDay2Low == true
                                    && isDay3Low == true && isDay4Low == true
                                    && isDay5Low == true)
                                {
                                    lowCnt = 5;
                                }
                                else if (isDay1Low == true && isDay2Low == true
                                    && isDay3Low == true && isDay4Low == true
                                    )
                                {
                                    lowCnt = 4;
                                }
                                else if (isDay1Low == true && isDay2Low == true
                                    && isDay3Low == true
                                    )
                                {
                                    lowCnt = 3;
                                }
                                else if (isDay1Low == true && isDay2Low == true
                                    )
                                {
                                    lowCnt = 2;
                                }
                                else if (isDay1Low == true)
                                {
                                    lowCnt = 1;
                                }
                                profit.high_count = highCnt;
                                profit.low_count = lowCnt;

                                if (isNew == true)
                                {
                                    context.tra_intra_day_profit.Add(profit);
                                }
                                else
                                {
                                    context.Entry(profit).State = System.Data.Entity.EntityState.Modified;
                                }
                                context.SaveChanges();

                                var updateCompany = (from q in context.tra_company where q.symbol == symbol select q).FirstOrDefault();
                                if (updateCompany != null)
                                {
                                    updateCompany.high_count = highCnt;
                                    updateCompany.low_count = lowCnt;
                                    context.Entry(updateCompany).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }
                                Console.WriteLine("Calculate profit completed symbol=" + symbol);
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private static int _INDEX = -1;
        private static string[] _COMPANIES;
        private static void GoogleData()
        {
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                IQueryable<tra_company> query = context.tra_company;
                DateTime targetTime = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:30PM");
                if (DateTime.Now <= targetTime)
                {
                    string IS_NIFTY_50 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_50"];
                    string IS_NIFTY_100 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_100"];
                    string IS_NIFTY_200 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_200"];
                    if (IS_NIFTY_50 == "true")
                    {
                        query = query.Where(q => (q.is_nifty_50 ?? false) == true);
                    }
                    if (IS_NIFTY_100 == "true")
                    {
                        query = query.Where(q => (q.is_nifty_100 ?? false) == true);
                    }
                    if (IS_NIFTY_200 == "true")
                    {
                        query = query.Where(q => (q.is_nifty_200 ?? false) == true);
                    }
                }
                companies = (from q in query
                             orderby (q.is_nifty_50 ?? false) descending, (q.is_nifty_100 ?? false) descending, (q.is_nifty_200 ?? false) descending, q.symbol ascending
                             select q).ToList();
            }
            _COMPANIES = (from q in companies select q.symbol).ToArray();
            _INDEX = -1;

            // Reset book mark
            string sql = "update tra_company set is_book_mark=0;";
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);

            // Delete yesterday tra_market_intraday
            sql = "delete from tra_market_intra_day where DATE_FORMAT(trade_date, '%Y-%m-%d') < DATE_FORMAT(curdate(), '%Y-%m-%d')";
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);

            // Delete before 3 months tra_market
            //sql = "delete from tra_market where DATE_FORMAT(trade_date, '%Y-%m-%d') < DATE_FORMAT(DATE_ADD(curdate(), INTERVAL -3 MONTH), '%Y-%m-%d')";
            //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);

            GoogleDownloadStart();
            //foreach (var company in companies)
            //{
            //    Thread thread = new Thread(() => GoogleDataDownload(company.symbol));
            //    thread.Start();
            //}
        }

        private static void GoogleDownloadStart()
        {
            int totalCount = _COMPANIES.Length;
            int queueCount = 64;
            // One event is used for each Fibonacci object
            ManualResetEvent[] doneEvents = new ManualResetEvent[queueCount];
            GoogleDownloadData[] downArray = new GoogleDownloadData[queueCount];
            //Random r = new Random();
            // Configure and launch threads using ThreadPool:
            Console.WriteLine("launching {0} tasks...", totalCount);
            for (int i = 0; i < queueCount; i++)
            {
                _INDEX += 1;
                string symbol = "";
                if (_INDEX < _COMPANIES.Length)
                {
                    symbol = _COMPANIES[_INDEX];
                }
                doneEvents[i] = new ManualResetEvent(false);
                GoogleDownloadData f = new GoogleDownloadData(symbol, doneEvents[i]);
                downArray[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
            }
            // Wait for all threads in pool to calculation...
            WaitHandle.WaitAll(doneEvents);
            if (_INDEX < _COMPANIES.Length)
            {
                Console.WriteLine("All calculations are complete.");
                GoogleDownloadStart();
            }
        }

        private static void GoogleHistoryDownloadStart()
        {
            int totalCount = _COMPANIES.Length;
            int queueCount = 64;
            // One event is used for each Fibonacci object
            ManualResetEvent[] doneEvents = new ManualResetEvent[queueCount];
            GoogleHistoryDownloadData[] downArray = new GoogleHistoryDownloadData[queueCount];
            //Random r = new Random();
            // Configure and launch threads using ThreadPool:
            Console.WriteLine("launching {0} tasks...", totalCount);
            for (int i = 0; i < queueCount; i++)
            {
                _INDEX += 1;
                string symbol = "";
                if (_INDEX < _COMPANIES.Length)
                {
                    symbol = _COMPANIES[_INDEX];
                }
                doneEvents[i] = new ManualResetEvent(false);
                GoogleHistoryDownloadData f = new GoogleHistoryDownloadData(symbol, doneEvents[i]);
                downArray[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
            }
            // Wait for all threads in pool to calculation...
            WaitHandle.WaitAll(doneEvents);
            if (_INDEX < _COMPANIES.Length)
            {
                Console.WriteLine("All calculations are complete.");
                GoogleHistoryDownloadStart();
            }
        }

        private static void GoogleHistoryData()
        {
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                companies = (from q in context.tra_company orderby q.symbol ascending select q).ToList();
            }
            _COMPANIES = (from q in companies select q.symbol).ToArray();
            _INDEX = -1;
            GoogleHistoryDownloadStart();
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

    class TempRSI
    {
        public int id { get; set; }
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
}
