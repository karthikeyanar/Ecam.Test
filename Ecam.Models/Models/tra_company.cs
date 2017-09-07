 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_company : BaseEntity<tra_company> {
								        public string company_name { get; set; }
				        public string symbol { get; set; }
				        public Nullable<decimal> open_price { get; set; }
				        public Nullable<decimal> high_price { get; set; }
				        public Nullable<decimal> low_price { get; set; }
				        public Nullable<decimal> ltp_price { get; set; }
				        public Nullable<decimal> close_price { get; set; }
				        public Nullable<decimal> prev_price { get; set; }
				        public Nullable<decimal> week_52_high { get; set; }
				        public Nullable<decimal> week_52_low { get; set; }
				        public Nullable<bool> is_book_mark { get; set; }
				        public Nullable<bool> is_nifty_50 { get; set; }
				        public Nullable<bool> is_nifty_100 { get; set; }
				        public Nullable<bool> is_nifty_200 { get; set; }
				        public Nullable<decimal> rsi { get; set; }
				        public Nullable<decimal> prev_rsi { get; set; }
				        public Nullable<int> mf_cnt { get; set; }
				        public Nullable<decimal> mf_qty { get; set; }
				        public Nullable<decimal> monthly_avg { get; set; }
				        public Nullable<decimal> weekly_avg { get; set; }
				        public Nullable<bool> is_current_stock { get; set; }
    }
}
