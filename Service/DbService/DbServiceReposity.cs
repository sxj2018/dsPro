using IService.IDbService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Web;
using IService.ICache;
using Service.CacheService;
using System.Linq.Expressions;
using Utils.Data;
using Utils.Attr;
using Utils.Enum;
using System.Reflection;

namespace Service.DbService
{
    public class DbServiceReposity : IDbServiceReposity, IDisposable
    {
        ICacheService _cache = null;
        DbContext _dbContext = null;
        public DbServiceReposity(DbContext __dbContext, ICacheService __cache)
        {
            _dbContext = __dbContext;
            _cache = __cache;
        }
        public DbServiceReposity(DbContext __dbContext)
        {
            _dbContext = __dbContext;
        }
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="t"></param>
        /// <param name="isCache">是否清除当前记录相关缓存</param>
        /// <param name="cacheKey">缓存名字</param>
        /// <returns></returns>
        public int Add<T>(T t, bool isCache = false, string cacheKey = null) where T : class
        {
            int num = 0;
            ///插入数据
            _dbContext.Set<T>().Add(t);
            num = _dbContext.SaveChanges();
            if (num > 0 && isCache)
            {
                string key = cacheKey ?? typeof(T).ToString();
                _cache = new LruCache();
                _cache.Remove(key);
            }
            return num;
        }
        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddRange<T>(List<T> list) where T : class
        {
            int num = 0;
            ///小批量插入数据
            _dbContext.Set<T>().AddRange(list);
            num = _dbContext.SaveChanges();
            return num;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="t"></param>
        /// <param name="isCache">是清除当前相关缓存</param>
        /// <returns></returns>
        public int Update<T>(T t, bool isCache = false) where T : class
        {
            int num = 0;
            ///更新数据
            _dbContext.Entry<T>(t).State = EntityState.Modified;
            num = _dbContext.SaveChanges();
            if (num > 0 && isCache)
            {
                string key = typeof(T).ToString();
                _cache = new LruCache();
                _cache.Remove(key);
            }
            // WriteOperateLog(t);
            return num;
        }
        public virtual int Update<T>(T t) where T : class
        {
            var _entry = _dbContext.Entry(t);
            _entry.State = EntityState.Unchanged;
            var _props = typeof(T).GetProperties().Where(c => c.GetGetMethod().IsVirtual == false);
            var _idName = t.GetType().Name.Replace("T_", "") + "Id";
            foreach (var prop in _props)
            {
                var _value = prop.GetValue(t);
                if (prop.Name == "PageIndex" || prop.Name == "PageSize" || prop.Name == "IsExport" || prop.Name == "IsImport" || prop.Name == _idName)
                    continue;
                if (_value != null && !string.IsNullOrEmpty(_value.ToString()))
                    _entry.Property(prop.Name).IsModified = true;
            }
            return _dbContext.SaveChanges();
        }
        public int Update(string _sql, params object[] _parms)
        {
            return _dbContext.Database.ExecuteSqlCommand(_sql, _parms);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Delete<T>(T t) where T : class
        {
            int num = 0;
            ///删除数据
            _dbContext.Entry<T>(t).State = EntityState.Deleted;
            num = _dbContext.SaveChanges();
            // WriteOperateLog(t);
            return num;
        }
        public int Delete<T>(List<T> list) where T : class
        {
            int num = 0;
            foreach (var item in list)
            {
                ///删除数据
                _dbContext.Entry<T>(item).State = EntityState.Deleted;
            }
            num += _dbContext.SaveChanges();
            return num;
        }
        /// <summary>
        /// 取得唯一数据
        /// </summary>
        /// <param name="whereLamdaba"></param>
        /// <returns></returns>
        public T FirstOrDefault<T>(Expression<Func<T, bool>> whereLamdaba) where T : class
        {
            return _dbContext.Set<T>().Where(whereLamdaba).FirstOrDefault();
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="whereLambada"></param>
        /// <param name="IsCache">是否缓存</param>
        /// <param name="cacheKey">缓存名字</param>
        /// <returns></returns>
        public List<T> Where<T>(Expression<Func<T, bool>> whereLambada, bool IsCache = false, string cacheKey = null) where T : class
        {
            List<T> list;
            if (IsCache)
            {
                _cache = new LruCache();
                #region 缓存数据
                string key = cacheKey ?? typeof(T).ToString();
                var cacheInfo = _cache.Get(key);
                if (cacheInfo != null)
                {
                    list = (List<T>)cacheInfo;
                }
                else
                {
                    if (whereLambada == null)
                    {
                        list = _dbContext.Set<T>().AsQueryable().Select(c => c).ToList();
                    }
                    else
                    {
                        list = _dbContext.Set<T>().AsQueryable().Where(whereLambada).ToList();
                    }
                    if (IsCache && list != null)
                    {
                        _cache.Set(key, list);
                    }
                }
                #endregion
            }
            else
            {
                if (whereLambada == null)
                {
                    list = _dbContext.Set<T>().AsQueryable().Select(c => c).ToList();
                }
                else
                {
                    list = _dbContext.Set<T>().AsQueryable().Where(whereLambada).ToList();
                }
            }
            return list;
        }
        /// <summary>
        /// 通过sql查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<T> Where<T>(string sql) where T : class
        {
            List<T> list;
            list = _dbContext.Database.SqlQuery<T>(sql).ToList();
            return list;
        }
        public IQueryable<T> Where<T>(T t) where T : class
        {
            if (t == null)
                throw new ArgumentNullException("缺少查询参数");
            var result = _dbContext.Set<T>().AsQueryable();
            var properties = t.GetType().GetProperties();
            foreach (var item in properties)
            {
                var _operate = "=";
                var name = item.Name;
                if (name == "PageIndex" || name == "PageSize" || name == "IsExport" || name == "IsImport")
                    continue;
                var value = item.GetValue(t);
                if (name != "IsDeleted" && (value == null || value.ToString() == "0"))
                    continue;
                if (name == "IsDeleted" && value.ToString() == "-1")
                    continue;
                if (item.Name.StartsWith("View_"))
                    continue;
                if (name == "StartTime")
                {
                   
                    name = "Createtime";
                    _operate = ">=";
                }
                if (name == "EndTime")
                {
                    name = "Createtime";
                    _operate = "<=";
                }
                var _attr = item.GetCustomAttribute(typeof(LikeAttribute));
                if (_attr != null)
                {
                    var _type = ((LikeAttribute)_attr).Type;
                    if (_type == LikeType.StartsWith)
                        _operate = "StartsWith";
                    if (_type == LikeType.EndsWith)
                        _operate = "EndsWith";
                    if (_type == LikeType.Contains)
                        _operate = "Contains";
                    result = result.Where(string.Format("{0}.{1}(@0)", name, _operate), value);
                }
                else
                    result = result.Where(string.Format("{0} {1} @0", name, _operate), value);
            }
            return result.AsNoTracking();
        }

        public IQueryable Where(Type type)
        {
            return _dbContext.Set(type).AsQueryable();

        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> whereLambada) where T : class
        {
            try
            {
                _dbContext.Set<T>().AsQueryable().Where(whereLambada);
            }
            catch (DbEntityValidationException e)
            {
                return null;
            }
            return _dbContext.Set<T>().AsQueryable().Where(whereLambada);
        }
        /// <summary>
        /// 通过sql删除数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Delete(string sql)
        {
            return _dbContext.Database.ExecuteSqlCommand(sql);
        }
        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Count(string sql)
        {
            return Convert.ToInt32(_dbContext.Database.SqlQuery<decimal>(sql).ToList()[0]);
        }
        public int Count<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _dbContext.Set<T>().AsQueryable().Where(predicate).Count();
        }
        public T Max<T>(Expression<Func<T, string>> predicate) where T : class
        {
            return _dbContext.Set<T>().AsQueryable().OrderByDescending(predicate).First();
        }
        public T Min<T>(Expression<Func<T, string>> predicate) where T : class
        {
            return _dbContext.Set<T>().AsQueryable().OrderBy(predicate).First();
        }
        /// <summary>
        /// 没实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Count<T>(T t) where T : class
        {
            var result = _dbContext.Set<T>().AsQueryable();
            return result.Count();
        }

        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int Sum<T>(Expression<Func<T, bool>> predicateWhere, Expression<Func<T, int>> predicateSelect) where T : class
        {
            return _dbContext.Set<T>().AsQueryable().Where(predicateWhere).Select(predicateSelect).Sum();
        }
        #region 资源释放
        private bool m_disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    _cache = null;
                }
                // Release unmanaged resources
                _dbContext = null;
                m_disposed = true;
            }
        }



        ~DbServiceReposity()
        {
            Dispose(false);
        }
        #endregion
    }
}