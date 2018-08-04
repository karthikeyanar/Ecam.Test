using Ecam.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Ecam.Models {
    [MetadataType(typeof(tra_holdingMD))]
    public partial class tra_holding {

        [NotMapped]
        public decimal? profit {
            get {
                if((this.sell_price ?? 0) > 0) {
                    return (((this.sell_price ?? 0) - (this.buy_price ?? 0)) / (this.buy_price ?? 0)) * 100;
                } else {
                    return 0;
                }
            }
        }

        public override void OnDeleting() {
            base.OnDeleting();
        }

        public override void OnDeleted() {
            base.OnDeleted();
        }

        public class tra_holdingMD {

        }
    }
}
