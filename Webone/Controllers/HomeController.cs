using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webone.Auth;

namespace Webone.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var strtoken=Testone();
            var msg = Testtwo(strtoken);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public string Testone()
        {
            //载荷（payload）
            var payload = new Dictionary<string, object>
            {
                { "iss","流月无双"},//发行人
                { "exp", DateTimeOffset.UtcNow.AddSeconds(60).ToUnixTimeSeconds() },//到期时间
                { "sub", "testJWT" }, //主题
                { "aud", "USER" }, //用户
                { "iat", DateTime.Now.ToString() }, //发布时间 
                { "data" ,new { name="111",age=11,address="hubei"} }
            };
            //生成JWT
            Console.WriteLine("******************生成JWT*******************");
            string JWTString = JwtHelper.CreateJWT(payload);
            Console.WriteLine(JWTString);
           
            //var aa = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiLmtYHmnIjml6Dlj4wiLCJleHAiOjE1ODQ0NTA3MTAsInN1YiI6InRlc3RKV1QiLCJhdWQiOiJVU0VSIiwiaWF0IjoiMjAyMC8zLzE3IDIxOjExOjQwIiwiZGF0YSI6eyJuYW1lIjoiMTExIiwiYWdlIjoxMSwiYWRkcmVzcyI6Imh1YmVpIn19.4N1yiHguaAkaiNaqzJppKXpunpJskQ-BZ415xs675ZI";
            //var bb = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiLmtYHmnIjml6Dlj4wiLCJleHAiOjE1ODQ0NTA3NDYsInN1YiI6InRlc3RKV1QiLCJhdWQiOiJVU0VSIiwiaWF0IjoiMjAyMC8zLzE3IDIxOjEyOjE3IiwiZGF0YSI6eyJuYW1lIjoiMTExIiwiYWdlIjoxMSwiYWRkcmVzcyI6Imh1YmVpIn19.dTGZLAomm9Zpx-502ktiGy_yARiJ6nWuWFh0ml_atyc";
            ////校验JWT
            //Console.WriteLine("*******************校验JWT，获得载荷***************");
            //string ResultMessage="";//需要解析的消息
            //string Payload;//获取负载
            //if (JwtHelper.ValidateJWT(JWTString, out Payload, out ResultMessage))
            //{
            //    Console.WriteLine(Payload);
            //}
            //Console.WriteLine(ResultMessage);//验证结果说明
            //Console.WriteLine("*******************END*************************");
            return JWTString;
        }
        public string Testtwo(string JWTString)
        {
            //载荷（payload）
            var payload = new Dictionary<string, object>
            {
                { "iss","流月无双"},//发行人
                { "exp", DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds() },//到期时间
                { "sub", "testJWT" }, //主题
                { "aud", "USER" }, //用户
                { "iat", DateTime.Now.ToString() }, //发布时间 
                { "data" ,new { name="111",age=11,address="hubei"} }
            };
            //校验JWT
            Console.WriteLine("*******************校验JWT，获得载荷***************");
            string ResultMessage = "";//需要解析的消息
            string Payload;//获取负载
            if (JwtHelper.ValidateJWT(JWTString, out Payload, out ResultMessage))
            {
                Console.WriteLine(Payload);
            }
            Console.WriteLine(ResultMessage);//验证结果说明
            Console.WriteLine("*******************END*************************");
            return ResultMessage;
        }


    }
}