
using Utils.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
namespace Utils.Weixin
{
    public class WeixinHelper
    {
        /// <summary>
        /// 是否关注
        /// </summary>
        /// <param name="_appId"></param>
        /// <param name="_appSecret"></param>
        /// <param name="_openId"></param>
        /// <returns></returns>
        public bool IsAttention(string _appId, string _appSecret, string _openId)
        {
            bool isAttention = false;
            CacheAccessToken token = GetBaseToken(_appId, _appSecret);
            string subscribeUrl = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN", token.access_token, _openId, "check_subscribe");
            string res = HttpHelper.CreateGetHttpResponse(subscribeUrl);

            JavaScriptSerializer json = new JavaScriptSerializer();
            CheckSubscribe gotData = json.Deserialize<CheckSubscribe>(res);
            if (gotData.openid == _openId)
            {
                if (gotData.subscribe == 1)
                {
                    isAttention = true;
                }
            }
            return isAttention;
        }
        /// <summary>
        /// 获取基础接口的token
        /// </summary>
        /// <returns></returns>
        public CacheAccessToken GetBaseToken(string _appId, string _appSecret)
        {
            string _token = "";
            string appid = _appId ?? ConfigurationManager.AppSettings["AppID"].ToString();
            string appSecret = _appSecret ?? ConfigurationManager.AppSettings["AppSecret"].ToString();
            string cacheName = appid + "Cache_Access_Token";
            CacheAccessToken basetoken = new CacheAccessToken();
            if (System.Web.HttpRuntime.Cache[cacheName] != null)
            {
                _token = System.Web.HttpRuntime.Cache[cacheName].ToString();
                basetoken.access_token = _token;
            }
            else
            {

                JavaScriptSerializer json = new JavaScriptSerializer();
                string tokenUrl2 = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, appSecret);
                string res2 = HttpHelper.CreateGetHttpResponse(tokenUrl2);
                try
                {
                    CacheAccessToken baseAccessToken = json.Deserialize<CacheAccessToken>(res2);
                    if (baseAccessToken != null)
                    {
                        System.Web.HttpRuntime.Cache.Insert(cacheName, baseAccessToken.access_token, null, DateTime.Now.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration);
                        _token = baseAccessToken.access_token;
                        basetoken.access_token = _token;
                    }

                }
                catch (Exception)
                {

                }
            }
            return new CacheAccessToken()
            {
                access_token = basetoken.access_token,
                expires_in = basetoken.expires_in
            };
        }


    }
    public class CacheAccessToken
    {
        public string access_token { set; get; }
        public int expires_in { set; get; }
    }
    public class CheckSubscribe
    {
        public int subscribe { set; get; }
        public string openid { set; get; }
    }
}
