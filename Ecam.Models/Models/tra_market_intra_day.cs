using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models
{
    public partial class tra_market_intra_day
    {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public decimal ltp_price { get; set; }
        public Nullable<decimal> rsi { get; set; }
    }
}
