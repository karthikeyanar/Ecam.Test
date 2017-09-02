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
            DownloadStart();
        }

        private static void ImportOldSymbols()
        {
            //string sql = "select c.symbol from tra_company c left outer join tra_market t on t.symbol = c.symbol where ifnull(t.market_id, 0) <= 0 group by c.symbol,c.company_name limit 0,1000";
            //List<string> rows = new List<string>();
            //using (EcamContext context = new EcamContext())
            //{
            //    rows = context.Database.SqlQuery<string>(sql).ToList();
            //}
            //string symbols = "";
            //foreach (string symbol in rows)
            //{
            //    symbols += symbol + ",";
            //}
            //if (string.IsNullOrEmpty(symbols) == false)
            //{
            //    symbols = symbols.Substring(0, symbols.Length - 1);
            //}
            string oldConnectionString = "server=localhost;port=3306;database=eb_2;uid=root;password=raga";
            string sql = "select * from tra_company";
            MySqlDataReader dr = MySqlHelper.ExecuteReader(oldConnectionString, sql);
            List<tra_company> oldCompanies = new List<tra_company>();
            List<tra_company_category> oldCompanyCategories = new List<tra_company_category>();
            using (dr)
            {
                while (dr.Read())
                {
                    oldCompanies.Add(new tra_company
                    {
                        company_name = Convert.ToString(dr["company_name"]),
                        symbol = Convert.ToString(dr["symbol"]),
                        is_nifty_50 = DataTypeHelper.ToBoolean(Convert.ToString(dr["is_nifty_50"])),
                        is_nifty_100 = DataTypeHelper.ToBoolean(Convert.ToString(dr["is_nifty_100"])),
                        is_nifty_200 = DataTypeHelper.ToBoolean(Convert.ToString(dr["is_nifty_200"])),
                    });
                }
            }
            sql = "select * from tra_company_category";
            dr = MySqlHelper.ExecuteReader(oldConnectionString, sql);
            using (dr)
            {
                while (dr.Read())
                {
                    oldCompanyCategories.Add(new tra_company_category
                    {
                        category_name = Convert.ToString(dr["category_name"]),
                        symbol = Convert.ToString(dr["symbol"]),
                    });
                }
            }
            foreach (var oldCompany in oldCompanies)
            {
                using (EcamContext context = new EcamContext())
                {
                    var company = (from q in context.tra_company
                                   where q.symbol == oldCompany.symbol
                                   select q).FirstOrDefault();
                    if (company == null)
                    {
                        context.tra_company.Add(new tra_company
                        {
                            company_name = oldCompany.company_name,
                            symbol = oldCompany.symbol,
                            is_nifty_50 = oldCompany.is_nifty_50,
                            is_nifty_100 = oldCompany.is_nifty_100,
                            is_nifty_200 = oldCompany.is_nifty_200,
                        });
                        context.SaveChanges();
                        var rows = (from q in oldCompanyCategories
                                    where q.symbol == oldCompany.symbol
                                    select q).ToList();
                        foreach (var row in rows)
                        {
                            var companyCategory = (from q in context.tra_company_category
                                                   where q.symbol == row.symbol
                                                   && q.category_name == row.category_name
                                                   select q).FirstOrDefault();
                            if (companyCategory == null)
                            {
                                context.tra_company_category.Add(new tra_company_category
                                {
                                    category_name = row.category_name,
                                    symbol = row.symbol
                                });
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        private static void DownloadStart()
        {
            //Helper.Log("DownloadStart=" + DateTime.Now.ToString(), "DOWNLOAD");
            DateTime morningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 9:15AM");
            DateTime morningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 10:15AM");
            DateTime eveningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:31PM");
            DateTime eveningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 11:59PM");
            DateTime now = DateTime.Now;
            if (IS_DOWNLOAD_HISTORY == "true")
            {
                GoogleHistoryData();
            }
            else
            {
                if ((now >= morningStart && now <= morningEnd) || (now >= eveningStart && now <= eveningEnd))
                {
                    GoogleData();
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
        }


        //using (EcamContext context = new EcamContext())
        //{
        //    //sql = "delete from tra_pre_calc_item";
        //    //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
        //    var totalMarkets = (from q in context.tra_market
        //                        where (q.prev_price ?? 0) > 0
        //                        //&& q.symbol == "3MINDIA"
        //                        orderby q.trade_date descending, q.symbol ascending
        //                        select q).ToList();
        //    List<string> symbols = (from q in totalMarkets
        //                            select q.symbol).Distinct().ToList();
        //    int total = symbols.Count();
        //    int index = 0;
        //    foreach (string symbol in symbols)
        //    {
        //        index += 1;
        //        tra_prev_calc calc = (from q in context.tra_prev_calc
        //                              where q.symbol == symbol
        //                              select q).FirstOrDefault();
        //        if (calc == null)
        //        {
        //            calc = new tra_prev_calc
        //            {
        //                symbol = symbol,
        //                positive_count = 0,
        //                negative_count = 0,
        //                success_count = 0,
        //                fail_count = 0,
        //                high_profit = 0,
        //                open_profit = 0,
        //                id = 0
        //            };
        //        }
        //        else
        //        {
        //            calc.symbol = symbol;
        //            calc.positive_count = 0;
        //            calc.negative_count = 0;
        //            calc.success_count = 0;
        //            calc.fail_count = 0;
        //            calc.high_profit = 0;
        //            calc.open_profit = 0;
        //        }
        //        var markets = (from q in totalMarkets
        //                       where q.symbol == symbol
        //                       orderby q.trade_date descending
        //                       select q).ToList();
        //        foreach (var market in markets)
        //        {
        //            if ((market.prev_price ?? 0) < (market.open_price ?? 0))
        //            {
        //                calc.positive_count += 1;
        //            }
        //            else
        //            {
        //                calc.negative_count += 1;
        //            }
        //            decimal? open_profit = (((market.open_price ?? 0) - (market.prev_price ?? 0)) / (market.prev_price ?? 0)) * 100;
        //            decimal? high_profit = (((market.high_price ?? 0) - (market.prev_price ?? 0)) / (market.prev_price ?? 0)) * 100;
        //            if ((calc.open_profit ?? 0) <= (open_profit ?? 0))
        //            {
        //                calc.open_profit = open_profit;
        //            }
        //            if ((calc.high_profit ?? 0) <= (high_profit ?? 0))
        //            {
        //                calc.high_profit = high_profit;
        //            }
        //            if ((open_profit ?? 0) > 0 && (high_profit ?? 0) >= (decimal)0.5)
        //            {
        //                calc.success_count += 1;
        //            }
        //            else
        //            {
        //                calc.fail_count += 1;
        //            }
        //            //var prev = (from q in markets
        //            //            where q.symbol == market.symbol
        //            //            && q.trade_date < market.trade_date
        //            //            orderby q.trade_date descending
        //            //            select q).FirstOrDefault();
        //            //if (prev != null)
        //            //{
        //            //    decimal? profit = (((prev.ltp_price ?? 0) - (prev.prev_price ?? 0)) / (prev.prev_price ?? 0)) * 100;
        //            //    int intProfit = 0;
        //            //    intProfit = (int)profit;
        //            //    if (intProfit == 0 && (profit ?? 0) < 0)
        //            //    {
        //            //        intProfit = -100000;
        //            //    }
        //            //    tra_pre_calc_item item = (from q in context.tra_pre_calc_item
        //            //                              where q.percentage == intProfit
        //            //                              select q).FirstOrDefault();
        //            //    if (item == null)
        //            //    {
        //            //        item = new tra_pre_calc_item
        //            //        {
        //            //            success_count = 0,
        //            //            percentage = 0,
        //            //            fail_count = 0,
        //            //        };
        //            //    }
        //            //    item.percentage = intProfit;
        //            //    if ((open_profit ?? 0) > 0)
        //            //    {
        //            //        item.success_count += 1;
        //            //    }
        //            //    else
        //            //    {
        //            //        item.fail_count += 1;
        //            //    }
        //            //    if (item.id <= 0)
        //            //    {
        //            //        context.tra_pre_calc_item.Add(item);
        //            //    }
        //            //    else
        //            //    {
        //            //        context.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //            //    }
        //            //    context.SaveChanges();
        //            //}
        //        }
        //        if (calc.id <= 0)
        //        {
        //            context.tra_prev_calc.Add(calc);
        //        }
        //        else
        //        {
        //            context.Entry(calc).State = System.Data.Entity.EntityState.Modified;
        //        }
        //        context.SaveChanges();
        //        Console.WriteLine("Total=" + total + ",Index=" + index);
        //    }
        //    //int total = markets.Count();
        //    //int index = 0;
        //    //foreach(var market in markets)
        //    //{
        //    //    index += 1;
        //    //    var prev = (from q in markets
        //    //                where q.symbol == market.symbol
        //    //                && q.trade_date < market.trade_date
        //    //                orderby q.trade_date descending
        //    //                select q).FirstOrDefault();
        //    //    if (prev != null)
        //    //    {
        //    //        market.prev_ltp_price = prev.ltp_price;
        //    //        sql = string.Format("update tra_market set prev_ltp_price={0} where market_id={1}", prev.ltp_price, market.id);
        //    //        MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
        //    //        //context.Entry(market).State = System.Data.Entity.EntityState.Modified;
        //    //        //context.SaveChanges();
        //    //    }
        //    //    Console.WriteLine("Total=" + total + ",Index=" + index);
        //    //}
        //}
        //Console.WriteLine("Completed");
        //Console.ReadLine();
        //MutualFunds();
        //CaculateIntraydayProfit();
        //using (EcamContext context = new EcamContext())
        //{
        //    List<tra_market> markets = (from q in context.tra_market
        //                                orderby q.symbol ascending
        //                                select q).ToList();
        //    foreach (var market in markets)
        //    {
        //        tra_market prev = (from q in context.tra_market
        //                           where q.symbol == market.symbol
        //                           && q.trade_date < market.trade_date
        //                           orderby q.trade_date descending
        //                           select q).FirstOrDefault();
        //        if (prev != null)
        //        {
        //            market.prev_price = prev.close_price;
        //            sql = string.Format("update tra_market set prev_price={0} where market_id={1}", prev.close_price, market.id);
        //            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
        //            Console.WriteLine("Update market price symbol=" + market.symbol + ",Date=" + market.trade_date.ToString("MMM/dd/yyyy"));
        //        }
        //    }
        //    List<tra_company> companies = (from q in context.tra_company
        //                                   orderby q.symbol ascending
        //                                   select q).ToList();
        //    foreach (var company in companies)
        //    {
        //        TradeHelper.UpdateCompanyPrice(company.symbol);
        //        Console.WriteLine("Update company price symbol=" + company.symbol);
        //    }
        //}
        //private static void CaculateIntraydayProfit()
        //{
        //    List<tra_market_intra_day> rows;
        //    List<tra_company> companies;
        //    using (EcamContext context = new EcamContext())
        //    {
        //        rows = (from q in context.tra_market_intra_day orderby q.symbol, q.trade_date ascending select q).ToList();
        //        companies = (from q in context.tra_company select q).ToList();
        //        //foreach (var market in rows)
        //        //{
        //        //    DateTime tradeDate = market.trade_date.Date;
        //        //    var prev = (from q in context.tra_market
        //        //                where q.symbol == market.symbol
        //        //                && q.trade_date < tradeDate
        //        //                orderby q.trade_date descending
        //        //                select q).FirstOrDefault();
        //        //    if (prev != null)
        //        //    {
        //        //        TempRSI value = new TempRSI
        //        //        {
        //        //            symbol = market.symbol,
        //        //            close = market.ltp_price,
        //        //            prev = (prev.close_price ?? 0),
        //        //            date = market.trade_date,
        //        //        };
        //        //        value.avg_downward = (((prev.avg_downward ?? 0) * (14 - 1) + value.downward) / 14);
        //        //        value.avg_upward = (((prev.avg_upward ?? 0) * (14 - 1) + value.upward) / 14);
        //        //        //market.avg_upward = value.avg_upward;
        //        //        //market.avg_downward = value.avg_downward;
        //        //        //market.prev_rsi = prev.rsi;
        //        //        market.rsi = value.rsi;
        //        //        context.Entry(market).State = System.Data.Entity.EntityState.Modified;
        //        //        context.SaveChanges();
        //        //    }
        //        //}
        //    }
        //    if (rows.Count > 0)
        //    {
        //        List<string> symbols = (from q in rows select q.symbol).Distinct().ToList();
        //        DateTime firstDate = (from q in rows orderby q.trade_date descending select q).FirstOrDefault().trade_date.Date;
        //        DateTime startTime = Convert.ToDateTime(firstDate.ToString("dd/MMM/yyyy") + " 9:19AM");
        //        DateTime endTime = Convert.ToDateTime(firstDate.ToString("dd/MMM/yyyy") + " 10:00AM");
        //        foreach (string symbol in symbols)
        //        {
        //            var company = (from q in companies where q.symbol == symbol select q).FirstOrDefault();
        //            var firstLTP = (from q in rows where q.symbol == symbol && q.trade_date >= firstDate && q.trade_date <= startTime orderby q.trade_date descending select q).FirstOrDefault();
        //            var lastLTP = (from q in rows where q.symbol == symbol && q.trade_date >= firstDate && q.trade_date <= endTime orderby q.trade_date descending select q).FirstOrDefault();
        //            if (firstLTP != null && lastLTP != null && company != null)
        //            {
        //                try
        //                {
        //                    decimal? firstPrice = firstLTP.ltp_price;
        //                    decimal? lastPrice = lastLTP.ltp_price;
        //                    decimal? finalPrice = company.ltp_price;
        //                    decimal? openPrice = company.open_price;

        //                    decimal? rsi = firstLTP.rsi;
        //                    decimal? prevRSI = 0;
        //                    using(EcamContext context = new EcamContext())
        //                    {
        //                        DateTime dt = firstLTP.trade_date.Date;
        //                        var prev = (from q in context.tra_market
        //                                    where q.symbol == firstLTP.symbol
        //                                    && q.trade_date < dt
        //                                    orderby q.trade_date descending
        //                                    select q).FirstOrDefault();
        //                        if (prev != null)
        //                        {
        //                            prevRSI = prev.rsi;
        //                        }
        //                    }

        //                    List<decimal> percentageList = new List<decimal>();

        //                    var nextList = (from q in rows
        //                                    where q.symbol == symbol && q.trade_date > firstLTP.trade_date
        //                                    orderby q.trade_date ascending
        //                                    select q).ToList();
        //                    foreach (var nextRow in nextList)
        //                    {
        //                        decimal? p = ((nextRow.ltp_price - (firstPrice ?? 0)) / (firstPrice ?? 0)) * 100;
        //                        percentageList.Add((p ?? 0));
        //                    }

        //                    decimal? percentage = 0;
        //                    if (percentageList.Count > 0)
        //                    {
        //                        percentage = (from q in percentageList orderby q descending select q).FirstOrDefault();
        //                    }
        //                    else
        //                    {
        //                        percentage = (((lastPrice ?? 0) - (firstPrice ?? 0)) / (firstPrice ?? 0)) * 100;
        //                    }

        //                    percentageList = new List<decimal>();
        //                    foreach (var nextRow in nextList)
        //                    {
        //                        decimal? p = (((firstPrice ?? 0) - nextRow.ltp_price) / nextRow.ltp_price) * 100;
        //                        percentageList.Add((p ?? 0));
        //                    }

        //                    decimal? reversePercentage = 0;
        //                    if (percentageList.Count > 0)
        //                    {
        //                        reversePercentage = (from q in percentageList orderby q descending select q).FirstOrDefault();
        //                    }
        //                    else
        //                    {
        //                        reversePercentage = (((firstPrice ?? 0) - (lastPrice ?? 0)) / (lastPrice ?? 0)) * 100;
        //                    }

        //                    decimal? finalPercentage = (((finalPrice ?? 0) - (firstPrice ?? 0)) / (firstPrice ?? 0)) * 100;

        //                    decimal? firstPercentage = (((firstPrice ?? 0) - (openPrice ?? 0)) / (openPrice ?? 0)) * 100;

        //                    decimal? lastPercentage = (((lastPrice ?? 0) - (openPrice ?? 0)) / (openPrice ?? 0)) * 100;

        //                    bool isDay1High = (company.day_1 ?? 0) <= (company.ltp_price ?? 0);
        //                    bool isDay2High = (
        //                            (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //                            && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //                            );
        //                    bool isDay3High = (
        //                    (company.day_3 ?? 0) < (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //                    );
        //                    bool isDay4High = (
        //                        (company.day_4 ?? 0) < (company.day_3 ?? 0)
        //                    && (company.day_3 ?? 0) < (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //                    );

        //                    bool isDay5High = (
        //                        (company.day_5 ?? 0) < (company.day_4 ?? 0)
        //                       && (company.day_4 ?? 0) < (company.day_3 ?? 0)
        //                    && (company.day_3 ?? 0) < (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //                    );

        //                    bool isDay1Low = (company.day_1 ?? 0) >= (company.ltp_price ?? 0);
        //                    bool isDay2Low = (
        //                            (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //                            && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //                            );
        //                    bool isDay3Low = (
        //                    (company.day_3 ?? 0) > (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //                    );
        //                    bool isDay4Low = (
        //                        (company.day_4 ?? 0) > (company.day_3 ?? 0)
        //                    && (company.day_3 ?? 0) > (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //                    );
        //                    bool isDay5Low = (
        //                        (company.day_5 ?? 0) > (company.day_4 ?? 0)
        //                        && (company.day_4 ?? 0) > (company.day_3 ?? 0)
        //                    && (company.day_3 ?? 0) > (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //                    );
        //                    using (EcamContext context = new EcamContext())
        //                    {
        //                        tra_intra_day_profit profit = (from q in context.tra_intra_day_profit
        //                                                       where q.symbol == symbol
        //                                                       && q.trade_date == firstDate
        //                                                       select q).FirstOrDefault();
        //                        bool isNew = false;
        //                        if (profit == null)
        //                        {
        //                            profit = new tra_intra_day_profit();
        //                            isNew = true;
        //                        }
        //                        profit.symbol = symbol;
        //                        profit.trade_date = firstDate;
        //                        profit.profit_percentage = (percentage ?? 0);
        //                        profit.reverse_percentage = (reversePercentage ?? 0);
        //                        profit.last_percentage = (lastPercentage ?? 0);
        //                        profit.first_percentage = (firstPercentage ?? 0);
        //                        profit.final_percentage = (finalPercentage ?? 0);
        //                        profit.rsi = rsi;
        //                        profit.prev_rsi = prevRSI;
        //                        profit.diff_rsi = (rsi ?? 0) - (prevRSI ?? 0);
        //                        int highCnt = 0;
        //                        int lowCnt = 0;
        //                        if (isDay1High == true && isDay2High == true
        //                            && isDay3High == true && isDay4High == true
        //                            && isDay5High == true)
        //                        {
        //                            highCnt = 5;
        //                        }
        //                        else if (isDay1High == true && isDay2High == true
        //                            && isDay3High == true && isDay4High == true
        //                            )
        //                        {
        //                            highCnt = 4;
        //                        }
        //                        else if (isDay1High == true && isDay2High == true
        //                            && isDay3High == true
        //                            )
        //                        {
        //                            highCnt = 3;
        //                        }
        //                        else if (isDay1High == true && isDay2High == true
        //                            )
        //                        {
        //                            highCnt = 2;
        //                        }
        //                        else if (isDay1High == true)
        //                        {
        //                            highCnt = 1;
        //                        }

        //                        if (isDay1Low == true && isDay2Low == true
        //                            && isDay3Low == true && isDay4Low == true
        //                            && isDay5Low == true)
        //                        {
        //                            lowCnt = 5;
        //                        }
        //                        else if (isDay1Low == true && isDay2Low == true
        //                            && isDay3Low == true && isDay4Low == true
        //                            )
        //                        {
        //                            lowCnt = 4;
        //                        }
        //                        else if (isDay1Low == true && isDay2Low == true
        //                            && isDay3Low == true
        //                            )
        //                        {
        //                            lowCnt = 3;
        //                        }
        //                        else if (isDay1Low == true && isDay2Low == true
        //                            )
        //                        {
        //                            lowCnt = 2;
        //                        }
        //                        else if (isDay1Low == true)
        //                        {
        //                            lowCnt = 1;
        //                        }
        //                        profit.high_count = highCnt;
        //                        profit.low_count = lowCnt;

        //                        if (isNew == true)
        //                        {
        //                            context.tra_intra_day_profit.Add(profit);
        //                        }
        //                        else
        //                        {
        //                            context.Entry(profit).State = System.Data.Entity.EntityState.Modified;
        //                        }
        //                        context.SaveChanges();

        //                        var updateCompany = (from q in context.tra_company where q.symbol == symbol select q).FirstOrDefault();
        //                        if (updateCompany != null)
        //                        {
        //                            updateCompany.high_count = highCnt;
        //                            updateCompany.low_count = lowCnt;
        //                            context.Entry(updateCompany).State = System.Data.Entity.EntityState.Modified;
        //                            context.SaveChanges();
        //                        }
        //                        Console.WriteLine("Calculate profit completed symbol=" + symbol);
        //                    }
        //                }
        //                catch { }
        //            }
        //        }
        //    }
        //}

        private static int _INDEX = -1;
        private static string[] _COMPANIES;
        private static void GoogleData()
        {
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                IQueryable<tra_company> query = context.tra_company;

                DateTime morningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 9:15AM");
                DateTime morningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 10:15AM");
                DateTime eveningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:31PM");
                DateTime eveningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 11:59PM");
                DateTime now = DateTime.Now;
                DateTime targetTime = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:30PM");

                if ((now >= morningStart && now <= eveningStart))
                {
                    //query = (from q in query
                    //         where (q.prev_rsi ?? 0) >= 70 || (q.prev_rsi ?? 0) <= 30
                    //         select q);
                    //string IS_NIFTY_50 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_50"];
                    //string IS_NIFTY_100 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_100"];
                    //string IS_NIFTY_200 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_200"];
                    //if (IS_NIFTY_50 == "true")
                    //{
                    //    query = query.Where(q => (q.is_nifty_50 ?? false) == true);
                    //}
                    //if (IS_NIFTY_100 == "true")
                    //{
                    //    query = query.Where(q => (q.is_nifty_100 ?? false) == true);
                    //}
                    //if (IS_NIFTY_200 == "true")
                    //{
                    //    query = query.Where(q => (q.is_nifty_200 ?? false) == true);
                    //}
                }
                companies = (from q in query
                             orderby (q.prev_rsi ?? 0) descending
                             select q).ToList();
            }
            _COMPANIES = (from q in companies select q.symbol).ToArray();
            _INDEX = -1;

            string sql = "";
            // Reset book mark
            //string sql = "update tra_company set is_book_mark=0;";
            //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);

            //sql = "update tra_company set is_book_mark=1 where ifnull(prev_rsi,0)>=70 or ifnull(prev_rsi,0)<=30;";
            //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);

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
            if (totalCount <= queueCount)
            {
                queueCount = totalCount;
            }
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
                IQueryable<tra_company> query = context.tra_company;
                string SYMBOLS = System.Configuration.ConfigurationManager.AppSettings["SYMBOLS"];
                if (string.IsNullOrEmpty(SYMBOLS) == false)
                {
                    List<string> symbolList = Helper.ConvertStringList(SYMBOLS);
                    if (symbolList.Count > 0)
                    {
                        query = (from q in query where symbolList.Contains(q.symbol) == true select q);
                    }
                }
                companies = (from q in query orderby q.symbol ascending select q).ToList();
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


}
