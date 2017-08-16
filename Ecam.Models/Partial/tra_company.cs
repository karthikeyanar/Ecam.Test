using Ecam.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace Ecam.Models
{
    [MetadataType(typeof(tra_companyMD))]
    public partial class tra_company
    {
		public override void OnSaving() {
			if(string.IsNullOrEmpty(this.company_name) == false) {
				this.company_name = this.company_name.Trim();
			}
			if(string.IsNullOrEmpty(this.symbol) == false) {
				this.symbol = this.symbol.Trim();
			}
			base.OnSaving();
		}

        public override void OnSaved()
        {
            base.OnSaved();
        }

        public override bool Validate()
        {
            if (base.Validate())
            {
                using (EcamContext context = new EcamContext())
                {
                    tra_company company = context.tra_company.Where(x => x.id != this.id && x.company_name.Equals(this.company_name)).FirstOrDefault();
                    if (company != null)
                    {
                        this.Errors.Add("company_name", "Company Name already exists");
                        return false;
                    }
                    company = context.tra_company.Where(x => x.id != this.id && x.symbol.Equals(this.symbol)).FirstOrDefault();
                    if (company != null)
                    {
                        this.Errors.Add("symbol", "Symbol already exists");
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public override void OnDeleting()
        {
            base.OnDeleting();
        }

        public override void OnDeleted()
        {
            base.OnDeleted();
            using(EcamContext context = new EcamContext())
            {
                List<tra_market> markets = (from q in context.tra_market where q.symbol == this.symbol select q).ToList();
                foreach(tra_market row in markets)
                {
                    context.tra_market.Remove(row);
                }
                List<tra_market_intra_day> marketDay = (from q in context.tra_market_intra_day where q.symbol == this.symbol select q).ToList();
                foreach (tra_market_intra_day row in marketDay)
                {
                    context.tra_market_intra_day.Remove(row);
                }
                List<tra_company_category> companyCategories = (from q in context.tra_company_category where q.symbol == this.symbol select q).ToList();
                foreach (tra_company_category row in companyCategories)
                {
                    context.tra_company_category.Remove(row);
                }
                List<tra_intra_day_profit> profits = (from q in context.tra_intra_day_profit where q.symbol == this.symbol select q).ToList();
                foreach (tra_intra_day_profit row in profits)
                {
                    context.tra_intra_day_profit.Remove(row);
                }
                List<tra_mutual_fund_pf> mfs = (from q in context.tra_mutual_fund_pf where q.symbol == this.symbol select q).ToList();
                foreach (tra_mutual_fund_pf row in mfs)
                {
                    context.tra_mutual_fund_pf.Remove(row);
                }
                List<tra_rsi_profit> rsi = (from q in context.tra_rsi_profit where q.symbol == this.symbol select q).ToList();
                foreach (tra_rsi_profit row in rsi)
                {
                    context.tra_rsi_profit.Remove(row);
                }
                context.SaveChanges();
            }
        }


        public class tra_companyMD
        {

            [Required(ErrorMessage = "Company name is required")]
            [StringLength(100, ErrorMessage = "Company name must be under 100 characters.")]
            public string company_name { get; set; }

            [Required(ErrorMessage = "Symbol is required")]
            [StringLength(50, ErrorMessage = "Symbol must be under 50 characters.")]
            public string symbol { get; set; }
        }
    }
}
