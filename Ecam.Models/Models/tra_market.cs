using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_market {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> high_price { get; set; }
        public Nullable<decimal> low_price { get; set; }
        public Nullable<decimal> ltp_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> prev_price { get; set; }
        public string super_trend_signal { get; set; }
        public Nullable<bool> is_indicator { get; set; }
        public Nullable<decimal> ema_50 { get; set; }
        public Nullable<decimal> ema_200 { get; set; }
        public Nullable<decimal> ema_cross { get; set; }
        public Nullable<decimal> ema_profit { get; set; }
        public string ema_signal { get; set; }
        public Nullable<decimal> ema_min_profit { get; set; }
        public Nullable<decimal> ema_max_profit { get; set; }
        public Nullable<int> ema_cnt { get; set; }
        public Nullable<int> ema_increase { get; set; }
        public Nullable<decimal> ema_increase_profit { get; set; }
        public Nullable<decimal> ema_min_cross { get; set; }
    }
}
