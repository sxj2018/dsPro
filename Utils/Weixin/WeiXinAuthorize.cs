using Utils.JSON;
using Utils.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Utils.Weixin
{
    public class WeiXinAuthorizeAttribute : ActionFilterAttribute
    {


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var _request = filterContext.HttpContext.Request;
            var _appId = ConfigurationManager.AppSettings["AppId"];
            var _appSecret = ConfigurationManager.AppSettings["AppSecret"];
            if (_request["Code"] == null)
            {

                string redirectUrl = _request.Url.OriginalString;
                redirectUrl = HttpUtility.UrlEncode(redirectUrl);
                redirectUrl = "http://login.o2o.cn/Wechat/WechatLogin.aspx?redirectUrl=" + redirectUrl;
                redirectUrl = HttpUtility.UrlEncode(redirectUrl);
                string url =
                  string.Format(
                      "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect ", _appId, redirectUrl, "snsapi_userinfo", 1);
                filterContext.Result = new RedirectResult(url);
            }
            else
            {
                string code = _request["Code"].ToString();//041ea2212c6a235da70ed8d6106b2d9u

                string tokenUrl = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type={3}", _appId, _appSecret, code, "authorization_code");
                string res = HttpHelper.CreateGetHttpResponse(tokenUrl);
                var _wxUserBase = JsonHelper.JsonPaser<WxUserBase>(res);
                var _userInfo = GetWxUserInfo(_wxUserBase);
                filterContext.Controller.ViewData["WxUserInfo"] = _userInfo;
                
            }
            base.OnActionExecuting(filterContext);

        }
        public WxUserInfo GetWxUserInfo(WxUserBase _wxUserBase)
        {
            string url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", _wxUserBase.Access_Token, _wxUserBase.OpenID);
            string res = HttpHelper.CreateGetHttpResponse(url);

            WxUserInfo user = JsonHelper.JsonPaser<WxUserInfo>(res);
            return user;
        }
    }

   
   

}