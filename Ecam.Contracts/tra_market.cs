﻿using Ecam.Framework;
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

        public Nullable<bool> is_archive { get; set; }
        public Nullable<bool> is_book_mark { get; set; }
        public Nullable<decimal> current_price { get; set; }
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
    }

    public class TRA_MARKET_SEARCH:TRA_MARKET {
        public string symbols { get; set; }
        public string categories { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
    }
}

