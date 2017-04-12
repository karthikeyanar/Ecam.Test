using Ecam.Contracts;
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


    public class ImportPaging {

        [Range((int)0,int.MaxValue,ErrorMessage = "PageIndex is required")]
        [Required(ErrorMessage = "PageIndex is required")]
        public int PageIndex { get; set; }

        [Range((int)1,int.MaxValue,ErrorMessage = "PageIndex is required")]
        [Required(ErrorMessage = "PageIndex is required")]
        public int PageSize { get; set; }

        [Required(ErrorMessage = "Session Key is required")]
        public string SessionKey { get; set; }

    }

    public class ImportResultModel {

        public int? TotalRows { get; set; }

        public int? SuccessCount { get; set; }

        public int? ErrorCount { get; set; }

		public int? WarningCount { get; set; }

        public bool IsSuccess {
            get {
                return ((this.ErrorCount ?? 0) == 0);
            }
        }
    }

    public class ImportCompanyAirportModel:ImportPaging {

        [DisplayName("Airport")]
        public string Airport { get; set; }
    }

    public class ImportCost2Model : ImportPaging
    {
        [DisplayName("Company Code")]
        public string CompanyCode { get; set; }
        
        [DisplayName("AWB No")]
        public string AWBNO { get; set; }

        [DisplayName("Currency")]
        public string Currency { get; set; }

        [DisplayName("Cost2")]
        public string Cost2 { get; set; }
    }

    public class ImportSalesModel:ImportPaging {

        [DisplayName("Company Code")]
        public string CompanyCode { get; set; }

        [DisplayName("Airline Code")]
        public string AirlineCode { get; set; }

        [DisplayName("Agent Name")]
        public string AgentName { get; set; }

        [DisplayName("Agent Airport")]
        public string AgentAirport { get; set; }

        [DisplayName("AWB No")]
        public string AWBNO { get; set; }

        [DisplayName("Flight No")]
        public string FlightNO { get; set; }

        [DisplayName("Flight Date")]
        public string FlightDate { get; set; }

        [DisplayName("Origin")]
        public string Origin { get; set; }

        [DisplayName("Destination")]
        public string Destination { get; set; }

        [DisplayName("Commodity")]
        public string Commodity { get; set; }

        [DisplayName("PP/CC")]
        public string PPCC { get; set; }

        [DisplayName("Gr.Wt")]
        public string GRWT { get; set; }

        [DisplayName("Ch.Wt")]
        public string CHWT { get; set; }

        [DisplayName("Currency")]
        public string Currency { get; set; }

        [DisplayName("Freight/KG")]
        public string FreightKG { get; set; }

        //[DisplayName("Freight Amount")]
        //public string FreightAmount { get; set; }

        //[DisplayName("FSC Amount")]
        //public string FSCAmount { get; set; }

        //[DisplayName("SSC Amount")]
        //public string SSCAmount { get; set; }

        //[DisplayName("AWC")]
        //public string AWC { get; set; }

        //[DisplayName("Screening")]
        //public string Screening { get; set; }

        //[DisplayName("Others")]
        //public string Others { get; set; }

        //[DisplayName("Total")]
        //public string Total { get; set; }

        //[DisplayName("IATA Commission")]
        //public string IATACommission { get; set; }

        //[DisplayName("Other Deduction")]
        //public string OtherDeduction { get; set; }

        //[DisplayName("Commission")]
        //public string Commission { get; set; }

        [DisplayName("Due Carrier")]
        public string DueCarrier { get; set; }

        //[DisplayName("Cost Freight")]
        //public string CostFreight { get; set; }

        //[DisplayName("Cost Interline")]
        //public string CostInterline { get; set; }

        //[DisplayName("Cost Surcharges")]
        //public string CostSurcharges { get; set; }

        //[DisplayName("Cost Trucking")]
        //public string CostTrucking { get; set; }

        //[DisplayName("Cost Handline")]
        //public string CostHndling { get; set; }

        //[DisplayName("Cost Others")]
        //public string CostOthers { get; set; }

        [DisplayName("Total Cost")]
        public string TotalCost { get; set; }

        [DisplayName("Profit")]
        public string Profit { get; set; }

        [DisplayName("Profit Per KG")]
        public string ProfitPerKG { get; set; }

        [DisplayName("Cargo Type")]
        public string CargoType { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("Cost2")]
        public string Cost2 { get; set; }
    }

    public class ImportSalesDetailModel:ImportPaging {

        [DisplayName("AWB No")]
        public string AWBNO { get; set; }

        [DisplayName("Company Code")]
        public string CompanyCode { get; set; }

        [DisplayName("From")]
        public string From { get; set; }

        [DisplayName("To")]
        public string To { get; set; }

        [DisplayName("Flight No")]
        public string FlightNO { get; set; }

        [DisplayName("Flight Date")]
        public string FlightDate { get; set; }

        [DisplayName("Gr.Wt")]
        public string GRWT { get; set; }

        [DisplayName("Ch.Wt")]
        public string CHWT { get; set; }

    }

    public class ImportForexModel:ImportPaging {
    }

    public class ImportAgentModel:ImportPaging {
        [DisplayName("Name")]
        public string AgentName { get; set; }

        [DisplayName("IATA")]
        public string IATACode { get; set; }

        [DisplayName("CASS")]
        public string CASSCode { get; set; }

        [DisplayName("Invoice Address")]
        public string InvoiceAddress { get; set; }

        [DisplayName("Envelop Invoice Address")]
        public string EnvelopInvoiceAddress { get; set; }
        
        [DisplayName("Short Name")]
        public string ShortName { get; set; }

        [DisplayName("Invoice Email")]
        public string InvoiceEmail { get; set; }

        public string Country { get; set; }

        [DisplayName("Airport")]
        public string AirportIATACode { get; set; }

        [DisplayName("Is Interline")]
        public string IsInterline { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class ImportSalesRateModel:ImportPaging {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string RouteID { get; set; }
        public string CommodityCode { get; set; }
        public string ShipmentType { get; set; }
        public string Sales_45 { get; set; }
        public string Sales_100 { get; set; }
        public string Sales_250 { get; set; }
        public string Sales_300 { get; set; }
        public string Sales_500 { get; set; }
        public string Sales_1000 { get; set; }
        public string Sales_1250 { get; set; }
        public string Sales_1500 { get; set; }
        public string Sales_2000 { get; set; }
        public string SalesMin { get; set; }
        public string SalesNormal { get; set; }
        public string CostMin { get; set; }
        public string CostNormal { get; set; }
        public string Cost_45 { get; set; }
        public string Cost_100 { get; set; }
        public string Cost_250 { get; set; }
        public string Cost_300 { get; set; }
        public string Cost_500 { get; set; }
        public string Cost_1000 { get; set; }
        public string Cost_1250 { get; set; }
        public string Cost_1500 { get; set; }
        public string Cost_2000 { get; set; } 
    }

    public class ImportIATARateModel:ImportPaging {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string CommodityCode { get; set; }
        public string ShipmentType { get; set; }
        public string IATA_45 { get; set; }
        public string IATA_100 { get; set; }
        public string IATA_250 { get; set; }
        public string IATA_300 { get; set; }
        public string IATA_500 { get; set; }
        public string IATA_1000 { get; set; }
        public string IATA_1250 { get; set; }
        public string IATA_1500 { get; set; }
        public string IATA_2000 { get; set; }
        public string IATAMin { get; set; }
        public string IATANormal { get; set; } 
    }

    public class ImportAWBLotModel:ImportPaging {
        public string Date { get; set; }
        public string AWBNo { get; set; }
        public string CompanyCode { get; set; }
        public string AgentName { get; set; }
        public string AWBType { get; set; }
    }

    public class ImportAWBLotCompressModel:ImportPaging {
        public ImportAWBLotCompressModel() {
            this.AWBNumbers = new List<string>();
        }
        public System.DateTime Date { get; set; }
        public List<string> AWBNumbers { get; set; }
        public string AWBPrefix { get; set; }
        public string CompanyCode { get; set; }
        public string AgentName { get; set; }
        public string AWBType { get; set; }
    }

    public class ImportAWBSlotNumberModel {
        public int StockNumber { get; set; }
        public bool IsStartsZero { get; set; }
    }


    public class ImportCompanyFlightModel:ImportPaging {
		public string Airline { get; set; }
		public string FlightNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
		public string From { get; set; }
		public string To1 { get; set; }
		public string To2 { get; set; }
		public string To3 { get; set; }
		public string ETDFrom { get; set; }
		public string ETAT1 { get; set; }
		public string T1ETA1 { get; set; }
		public string ETDT1 { get; set; }
		public string ETAT2 { get; set; }
		public string T2ETA1 { get; set; }
		public string ETDT2 { get; set; }
		public string ETAT3 { get; set; }
		public string T3ETA1 { get; set; }
		public string FlightType { get; set; }
		public string Days { get; set; }
		public string Length { get; set; }
		public string Width { get; set; }
		public string Height { get; set; }
		public string MaxWeight { get; set; }
		public string Payload { get; set; }
		public string Interval { get; set; }
		public string OpenDays { get; set; }		
    }
   
    public class ImportCompanyRouteModel:ImportPaging {

        //Company AirLine	From	To 1	Flt 1 #	Settlement 1	To 2	Flt 2 #	Settlement 2	To 3	Flt 3 #	Settlement 3	To 4	Flt 4 #	Settlement 4	To 5	Flt 5 #	Settlement 5	To 6	Flt 6 #	Settlement 6	To 7	Flt 7 #	Settlement 7

        public string CompanyAirline { get; set; }
        public string From { get; set; }

        public string To1 { get; set; }
        public string Flt1 { get; set; }
        public string Settlement1 { get; set; }

        public string To2 { get; set; }
        public string Flt2 { get; set; }
        public string Settlement2 { get; set; }

        public string To3 { get; set; }
        public string Flt3 { get; set; }
        public string Settlement3 { get; set; }

        public string To4 { get; set; }
        public string Flt4 { get; set; }
        public string Settlement4 { get; set; }

        public string To5 { get; set; }
        public string Flt5 { get; set; }
        public string Settlement5 { get; set; }

        public string To6 { get; set; }
        public string Flt6 { get; set; }
        public string Settlement6 { get; set; }

        public string To7 { get; set; }
        public string Flt7 { get; set; }
        public string Settlement7 { get; set; }

        public string RouteTitle { get; set; }
        public string Rkey { get; set; }
    }
      
    public class ImportMyForexModel : ImportPaging
    {
        public string Date { get; set; }
        public string ConversionType { get; set; }
        public string CompanyCode { get; set; }
        public string CurrencyCode { get; set; }
        public string Sales { get; set; }
        public string Cost { get; set; }        
    }

    public class ImportParticipantModel:ImportPaging {
        public string Participant { get; set; }
        public string Name { get; set; }
        public string RefNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string Account { get; set; }
    }

    public class ImportParticipantCompressModel : ImportPaging
    {
        public ImportParticipantCompressModel()
        {
            this.sno = 0;
        }
        public int sno { get; set; }
        public string participant { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string account { get; set; }
    }
}
