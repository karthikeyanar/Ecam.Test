using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Contracts {
    public class ENQUIRY_ROUTES {
        public int route_id { get; set; }
        public string route_key { get; set; }

        public string origin_code { get; set; }
        public string dest_code { get; set; }
        public string origin_name { get; set; }
        public string dest_name { get; set; }

        public string from_airport_code { get; set; }
        public string to_airport_code { get; set; }
        public int group_id { get; set; }
        public int company_id { get; set; }
        public string airline_code { get; set; }
        public string flight_nos { get; set; }
        public string settlement_type { get; set; }

        public string from_airport_name { get; set; }
        public string from_airport_city { get; set; }
        public string from_airport_country { get; set; }

        public string to_airport_name { get; set; }
        public string to_airport_city { get; set; }
        public string to_airport_country { get; set; }

        public string airline_name { get; set; }
    }

    public class ENQUIRY_ROUTE_GROUPS {
        public int route_id { get; set; }
        public List<ENQUIRY_ROUTES> stations { get; set; }
    }

    public class ENQUIRY_COMPANY_FLIGHTS {
        public int company_flight_id { get; set; }
        public int day_index { get; set; }
        public string origin_etd { get; set; }
        public string dest1_eta { get; set; }
        public string origin1_etd { get; set; }
        public string dest2_eta { get; set; }
        public string origin2_etd { get; set; }
        public string dest_eta { get; set; }
        public Nullable<int> dest_eta_days { get; set; }
        public int flight_type_id { get; set; }
        public decimal? payload { get; set; }

        public decimal? length { get; set; }
        public decimal? height { get; set; }
        public decimal? width { get; set; }
        public decimal? max_weight { get; set; }
        public decimal? interval { get; set; }

        public int company_id { get; set; }
        public string flight_no { get; set; }
        public int flight_allotment_id { get; set; }
        public string origin_code { get; set; }
        public string dest_code { get; set; }
        public int open_days { get; set; }
        public int? group_id { get; set; }
        public bool? is_truck { get; set; }
        public bool? is_company_flight { get; set; }
        public string dest1_code { get; set; }
        public string dest2_code { get; set; }

        public string flight_name { get; set; }
        public string origin_name { get; set; }
        public string dest1_name { get; set; }
        public string dest2_name { get; set; }
        public string dest_name { get; set; }
        public string flight_allotment_name { get; set; }
        public string flight_type_name { get; set; }
    }

    public class ENQUIRY_RESULT {
        public int route_id { get; set; }
        public string origin_code { get; set; }
        public string dest_code { get; set; }
        public string airline_code { get; set; }
        public int company_id { get; set; }
        public int group_id { get; set; }
        public DateTime flight_date { get; set; }
        List<ENQUIRY_FLIGHT_DETAIL> flights { get; set; }
    }

    public class ENQUIRY_FLIGHT_DETAIL: ENQUIRY_RESULT {
        public string route_key { get; set; }
        public string origin_code { get; set; }
        public string dest_code { get; set; }
        public string airline_code { get; set; }
        public string flight_no { get; set; }
        public DateTime origin_flight_date { get; set; }
        public DateTime dest_flight_date { get; set; }
        public string origin_eta { get; set; }
        public string dest_etd { get; set; }
    }



    public class ENQUIRY_RESULT_SEARCH:ENQUIRY_RESULT {
        public ENQUIRY_RESULT_SEARCH() {
            this.days = 10;
        }
        public string origin_codes { get; set; }
        public string dest_codes { get; set; }
        public string group_ids { get; set; }
        public string company_ids { get; set; }
        public string airline_codes { get; set; }
        public int days { get; set; }
    }
}
