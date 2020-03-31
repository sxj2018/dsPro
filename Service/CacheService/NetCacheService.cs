

using IService.ICache;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
namespace Service.CacheService
{
    public class NetCacheService : ICacheService, ICacheAbsoluteExpiration
    {
        private string _cacheExName = string.Empty;
        private int _cacheTime = 20;
        public NetCacheService()
        {
            _cacheExName = ConfigurationManager.AppSettings["CacheExName"];
            int.TryParse(ConfigurationManager.AppSettings["NetCacheTime"].ToString(), out _cacheTime);
        }
        public ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }
        public bool Remove(string _key)
        {
            Cache.Remove(_cacheExName + _key);
            return Cache[_cacheExName + _key] == null;
        }
        public bool Set(string _key, object _value)
        {
            _key = _cacheExName + _key;
            var _policy = new CacheItemPolicy();
            _policy.SlidingExpiration = TimeSpan.FromSeconds(_cacheTime);
            return Cache.Add(_key, _value, _policy);
        }
        public object Get(string _key)
        {
            if (Cache.Contains(_cacheExName + _key))
                return Cache[_cacheExName + _key];
            else
                return null;
        }
        public bool Set(string _key, object _value, int _cacheSecond)
        {
            _key = _cacheExName + _key;
            var _policy = new CacheItemPolicy();
            //if (_isAbsoluteExpiration)
            //    _policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(_cacheSecond);
            //else
            //    _policy.SlidingExpiration = TimeSpan.FromSeconds(_cacheSecond);
            _policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(_cacheTime);
            return Cache.Add(_key, _value, _policy);
        }
    }
}
