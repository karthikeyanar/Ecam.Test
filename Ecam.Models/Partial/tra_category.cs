using Ecam.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace Ecam.Models
{
    [MetadataType(typeof(tra_categoryMD))]
    public partial class tra_category
    {
		public override void OnSaving() {
			if(string.IsNullOrEmpty(this.category_name) == false) {
				this.category_name = this.category_name.Trim();
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
                    tra_category category = context.tra_category.Where(x => x.id != this.id && x.category_name.Equals(this.category_name)).FirstOrDefault();
                    if (category != null)
                    {
                        this.Errors.Add("category_name", "Category Name already exists");
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
                List<tra_company_category> companyCategories = (from q in context.tra_company_category where q.category_name == this.category_name select q).ToList();
                foreach(tra_company_category row in companyCategories)
                {
                    context.tra_company_category.Remove(row);
                }
                context.SaveChanges();
            }
        }


        public class tra_categoryMD
        {
            [Required(ErrorMessage = "Category name is required")]
            [StringLength(50, ErrorMessage = "Category name must be under 50 characters.")]
            public string category_name { get; set; }
        }
    }
}
