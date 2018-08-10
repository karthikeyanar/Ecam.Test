using Ecam.Framework;
using Ecam.Framework.ExcelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Contracts {
    public class TRA_MARKET {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> high_price { get; set; }
        public Nullable<decimal> low_price { get; set; }
        public Nullable<decimal> ltp_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> prev_price { get; set; }
        public Nullable<decimal> rsi { get; set; }
        public Nullable<decimal> prev_rsi { get; set; }
        public Nullable<decimal> upward { get; set; }
        public Nullable<decimal> downward { get; set; }
        public Nullable<decimal> avg_upward { get; set; }
        public Nullable<decimal> avg_downward { get; set; }
        public Nullable<decimal> rs { get; set; }
        public Nullable<decimal> prev_ltp_price { get; set; }
        public Nullable<decimal> turn_over { get; set; }
        public Nullable<decimal> percentage { get; set; }
        public string super_trend_signal { get; set; }
        public string macd_signal { get; set; }
        public Nullable<decimal> macd { get; set; }
        public Nullable<decimal> sp_profit { get; set; }
        public Nullable<bool> is_indicator { get; set; }
        public Nullable<decimal> macd_histogram { get; set; }
        public string heikin_ashi_signal { get; set; }
        public Nullable<decimal> heikin_ashi_profit { get; set; }
        public Nullable<decimal> ema_5 { get; set; }
        public Nullable<decimal> ema_20 { get; set; }
        public Nullable<decimal> ema_cross { get; set; }
        public Nullable<decimal> ema_profit { get; set; }
        public string ema_signal { get; set; }
        public Nullable<decimal> ema_min_profit { get; set; }
        public Nullable<decimal> ema_max_profit { get; set; }
        public Nullable<int> ema_cnt { get; set; }
        public Nullable<int> ema_increase { get; set; }
        public Nullable<decimal> ema_increase_profit { get; set; }
        public Nullable<decimal> ema_min_cross { get; set; }

        public Nullable<bool> is_archive { get; set; }
        public Nullable<bool> is_book_mark { get; set; }
        public Nullable<decimal> current_price { get; set; }
        public Nullable<DateTime> last_date { get; set; }
        public string company_name { get; set; } 
        public string money_control_url { get; set; }

        public decimal current_profit {
            get {
                if((this.close_price ?? 0) > 0)
                    return (((this.current_price ?? 0) - (this.close_price ?? 0)) / (this.close_price ?? 0)) * 100;
                else
                    return 0;
            }
        }
        public double diff_days {
            get {
                DateTime minDate = Convert.ToDateTime("01/01/1900");
                TimeSpan ts = (this.last_date ?? minDate) - (this.trade_date);
                return ts.TotalDays;
            }
        }
    }

    public class TRA_MARKET_SEARCH:TRA_MARKET {
        public string symbols { get; set; }
        public string categories { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
    }
}

