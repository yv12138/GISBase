using System.Data;
using System.Data.Common;

using System.Data.OleDb;

using System.Data.SqlClient;

//using OraOracleConnection = System.Data.OracleClient.OracleConnection;
//using OraOracleCommand = System.Data.OracleClient.OracleCommand;
//using OraOracleCommandBuilder = System.Data.OracleClient.OracleCommandBuilder;
//using OraOracleDataAdapter = System.Data.OracleClient.OracleDataAdapter;
//using OraOracleDataReader = System.Data.OracleClient.OracleDataReader;
//using OraOracleParameter = System.Data.OracleClient.OracleParameter;
//using OraOracleTransaction = System.Data.OracleClient.OracleTransaction;

using OraOracleConnection = Oracle.DataAccess.Client.OracleConnection;
using OraOracleCommand = Oracle.DataAccess.Client.OracleCommand;
using OraOracleCommandBuilder = Oracle.DataAccess.Client.OracleCommandBuilder;
using OraOracleDataAdapter = Oracle.DataAccess.Client.OracleDataAdapter;
using OraOracleDataReader = Oracle.DataAccess.Client.OracleDataReader;
using OraOracleParameter = Oracle.DataAccess.Client.OracleParameter;
using OraOracleTransaction = Oracle.DataAccess.Client.OracleTransaction;

//using TekOracleConnection = DDTek.Oracle.OracleConnection;
//using TekOracleCommand = DDTek.Oracle.OracleCommand;
//using TekOracleCommandBuilder = DDTek.Oracle.OracleCommandBuilder;
//using TekOracleDataAdapter = DDTek.Oracle.OracleDataAdapter;
//using TekOracleDataReader = DDTek.Oracle.OracleDataReader;
//using TekOracleParameter = DDTek.Oracle.OracleParameter;
//using TekOracleTransaction = DDTek.Oracle.OracleTransaction;

namespace Service.DBOperator
{
    /// <summary>
    /// 目    的：数据库操作工厂
    /// </summary>
    internal class AccessFactory
    {
        #region Create IDbConnection

        /// <summary>
        /// 创建　IDbConnection
        /// </summary>
        /// <param name="databaseType">数据库类型</param>
        /// <returns>IDbConnection接口</returns>
        public IDbConnection CreateDbConnection(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MSSQL:
                    return new SqlConnection();

                case DatabaseType.MSAccess:
                    return new OleDbConnection();

                case DatabaseType.Oracle:
                    return new OraOracleConnection();

                //case DatabaseType.TekOracle:
                //    return new TekOracleConnection();

                default:
                    return null;
            }
        }

        #endregion

        #region Create IDbCommand

        /// <summary>
        /// 创建　IDbCommand
        /// </summary>
        /// <param name="databaseType">数据库类型</param>
        /// <returns>IDbCommand接口</returns>
        public IDbCommand CreateDbCommand(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MSSQL:
                    return new SqlCommand();

                case DatabaseType.MSAccess:
                    return new OleDbCommand();

                case DatabaseType.Oracle:
                    return new OraOracleCommand();

                //case DatabaseType.TekOracle:
                //    return new TekOracleCommand();

                default:
                    return null;
            }
        }

        #endregion

        #region Create IDbDataAdapter

        /// <summary>
        /// 创建　IDbDataAdapter
        /// </summary>
        /// <param name="databaseType">数据连接类型</param>
        /// <param name="commandText">sql查询语句</param>
        /// <param name="dbConnection">当前活动连接</param>
        /// <returns></returns>
        public IDbDataAdapter CreateDbDataAdapter(DatabaseType databaseType,string commandText,IDbConnection dbConnection)
        {
            switch (databaseType)
            {
                case DatabaseType.MSSQL:
                    return new SqlDataAdapter(commandText,(SqlConnection)dbConnection);

                case DatabaseType.MSAccess:
                    return new OleDbDataAdapter(commandText,(OleDbConnection)dbConnection);
                     
                case DatabaseType.Oracle:
                    return new Oracle.DataAccess.Client.OracleDataAdapter(commandText, (Oracle.DataAccess.Client.OracleConnection)dbConnection);
                     
                //case DatabaseType.TekOracle:
                //    return new TekOracleDataAdapter(commandText,(DDTek.Oracle.OracleConnection)dbConnection);

                default:
                    return null;
            }
        }

        #endregion

        #region Create IDbDataParameter

        /// <summary>
        /// 创建　IDbDataParameter
        /// </summary>
        /// <param name="databaseType">数据库类型</param>
        /// <returns>IDbDataParameter接口</returns>
        public IDbDataParameter CreateDbDataParameter(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MSSQL:
                    return new SqlParameter();

                case DatabaseType.MSAccess:
                    return new OleDbParameter();

                case DatabaseType.Oracle:
                    return new OraOracleParameter();

                //case DatabaseType.TekOracle:
                //    return new TekOracleParameter();

                default:
                    return null;
            }
        }

        #endregion

        #region Create IDbHelperParameterCache
        /// <summary>
        /// 创建　IDbHelperParameterCache
        /// </summary>
        /// <param name="databaseType">数据库类型</param>
        /// <returns>IDbHelperParameterCache接口</returns>
        //public IDbHelperParameterCache CreateDbHelperParameterCache(DatabaseType databaseType)
        //{
        //    switch (databaseType)
        //    {
        //        case DatabaseType.MSSQL:
        //            return new SqlHelperParameterCache();

        //        case DatabaseType.MSAccess:
        //            return new OleDbHelperParameterCache();

        //        case DatabaseType.Oracle:
        //            return new OraHelperParameterCache();

        //        case DatabaseType.TekOracle:
        //            return new TekOraHelperParameterCache();

        //        default:
        //            return null;
        //    }
        //}
        #endregion
    }
}
