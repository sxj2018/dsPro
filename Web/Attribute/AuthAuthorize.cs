using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Attribute
{
    public class AuthAuthorize : AuthorizeAttribute
    {

        public override void OnAuthorization(AuthorizationContext actionContext)
        {
            var authorization = actionContext.Request.Headers.Authorization;


            var content = actionContext.Request.Properties["MS_HttpContext"] as HttpContextBase;
            var token = content.Request.Headers["Test"];   //这里是拿到了token 的值 也就是  “woshiyanzhengma” 

            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Count != 0 || actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Count != 0)
            {
                base.OnAuthorization(actionContext);//正确的访问方法 

            }
        }
    }
}