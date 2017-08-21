 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_market_avg : BaseEntity<tra_market_avg> {
								        public string symbol { get; set; }
				        public string avg_type { get; set; }
				        public System.DateTime avg_date { get; set; }
				        public decimal percentage { get; set; }
    }
}
