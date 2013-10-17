/*
 * User: zouli
 * Date: 2010-8-3
 * Time: 14:24
 */

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using AccidentallyORM.Common;
using AccidentallyORM.Entity;

namespace AccidentallyORM.DBHelper
{
    /// <summary>
    ///     DB帮助类
    /// </summary>
    public class DbService
    {
        #region 属性

        /// <summary>
        ///     连接字名称
        /// </summary>
        private static string _connectionName = "ConnectionString";

        private static string ConnectionName
        {
            set { _connectionName = value; }
        }

        public string ConnectionProviderName { get; set; }

        public string ConnectionString { get; set; }

        #endregion

        #region 构造函数

        public DbService() : this(_connectionName)
        {
        }

        /// <param name="connectionName">连接字名称</param>
        public DbService(string connectionName)
            : this(
                ConfigHelper.GetConnectionStrings(connectionName),
                ConfigHelper.GetConnectionProviderName(connectionName)
                )
        {
            ConnectionName = connectionName;
        }

        /// <param name="connectionString">连接字符串</param>
        /// <param name="connectionProviderName">DB驱动类名</param>
        public DbService(string connectionString, string connectionProviderName)
        {
            ConnectionProviderName = connectionProviderName;
            ConnectionString = connectionString;

            SqlHelperDbProviderFactories.ConnectionProviderName = connectionProviderName;
        }

        #endregion

        #region ExecuteReader

        /// <summary>
        ///     执行查询，取得DataTable
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>查询结果</returns>
        public DataTable ExecuteReaderDataTable(string sql)
        {
            var parameters = new DbParameter[0];
            return ExecuteReaderDataTable(sql, parameters);
        }

        /// <summary>
        ///     执行查询，取得DataTable
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>查询结果</returns>
        public DataTable ExecuteReaderDataTable(string sql, DbParameter[] parameters)
        {
            var dt = new DataTable();
            dt.Load(SqlHelperDbProviderFactories.ExecuteReader(ConnectionString,
                                                               CommandType.Text,
                                                               sql,
                                                               parameters));
            return dt;
        }

        /// <summary>
        ///     执行查询，取得List T
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>查询结果</returns>
        public List<T> ExecuteReader<T>(string sql, params DbParameter[] parameters) where T : new()
        {
            IDataReader reader = SqlHelperDbProviderFactories.ExecuteReader(ConnectionString,
                                                                            CommandType.Text,
                                                                            sql,
                                                                            parameters);
            return EntityHelper.EntityParse<T>(reader);
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        ///     执行添加，修改，删除
        /// </summary>
        /// <param name="sql">添加，修改，删除语句</param>
        /// <returns>影响行数</returns>
        public int ExecuteNonQuery(string sql)
        {
            var parameters = new DbParameter[0];
            return ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        ///     执行添加，修改，删除
        /// </summary>
        /// <param name="sql">添加，修改，删除语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>影响行数</returns>
        public int ExecuteNonQuery(string sql, DbParameter[] parameters)
        {
            int result;
            if (null == _dbTransaction)
            {
                result = SqlHelperDbProviderFactories.ExecuteNonQuery(ConnectionString,
                                                                      CommandType.Text,
                                                                      sql,
                                                                      parameters);
            }
            else
            {
                result = SqlHelperDbProviderFactories.ExecuteNonQuery(_dbTransaction,
                                                                      CommandType.Text,
                                                                      sql,
                                                                      parameters);
            }
            return result;
        }

        #endregion

        #region 事务

        private DbConnection _connectionTransaction;
        private DbTransaction _dbTransaction;

        /// <summary>
        ///     启动事务
        /// </summary>
        public void BeginTransaction()
        {
            _connectionTransaction = SqlHelperDbProviderFactories.GetProviderFactory().CreateConnection();
            if (_connectionTransaction == null) return;
            _connectionTransaction.ConnectionString = ConnectionString;
            _connectionTransaction.Open();

            _dbTransaction = _connectionTransaction.BeginTransaction();
        }

        /// <summary>
        ///     回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            _dbTransaction.Rollback();
            _connectionTransaction.Close();
        }

        /// <summary>
        ///     提交事务
        /// </summary>
        public void CommitTransaction()
        {
            _dbTransaction.Commit();
            _connectionTransaction.Close();
        }

        #endregion

        /// <summary>
        ///     创建Parameter
        /// </summary>
        public static DbParameter CreateParameter(string field, DbType dbtype, object value)
        {
            var param = SqlHelperDbProviderFactories.GetProviderFactory().CreateParameter();
            if (param != null)
            {
                param.ParameterName = field;
                param.DbType = dbtype;
                param.Value = value;
                return param;
            }
            return null;
        }
    }
}