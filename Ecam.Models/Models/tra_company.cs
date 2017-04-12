using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models
{
    public partial class tra_company : BaseEntity<tra_company>
    {
        public string company_name { get; set; }
        public string symbol { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> high_price { get; set; }
        public Nullable<decimal> low_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> prev_price { get; set; }
        public Nullable<decimal> week_52_high { get; set; }
        public Nullable<decimal> months_3_high { get; set; }
        public Nullable<decimal> months_1_high { get; set; }
        public Nullable<decimal> day_5_high { get; set; }
        public Nullable<decimal> ltp_price { get; set; }
    }
}
