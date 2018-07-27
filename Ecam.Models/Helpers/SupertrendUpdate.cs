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
                               close_price = q.close_price
                           }).ToArray();
            }
            ATR atr = new ATR();
            atr.Calculate(candles);
            Supertrend supertrend = new Supertrend();
            supertrend.Calculate(candles);
            candles = (from q in candles
                       where string.IsNullOrEmpty(q.super_trend_signal) == false
                       select q).ToArray();
            string sql = string.Empty;
            for(int i = 0;i < candles.Length;i++) {
                sql = string.Format("update tra_market set super_trend={0},super_trend_signal='{1}' where trade_date='{2}' and symbol='{3}'",decimal.Round(candles[i].super_trend,4),candles[i].super_trend_signal,candles[i].trade_date.ToString("yyyy-MM-dd"),candles[i].symbol);
                MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
            }
            Console.WriteLine("Completed symbol=" + symbol);
        }

        public string file_name { get { return _SYMBOL; } }
        private string _SYMBOL;
        private string _ORIGINAL_SYMBOL;

        private ManualResetEvent _doneEvent;
    }

}