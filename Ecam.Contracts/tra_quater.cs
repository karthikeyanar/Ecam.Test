using Ecam.Framework;
using Ecam.Framework.ExcelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Contracts {

    public class TRA_COMPANY_QUATER {
        public string company_name { get; set; }
        public string symbol { get; set; }
        public string period { get; set; }
        public DateTime? quater_first_date { get; set; }
        public DateTime? quater_last_date { get; set; }
        public DateTime? next_quater_first_date { get; set; }
        public DateTime? next_quater_last_date { get; set; }
        public decimal? value { get; set; }
        public decimal? prev_value { get; set; }
        public decimal? percentage {
            get {
                if((this.prev_value ?? 0) > 0)
                    return (((this.value ?? 0) - (this.prev_value ?? 0)) / (this.prev_value ?? 0)) * 100;
                else
                    return 0;
            }
        }
        public int index { get; set; }
    }

    public class TRA_COMPANY_QUATER_SUMMARY {
        public int page;
        public int total;
        public List<string> columns { get; set; }
        public List<TRA_COMPANY_QUATER_SUMMARY_DETAIL> rows = new List<TRA_COMPANY_QUATER_SUMMARY_DETAIL>();
    }
    public class TRA_COMPANY_QUATER_SUMMARY_DETAIL {
        public string company_name { get; set; }
        public string symbol { get; set; }
        public int index { get; set; }
        public List<TRA_COMPANY_QUATER> cell = new List<TRA_COMPANY_QUATER>();
    }
}

