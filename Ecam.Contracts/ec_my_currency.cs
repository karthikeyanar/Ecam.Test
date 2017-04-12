 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Contracts {
   public class EC_MY_CURRENCY:BaseContract {
								
		        public string currency_name { get; set; }
						
		        public string currency_code { get; set; }
						
		        public string currency_symbol { get; set; }
						
		        public string currency_remarks { get; set; }
		    }
}
