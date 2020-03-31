using System;
using System.Data;
using System.Data.SqlClient;
namespace SMMS.Task.Comm
{
    public class SqlCommHelper
    {
        public static void SqlBulkCopy(DataTable _dtSource, string _connStr, string _destinationTableName, int _batchSize = 100000, int _timeOut = 180)
        {
            if (_dtSource.Rows.Count > 0)
            {
                try
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_connStr))
                    {
                        bulkCopy.BatchSize = 100000;
                        bulkCopy.BulkCopyTimeout = 120;
                        //将DataTable表名作为待导入库中的目标表名 
                        bulkCopy.DestinationTableName = _destinationTableName;
                        //将数据集合和目标服务器库表中的字段对应  
                        for (int i = 0; i < _dtSource.Columns.Count; i++)
                        {
                            //列映射定义数据源中的列和目标表中的列之间的关系
                            //bulkCopy
                            bulkCopy.ColumnMappings.Add(_dtSource.Columns[i].ColumnName, _dtSource.Columns[i].ColumnName);
                        }
                        bulkCopy.WriteToServer(_dtSource);
                    }
                }
                catch (Exception ex)
                {
                    //LogHelper _log = new LogHelper();
                    //_log.WriteLog("BulkCopy Error:" + ex.Message);
                }
            }
        }

        public static int ExecuteQuery(string _connStr, string _sql)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();


                SqlCommand _cmd = new SqlCommand(_sql, conn);
                _cmd.CommandTimeout = 3600;

                int val = _cmd.ExecuteNonQuery();

                return val;
            }
        }
        public static DataTable GetTable(string sql, string _conStr)
        {
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_conStr))
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
    }
}
