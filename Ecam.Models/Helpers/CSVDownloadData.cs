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
    public class CSVDownloadData
    {
        public CSVDownloadData(string symbol, ManualResetEvent doneEvent)
        {
            _OriginalSymbol = symbol;
            _Symbol = symbol;
            _doneEvent = doneEvent;
        }

        public CSVDownloadData()
        {
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...", threadIndex);
            if (string.IsNullOrEmpty(_Symbol) == false)
            {
                CSVDataDownload(_Symbol);
                //CalculateRSI(_Symbol);
            }
            Console.WriteLine("thread {0} result calculated...", threadIndex);
            string IMPORT_CSV = System.Configuration.ConfigurationManager.AppSettings["IMPORT_CSV"];
            string fileName = IMPORT_CSV + "\\" + _OriginalSymbol + ".csv";
            if (File.Exists(fileName) == true)
            {
                File.Delete(fileName);
            }
            _doneEvent.Set();
        }

        private void CSVDataDownload(string tempSymbol)
        {
            if (string.IsNullOrEmpty(tempSymbol) == false)
            {
                string url = string.Empty;
                string html = string.Empty;
                string IMPORT_CSV = System.Configuration.ConfigurationManager.AppSettings["IMPORT_CSV"];
                string fileName = IMPORT_CSV + "\\" + tempSymbol + ".csv";
                CsvReader csv = null;
                int i = 0;
                if (File.Exists(fileName) == true)
                {
                    using (TextReader reader = File.OpenText(fileName))
                    {
                        csv = new CsvReader(reader);
                        i = 0;
                        while (csv.Read())
                        {
                            i += 1;
                            string symbol = csv.GetField<string>("Symbol");
                            string series = csv.GetField<string>("Series");
                            string date = csv.GetField<string>("Date");
                            string open = csv.GetField<string>("Open Price");
                            string high = csv.GetField<string>("High Price");
                            string low = csv.GetField<string>("Low Price");
                            string close = csv.GetField<string>("Close Price");
                            string lastTrade = csv.GetField<string>("Last Price");
                            string prev = csv.GetField<string>("Prev Close");
                            DateTime dt = DataTypeHelper.ToDateTime(date);
                            if (string.IsNullOrEmpty(symbol) == false
                                && series == "EQ")
                            {
                                _Symbol = symbol;
                                TradeHelper.ImportPrice(new TempClass
                                {
                                    symbol = symbol,
                                    trade_date = dt,
                                    close_price = DataTypeHelper.ToDecimal(close),
                                    high_price = DataTypeHelper.ToDecimal(high),
                                    low_price = DataTypeHelper.ToDecimal(low),
                                    open_price = DataTypeHelper.ToDecimal(open),
                                    ltp_price = DataTypeHelper.ToDecimal(lastTrade),
                                    prev_price = DataTypeHelper.ToDecimal(prev),
                                    is_prev_price_exist = true
                                });
                            }
                        }
                    }
                }
            }
        }

        /*
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
                                   //&& q.trade_date <= today
                                   //&& q.trade_date >= endDate
                                   orderby q.trade_date ascending
                                   select q).ToList();
                    foreach (var market in markets)
                    {
                        CalculateRSI(markets, market.trade_date, market.symbol);
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

        public static void CalculateRSI(List<tra_market> fullMarkets, DateTime date, string symbol)
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
                                      where q.symbol == symbol
                                      && q.date == date
                                      select q).FirstOrDefault();
                if (existValue != null)
                {
                    using (EcamContext context = new EcamContext())
                    {
                        var market = (from q in context.tra_market where q.symbol == existValue.symbol && q.trade_date == existValue.date select q).FirstOrDefault();
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
                            var preMarket = (from q in markets
                                             where q.symbol == existValue.symbol
                           && q.trade_date == existValue.date
                                             select q).FirstOrDefault();
                            preMarket.avg_downward = existValue.avg_downward;
                            preMarket.avg_upward = existValue.avg_upward;
                            preMarket.upward = existValue.upward;
                            preMarket.downward = existValue.downward;
                            preMarket.rs = existValue.rs;
                            preMarket.rsi = existValue.rsi;
                            Console.WriteLine("Update RSI Symbol=" + market.symbol + ",Date=" + market.trade_date.ToString("dd-MMM-yyy") + ",symbol=" + market.symbol + ",rsi=" + existValue.rsi);
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
        */

        public string SYMBOL { get { return _Symbol; } }
        private string _Symbol;
        private string _OriginalSymbol;

        private ManualResetEvent _doneEvent;
    }
}