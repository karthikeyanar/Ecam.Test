 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
	public partial class tra_mutual_fund_pf {
				        public int fund_id { get; set; }
				        public string symbol { get; set; }
				        public Nullable<decimal> quantity { get; set; }
				        public Nullable<decimal> stock_value { get; set; }
				        public Nullable<decimal> stock_percentage { get; set; }
				        public string stock_name { get; set; }
    }
}
