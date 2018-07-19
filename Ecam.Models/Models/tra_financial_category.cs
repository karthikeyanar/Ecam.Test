using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models
{
    public partial class tra_financial_category: BaseEntity<tra_financial_category>
    {
        public string category_name { get; set; }
        public Nullable<bool> is_archive { get; set; }
    }
}
