using Code.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Attribute;
using Web.AuthToken;
using Webone.Auth;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        [AuthEscape]
        public ActionResult Login()
        {

            return View();
        }
        [AuthEscape]
        public string getLogin(TokenInfo tokenInfo)
        {
            var strToken = JwtHelper.GetToken(tokenInfo);

            return strToken;
        }

        public ActionResult Index()
        {
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
       
    }
}