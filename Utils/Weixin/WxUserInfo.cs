using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Weixin
{
    public class WxUserInfo
    {
        public string Subscribe { set; get; }
        public string OpenID { set; get; }
        public string Nickname { set; get; }
        public string Sex { set; get; }
        public string Province { set; get; }
        public string City { set; get; }
        public string Country { set; get; }
        public string HeadImgUrl { set; get; }
        public string UnionID { set; get; }
    }
}
