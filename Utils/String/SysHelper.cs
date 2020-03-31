using System;
using System.Web;
using System.Threading;
using System.Diagnostics;

namespace Utils.QueryString
{
    /// <summary>
    /// 系统操作相关的公共类
    /// </summary>    
    public static class SysHelper
    {
        #region 获取文件相对路径映射的物理路径
        /// <summary>
        /// 获取文件相对路径映射的物理路径
        /// </summary>
        /// <param name="virtualPath">文件的相对路径</param>        
        public static string GetPath(string virtualPath)
        {

            return HttpContext.Current.Server.MapPath(virtualPath);

        }
        #endregion

        #region 判断是否未GUID
        public static bool IsGuidByReg(string strSrc)
        {
            try
            {
                Guid guid = new Guid(strSrc);
                return true;
            }
            catch
            {
                return false;
            }
            //Regex reg = new Regex("^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$", RegexOptions.Compiled);  
            //bool c = reg.IsMatch(strSrc);  
            //log.Error("------------------------");  
            //log.Error(c);  
            //log.Error("------------------------");  
            //return  c;  
        }
        #endregion

        #region 获取指定调用层级的方法名
        /// <summary>
        /// 获取指定调用层级的方法名
        /// </summary>
        /// <param name="level">调用的层数</param>        
        public static string GetMethodName(int level)
        {
            //创建一个堆栈跟踪
            StackTrace trace = new StackTrace();

            //获取指定调用层级的方法名
            return trace.GetFrame(level).GetMethod().Name;
        }
        #endregion

        #region 获取GUID值
        /// <summary>
        /// 获取GUID值
        /// </summary>
        public static string NewGUID
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
        #endregion

        #region 获取换行字符
        /// <summary>
        /// 获取换行字符
        /// </summary>
        public static string NewLine
        {
            get
            {
                return Environment.NewLine;
            }
        }
        #endregion

        #region 获取当前应用程序域
        /// <summary>
        /// 获取当前应用程序域
        /// </summary>
        public static AppDomain CurrentAppDomain
        {
            get
            {
                return Thread.GetDomain();
            }
        }
        #endregion


    }
}
