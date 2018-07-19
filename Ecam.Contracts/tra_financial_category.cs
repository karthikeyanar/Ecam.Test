using Ecam.Framework;
using Ecam.Framework.ExcelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Contracts {
    public class TRA_FINANCIAL_CATEGORY:BaseContract {
        public string category_name { get; set; }
        public Nullable<bool> is_archive { get; set; }
    }

    public class TRA_FINANCIAL_CATEGORY_SEARCH:TRA_FINANCIAL_CATEGORY {
    }
}

