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

        public Nullable<decimal> prev_percentage { get; set; }
        public Nullable<decimal> week_52_percentage { get; set; }
        public Nullable<decimal> week_52_low_percentage { get; set; }

        public Nullable<decimal> last_5_day_price { get; set; }
        public Nullable<decimal> last_5_day_percentage { get; set; }
        public Nullable<decimal> last_1_month_price { get; set; }
        public Nullable<decimal> last_1_month_percentage { get; set; }
        public Nullable<decimal> last_2_month_price { get; set; }
        public Nullable<decimal> last_2_month_percentage { get; set; }
        public Nullable<decimal> last_3_month_price { get; set; }
        public Nullable<decimal> last_3_month_percentage { get; set; }
        public string category_name { get; set; }
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

