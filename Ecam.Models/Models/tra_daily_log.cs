 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_daily_log : BaseEntity<tra_daily_log> {
		public System.DateTime trade_date { get; set; }
		public int positive { get; set; }
        public int negative { get; set; }
        public bool? is_book_mark { get; set; }
    }
}
