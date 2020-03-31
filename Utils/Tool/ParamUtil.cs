using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Utils.Tool
{
    public class ParamUtil
    {
        public const string CST_DOUBLE_HYPHEN = "--";
        public const string CST_DOUBLE_SPACE = "  ";

        /// <summary>
        /// FormCollection转到Hashtable
        /// </summary>
        /// <param name="collection">界面传过来的FormCollection</param>
        /// <returns>转换后的Hashtable</returns>
        public static Hashtable FormCollectionToHashtable(FormCollection collection)
        {
            Hashtable ht = new Hashtable();
            for (int i = 0; i < collection.Count; i++)
            {
                ht.Add(collection.GetKey(i), CharacterUtil.SQLEncode(collection.GetValue(collection.GetKey(i)).AttemptedValue));
            }
            return ht;
        }

        public static string ReplaceHyphenOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
                return CST_DOUBLE_HYPHEN;
            else
                return CST_DOUBLE_SPACE;
        }

        public static string ReplaceHyphenOrEmpty(string value, string obj)
        {
            if (string.IsNullOrEmpty(value) || value == obj)
                return CST_DOUBLE_HYPHEN;
            else
                return CST_DOUBLE_SPACE;
        }

        private static Hashtable GetPropertyInfo(object target)
        {
            Hashtable hashtable;
            if (target == null)
            {
                return null;
            }
            try
            {
                PropertyInfo[] properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Hashtable hashtable2 = new Hashtable();
                foreach (PropertyInfo info in properties)
                {
                    if ((info != null) && (info.CanWrite & !string.IsNullOrEmpty(info.Name)))
                    {
                        hashtable2.Add(info.Name, info);
                    }
                }
                hashtable = hashtable2;
            }
            catch (Exception)
            {
                hashtable = null;

            }
            return hashtable;
        }

        public static object ParamReplace(object inParam, object outParam, bool toUpperFlag = true, string[] trimArray = null)
        {
            string expression = null;
            MethodInfo getMethod = null;
            Hashtable hashtable = null;
            object objectValue = null;
            PropertyInfo info2 = null;
            Hashtable propertyInfo = null;
            System.Type propertyType = null;
            Hashtable hashtable3 = null;
            string str2 = null;
            PropertyInfo info3 = null;
            Hashtable hashtable4 = null;
            System.Type type2 = null;
            object[] parameters = null;
            object obj3;
            MethodInfo setMethod = null;
            try
            {
                IEnumerator enumerator = null;
                propertyInfo = GetPropertyInfo(RuntimeHelpers.GetObjectValue(inParam));
                hashtable4 = GetPropertyInfo(RuntimeHelpers.GetObjectValue(outParam));
                hashtable = GetPropertyAliasName(RuntimeHelpers.GetObjectValue(inParam), toUpperFlag, trimArray);
                hashtable3 = GetPropertyAliasName(RuntimeHelpers.GetObjectValue(outParam), toUpperFlag, trimArray);
                try
                {
                    enumerator = hashtable4.Keys.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        str2 = enumerator.Current.ToString();
                        if (str2 != null)
                        {
                            expression = str2;
                        }
                        else
                        {
                            expression = "";
                        }
                        if (toUpperFlag)
                        {
                            expression = expression.ToUpper();
                        }
                        if ((trimArray != null) && (trimArray.Length > 0))
                        {
                            int num2 = trimArray.Length - 1;
                            for (int i = 0; i <= num2; i++)
                            {
                                if (trimArray[i] != null)
                                {
                                    //expression = Strings.Replace(expression, trimArray[i], "", 1, -1, CompareMethod.Binary);
                                    expression = expression.Replace(trimArray[i], "");
                                }
                            }
                        }
                        info2 = (PropertyInfo)propertyInfo[CharacterUtil.ReplaceNothing(hashtable[expression].ToString(), "")];
                        info3 = (PropertyInfo)hashtable4[CharacterUtil.ReplaceNothing(hashtable3[expression].ToString(), "")];
                        if (info3.CanWrite && !((info2 == null) | (info3 == null)))
                        {
                            propertyType = info2.PropertyType;
                            type2 = info3.PropertyType;
                            if (propertyType.FullName == type2.FullName)
                            {
                                getMethod = info2.GetGetMethod();
                                setMethod = info3.GetSetMethod();
                                if (!((getMethod == null) | (setMethod == null)))
                                {
                                    objectValue = RuntimeHelpers.GetObjectValue(getMethod.Invoke(RuntimeHelpers.GetObjectValue(inParam), null));
                                    parameters = new object[] { RuntimeHelpers.GetObjectValue(objectValue) };
                                    setMethod.Invoke(RuntimeHelpers.GetObjectValue(outParam), parameters);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                obj3 = outParam;
            }
            catch (Exception)
            {
                obj3 = null;
            }
            return obj3;
        }

        private static Hashtable GetPropertyAliasName(object target, bool toUpperFlag = true, string[] trimArray = null)
        {
            Hashtable hashtable;
            if (target == null)
            {
                return null;
            }
            try
            {
                PropertyInfo[] properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Hashtable hashtable2 = new Hashtable();
                foreach (PropertyInfo info in properties)
                {
                    if ((info != null) && (info.CanWrite & !string.IsNullOrEmpty(info.Name)))
                    {
                        string name = info.Name;
                        if (toUpperFlag)
                        {
                            name = name.ToUpper();
                        }
                        if ((trimArray != null) && (trimArray.Length > 0))
                        {
                            int num3 = trimArray.Length - 1;
                            for (int i = 0; i <= num3; i++)
                            {
                                if (trimArray[i] != null)
                                {
                                    //name = Strings.Replace(name, trimArray[i], "", 1, -1, CompareMethod.Binary);
                                    name = name.Replace(trimArray[i], "");
                                }
                            }
                        }
                        hashtable2.Add(name, info.Name);
                    }
                }
                hashtable = hashtable2;
            }
            catch (Exception)
            {
                hashtable = null;
            }
            return hashtable;
        }

    }
}