using Ecam.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecam.Views.Models {
    public class FFRModel {
        public EC_FLIGHT_BOOK FlightBook { get; set; }
        public EC_COMPANY_FFR CompanyFFR { get; set; }
    }
}