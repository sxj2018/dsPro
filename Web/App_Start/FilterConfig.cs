using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Web.Attribute;

namespace Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            var isOpenCheck = false;
            string opencheck = ConfigurationManager.AppSettings["opencheck"];
            if (opencheck.Equals("true"))
            {
                isOpenCheck = true;
            }
            filters.Add(new AuthFilter() { IsCheck = isOpenCheck });

        }
    }
}
