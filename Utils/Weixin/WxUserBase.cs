using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Weixin
{
    public class WxUserBase
    {
        public string Access_Token { set; get; }
        public string Expires_In { set; get; }
        public string Refresh_Token { set; get; }
        public string OpenID { set; get; }
        public string Scope { set; get; }
        public string Unionid { set; get; }
    }
}
