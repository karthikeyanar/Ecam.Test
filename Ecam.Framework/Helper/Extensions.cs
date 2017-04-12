using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Collections.Specialized;

namespace System {
    public static class Extensions {
        public static NameValueCollection Combine(this NameValueCollection target, NameValueCollection source) {
            foreach (string key in source.Keys) {
                if (target.AllKeys.Contains(key)) {
                    string value = source[key];
                    if (!string.IsNullOrEmpty(value)) {
                        string targetVal = target[key];
                        string newValue = value;
                        if (!string.IsNullOrEmpty(targetVal)) {
                            newValue = targetVal + "," + value;
                        }
                    }
                } else {
                    target.Add(key, source[key]);
                }
            }
            return target;
        }

        /// <summary>
        /// Used to determine if the ID on an entityID is valid so that we know if we want to create or update that entity
        /// </summary>
        /// <param name="id"></param>       /// <returns></returns>
        public static bool IsNew(this int? id) {
            return !(id.HasValue && !IsNew(id.Value));
        }

        public static bool IsNew(this int id)
        {
            return id <= 0;
        }
    }
}