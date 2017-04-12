using Ecam.Framework;
using Ecam.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace Ecam.Views.Models {

	public class AgentInvitationModel  {

        public AgentInvitationModel() {
            this.is_already_register = false;
        }

        public string agent_name { get; set; }

        public string agent_iata_code { get; set; }

        public int agent_id { get; set; }

        public string contact_person { get; set; }

        public List<AgentInvitationAirlines> airlines  { get; set; }

        public string designation { get; set; }

        public string phone { get; set; }

        public string to_email { get; set; }

        public string cc_email { get; set; }

        public string is_awb { get; set; }

        public string is_awb_accounting { get; set; }

        public string is_sales_customer { get; set; }

        public bool is_already_register { get; set; }
        		 
	}

    public class AgentInvitationAirlines {

        public int zone_id { get; set; }

        public int company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }

        public string airline_code { get; set; }

        public string airline_name { get; set; }

    }

     
}