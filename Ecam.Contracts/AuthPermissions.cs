using System;
using System.Collections.Generic;

namespace Ecam.Contracts {
    public class AuthPermissions {
        public AuthPermissions() {
            this.agent_ids = string.Empty;
            this.company_airline_codes = string.Empty;
            this.company_airport_codes = string.Empty;
            this.role = string.Empty;
            this.company_ids = string.Empty;            
        }
        public string agent_ids { get; set; }
        public string company_ids { get; set; }
        public string company_airline_codes { get; set; }
        public string company_airport_codes { get; set; }
        public string role { get; set; }        
    }

    public class UserCompanyAirline {
        public int company_id { get; set; }
        public string airline_code { get; set; }
    }

    public class UserAgent {
        public int user_id { get; set; }
        public int agent_id { get; set; }
    }

    public class UserCompanyAirport
    {
        public int company_id { get; set; }
        public string airline_code { get; set; }
        public string airport_code { get; set; }
    }
}
