using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models
{
    public partial class tra_company : BaseEntity<tra_company>
    {
        public string company_name { get; set; }
        public string symbol { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> high_price { get; set; }
        public Nullable<decimal> low_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> prev_price { get; set; }
        public Nullable<decimal> week_52_high { get; set; }
        public Nullable<decimal> ltp_price { get; set; }
        public Nullable<bool> is_book_mark { get; set; }
        public Nullable<decimal> week_52_low { get; set; }
        public Nullable<decimal> day_5 { get; set; }
        public Nullable<decimal> day_10 { get; set; }
        public Nullable<decimal> day_15 { get; set; }
        public Nullable<decimal> day_20 { get; set; }
        public Nullable<decimal> day_25 { get; set; }
        public Nullable<decimal> day_30 { get; set; }
        public Nullable<decimal> day_35 { get; set; }
        public Nullable<decimal> day_60 { get; set; }
        public Nullable<bool> is_nifty_50 { get; set; }
        public Nullable<bool> is_nifty_100 { get; set; }
        public Nullable<bool> is_nifty_200 { get; set; }
    }
}
