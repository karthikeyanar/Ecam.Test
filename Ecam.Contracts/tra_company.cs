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
    public class TRA_COMPANY : BaseContract
    {
        public string company_name { get; set; }
        public string symbol { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> high_price { get; set; }
        public Nullable<decimal> low_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> prev_price { get; set; }
        public Nullable<decimal> week_52_high { get; set; }
        public Nullable<decimal> ltp_price { get; set; }
        public Nullable<bool> is_book_mark { get; set; }
        public Nullable<decimal> week_52_low { get; set; }
        public Nullable<decimal> day_5 { get; set; }
        public Nullable<decimal> day_10 { get; set; }
        public Nullable<decimal> day_15 { get; set; }
        public Nullable<decimal> day_20 { get; set; }
        public Nullable<decimal> day_25 { get; set; }
        public Nullable<decimal> day_30 { get; set; }
        public Nullable<decimal> day_35 { get; set; }
        public Nullable<decimal> day_40 { get; set; }
        public Nullable<decimal> day_45 { get; set; }
        public Nullable<decimal> day_50 { get; set; }
        public Nullable<decimal> day_55 { get; set; }
        public Nullable<decimal> day_60 { get; set; }
        public Nullable<decimal> day_65 { get; set; }
        public Nullable<decimal> day_70 { get; set; }
        public Nullable<decimal> day_75 { get; set; }
        public Nullable<decimal> day_80 { get; set; }
        public Nullable<decimal> day_85 { get; set; }
        public Nullable<decimal> day_90 { get; set; }
        public Nullable<bool> is_nifty_50 { get; set; }
        public Nullable<bool> is_nifty_100 { get; set; }
        public Nullable<bool> is_nifty_200 { get; set; }

        public string category_name { get; set; }
        public Nullable<decimal> prev_percentage { get; set; }
        public Nullable<decimal> day_5_percentage { get; set; }
        public Nullable<decimal> day_10_percentage { get; set; }
        public Nullable<decimal> day_15_percentage { get; set; }
        public Nullable<decimal> day_20_percentage { get; set; }
        public Nullable<decimal> day_25_percentage { get; set; }
        public Nullable<decimal> day_30_percentage { get; set; }
        public Nullable<decimal> day_35_percentage { get; set; }
        public Nullable<decimal> day_40_percentage { get; set; }
        public Nullable<decimal> day_45_percentage { get; set; }
        public Nullable<decimal> day_50_percentage { get; set; }
        public Nullable<decimal> day_55_percentage { get; set; }
        public Nullable<decimal> day_60_percentage { get; set; }
        public Nullable<decimal> day_65_percentage { get; set; }
        public Nullable<decimal> day_70_percentage { get; set; }
        public Nullable<decimal> day_75_percentage { get; set; }
        public Nullable<decimal> day_80_percentage { get; set; }
        public Nullable<decimal> day_85_percentage { get; set; }
        public Nullable<decimal> day_90_percentage { get; set; }

        public Nullable<decimal> week_52_low_percentage { get; set; }
        public Nullable<decimal> week_52_percentage { get; set; }

        public List<string> category_list { get; set; }
    }

    public class TRA_COMPANY_SEARCH : TRA_COMPANY
    {
        public string symbols { get; set; }
        public string categories { get; set; }
        public decimal? from_price { get; set; }
        public decimal? to_price { get; set; }
    }
}

