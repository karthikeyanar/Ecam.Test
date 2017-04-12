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
    public class TRA_EXPERTS_RECOMMEND : BaseContract
    {
        public int experts_id { get; set; }
        public System.DateTime record_date { get; set; }
        public int company_id { get; set; }
        public string recommend_type { get; set; }
        public Nullable<decimal> stop_loss { get; set; }
        public Nullable<decimal> target { get; set; }
        public string url { get; set; }

        public string experts_name { get; set; }
        public string company_name { get; set; }
    }

    public class TRA_EXPERTS_RECOMMEND_SEARCH : TRA_EXPERTS_RECOMMEND
    {
    }
}

