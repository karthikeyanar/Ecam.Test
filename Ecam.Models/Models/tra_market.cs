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
    }
}
