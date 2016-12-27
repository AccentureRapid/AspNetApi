using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheManager.Core;

namespace LCMS.Common.Cache
{
    public static class SmartCache<T>
    {
        static CacheManagerConfiguration cfg;
        static ICacheManager<T> _cache;
        static SmartCache()
        {
            cfg = ConfigurationBuilder.BuildConfiguration(settings =>
            {
                settings.WithUpdateMode(CacheUpdateMode.Up)
                        .WithSystemRuntimeCacheHandle("SmartCache")
                        .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(60));
            });
        }

        public static ICacheManager<T> GetDefaultCacheManager()
        {
            return GetCacheManager("SmartFire"); ;
        }

        public static ICacheManager<T> GetCacheManager(string name)
        {
            if (_cache == null)
            {
                _cache = CacheFactory.FromConfiguration<T>(name, cfg);
            }

            return _cache;
        }

        public static T Get(string key, Func<string, T> load)
        {
            var result = GetDefaultCacheManager().Get(key);
            if (result == null)
            {
                result = load(key);
                GetDefaultCacheManager().Add(key,result);
            }

            return result;
        }

       

    }
}
