using Ecam.Framework;
using Ecam.Framework.ExcelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Contracts
{
    public class TRA_SPLIT : BaseContract
    {
        public string symbol { get; set; }
        public System.DateTime split_date { get; set; }
        public decimal split_factor { get; set; }

        public string company_name { get; set; }
    }

    public class TRA_SPLIT_SEARCH : TRA_SPLIT
    {
        public string symbols { get; set; }
    }
}

