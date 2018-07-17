using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_financial {
        public string symbol { get; set; }
        public int financial_category_id { get; set; }
        public System.DateTime financial_date { get; set; }
        public decimal value { get; set; }
        public decimal prev_value { get; set; }
    }
}
