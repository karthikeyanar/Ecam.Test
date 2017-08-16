 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_prev_calc : BaseEntity<tra_prev_calc> {
								        public string symbol { get; set; }
				        public Nullable<int> positive_count { get; set; }
				        public Nullable<int> negative_count { get; set; }
				        public Nullable<decimal> open_profit { get; set; }
				        public Nullable<decimal> high_profit { get; set; }
				        public Nullable<int> success_count { get; set; }
				        public Nullable<int> fail_count { get; set; }
    }
}
