 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_market : BaseEntity<tra_market> {
								        public string symbol { get; set; }
				        public System.DateTime trade_date { get; set; }
				        public string trade_type { get; set; }
				        public Nullable<decimal> open_price { get; set; }
				        public Nullable<decimal> high_price { get; set; }
				        public Nullable<decimal> low_price { get; set; }
				        public Nullable<decimal> close_price { get; set; }
				        public Nullable<decimal> prev_price { get; set; }
				        public Nullable<decimal> ltp_price { get; set; }
    }
}
