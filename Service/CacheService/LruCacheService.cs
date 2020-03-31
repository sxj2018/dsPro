
using IService.ICache;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Threading;
namespace Service.CacheService
{
    public class LruCache : ICacheService
    {
        static int _discardAge = 0;//要删除的年龄
        static int _currentAge = 0;//当前年龄
        int _maxSize = 50;//缓存容量
        static ConcurrentDictionary<string, LruCacheData> _cache = new ConcurrentDictionary<string, LruCacheData>();
        public LruCache()
        {
            int.TryParse(ConfigurationManager.AppSettings["CacheMaxSize"].ToString(), out _maxSize);
        }
        /// <summary>
        /// 缓存池清理
        /// </summary>
        protected void ClearUnUsedCache()
        {
            while (_cache.Count >= _maxSize)
            {
                int discardAge = Interlocked.Increment(ref _discardAge);
                var discardItem = _cache.FirstOrDefault(c => c.Value.Age == discardAge);
                if (discardItem.Value == null) continue;
                LruCacheData removeData;
                _cache.TryRemove(discardItem.Key, out removeData);
            }
        }
        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <param name="_key">键</param>
        /// <param name="_value">值</param>
        /// <returns></returns>
        public bool Set(string _key, object _value)
        {
            ClearUnUsedCache();
            LruCacheData data = new LruCacheData(_value);
            var _resultData = _cache.AddOrUpdate(_key, data, (b, c) => data);
            return _resultData == null;
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public object Get(string _key)
        {
            LruCacheData data = null;
            if (_cache.TryGetValue(_key, out data))
            {
                data.Age = Interlocked.Increment(ref _currentAge);
            }
            return data != null ? data.Value : null;
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool Remove(string _key)
        {
            LruCacheData data = null;
            return _cache.TryRemove(_key, out data);
        }
        protected class LruCacheData
        {
            public int Age { set; get; }
            public object Value { set; get; }
            //TrackValue增加创建时间和过期时间
            public DateTime CreateTime { set; get; }
            public TimeSpan ExpireTime { set; get; }
            public LruCacheData(object value)
            {
                Age = Interlocked.Increment(ref _currentAge);
                Value = value;
            }
        }
    }
}
