﻿using Ecam.Framework;
using Ecam.Framework.ExcelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Contracts
{ 
    public class TRA_EXPERTS: BaseContract
    {
        public string experts_name { get; set; }
    }

    public class TRA_EXPERTS_SEARCH : TRA_EXPERTS
    {
    }
}

