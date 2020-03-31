using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Utils.DEncrypt
{
    public static class MD5
    {

        private const string key = "1qazXSW@";
        public static string MD5Encode(string str)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            //将输入的字符串转换成字节数组
            byte[] bt = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
            //加密后的字符串为strMD5
            str = BitConverter.ToString(bt).Replace("-", "");
            return str;
        }
        /// <summary>
        /// 排序拼接
        /// </summary>
        /// <param name="_dic"></param>
        /// <returns></returns>
        public static string SortParms(Dictionary<string, string> _dic)
        {
            var _dicSort = _dic.OrderBy(c => c.Key);
            var _str = "";
            foreach (var item in _dicSort)
            {
                _str += item.Key + "=" + item.Value + "&";
            }
            return _str.Substring(0, _str.Length - 1);
        }
        /// <summary>
        /// 排序拼接加密
        /// </summary>
        /// <param name="_dic">字典</param>
        /// <param name="_key">密匙</param>
        /// <returns></returns>
        public static string SortParmsEncode(Dictionary<string, string> _dic,string _key) {
            
            var _str = _key + "&" + SortParms(_dic) + "&" + _key;
            return MD5Encode(_str);

        }

        /// <summary>
        /// 排序拼接加密
        /// </summary>
        /// <param name="_dic">字典</param>
        /// <param name="_key">密匙</param>
        /// <returns></returns>
        public static string SortParmsEncode(string pass)
        {
            Dictionary<string, string> _dic = new Dictionary<string, string>();
            _dic.Add("MD5", pass);
            string _key = key;
            var _str = _key + "&" + SortParms(_dic) + "&" + _key;
            return MD5Encode(_str);

        }
    }
}