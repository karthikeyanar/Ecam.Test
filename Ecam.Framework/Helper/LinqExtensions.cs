using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Data.Linq;

namespace Ecam.Framework {
	public static class LinqExtensions {
        public static DbSet<T> GetTable<T>(this DbContext context)  where T: class {
            return context.GetTable<T>();
        }
    }
}
