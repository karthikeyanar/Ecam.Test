using System;
using System.Collections.Generic;
using Ecam.Framework;
namespace Ecam.Models
{
    public partial class tra_category : BaseEntity<tra_category>
    {
        public string category_name { get; set; }
        public Nullable<bool> is_archive { get; set; }
        public Nullable<bool> is_book_mark { get; set; }
    }
}
