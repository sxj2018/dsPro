using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace IService.IDbService
{
    public interface IDbServiceReposity
    {

        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int Add<T>(T t, bool isCache = false, string cacheKey = null) where T : class;
        /// <summary>
        /// 插入多条数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int AddRange<T>(List<T> list) where T : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int Update<T>(T t, bool isCache = false) where T : class;
        int Update<T>(T t) where T : class;

        int Update(string _sql, params object[] _parms);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int Delete<T>(T t) where T : class;
        int Delete<T>(List<T> list) where T : class;

        /// <summary>
        /// 取出数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="whereLambada"></param>
        /// <param name="orderLambda"></param>
        /// <param name="pageCount">每页条数</param>
        /// <param name="page">第几页</param>
        /// <returns></returns>
        List<T> Where<T>(Expression<Func<T, bool>> whereLambada, bool IsCache = false, string cacheKey = null) where T : class;
        IQueryable<T> Where<T>(Expression<Func<T, bool>> whereLambada) where T : class;
        IQueryable<T> Where<T>(T t) where T : class;
        IQueryable Where(Type type);
        /// <summary>
        /// 取出单一数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="whereLamdaba"></param>
        /// <returns></returns>
        T FirstOrDefault<T>(Expression<Func<T, bool>> whereLamdaba) where T : class;

        /// <summary>
        /// 通过sql查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        IEnumerable<T> Where<T>(string sql) where T : class;

        /// <summary>
        /// 物理删除数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int Delete(string sql);

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int Count(string sql);

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count<T>(Expression<Func<T, bool>> predicate) where T : class;
        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count<T>(T t) where T : class;

        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Sum<T>(Expression<Func<T, bool>> predicateWhere, Expression<Func<T, int>> predicateSelect) where T : class;

        /// <summary>
        /// 取最大值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T Max<T>(Expression<Func<T, string>> predicate) where T : class;

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T Min<T>(Expression<Func<T, string>> predicate) where T : class;
        
        

    }
}