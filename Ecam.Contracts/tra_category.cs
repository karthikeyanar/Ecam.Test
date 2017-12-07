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
    public class TRA_CATEGORY : BaseContract
    {
        public string category_name { get; set; }
        public decimal? profit_2016 { get; set; }
        public decimal? profit_2017 { get; set; }
        public decimal? profit_2018 { get; set; }
    }

    public class TRA_CATEGORY_SEARCH : TRA_CATEGORY
    {
    }
}

