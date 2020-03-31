using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.ExtentHelper { 
    public class ResultModel
    {
        public bool Success { set; get; }
        public string Msg { set; get; }
        public object Data { set; get; }
    }
}
