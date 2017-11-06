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
        public decimal? target_price {
            get {
                return ((this.target_percentage ?? 0) > 0 ? (((this.avg_price ?? 0) * (this.target_percentage ?? 0)) / 100) + (this.avg_price ?? 0) : 0);
            }
        }

        public decimal? investment { get; set; }
        public decimal? current_market_value { get; set; }

        public decimal profit {
            get {
                decimal result = 0;
                decimal buy = (this.quantity * (this.avg_price ?? 0));
                decimal sell = (this.quantity * (this.ltp_price ?? 0));
                result = this.CalcTotal(buy, sell, (buy + sell));
                return result;
            }
        }
        public decimal? final_total {
            get {
                return (this.investment ?? 0) + this.profit;
            }
        }

        public decimal target_profit {
            get {
                decimal result = 0;
                decimal buy = (this.quantity * (this.avg_price ?? 0));
                decimal sell = (this.quantity * (this.target_price ?? 0));
                result = this.CalcTotal(buy, sell, (buy + sell));
                return result;
            }
        }
        public decimal? target_total {
            get {
                return (this.investment ?? 0) + this.profit;
            }
        }

        public decimal? change_value { get; set; }
        public decimal? change_percentage { get; set; }
        public decimal? target_percentage { get; set; }

        public decimal CalcTotal(decimal buy, decimal sell, decimal turnover)
        {
            decimal result = 0;
            decimal stt = (decimal)(turnover * (decimal)0.1) / 100;
            decimal txn = (decimal)(turnover * (decimal)0.00325) / 100;
            decimal gst = (decimal)(txn * (decimal)18) / 100;
            decimal stamb = (decimal)(turnover * (decimal)0.006) / 100;
            decimal sebi = (decimal)(((decimal)15 * turnover) / 10000000);
            decimal dpcharges = (decimal)15.93;
            result = ((sell - buy) - (stt + txn + gst + stamb + sebi + dpcharges));
            return result;
        }
    }

    public class TRA_HOLDING_SEARCH : TRA_HOLDING
    {
        public string symbols { get; set; }
    }
}

