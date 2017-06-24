 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
	public partial class tra_intra_day_profit {
				        public string symbol { get; set; }
				        public System.DateTime trade_date { get; set; }
				        public Nullable<decimal> first_percentage { get; set; }
				        public Nullable<decimal> last_percentage { get; set; }
				        public Nullable<decimal> profit_percentage { get; set; }
				        public Nullable<decimal> reverse_percentage { get; set; }
				        public Nullable<decimal> final_percentage { get; set; }
				        public Nullable<int> high_count { get; set; }
				        public Nullable<int> low_count { get; set; }
    }
}
