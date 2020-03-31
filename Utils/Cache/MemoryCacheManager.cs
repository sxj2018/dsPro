using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;


namespace Utils.Cache
{
    public partial class MemoryCacheManager : ICacheManager
    {
        #region Methods
        public T Get<T>(string key)
        {
            return (T)MemoryCache.Default[key];
        }

        public void Set<T>(string key, T value, int? cacheTime = null, int? slidingExpiration = null)
        {
            if (value == null)
            {
                return;
            }
            var policy = new CacheItemPolicy();
            if (cacheTime.HasValue)
            {
                policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromHours(cacheTime.Value);
            }
            if (slidingExpiration.HasValue)
            {
                policy.SlidingExpiration = TimeSpan.FromHours(slidingExpiration.Value);
            }
            policy.Priority = CacheItemPriority.Default;
            MemoryCache.Default.Add(new CacheItem(key, value), policy);
        }

        public bool IsSet(string key)
        {
            return (MemoryCache.Default.Contains(key));
        }


        public void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        public void Clear()
        {
            foreach (var item in MemoryCache.Default)
            {
                Remove(item.Key);
            }
        }
        #endregion
    }
}
