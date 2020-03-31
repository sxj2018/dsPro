using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Xml;

namespace Utils.SQL
{
    public class SQLLoaderComponent
    {
        private const string DEFAULT_REPLACE_END_SYMBOL = "%}";
        private const string DEFAULT_REPLACE_START_SYMBOL = "{%";
        private const string DIRECTORY_SEARCHPATTERN = "*.xml";
        private const string SQL_FILE_PATH = "SQLXMLFilePath";
        private const string SQL_GROUP_ATTRIBUTE_NAME = "PGID";
        private const string SQL_GROUP_FORMAT = "G:{0}|K:{1}";
        private const string SQL_GROUP_NODE_NAME = "PART";
        private const string SQL_QUERY_ATTRIBUTE_NAME = "ID";
        private const string SQL_QUERY_NODE_NAME = "SQL";
        private const string SQL_REPLACE_END_KEY = "SQLXMLReplaceEndSymble";
        private const string SQL_REPLACE_START_KEY = "SQLXMLReplaceStartSymble";
        //private const string SINGLE_QUOTATION_PREFIX_ENCODE = "IsSingleQuotationPrefixEncode";
        //private bool IsSingleQuotationPrefixEncode = false;
        private string replaceEndSymbol = null;
        private string replaceStartSymbol = null;
        private static SQLLoaderComponent singleton = null;
        private Hashtable sqlQueryHash = null;

        private SQLLoaderComponent()
        {
            this.sqlQueryHash = this.PropertyLoad(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings[SQL_FILE_PATH]);
           // string appConfig = ConfigUtil.GetAppConfig(SQL_REPLACE_START_KEY);
            //if (string.IsNullOrEmpty(appConfig))
            {
                this.replaceStartSymbol = DEFAULT_REPLACE_START_SYMBOL;
            }
            //else
            //{
            //    this.replaceStartSymbol = appConfig;
            //}
            //appConfig = ConfigUtil.GetAppConfig(SQL_REPLACE_END_KEY);
            //if (string.IsNullOrEmpty(appConfig))
            {
                this.replaceEndSymbol = DEFAULT_REPLACE_END_SYMBOL;
            }
            //else
            //{
            //    this.replaceEndSymbol = appConfig;
            //}


            //try
            //{
            //    appConfig = ConfigUtil.GetAppConfig(SINGLE_QUOTATION_PREFIX_ENCODE);
            //    if (string.IsNullOrEmpty(appConfig))
            //    {
            //        this.IsSingleQuotationPrefixEncode = false;
            //    }
            //    else
            //    {
            //        this.IsSingleQuotationPrefixEncode = bool.Parse(appConfig);
            //    }
            //}
            //catch
            //{
            //    this.IsSingleQuotationPrefixEncode = false;
            //}
        }

        public static string GetSQLQuery(string id, Hashtable replaceQueryHash)
        {
            string str;
            Type type = typeof(SQLLoaderComponent);
            Monitor.Enter(type);
            try
            {
                SQLLoaderComponent getInstance = GetInstance;
                string targetString = null;
                targetString = getInstance.sqlQueryHash[id].ToString();
                if (targetString == null)
                {
                    throw new ArgumentException(string.Format("SQL资源文件内，指定的ID不存在！【ID:{0}】", id));
                }
                str = QueryReplace(targetString, replaceQueryHash, getInstance.replaceStartSymbol, getInstance.replaceEndSymbol);
            }
            catch (Exception innerException)
            {
                throw new SystemException("从SQL资源文件取得SQL失败！", innerException);
            }
            finally
            {
                Monitor.Exit(type);
            }
            return str;
        }

        public static string GetSQLQuery(string groupId, string id, Hashtable replaceQueryHash)
        {
            return GetSQLQuery(string.Format(SQL_GROUP_FORMAT, (object[])new string[] { groupId, id }), replaceQueryHash);
        }
        public static string GetSQLQuery(string targetString, Hashtable replaceQueryHash, string replaceStartSymbol, string replaceEndSymbol)
        {
            return QueryReplace(targetString, replaceQueryHash, replaceStartSymbol, replaceEndSymbol);
        }
        private Hashtable PropertyFileLoad(string sqlFilePath, Hashtable queryHash)
        {
            IEnumerator enumerator = null;
            IEnumerator enumerator2 = null;
            if (!File.Exists(sqlFilePath))
            {
                throw new FileNotFoundException("指定的SQL资源文件不存在！", sqlFilePath);
            }
            if (queryHash == null)
            {
                queryHash = new Hashtable();
            }
            XmlDocument document = new XmlDocument();
            XmlNode current = null;
            XmlElement element2 = null;
            XmlNode node = null;
            XmlElement element = null;
            string str2 = null;
            string innerText = null;
            string attribute = null;
            document.Load(sqlFilePath);

            try
            {
                enumerator = document.DocumentElement.ChildNodes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (XmlNode)enumerator.Current;
                    if (current is XmlElement)
                    {
                        element2 = (XmlElement)current;
                        if (SQL_GROUP_NODE_NAME.Equals(element2.Name))
                        {
                            attribute = element2.GetAttribute(SQL_GROUP_ATTRIBUTE_NAME);
                            if ((attribute != null) && !string.Empty.Equals(attribute))
                            {

                                try
                                {
                                    enumerator2 = element2.ChildNodes.GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        node = (XmlNode)enumerator2.Current;
                                        if (node is XmlElement)
                                        {
                                            element = (XmlElement)node;
                                            if (SQL_QUERY_NODE_NAME.Equals(element.Name))
                                            {
                                                str2 = element.GetAttribute(SQL_QUERY_ATTRIBUTE_NAME);
                                                if ((str2 != null) && !string.Empty.Equals(str2))
                                                {
                                                    innerText = element.InnerText;
                                                    try
                                                    {
                                                        queryHash.Add(string.Format(SQL_GROUP_FORMAT, (object[])new string[] { attribute, str2 }), innerText);
                                                        continue;
                                                    }
                                                    catch (ArgumentException aex)
                                                    {
                                                        throw new ArgumentException(string.Format("SQL资源文件中存在相同的ID！", attribute, str2), aex);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    if (enumerator2 is IDisposable)
                                    {
                                        (enumerator2 as IDisposable).Dispose();
                                    }
                                }
                            }
                        }
                        else if (SQL_QUERY_NODE_NAME.Equals(element2.Name))
                        {
                            str2 = element2.GetAttribute(SQL_QUERY_ATTRIBUTE_NAME);
                            if ((str2 != null) && !string.Empty.Equals(str2))
                            {
                                innerText = element2.InnerText;
                                try
                                {
                                    queryHash.Add(str2, innerText);
                                    continue;
                                }
                                catch (ArgumentException aex)
                                {
                                    throw new ArgumentException(string.Format("SQL资源文件中存在相同的ID！", str2), aex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            return queryHash;
        }

        private Hashtable PropertyLoad(string sqlPath)
        {
            Hashtable queryHash = null;
            if (File.Exists(sqlPath))
            {
                return this.PropertyFileLoad(sqlPath, queryHash);
            }
            if (!Directory.Exists(sqlPath))
            {
                throw new FileNotFoundException("指定的SQL资源文件路径不存在！", sqlPath);
            }
            string[] files = null;
            files = Directory.GetFiles(sqlPath, DIRECTORY_SEARCHPATTERN);
            int num2 = files.Length - 1;
            try
            {
                for (int i = 0; i <= num2; i++)
                {
                    if ((files[i] != null) && files[i].ToLower().EndsWith(".xml"))
                    {
                        queryHash = this.PropertyFileLoad(files[i], queryHash);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return queryHash;
        }

        private static string QueryReplace(string targetString, Hashtable replaceHash, string replaceStart, string replaceEnd)
        {
            if ((targetString != null) && (replaceHash != null))
            {
                IDictionaryEnumerator enumerator = replaceHash.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string str = enumerator.Key.ToString();
                    object newValue = enumerator.Value;
                    //if (IsSingleQuotationPrefixEncode == true)
                    //{
                    //    newValue = CharacterUtil.SQLEncode(newValue);
                    //}
                    if ((str != null))
                    {
                        string replaceValue = "null";
                        if (newValue != null)
                            replaceValue = newValue.ToString();
                        targetString = targetString.Replace(replaceStart + str + replaceEnd, replaceValue);
                    }
                }
            }
            return targetString;
        }

        private static SQLLoaderComponent GetInstance
        {
            get
            {
                if (GetCompilationDebugMode())
                {
                    singleton = new SQLLoaderComponent();
                }
                else if (singleton == null)
                {
                    singleton = new SQLLoaderComponent();
                }
                return singleton;
            }
        }

        public static bool GetCompilationDebugMode()
        {
            bool debug;
            try
            {
                debug = ((CompilationSection)ConfigurationManager.GetSection("system.web/compilation")).Debug;
            }
            catch
            {
                debug = false;
            }
            return debug;
        }
    }
}