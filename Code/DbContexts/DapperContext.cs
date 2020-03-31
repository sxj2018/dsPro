using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using Code.Result;
using System.Threading.Tasks;
using Utils.Tool;
using System.Security.Principal;
using System.Collections;
using Utils.SQL;
using Utils.JSON;

namespace Code.DbContexts
{
    public class DapperContext
    {
        private readonly static string _conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static object _lockService = new object();
        private static string _sql = "";

        public IDbConnection GetConn()
        {
            return new SqlConnection(_conn);
        }

        public static string ControlNPrefix(string sql)
        {
            var str = CharacterUtil.NPrefixEncode(sql).Replace("NN'", "N'");
            return CharacterUtil.NPrefixEncode(sql);

        }

        public static DapperResult QueryNormal(string sql, bool npre = true)
        {
            DapperResult dar = new DapperResult();
            using (IDbConnection dbConnection = new SqlConnection(_conn))
            {

                try
                {
                    var sql1 = ControlNPrefix(sql);
                    if (npre == false) sql1 = sql;
                    IEnumerable<dynamic> obj = dbConnection.Query<dynamic>(sql1);
                    dar.rows = returnValue(obj);
                    dar.success = 1;
                    dar.msg = "searchsuccessful";
                    dar.total = obj.Count();
                }
                catch (Exception e)
                {
                    dar.rows = null;
                    dar.success = 0;
                    dar.msg = e.Message;
                    dar.total = 0;
                }
            }
            return dar;
        }

        public static DataTable QueryDataTable(string sql)
        {


            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(ds);
            }
            DataTable table = ds.Tables[0];
            return table;

        }

        public static DapperResult QueryPage(string sql, int offset, int limit, string orderby = "")
        {

            int startpage = offset;
            string pagetype = ConfigurationManager.AppSettings["PageType"];
            if (pagetype.Equals("1"))
            {
                startpage = offset / limit;
            }
            DapperResult dar = new DapperResult();
            //var end = startpage * limit;
            //var start = (startpage - 1) * limit + 1;
            ///以下是从0 开始
            var end = (startpage + 1) * limit;
            var start = (startpage) * limit + 1;

            if (string.IsNullOrEmpty(orderby))
            {
                int pos = sql.ToLower().LastIndexOf("order by");
                orderby = sql.Substring(pos + 8);
                sql = sql.Substring(0, pos);
            }
            var newsql = string.Format(@"select * from  (
                                             select ROW_NUMBER() over(order by {1}) as num,  m.* from ( 
                                                                      select TOP 100 PERCENT * from ({0}) a order BY  {1}
                                                                      ) m where 1=1  
                                             ) as t
                          where num between {2} and {3} ", sql, orderby, start, end);
            using (IDbConnection dbConnection = new SqlConnection(_conn))
            {
                try
                {
                    int obj = dbConnection.ExecuteScalar<int>(ControlNPrefix("SELECT COUNT(1) from(" + sql + ") a"));
                    var obj1 = dbConnection.Query<dynamic>(ControlNPrefix(newsql));
                    dar.rows = returnValue(obj1);
                    dar.success = 1;
                    dar.msg =  "searchsuccessful";
                    dar.total = obj;
                }
                catch (Exception e)
                {
                    dar.rows = null;
                    dar.success = 0;
                    dar.msg = e.Message;
                    dar.total = 0;
                }
            }
            return dar;
        }

        public static DapperResult QueryPageToContract(string sql, int offset, int limit, string orderby = "")
        {

            int startpage = offset;
            string pagetype = ConfigurationManager.AppSettings["PageType"];
            if (pagetype.Equals("1"))
            {
                startpage = offset / limit;
            }
            DapperResult dar = new DapperResult();
            //var end = startpage * limit;
            //var start = (startpage - 1) * limit + 1;
            ///以下是从0 开始
            var end = (startpage + 1) * limit;
            var start = (startpage) * limit + 1;

            if (string.IsNullOrEmpty(orderby))
            {
                int pos = sql.ToLower().LastIndexOf("order by");
                orderby = sql.Substring(pos + 8);
                sql = sql.Substring(0, pos);
            }
            var newsql = string.Format(@"select * from  (
                                             select ROW_NUMBER() over(order by {1}) as num,  m.* from ( 
                                                                      select TOP 100 PERCENT * from ({0}) a order BY  {1}
                                                                      ) m where 1=1  
                                             ) as t
                          where num between {2} and {3} ", sql, orderby, start, end);
            using (IDbConnection dbConnection = new SqlConnection(_conn))
            {
                try
                {
                    //int obj = dbConnection.ExecuteScalar<int>(ControlNPrefix("SELECT COUNT(1) from(" + sql + ") a"));
                    //var obj1 = dbConnection.Query<dynamic>(ControlNPrefix(newsql));
                    var obj3 = dbConnection.Query<dynamic>(ControlNPrefix(sql));
                    var obj = obj3.Count();
                    var obj1 = obj3.Skip(limit * startpage).Take(limit).ToList();//进行分页
                    dar.rows = returnValue(obj1);
                    dar.success = 1;
                    dar.msg = "searchsuccessful";
                    dar.total = obj;
                }
                catch (Exception e)
                {
                    dar.rows = null;
                    dar.success = 0;
                    dar.msg = e.Message;
                    dar.total = 0;
                }
            }
            return dar;
        }

        public static DapperResult Execute(string sql)
        {
            DapperResult dar = new DapperResult();
            using (IDbConnection dbConnection = new SqlConnection(_conn))
            {
                lock (_lockService)
                {
                    try
                    {
                        int obj = dbConnection.Execute(ControlNPrefix(sql));
                        if (obj > 0)
                        {
                            dar.rows = null;
                            dar.success = 1;
                            dar.msg = "插入/更新/删除条目为" + obj;
                            dar.total = 0;
                        }
                        else
                        {
                            dar.rows = null;
                            dar.success = 0;
                            dar.msg = "插入/更新/删除条目为0";
                            dar.total = 0;
                        }

                    }
                    catch (Exception e)
                    {
                        dar.rows = null;
                        dar.success = 0;
                        dar.msg = e.Message;
                        dar.total = 0;
                    }
                }
            }

            return dar;
        }
        public static async Task<int> ExecuteAsync(string sql)
        {
            DapperResult dar = new DapperResult();
            using (IDbConnection dbConnection = new SqlConnection(_conn))
            {
                try
                {
                    Task<int> obj = dbConnection.ExecuteAsync(ControlNPrefix(sql));
                    obj.Start();
                    int result = obj.Result;
                    if (result > 0)
                    {
                        dar.rows = null;
                        dar.success = 1;
                        dar.msg = "插入/更新/删除条目为" + obj;
                        dar.total = 0;
                    }
                    else
                    {
                        dar.rows = null;
                        dar.success = 0;
                        dar.msg = "插入/更新/删除条目为0";
                        dar.total = 0;
                    }

                }
                catch (Exception e)
                {
                    dar.rows = null;
                    dar.success = 0;
                    dar.msg = e.Message;
                    dar.total = 0;
                }
            }

            return dar.success;
        }

        public static DapperResult ExecuteTrans(List<string> sql)
        {
            DapperResult dar = new DapperResult();
            using (IDbConnection dbConnection = new SqlConnection(_conn))
            {
                lock (_lockService)
                {
                    dbConnection.Open();
                    IDbTransaction transaction = dbConnection.BeginTransaction();
                    try
                    {

                        foreach (string a in sql)
                        {
                            dbConnection.Execute(ControlNPrefix(a), null, transaction);
                        }
                        transaction.Commit();


                        dar.rows = null;
                        dar.success = 1;
                        dar.msg = "插入/更新/删除成功";
                        dar.total = 0;


                    }
                    catch (Exception e)
                    {
                        dar.rows = null;
                        dar.success = 0;
                        dar.msg = e.Message;
                        dar.total = 0;
                        transaction.Rollback();
                    }
                }
            }
            return dar;
        }

        public static List<Dictionary<string, object>> returnValue(IEnumerable<dynamic> obj)
        {
            if (obj.Count() == 0) return null;

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            var Keys = ((IDictionary<string, object>)obj.FirstOrDefault()).Keys.ToArray();
            foreach (var a in obj)//每一行信息，新建一个Dictionary<string,object>,将该行的每列信息加入到字典
            {

                Dictionary<string, object> result = new Dictionary<string, object>();

                // var proper = _model.GetType().GetProperties();
                var details = ((IDictionary<string, object>)a);

                foreach (var p in Keys)
                {
                    string fieldName = p;
                    var fieldValue = details[p];
                    if (!result.ContainsKey(fieldName))
                        result.Add(fieldName, fieldValue);
                }
                list.Add(result);
            }
            return list;

        }
        public static DapperResult CommonSql(string sqlid, IIdentity _identity)
        {
            var result = string.Empty;
            var dar = new DapperResult();

            var _sql = string.Format(@"
                            SELECT sqlcontent
                FROM TBL_T_DataSql a
                LEFT JOIN tb_SQL b ON a.sqlid = b.NewSqlID
                WHERE 
                CAST(a.sqlid AS NVARCHAR(50)) = '{0}' 
                OR b.sqlid = '{0}'", sqlid);

            var dr = QueryNormal(_sql, false);
            if (dr == null || dr.success == 0 || dr.rows == null)
            {
                dar.rows = null;
                dar.total = 0;
                dar.msg = "Querynodata";
                dar.success = 0;
                return dar;
            }
            List<Dictionary<string, object>> sqls = dr.rows;
            if (sqls.Count == 0)
            {
                dar.rows = null;
                dar.total = 0;
                dar.msg = "Querynodata";
                dar.success = 0;
                return dar;
            }


            string sql = sqls[0]["sqlcontent"].ToString();

            sql = sql.Replace("@RenYuanId", CharacterUtil.SQLEncode(ExtendIdentity.GetUserId(_identity))).Replace("@CompanyCode", CharacterUtil.SQLEncode(ExtendIdentity.GetOrganizationId(_identity)));
            //  sql = ControlNPrefix(sql);


            return QueryNormal(sql, false);
        }

        public static DapperResult CommonSqlPage(string sqlid, IIdentity _identity, int startpage, int limit, string orderby)
        {
            var result = string.Empty;
            var dar = new DapperResult();

            var _sql = $@"
                            SELECT sqlcontent
                FROM TBL_T_DataSql a
                LEFT JOIN tb_SQL b ON a.sqlid = b.NewSqlID
                WHERE 
                CAST(a.sqlid AS NVARCHAR(50)) = '{sqlid}' 
                OR b.sqlid = '{sqlid}'";


            List<Dictionary<string, object>> sqls = QueryNormal(_sql, false).rows;
            if (sqls.Count == 0)
            {
                dar.rows = null;
                dar.total = 0;
                dar.msg = "Querynodata";
                dar.success = 0;
                return dar;
            }


            string sql = sqls[0]["sqlcontent"].ToString();

            sql = sql.Replace("@RenYuanId", CharacterUtil.SQLEncode(ExtendIdentity.GetUserId(_identity))).Replace("@CompanyCode", CharacterUtil.SQLEncode(ExtendIdentity.GetOrganizationId(_identity)));
           

            return QueryPage(sql, startpage, limit, orderby);
            
        }

        public static DapperResult PageResult(string aa, string xml, string sql)
        {
            //Hashtable ht =  Utility.HtFromPage(_identity);
            Hashtable ht = new Hashtable();
            var esql = SQLLoaderComponent.GetSQLQuery(xml, sql, ht);
            var dar = new DapperResult();
            if (ht["offset"] == null)
                dar = DapperContext.QueryNormal(esql);
            else
                dar = DapperContext.QueryPage(esql, int.Parse(ht["offset"].ToString().Replace("'", "")), int.Parse(ht["limit"].ToString().Replace("'", "")));

            dar.msg = esql;
            return dar;
        }

        public static DapperResult QueryPageToContract(IIdentity _identity, string xml, string sql)
        {
            Hashtable ht = new Hashtable();
            ht = Utility.HtFromPage(_identity);
            //ht.Add("CompanyCode", CharacterUtil.SQLEncode(orgId));
            var esql = SQLLoaderComponent.GetSQLQuery(xml, sql, ht);
            var dar = new DapperResult();
            if (ht["offset"] == null)
                dar = DapperContext.QueryNormal(esql);
            else
                dar = DapperContext.QueryPageToContract(esql, int.Parse(ht["offset"].ToString().Replace("'", "")), int.Parse(ht["limit"].ToString().Replace("'", "")));
            return dar;
        }

        public static DapperResult SaveResult(IIdentity _identity, string xml, string sqlId = "")
        {

            if (string.IsNullOrEmpty(sqlId)) sqlId = xml;
            Hashtable ht =  Utility.HtFromPage(_identity);
            if (ht["RecordID"] == null || string.IsNullOrEmpty(ht["RecordID"].ToString()))
            {
                sqlId += "_Insert";
            }
            else
            {
                sqlId += "_Update";
            }
            var sql = SQLLoaderComponent.GetSQLQuery(xml, sqlId, ht);
            var dar = DapperContext.QueryNormal(sql);
            return dar;
        }

        public static DapperResult SaveResultForOneToMore(IIdentity _identity, string xml, string sql)
        {
            string[] sqls = { };
            List<string> lists = new List<string>();
            var list = Utility.GetRecordIDList(_identity);
            Hashtable ht = Utility.HtFromPage(_identity);
            int i = 0;
            
            foreach (var id in list)
            {
                if (string.IsNullOrEmpty(id))
                    continue;              
                ht.Remove("RecordID");
                ht.Add("RecordID", CharacterUtil.SQLEncode(id));
                lists.Add(SQLLoaderComponent.GetSQLQuery(xml, sql + "_Insert", ht));
                i++;

            }
            var dar = DapperContext.ExecuteTrans(lists);
            return dar;
        }

        public static DapperResult SaveResultForDiscount(IIdentity _identity, string xml, string sql,string Op)
        {

            string[] sqls = { };
            List<string> lists = new List<string>();           
            Hashtable ht  = Utility.HtFromPage(_identity);       
            string jsonText = Newtonsoft.Json.Linq.JObject.Parse(Utility.HtFromPageForDiscount(_identity))["RecordIDList"].ToString();
            var mJObj = Newtonsoft.Json.Linq.JArray.Parse(jsonText);
            for (int i=0;i< mJObj.Count;i++)
            {

                Newtonsoft.Json.Linq.JObject oData = Newtonsoft.Json.Linq.JObject.Parse(mJObj[i].ToString());
                ht.Remove("ApplyNo");
                ht.Add("ApplyNo", CharacterUtil.SQLEncode(oData["ApplyNo"].ToString()));
                ht.Remove("OrderProductId");
                ht.Add("OrderProductId", CharacterUtil.SQLEncode(oData["OrderProductId"].ToString()));
                ht.Remove("OrderId");
                ht.Add("OrderId", CharacterUtil.SQLEncode(oData["OrderId"].ToString()));
                ht.Remove("ProductCode");
                ht.Add("ProductCode", CharacterUtil.SQLEncode(oData["ProductCode"].ToString()));
                ht.Remove("SerSalesDiscountAttachId");
                ht.Add("SerSalesDiscountAttachId", CharacterUtil.SQLEncode(oData["SerSalesDiscountAttachId"].ToString()));
                ht.Remove("UseAmount");
                ht.Add("UseAmount", CharacterUtil.SQLEncode(oData["UseAmount"].ToString()));
                ht.Remove("ActualAmount");
                ht.Add("ActualAmount", CharacterUtil.SQLEncode(oData["ActualAmount"].ToString()));

                if (i == 0)
                {                    
                    lists.Add(SQLLoaderComponent.GetSQLQuery(xml, sql + "_Update", ht));
                }
                if (Op == "S")
                {
                    lists.Add(SQLLoaderComponent.GetSQLQuery(xml, sql + "_Insert", ht));
                }
            }
            var dar = DapperContext.ExecuteTrans(lists);
            return dar;
        }
        public static DapperResult SaveResultForDiscountForBFP(IIdentity _identity, string xml, string sql, string Op)
        {

            string[] sqls = { };
            List<string> lists = new List<string>();
            Hashtable ht = Utility.HtFromPage(_identity);
            string jsonText = Newtonsoft.Json.Linq.JObject.Parse(Utility.HtFromPageForDiscount(_identity))["RecordIDList"].ToString();
            var mJObj = Newtonsoft.Json.Linq.JArray.Parse(jsonText);
            if (!string.IsNullOrEmpty(ht["OrderId"].ToString()))
            {
                Hashtable htd = new Hashtable();
                htd.Add("OrderId", CharacterUtil.SQLEncode(ht["OrderId"].ToString()));
                htd.Add("OrderProductId", CharacterUtil.SQLEncode(ht["OrderProductId"].ToString()));           
                lists.Add(SQLLoaderComponent.GetSQLQuery(xml, sql + "_Delete", ht));
            }
            for (int i = 0; i < mJObj.Count; i++)
            {
                Newtonsoft.Json.Linq.JObject oData = Newtonsoft.Json.Linq.JObject.Parse(mJObj[i].ToString());             
                ht.Remove("ApplyNo");
                ht.Add("ApplyNo", CharacterUtil.SQLEncode(oData["ApplyNo"].ToString()));
                ht.Remove("OrderProductId");
                ht.Add("OrderProductId", CharacterUtil.SQLEncode(oData["OrderProductId"].ToString()));
                ht.Remove("OrderId");
                ht.Add("OrderId", CharacterUtil.SQLEncode(oData["OrderId"].ToString()));
                ht.Remove("ProductCode");
                ht.Add("ProductCode", CharacterUtil.SQLEncode(oData["ProductCode"].ToString()));
                ht.Remove("SerSalesDiscountAttachId");
                ht.Add("SerSalesDiscountAttachId", CharacterUtil.SQLEncode(oData["SerSalesDiscountAttachId"].ToString()));
                ht.Remove("UseAmount");
                ht.Add("UseAmount", CharacterUtil.SQLEncode(oData["UseAmount"].ToString()));
                ht.Remove("ActualAmount");
                ht.Add("ActualAmount", CharacterUtil.SQLEncode(oData["ActualAmount"].ToString()));

                if (Op == "S")
                {
                    lists.Add(SQLLoaderComponent.GetSQLQuery(xml, sql + "_Insert", ht));
                }             
            }
            var dar = DapperContext.ExecuteTrans(lists);
            return dar;
        }
        public static DapperResult SaveResultForSigleProduct(IIdentity _identity, string xml, string sql)
        {
            string[] sqls = { };
            List<string> lists = new List<string>();
            Hashtable ht = Utility.HtFromPage(_identity);
            string jsonText = Newtonsoft.Json.Linq.JObject.Parse(Utility.HtFromPageForDiscount(_identity))["RecordIDList"].ToString();
            var mJObj = Newtonsoft.Json.Linq.JArray.Parse(jsonText);
            for (int i = 0; i < mJObj.Count; i++)
            {
                Newtonsoft.Json.Linq.JObject oData = Newtonsoft.Json.Linq.JObject.Parse(mJObj[i].ToString());
                ht.Remove("OrderProductId");
                ht.Add("OrderProductId", CharacterUtil.SQLEncode(oData["OrderProductId"].ToString()));
                ht.Remove("OrderId");
                ht.Add("OrderId", CharacterUtil.SQLEncode(oData["OrderId"].ToString()));   
                if (i == 0)
                {
                    lists.Add(SQLLoaderComponent.GetSQLQuery(xml, sql + "_Update", ht));
                }                
            }
            var dar = DapperContext.ExecuteTrans(lists);
            return dar;
        }

        public static DapperResult SeachResult(IIdentity _identity, string xml, string sql)
        {
            Hashtable ht =  Utility.HtFromPage(_identity);         
            var esql = SQLLoaderComponent.GetSQLQuery(xml, sql, ht);
            var dar = DapperContext.QueryNormal(esql);
            dar.msg = esql;
            return dar;
        }

        public static DapperResult DeleteResult(IIdentity _identity, string xml, string sql)
        {
            string[] sqls = { };
            List<string> lists = new List<string>();
            var list = Utility.GetRecordIDList(_identity);
            Hashtable ht = new Hashtable();
            int i = 0;            
            foreach (var id in list)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                ht.Remove("RecordID");
                ht.Add("RecordID", CharacterUtil.SQLEncode(id));               
                lists.Add(SQLLoaderComponent.GetSQLQuery(xml, sql, ht));
                i++;
            }
            var dar = DapperContext.ExecuteTrans(lists);          
            return dar;
        }
        public static DapperResult DeleteResultForRole(IIdentity _identity, string xml, string sql)
        {
            string[] sqls = { };
            List<string> lists = new List<string>();
            var list = Utility.GetRecordIDList(_identity);
            Hashtable ht = Utility.HtFromPage(_identity);
            int i = 0;           
            foreach (var id in list)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                ht.Remove("RecordID");
                ht.Add("RecordID", CharacterUtil.SQLEncode(id));                
                lists.Add(SQLLoaderComponent.GetSQLQuery(xml, sql, ht));
                i++;
            }
            var dar = DapperContext.ExecuteTrans(lists);          
            return dar;
        }
        public static DapperResult DeleteBySpecifiedColunm(IIdentity _identity, string xml, string sql, string columnName)
        {
            var executeSql = string.Empty;

            var parameterValue = "'";

            var ht = Utility.HtFromPage(_identity);

            var valueList = ht[columnName]?.ToString().Replace("'", "").Replace("'[", "").Replace("]'", "").Replace("\r\n", "").Replace(" ", "").Split(',');

            if (valueList == null || valueList.Count() == 0)
            {
                ht.Remove($"{columnName}_Off");
                ht.Add($"{columnName}_Off", "--");
            }
            else
            {
                ht.Remove($"{columnName}_Off");
                ht.Add($"{columnName}_Off", string.Empty);

                foreach (var value in valueList)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    parameterValue += value + "','";
                }

                ht.Remove(columnName);
                ht.Add(columnName, parameterValue.Substring(0, parameterValue.Length - 2));
            }

            ht.Remove("CompanyCode");
            var orgId = ExtendIdentity.GetOrganizationId(_identity);
            ht.Add("CompanyCode", CharacterUtil.SQLEncode(orgId));

            executeSql = SQLLoaderComponent.GetSQLQuery(xml, sql, ht);

            var dar = DapperContext.Execute(executeSql);

            return dar;
        }

        public static DataTable searchInAllRecord(IIdentity _identity, string xml, string sql)
        {
            string[] sqls = { };
            string lists = "";
            var list = Utility.RecordFromPage(_identity);
            Hashtable ht = new Hashtable();
            int i = 0;
            string RecordIDs = "'";
            foreach (var id in list)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                RecordIDs += id + "','";
            }
            ht.Remove("RecordID");
            ht.Add("RecordID", RecordIDs.Substring(0, RecordIDs.Length - 2));
            lists = SQLLoaderComponent.GetSQLQuery(xml, sql, ht);

            var dar = DapperContext.QueryDataTable(lists);


            return dar;
        }

        /// <summary>
        /// 对指定列名使用IN表达式
        /// </summary>
        /// <param name="_identity"></param>
        /// <param name="xml"></param>
        /// <param name="sql"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DapperResult searchInSpecifiedColunm(IIdentity _identity, string xml, string sql, string columnName)
        {
            var executeSql = string.Empty;

            var parameterValue = "'";

            var ht = Utility.HtFromPage(_identity);

            var valueList = ht[columnName]?.ToString().Replace("'[", "").Replace("]'", "").Replace("\r\n", "").Replace(" ", "").Split(',');

            if (valueList == null || valueList.Count() == 0)
            {
                ht.Remove($"{columnName}_Off");
                ht.Add($"{columnName}_Off", "--");
            }
            else
            {
                ht.Remove($"{columnName}_Off");
                ht.Add($"{columnName}_Off", string.Empty);

                foreach (var value in valueList)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    parameterValue += value + "','";
                }

                ht.Remove(columnName);
                ht.Add(columnName, parameterValue.Substring(0, parameterValue.Length - 2));
            }

            ht.Remove("CompanyCode");
            var orgId = ExtendIdentity.GetOrganizationId(_identity);
            ht.Add("CompanyCode", CharacterUtil.SQLEncode(orgId));

            executeSql = SQLLoaderComponent.GetSQLQuery(xml, sql, ht);

            var dar = DapperContext.QueryPage(executeSql, int.Parse(ht["offset"].ToString().Replace("'", "")), int.Parse(ht["limit"].ToString().Replace("'", "")));

            return dar;
        }

        public static DapperResult ExcuteResult(IIdentity _identity, string xml, string sql)
        {
            Hashtable ht = new Hashtable();
            ht = Utility.HtFromPage(_identity);
            //ht.Add("CompanyCode", CharacterUtil.SQLEncode(orgId));
            var esql = SQLLoaderComponent.GetSQLQuery(xml, sql, ht);
            var dar = DapperContext.Execute(esql);
            return dar;
            
        }
        
    }
}