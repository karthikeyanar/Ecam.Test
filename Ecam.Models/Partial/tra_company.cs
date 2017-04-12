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
