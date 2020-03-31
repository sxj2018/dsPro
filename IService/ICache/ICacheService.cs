using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IService.ICache
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <param name="_key">键</param>
        /// <param name="_value">值</param>
        /// <returns></returns>
        bool Set(string _key, object _value);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="_key">键</param>
        /// <returns></returns>
        object Get(string _key);
        /// <summary>
        /// 删除对应缓存
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        bool Remove(string _key);



    }
}