 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_rsi_profit : BaseEntity<tra_rsi_profit> {
								        public string symbol { get; set; }
				        public Nullable<decimal> buy_rsi { get; set; }
				        public Nullable<decimal> sell_rsi { get; set; }
				        public Nullable<decimal> buy_price { get; set; }
				        public Nullable<decimal> sell_price { get; set; }
				        public Nullable<System.DateTime> buy_date { get; set; }
				        public Nullable<System.DateTime> sell_date { get; set; }
				        public Nullable<decimal> profit { get; set; }
    }
}
