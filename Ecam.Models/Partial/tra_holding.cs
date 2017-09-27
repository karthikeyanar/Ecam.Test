using Ecam.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace Ecam.Models
{
    [MetadataType(typeof(tra_holdingMD))]
    public partial class tra_holding
    { 
        public override void OnDeleting()
        {
            base.OnDeleting();
        }

        public override void OnDeleted()
        {
            base.OnDeleted();
            using(EcamContext context = new EcamContext())
            {
                tra_company company = (from q in context.tra_company where q.symbol == this.symbol select q).FirstOrDefault();
                if (company != null)
                {
                    company.is_current_stock = false;
                    context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public class tra_holdingMD
        {
        }
    }
}
