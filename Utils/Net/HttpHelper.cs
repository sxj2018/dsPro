using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Utils.Net
{
    public class HttpHelper
    {
        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        public static string CreateGetHttpResponse(string url, int timeout = 20000, string userAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36", CookieCollection cookies = null)
        {
            HttpWebRequest request = null;
            //if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            //{
            //    //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
            //    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            //    request = WebRequest.Create(url) as HttpWebRequest;
            //    request.ProtocolVersion = HttpVersion.Version10;

            //}
            //else
            //{
            //    request = WebRequest.Create(url) as HttpWebRequest;
            //}
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";

            //设置代理UserAgent和超时
            request.UserAgent = userAgent;
            request.Timeout = timeout;
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            using (StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }
        /// <summary>
        /// 返回响应的byte[]数组
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static byte[] CreateGetHttpResponseBytes(string url, int timeout = 20000, string userAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36", CookieCollection cookies = null)
        {
            HttpWebRequest request = null;
            
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";

            //设置代理UserAgent和超时
            request.UserAgent = userAgent;
            request.Timeout = timeout;
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            var _response = request.GetResponse();
            using (var _binaryReader=new BinaryReader(_response.GetResponseStream()))
            {
                return _binaryReader.ReadBytes((int)_response.ContentLength);
            }
          
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public static string CreatePostHttpResponse(string url, IDictionary<string, string> parameters = null, int timeout = 20, string userAgent = "", CookieCollection cookies = null)
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";


            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            if (parameters == null)
                parameters = new Dictionary<string, string>() { { "_remark", "" } };

            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            var _webResponse = request.GetResponse() as HttpWebResponse;
            using (Stream s = _webResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();

            }

        }

      

        /// <summary>
        /// 验证证书
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
        public static T GetResponseString<T>(string url, string streamParm) where T : class
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            if (!string.IsNullOrEmpty(streamParm))
            {
                byte[] bt = Encoding.ASCII.GetBytes(streamParm);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bt, 0, bt.Length);
                }
            }
            string _result = "";
            using (Stream stream = request.GetResponse().GetResponseStream())
            {
                StreamReader read = new StreamReader(stream);
                _result = read.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(_result))
            {
                try
                {
                    T t = JsonConvert.DeserializeObject<T>(_result);
                    return t;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
