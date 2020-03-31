using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Webone.Auth;

namespace Web.Attribute
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //如果用户未登录，且action未明确标识可跳过登录授权，则跳转到登录页面
            //if (!CacheUtil.IsLogin && !filterContext.ActionDescriptor.IsDefined(typeof(AuthEscape), false))
            //var aa=HttpContext.Current.
            var request = HttpContext.Current.Request;
            var aadd = filterContext.RequestContext.HttpContext.Request.IsAuthenticated;
            var aabb = filterContext.RequestContext.HttpContext.Request.Headers["Authorization"];

            //request.ContentEncoding("UTF-8");
            request.InputStream.Position = 0;//核心代码

            byte[] byts = new byte[request.InputStream.Length];
            request.InputStream.Read(byts, 0, byts.Length);
            string data = Encoding.UTF8.GetString(byts);
            string strtoken = "";
            string payload = "";
            string message = "";
            //var islogin = JwtHelper.ValidateJWT(strtoken,out payload, out message);
            var islogin = true;
            if (!islogin)
            {
                const string loginUrl = "~/Home/Login";
                filterContext.Result = new RedirectResult(loginUrl);
            }
            base.OnActionExecuting(filterContext);
        }

    }
}