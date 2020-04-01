using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {


           
        }
        #region first过滤器
        //--- 利用 事件自动注册机制 来 为 当前网站的 HttpApplicaiton里的 事件 注册方法 ----------------
        //管道事件方法 的 命名规则 ：一定 以 Application_事件名
        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    System.Web.HttpApplication app = sender as System.Web.HttpApplication;
        //    //app.Context.Response.Write("本网站的 Global 中 自动添加的方法 ~~~~~~~~~~~！");
        //    FakeStaticProcess(app);

        //}
        /// <summary>
        /// 伪静态处理（在内部根据静态url，换成 动态url）
        /// </summary>
        /// <param name="app"></param>
        void FakeStaticProcess(System.Web.HttpApplication app)
        {
            HttpContext context = app.Context;
            context.Response.ContentType = "text/html";
            //1.获取当前静态url
            string strStaticUrl = context.Request.RawUrl;
            //context.Response.Write(context.Request.RawUrl);
            //2.解析 url
            string[] strUrlParts = strStaticUrl.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (strUrlParts.Length > 0)
            {
                //2.1 url 开头 为 student
                if (strUrlParts[0] == "student")// /student
                {
                    if (strUrlParts[1] == "list")//  /student/list
                    {
                    }
                    else if (strUrlParts[1] == "details")//  /student/details
                    {
                        //   /student/details/1
                        if (strUrlParts.Length == 3 && !string.IsNullOrEmpty(strUrlParts[2]))
                        {
                            //如果符合 url 则 重写 到 对应的 动态页面
                            context.RewritePath("/C03Student.aspx?id=" + strUrlParts[2]);
                        }
                    }
                }
            }

            //context.Response.End();
        } 
        #endregion
    }
}
