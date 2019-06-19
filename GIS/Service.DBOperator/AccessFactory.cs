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
    /// Ŀ    �ģ����ݿ��������
    /// </summary>
    internal class AccessFactory
    {
        #region Create IDbConnection

        /// <summary>
        /// ������IDbConnection
        /// </summary>
        /// <param name="databaseType">���ݿ�����</param>
        /// <returns>IDbConnection�ӿ�</returns>
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
        /// ������IDbCommand
        /// </summary>
        /// <param name="databaseType">���ݿ�����</param>
        /// <returns>IDbCommand�ӿ�</returns>
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
        /// ������IDbDataAdapter
        /// </summary>
        /// <param name="databaseType">������������</param>
        /// <param name="commandText">sql��ѯ���</param>
        /// <param name="dbConnection">��ǰ�����</param>
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
        /// ������IDbDataParameter
        /// </summary>
        /// <param name="databaseType">���ݿ�����</param>
        /// <returns>IDbDataParameter�ӿ�</returns>
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
        /// ������IDbHelperParameterCache
        /// </summary>
        /// <param name="databaseType">���ݿ�����</param>
        /// <returns>IDbHelperParameterCache�ӿ�</returns>
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
