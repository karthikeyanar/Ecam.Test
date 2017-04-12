using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using System.Text.RegularExpressions;


namespace Ecam.Framework {
    /// <summary>
    /// Represents a MemoryCacheCache
    /// </summary>
    public partial class MemoryCacheManager:ICacheManager {

        public const string IMPORT_ALL_AIRPORTS = "{0}-IMPORT_ALL_AIRPORTS";
        public const string IMPORT_ALL_COMPANIES = "{0}-IMPORT_ALL_COMPANIES";
        public const string IMPORT_ALL_AIRLINES = "{0}-IMPORT_ALL_AIRLINES";
        public const string IMPORT_ALL_CURRENCIES = "{0}-IMPORT_ALL_CURRENCIES";
        public const string IMPORT_ALL_FLIGHTS = "{0}-IMPORT_ALL_FLIGHTS";
        public const string IMPORT_ALL_AGENTS = "{0}-IMPORT_ALL_AGENTS";
        public const string IMPORT_ALL_ROUTES = "{0}-IMPORT_ALL_ROUTES";
        public const string IMPORT_ALL_COMMOTITY_TYPES = "{0}-IMPORT_ALL_COMMOTITY_TYPES";
        public const string IMPORT_ALL_SHIPMENT_TYPES = "{0}-IMPORT_ALL_SHIPMENT_TYPES";
        public const string IMPORT_ALL_FLIGHT_TYPES = "{0}-IMPORT_ALL_FLIGHT_TYPES";
        public const string IMPORT_ALL_COMPANY_FLIGHTS = "{0}-IMPORT_ALL_COMPANY_FLIGHTS";
        public const string IMPORT_ALL_SETTLEMENT_TYPES = "{0}-IMPORT_ALL_SETTLEMENT_TYPES";

        protected ObjectCache Cache {
            get {
                return MemoryCache.Default;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public T Get<T>(string key) {
            return (T)Cache[key];
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public void Set(string key,object data,int cacheTime) {
            if(data == null)
                return;

            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Cache.Add(new CacheItem(key,data),policy);
        }

        public void Set(string key,object data,DateTime cacheDateTime) {
            if(data == null)
                return;

            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = cacheDateTime;
            Cache.Add(new CacheItem(key,data),policy);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public bool IsSet(string key) {
            return (Cache.Contains(key));
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public void Remove(string key) {
            Cache.Remove(key);
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public void RemoveByPattern(string pattern) {
            var regex = new Regex(pattern,RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach(var item in Cache)
                if(regex.IsMatch(item.Key))
                    keysToRemove.Add(item.Key);

            foreach(string key in keysToRemove) {
                Remove(key);
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public void Clear() {
            foreach(var item in Cache)
                Remove(item.Key);
        }
    }

    public static class CacheExtensions {
        public static T Get<T>(this ICacheManager cacheManager,string key,Func<T> acquire,int cacheExpire = 0) {
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["cache_time"],out cacheExpire);
            if(cacheExpire <= 0) { cacheExpire = 60; }
            return Get(cacheManager,key,cacheExpire,acquire);
        }

        public static T Get<T>(this ICacheManager cacheManager,string key,int cacheTime,Func<T> acquire) {
            if(cacheManager.IsSet(key)) {
                return cacheManager.Get<T>(key);
            } else {
                var result = acquire();
                //if (result != null)
                cacheManager.Set(key,result,cacheTime);
                return result;
            }
        }
    }
}
