using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utils.Enum;

namespace Utils.Attr
{
    public class LikeAttribute : Attribute
    {
        public LikeType Type { get; set; } 
    }
}