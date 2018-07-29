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

namespace Ecam.Models {
    public class SupertrendData {
        private List<string> _SYMBOLS_LIST = null;
        private bool _IS_NOT_SUCCESS = false;
        public SupertrendData(string file_name,List<string> symbols,ManualResetEvent doneEvent) {
            _SYMBOLS_LIST = symbols;
            _ORIGINAL_SYMBOL = file_name;
            _SYMBOL = file_name;
            _doneEvent = doneEvent;
        }

        public SupertrendData() {
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext) {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...",threadIndex);
            if(string.IsNullOrEmpty(_SYMBOL) == false) {
                Update(_SYMBOL);
            }
            Console.WriteLine("thread {0} result calculated...",threadIndex);
            _doneEvent.Set();
        }

        private void Update(string symbol) {
            Price[] candles = null;
            using(EcamContext context = new EcamContext()) {
                candles = (from q in context.tra_market
                           where q.symbol == symbol
                           select new Price {
                               symbol = q.symbol,
                               trade_date = q.trade_date,
                               high_price = q.high_price,
                               low_price = q.low_price,
                               open_price = q.open_price,
                               close_price = q.close_price,
                               super_trend_signal = q.super_trend_signal,
                               macd_signal = q.macd_signal,
                               macd = q.macd,
                               sp_profit = q.sp_profit,
                               is_indicator = q.is_indicator,
                               macd_histogram = q.macd_histogram,
                           }).ToArray();
            }
            ATR atr = new ATR();
            atr.Calculate(candles);
            MACD macd = new MACD();
            macd.Calculate(candles);
            Supertrend supertrend = new Supertrend();
            supertrend.Calculate(candles);
            DateTime minDate = Convert.ToDateTime("01/01/1900");
            //candles = (from q in candles
            //           where string.IsNullOrEmpty(q.super_trend_signal) == false
            //           select q).ToArray();
            string sql = string.Empty;
            for(int i = 0;i < candles.Length;i++) {
                if(
                    ((candles[i].super_trend_signal == "B" || candles[i].super_trend_signal == "S" || candles[i].macd_signal == "B" || candles[i].macd_signal == "S"))
                    && (candles[i].is_indicator ?? false) == false
                    ) {
                    if(candles[i].super_trend_signal == "B") {
                        var candle = (from q in candles
                                      where q.trade_date > candles[i].trade_date
                                      && q.super_trend_signal == "S"
                                      orderby q.trade_date ascending
                                      select q).FirstOrDefault();
                        if(candle == null) {
                            candle = (from q in candles
                                      where q.trade_date > candles[i].trade_date
                                      orderby q.trade_date ascending
                                      select q).FirstOrDefault();
                        }
                        if(candle != null) {
                            candles[i].sp_sell_date = candle.trade_date;
                            //var max = (from q in candles
                            //           where q.trade_date > candles[i].trade_date
                            //           && q.trade_date <= candle.trade_date
                            //           orderby q.close_price descending
                            //           select q).FirstOrDefault();
                            //if(max != null) {
                            //    candles[i].sp_max_profit = (((max.close_price ?? 0) - (candles[i].close_price ?? 0)) / (candles[i].close_price ?? 0)) * 100;
                            //}
                            //var min = (from q in candles
                            //           where q.trade_date > candles[i].trade_date
                            //           && q.trade_date <= candle.trade_date
                            //           orderby q.close_price ascending
                            //           select q).FirstOrDefault();
                            //if(min != null) {
                            //    candles[i].sp_min_profit = (((min.close_price ?? 0) - (candles[i].close_price ?? 0)) / (candles[i].close_price ?? 0)) * 100;
                            //}
                        }
                    } else {
                        var candle = (from q in candles
                                      where q.trade_date < candles[i].trade_date
                                      && q.super_trend_signal == "B"
                                      orderby q.trade_date descending
                                      select q).FirstOrDefault();
                        if(candle != null) {
                            candles[i].sp_profit = (((candles[i].close_price ?? 0) - (candle.close_price ?? 0)) / (candle.close_price ?? 0)) * 100;
                        }
                        candle = (from q in candles
                                  where q.trade_date < candles[i].trade_date
                                  && (q.super_trend_signal == "B" || q.super_trend_signal == "S")
                                  orderby q.trade_date descending
                                  select q).FirstOrDefault();
                        if(candle != null) {
                            candles[i].super_trend_signal = candle.super_trend_signal;
                        }
                    }
                    sql = string.Format("update tra_market set super_trend_signal='{0}',macd_signal='{1}',macd={2},sp_profit={3},macd_histogram={4},is_indicator={5} " +
                        " where trade_date='{6}' and symbol='{7}'"
                        ,candles[i].super_trend_signal
                        ,candles[i].macd_signal
                        ,(candles[i].macd ?? 0)
                        ,(candles[i].sp_profit ?? 0)
                        ,(candles[i].macd_histogram ?? 0)
                        ,1
                        ,candles[i].trade_date.ToString("yyyy-MM-dd")
                        ,candles[i].symbol);
                    MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
                }
            }
            Console.WriteLine("Completed symbol=" + symbol);
        }

        public string file_name { get { return _SYMBOL; } }
        private string _SYMBOL;
        private string _ORIGINAL_SYMBOL;

        private ManualResetEvent _doneEvent;
    }

}