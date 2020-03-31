using System.Collections.Generic;

namespace Code.Result
{
    public class BaseResult
    {
        public int success { set; get; }

        public string msg { set; get; }

        public List<Dictionary<string, object>> rows { set; get; }

        public int total { set; get; }
    }

    public class DapperResult
    {
        public int success { set; get; }

        public string msg { set; get; }

        public List<Dictionary<string, object>> rows { set; get; }

        public int total { set; get; }
    }

    public class DapperResult<T> : DapperResult
    {
        public IEnumerable<T> list { set; get; }
    }

}