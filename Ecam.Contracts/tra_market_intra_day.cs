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
    public class TRA_MARKET_INTRA_DAY
    {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public decimal ltp_price { get; set; }

        public decimal open_price { get; set; }
        public decimal ltp_percentage { get; set; }
        public string time { get; set; }
        public int company_id { get; set; }
    }
}

