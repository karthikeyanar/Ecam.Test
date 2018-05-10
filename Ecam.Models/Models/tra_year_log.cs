using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_year_log {
        public string symbol { get; set; }
        public int year { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> percentage { get; set; }
    }
}
