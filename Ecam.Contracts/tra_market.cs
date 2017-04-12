using Ecam.Framework;
using Ecam.Framework.ExcelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Contracts
{
    public class TRA_MARKET : BaseContract
    {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public string trade_type { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> high_price { get; set; }
        public Nullable<decimal> low_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> prev_price { get; set; }
        public Nullable<decimal> week_52_high { get; set; }
        public Nullable<decimal> months_3_high { get; set; }
        public Nullable<decimal> months_1_high { get; set; }
        public Nullable<decimal> day_5_high { get; set; }

        public string company_name { get; set; }
        public Nullable<decimal> prev_percentage { get; set; }
        public Nullable<decimal> week_52_percentage { get; set; }
        public Nullable<decimal> months_3_percentage { get; set; }
        public Nullable<decimal> months_1_percentage { get; set; }
        public Nullable<decimal> day_5_percentage { get; set; }
    }

    public class TRA_MARKET_SEARCH : TRA_MARKET
    {
        public string symbols { get; set; }
        public string categories { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
    }
}

