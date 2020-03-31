using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IService.ICache
{
    public interface ICacheAbsoluteExpiration
    {
        /// <summary>
        /// 默认绝对过期
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        /// <param name="_cacheSecond"></param>
        /// <returns></returns>
        bool Set(string _key, object _value, int _cacheSecond);
    }
}