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
    public class TRA_HOLDING : BaseContract
    {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public int quantity { get; set; }
        public decimal? avg_price { get; set; }

        public string company_name { get; set; }
        public decimal? ltp_price { get; set; }

        public decimal? investment {
            get {
                return (this.quantity) * (this.avg_price ?? 0);
            }
        }

        public decimal? current_market_value {
            get {
                return (this.quantity) * (this.ltp_price ?? 0);
            }
        }

        public decimal? change_percentage { get; set; }
    }

    public class TRA_HOLDING_SEARCH : TRA_HOLDING
    {
        public string symbols { get; set; }
    }
}

