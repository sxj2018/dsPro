using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Utils.Tool
{
    public class CharacterUtil
    {
        public enum SQLLikeType
        {
            InitialMatch,
            EndMatch,
            PartMatch
        }
        /// <summary>
        /// 取得字符串的字节数
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>字节数</returns>
        public static int GetByteLength(string value)
        {
            int byteCount;
            try
            {
                byteCount = Encoding.GetEncoding(Tools.GetDefaultEncoding("utf-8")).GetByteCount(value);
            }
            catch (Exception)
            {
                byteCount = 0;
            }
            return byteCount;
        }

        /// <summary>
        /// 判断字符串字节数是否在范围内
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsByteValidate(string value, int minByte, int maxByte)
        {
            bool flag;
            try
            {
                if (GetByteLength(value) < minByte || GetByteLength(value) > maxByte)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否是浮点数
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsDecimalPoint(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, @"^[-+]?[0-9]+([\.][0-9]*)?$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否数字
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsNumber(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, @"^[-+]?[0-9]+$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否自然数
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsNaturalNumber(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, @"^[0-9]+$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否是金额
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsMoney(string value)
        {
            bool flag;
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                flag = Regex.IsMatch(value, @"^[0-9]+$|^[0-9]+\.[0-9]{1,2}$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否是日期 
        /// 例：20110101,2011-01-01,2011/01/01
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsDate(string value)
        {
            bool flag;
            try
            {
                DateTime datetime;
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                if (value.Length == 8)
                {
                    return (Regex.IsMatch(value, "^[0-9]+$") && DateTime.TryParse(value.Insert(4, "/").Insert(7, "/"), out datetime));

                }
                if (value.Length == 10)
                {
                    if (value.Substring(4, 1) == "-" && value.Substring(7, 1) == "-")
                    {

                        value = value.Replace("-", "/");
                    }
                    return ((((Regex.IsMatch(value.Substring(0, 4), "^[0-9]+$") && Regex.IsMatch(value.Substring(4, 1), "/")) && (Regex.IsMatch(value.Substring(5, 2), "^[0-9]+$") && Regex.IsMatch(value.Substring(7, 1), "/"))) && Regex.IsMatch(value.Substring(8, 2), "^[0-9]+$")) && DateTime.TryParse(value, out datetime));
                }
                flag = false;
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否是电子邮件地址
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsMail(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, @"^[A-Za-z0-9\-_\.]+[@][A-Za-z0-9\-_]+[\.][A-Za-z0-9\-_\.]+[A-Za-z0-9]$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否是电话号码
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsTel(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, @"^[0-9\-\#\*]+$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否是邮编
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsZipCode(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, @"^\\d{6}$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否全角字符
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsFullWidth(string value)
        {
            bool flag;
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }

                if (GetByteLength(value) == value.Length * 2)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否半角字符
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsHalfWidth(string value)
        {
            bool flag;
            try
            {

                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }

                flag = value.Length.Equals(GetByteLength(value));

            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否半角英文字符
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsHalfWidthAlphabet(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, "^[A-Za-z]+$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否半角英文字符或符号
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsHalfWidthAlphabetSign(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, "^[A-Za-z!-/:-@[-`{-~｡-･ ]+$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否半角英文字符或数字
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsHalfWidthNumericAlphabet(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, "^[0-9A-Za-z]+$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否半角英文字符或数字或符号
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsHalfWidthNumericAlphabetSign(string value)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(value, "^[!-~｡-･ ]+$");
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }



        /// <summary>
        /// 判断字符串是否年月
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>判断结果</returns>
        public static bool IsYearMonth(string value)
        {
            bool flag;
            try
            {
                value = value + "01";
                flag = IsDate(value);
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 将SQL参数中的单引号进行编码
        /// </summary>
        /// <param name="value">待编码的字符串</param>
        /// <returns>编码后的字符串</returns>
        public static string SQLEncode(string value, bool encloseFlag = true)
        {
            string str;
            try
            {
                //if (value == null || value == string.Empty)
                //{
                //    return null;
                //}
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                if (encloseFlag)
                {
                    str = "'" + Regex.Replace(value, "'", "''") + "'";
                }
                else
                {
                    str = Regex.Replace(value, "'", "''");
                }
            }
            catch
            {
                str = null;
            }
            return str;
        }

        public static string SQLEncodeWithEmpty(string value, bool encloseFlag = true)
        {
            string str;
            try
            {

                if (encloseFlag)
                {
                    str = "'" + Regex.Replace(value, "'", "''") + "'";
                }
                else
                {
                    str = Regex.Replace(value, "'", "''");
                }
            }
            catch
            {
                str = null;
            }
            return str;
        }

        public static string SQLLikeEncode(string value, SQLLikeType type, string escapeChar = "\b")
        {
            string strRet = string.Empty;

            try
            {
                string input = value;
                string str2 = "";
                string str = "";
                if (input == null)
                {
                    input = string.Empty;
                }
                input = Regex.Replace(Regex.Replace(Regex.Replace(input, "%", escapeChar + "%"), "％", escapeChar + "％"), "_", escapeChar + "_");
                input = Regex.Replace(input, @"\[", escapeChar + "[");
                if ((type == SQLLikeType.PartMatch) | (type == SQLLikeType.InitialMatch))
                {
                    str = "%";
                }
                if ((type == SQLLikeType.PartMatch) | (type == SQLLikeType.EndMatch))
                {
                    str2 = "%";
                }
                strRet = "'" + str2 + Regex.Replace(input, "'", "''") + str + "' ESCAPE '" + escapeChar + "'";
            }
            catch
            {
                strRet = "";
            }
            return strRet;

        }

        //空格分割要查询的条件 组合like条件
        public static string SQLLikeEncodeBySpace(string value, SQLLikeType type, string escapeChar = "\b")
        {
            string strRet = string.Empty;

            try
            {
                string input = value;
                string str2 = "";
                string str = "";
                if (input == null)
                {
                    input = string.Empty;
                }
                input = Regex.Replace(Regex.Replace(Regex.Replace(input, "%", escapeChar + "%"), "％", escapeChar + "％"), "_", escapeChar + "_");
                input = Regex.Replace(input, @"\[", escapeChar + "[");
                if ((type == SQLLikeType.PartMatch) | (type == SQLLikeType.InitialMatch))
                {
                    str = "%";
                }
                if ((type == SQLLikeType.PartMatch) | (type == SQLLikeType.EndMatch))
                {
                    str2 = "%";
                }
                strRet = "'" + str2 + Regex.Replace(input, "'", "''") + str + "' ESCAPE '" + escapeChar + "'";

                //根据要求需要把查询条件改为
                //如（输入的"上海血液" like 变成"%上%海%血%液%"） 中间也需要% 所以修改如下
                //==========
                //int a = 0;
                //string strvalue = Regex.Replace(input, "'", "''");
                //string restr = "";
                //string[] array = restr.Split(' ');

                //while (a < array.Length)
                //{
                //    restr += strvalue.Substring(a, 1) + "|";
                //    a++;
                //}
                //strRet = "'" + str2 + "[" + restr.Substring(0, restr.Length - 1) + "]" + str + "' ESCAPE '" + escapeChar + "'";
                //=========
            }
            catch
            {
                strRet = "";
            }
            return strRet;
        }

        //普通like条件 
        public static string SQLLikeEncodeNone(string value, SQLLikeType type, string escapeChar = "\b")
        {
            string strRet = string.Empty;

            try
            {
                string input = value;
                string str2 = "";
                string str = "";
                if (input == null)
                {
                    input = string.Empty;
                }
                input = Regex.Replace(Regex.Replace(Regex.Replace(input, "%", escapeChar + "%"), "％", escapeChar + "％"), "_", escapeChar + "_");
                input = Regex.Replace(input, @"\[", escapeChar + "[");
                if ((type == SQLLikeType.PartMatch) | (type == SQLLikeType.InitialMatch))
                {
                    str = "%";
                }
                if ((type == SQLLikeType.PartMatch) | (type == SQLLikeType.EndMatch))
                {
                    str2 = "%";
                }
                strRet = "'" + str2 + Regex.Replace(input, "'", "''") + str + "' ESCAPE '" + escapeChar + "'";

                //根据要求需要把查询条件改为
                //如（输入的"上海血液" like 变成"%上%海%血%液%"） 中间也需要% 所以修改如下
                //==========
                //int a = 0;
                //string strvalue = Regex.Replace(input, "'", "''");
                //string restr = "";
                //while (a < strvalue.Length)
                //{
                //    restr += strvalue.Substring(a, 1) + "|";
                //    a++;
                //}
                //strRet = "'" + str2 + "[" + restr.Substring(0, restr.Length - 1) + "]" + str + "' ESCAPE '" + escapeChar + "'";
                //=========
            }
            catch
            {
                strRet = "";
            }
            return strRet;
        }



        public static string NPrefixEncode(string sql)
        {
            if ((sql == null) || (sql.Length <= 0))
            {
                return sql;
            }
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            string str2 = string.Empty;
            TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(sql);
            while (textElementEnumerator.MoveNext())
            {
                if (!flag)
                {
                    if (textElementEnumerator.Current.ToString() == "'")
                    {
                        if (!str2.Equals("N"))
                        {
                            builder.Append("N");
                        }
                        builder.Append(RuntimeHelpers.GetObjectValue(textElementEnumerator.Current));
                        flag = true;
                    }
                    else
                    {
                        builder.Append(RuntimeHelpers.GetObjectValue(textElementEnumerator.Current));
                    }
                    //str2 = textElementEnumerator.Current.ToString();
                }
                else
                {
                    //str2 = textElementEnumerator.Current.ToString();
                    if (textElementEnumerator.Current.ToString() == "'")
                    {
                        if (textElementEnumerator.MoveNext())
                        {
                            if (textElementEnumerator.Current.ToString() == "'")
                            {
                                builder.Append("''");
                            }
                            else
                            {
                                builder.Append("'" + textElementEnumerator.Current.ToString());
                                flag = false;
                            }
                        }
                        else
                        {
                            builder.Append("'");
                            flag = false;
                        }
                    }
                    else
                    {
                        builder.Append(RuntimeHelpers.GetObjectValue(textElementEnumerator.Current));
                    }
                }
            }
            return builder.ToString();
        }

        public static string ReplaceNothing(string value, string replaceValue)
        {
            string str;
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    str = replaceValue;
                }
                else
                {
                    str = value;
                }
            }
            catch (Exception)
            {
                str = null;
            }
            return str;
        }


    }
}