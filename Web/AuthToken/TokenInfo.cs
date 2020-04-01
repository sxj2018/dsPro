using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.AuthToken
{
    public class TokenInfo
    {

        public TokenInfo()
        {
            UserName = "admin";
            Pwd = "123456";
        }
        public string UserName { get; set; }
        public string Pwd { get; set; }
    }
}