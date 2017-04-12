 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Contracts {
   public class EC_USER_AIRPORT {				
		        public int user_id { get; set; }
                public int zone_id { get; set; }
                public int company_id { get; set; }
                public string airline_code { get; set; }
                public string airport_code { get; set; }
    }

    public class EC_USER_AIRPORT_SEARCH : EC_USER_AIRPORT
    {
        public string company_ids { get; set; }
    }
}
