 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_split : BaseEntity<tra_split> {
								        public string symbol { get; set; }
				        public System.DateTime split_date { get; set; }
				        public decimal split_factor { get; set; }
    }
}
