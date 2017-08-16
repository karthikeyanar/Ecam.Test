 using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models {
    public partial class tra_pre_calc_item : BaseEntity<tra_pre_calc_item> {
								        public Nullable<int> percentage { get; set; }
				        public Nullable<int> success_count { get; set; }
				        public Nullable<int> fail_count { get; set; }
    }
}
