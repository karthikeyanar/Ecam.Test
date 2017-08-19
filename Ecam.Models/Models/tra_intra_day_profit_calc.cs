 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_intra_day_profit_calc : BaseEntity<tra_intra_day_profit_calc> {
								        public System.DateTime trade_date { get; set; }
				        public Nullable<decimal> profit { get; set; }
				        public Nullable<decimal> loss { get; set; }
    }
}
