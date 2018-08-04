using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_holding:BaseEntity<tra_holding> {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public int quantity { get; set; }
        public decimal avg_price { get; set; }
        public decimal? buy_price { get; set; }
        public DateTime? sell_date { get; set; }
        public decimal? sell_price { get; set; }
    }
}
