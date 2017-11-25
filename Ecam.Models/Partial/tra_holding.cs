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
        }

        public class tra_holdingMD
        {
        }
    }
}
