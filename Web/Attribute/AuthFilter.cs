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
        public bool IsCheck { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //如果用户未登录，且action未明确标识可跳过登录授权，则跳转到登录页面
            var strtoken = filterContext.RequestContext.HttpContext.Request.Headers["Authorization"];
            string url = filterContext.RequestContext.HttpContext.Request.Path.ToString();
            //request.ContentEncoding("UTF-8");AuthEscapeAttribute

            if (IsCheck)
            {
                string payload = "";
                string message = "";
                var islogin = false;
                if (!string.IsNullOrEmpty(strtoken))
                {
                    islogin = JwtHelper.ValidateJWT(strtoken, out payload, out message);
                }
                if (!islogin && !filterContext.ActionDescriptor.IsDefined(typeof(AuthEscape), false))
                {
                    const string loginUrl = "~/Home/Login";
                    filterContext.Result = new RedirectResult(loginUrl);
                }
            }
          
            base.OnActionExecuting(filterContext);
        }

    }
}