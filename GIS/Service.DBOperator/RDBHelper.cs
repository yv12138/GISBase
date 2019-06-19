using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Xml;
using System;
using System.Data.Common;
//using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Data.OleDb;
//using Kingo.Common.OracleSpatial.Geometry;
//using TekOracleDbType = DDTek.Oracle.OracleDbType;
//using TekOracleConnection = DDTek.Oracle.OracleConnection;
//using TekOracleCommand = DDTek.Oracle.OracleCommand;
//using TekOracleCommandBuilder = DDTek.Oracle.OracleCommandBuilder;
//using TekOracleDataAdapter = DDTek.Oracle.OracleDataAdapter;
//using TekOracleDataReader = DDTek.Oracle.OracleDataReader;
//using TekOracleParameter = DDTek.Oracle.OracleParameter;
//using TekOracleTransaction = DDTek.Oracle.OracleTransaction;
//using System.Data.OracleClient;
using Oracle.DataAccess.Client;
//using System.Windows.Forms;

namespace Service.DBOperator
{
    public class RDBHelper : IRDBHelper
    {
        #region 私有变量

        /// <summary>
        /// 当前连接
        /// </summary>
        private IDbConnection activeConn;
        /// <summary>
        /// 查询命令
        /// </summary>
        private IDbCommand pDbCommand;
        /// <summary>
        /// 数据库连接类型
        /// </summary>
        private DatabaseType connDbType;
        /// <summary>
        /// 数据连接字符串生成
        /// </summary>
        private DbConnectionStringBuilder dcsBuilder;
        /// <summary>
        /// 数据库用户
        /// </summary>
        private string dbUser;
        /// <summary>
        /// 密码
        /// </summary>
        private string dbPWD;
        /// <summary>
        /// Oracle服务名
        /// </summary>
        private string SID;
        /// <summary>
        /// Oracle或者SQLServer的机器名
        /// </summary>
        private string server;
        /// <summary>
        /// Oracle数据库端口
        /// </summary>
        private string port;


        /// <summary>
        /// Access数据库全路径名
        /// </summary>
        public string dbFileName = "";
        /// <summary>
        /// 初始连接的SQL Server库
        /// </summary>
        private string sInitCatalog = "";
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string connString = "";
        /// <summary>
        /// 连接重试次数
        /// </summary>
        private int retryTimes = 3;
        /// <summary>
        /// 事务
        /// </summary>
        private IDbTransaction pDbTransaction;
        /// <summary>
        /// 启动事务时间
        /// </summary>
        private long startTransactionTime = 0;

        /// <summary>
        /// 数据库操作参数缓存
        /// </summary>
        //private IDbHelperParameterCache pDbHelperParameterCache;
        /// <summary>
        /// 数据库操作DataAdapter集合
        /// </summary>
        private IList<ItemInfo<IDataAdapter, string>> pDataAdapterCollection = new List<ItemInfo<IDataAdapter, string>>();

        /// <summary>
        /// 当前活动的DataAdapter
        /// </summary>
        private IDataAdapter pDataAdapter;
        /// <summary>
        /// 当前活动的DataSet
        /// </summary>
        private DataSet activeDataSet = new DataSet();
        /// <summary>
        ///　访问工厂
        /// </summary>
        private AccessFactory pAccessFactory = new AccessFactory();
        /// <summary>
        /// 初始化sdo_geometry
        /// </summary>
        //private SdoGeometry sdo_geo = new SdoGeometry();
        #endregion

        #region 属性
        public DatabaseType CnnDbType
        {
            get { return connDbType; }
        }

        /// <summary>
        /// 当前连接
        /// </summary>
        public IDbConnection ActiveConn
        {
            get
            {
                return activeConn;
            }
            set
            {
                activeConn = value;
            }
        }
        /// <summary>
        /// 初始连接的SQL Server库
        /// </summary>
        public string InitCatalog
        {
            get
            {
                return sInitCatalog;
            }
            set
            {
                sInitCatalog = value;
            }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string User
        {
            get
            {
                return dbUser;
            }
            set
            {
                dbUser = value;
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string PWD
        {
            get
            {
                return dbPWD;
            }
            set
            {
                dbPWD = value;
            }
        }
        /// <summary>
        /// Oracle服务名
        /// </summary>
        public string Service
        {
            get
            {
                return SID;
            }
            set
            {
                SID = value;
            }
        }
        /// <summary>
        /// Oracle数据库端口
        /// </summary>
        public string Port
        {
            get { return port; }
            set { port = value; }
        }
        /// <summary>
        /// Oracle或SQLServer的机器名
        /// </summary>
        public string Server
        {
            get
            {
                return server;
            }
            set
            {
                server = value;
            }
        }

        #endregion

        #region 初始化环境

        public RDBHelper(DatabaseType dbType)
        {
            this.DataBase = dbType;
            //根据连接类型，创建数据库连接
            activeConn = pAccessFactory.CreateDbConnection(dbType);
            //实例化数据命令集
            pDbCommand = pAccessFactory.CreateDbCommand(dbType);
            //实例化参数缓存
            //pDbHelperParameterCache = pAccessFactory.CreateDbHelperParameterCache(dbType);
        }

        internal RDBHelper(IDbConnection DbConnection)
        {
            try
            {

                this.DataBase = GetType4IDbConnection(DbConnection);

                this.ActiveConn = DbConnection;

                //实例化数据命令集
                this.pDbCommand = pAccessFactory.CreateDbCommand(this.DataBase);
                //实例化参数缓存
                // this.pDbHelperParameterCache = pAccessFactory.CreateDbHelperParameterCache(this.DataBase);
                this.ConnectionString = DbConnection.ConnectionString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        DatabaseType GetType4IDbConnection(IDbConnection DbConnection)
        {
            if (DbConnection is OleDbConnection)
                return DatabaseType.MSAccess;
            if (DbConnection is SqlConnection)
                return DatabaseType.MSSQL;
            if (DbConnection is OracleConnection)
                return DatabaseType.Oracle;
            //if (DbConnection is TekOracleConnection)
            //    return DatabaseType.TekOracle;
            return DatabaseType.Unknown;
        }

        /// <summary>
        /// 初始化连接环境
        /// </summary>
        public void InitEnvironment()
        {
            //根据连接类型，创建数据库连接
            activeConn = pAccessFactory.CreateDbConnection(connDbType);
            //实例化数据命令集
            pDbCommand = pAccessFactory.CreateDbCommand(connDbType);
            //实例化参数缓存
            //pDbHelperParameterCache = pAccessFactory.CreateDbHelperParameterCache(connDbType);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 绑定参数
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="commandParameters">参数集合</param>
        private void AttachParameters(IDbCommand command, IDbDataParameter[] commandParameters)
        {
            foreach (IDbDataParameter p in commandParameters)
            {
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }
        }
        /// <summary>
        /// 给参数赋值
        /// </summary>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="parameterValues">参数值集合</param>
        private void AssignParameterValues(IDbDataParameter[] commandParameters, object[] parameterValues)
        {
            try
            {
                if ((commandParameters == null) || (parameterValues == null))
                {
                    return;
                }

                if (commandParameters.Length != parameterValues.Length)
                {
                    throw new ArgumentNullException("参数集合数目与参数值数不一致");
                }

                for (int i = 0, j = commandParameters.Length; i < j; i++)
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 查询命令准备
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="connection">所属连接</param>
        /// <param name="transaction">所属事务</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="mustCloseConnection">是否关闭连接</param>
        private void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDbDataParameter[] commandParameters, out bool mustCloseConnection)
        {
            try
            {
                //查询命令和语句是否为空
                if (command == null)
                {
                    throw new ArgumentNullException("command为空!");
                }
                if (commandText == null || commandText.Length == 0)
                {
                    throw new ArgumentNullException("commandText为空!");
                }
                //连接是否打开
                if (connection.State != ConnectionState.Open)
                {
                    mustCloseConnection = true;
                    connection.Open();
                }
                else
                {
                    mustCloseConnection = false;
                }
                //绑定连接
                command.Connection = connection;
                command.CommandText = commandText;
                //绑定事务
                if (transaction != null)
                {
                    if (transaction.Connection == null)
                    {
                        return;
                    }
                    command.Transaction = transaction;
                }
                //绑定参数
                command.CommandType = commandType;
                if (commandParameters != null)
                {
                    AttachParameters(command, commandParameters);
                }
                return;
            }
            catch (Exception ex)
            {
                mustCloseConnection = false;
            }
        }
        /// <summary>
        /// 根据健值获取DataAdapter集合中的DataAdapter
        /// </summary>
        /// <param name="strKey">键值</param>
        /// <returns></returns>
        private IDataAdapter CurrentDataAdapter(string strKey)
        {
            foreach (ItemInfo<IDataAdapter, string> item in pDataAdapterCollection)
            {
                if (item.DisplayValue == strKey)
                {
                    return item.InnerValue;
                }
            }
            return null;
        }
        /// <summary>
        /// 根据健值移除DataAdapter集合中的DataAdapter
        /// </summary>
        /// <param name="strKey">键值</param>
        /// <returns></returns>
        private Boolean RemoveDataAdapter(string strKey)
        {
            foreach (ItemInfo<IDataAdapter, string> item in pDataAdapterCollection)
            {
                if (item.DisplayValue == strKey)
                {
                    pDataAdapterCollection.Remove(item);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 开始事务记时
        /// </summary>
        private void StartTransaction()
        {
            startTransactionTime = DateTime.Now.Ticks;
        }
        /// <summary>
        /// 结束事务记时
        /// </summary>
        /// <returns>事务耗时(毫秒)</returns>
        private Double OverTransaction()
        {
            return (DateTime.Now.Ticks - startTransactionTime) / Math.Pow(10, 4);
        }

        #endregion

        #region IDbHelper 成员

        #region 通用成员

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DataBase
        {
            get { return this.connDbType; }
            internal set { this.connDbType = value; }
        }

        /// <summary>
        /// 连接状态
        /// </summary>
        public ConnectionState State
        {
            get
            {
                if (ActiveConn != null)
                {
                    return ActiveConn.State;
                }
                else
                {
                    return ConnectionState.Closed;
                }
            }
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                dcsBuilder = new DbConnectionStringBuilder();
                switch (connDbType)
                {
                    case DatabaseType.MSAccess:
                        {
                            dcsBuilder.Clear();
                            dcsBuilder.Add("Provider", "Microsoft.Jet.Oledb.4.0");
                            dcsBuilder.Add("User ID", User);
                            dcsBuilder.Add("Password", PWD);
                            dcsBuilder.Add("Data Source", @dbFileName);
                            return dcsBuilder.ConnectionString;
                        }
                    case DatabaseType.Oracle:
                        {
                            dcsBuilder.Clear();
                            dcsBuilder.Add("User ID", User);
                            dcsBuilder.Add("Password", PWD);
                            // dcsBuilder.Add("Data Source", Service);
                            dcsBuilder.Add("Data Source", string.Format("(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={2})))", Server, Port, Service));
                            //dcsBuilder.Add("Integrated Security", false);
                            dcsBuilder.Add("Unicode", true);
                            return dcsBuilder.ConnectionString;
                        }
                    case DatabaseType.MSSQL:
                        {
                            dcsBuilder.Clear();
                            dcsBuilder.Add("User ID", User);
                            dcsBuilder.Add("Password", PWD);
                            dcsBuilder.Add("Server", Server);
                            dcsBuilder.Add("initial catalog", InitCatalog);
                            dcsBuilder.Add("Integrated Security", false);
                            return dcsBuilder.ConnectionString;
                        }
                    case DatabaseType.TekOracle:
                        {
                            dcsBuilder.Clear();
                            dcsBuilder.Add("User ID", User);
                            dcsBuilder.Add("Password", PWD);
                            dcsBuilder.Add("Service Name", Service);
                            dcsBuilder.Add("Host", Server);
                            dcsBuilder.Add("Integrated Security", false);
                            return dcsBuilder.ConnectionString;
                        }
                    default:
                        {
                            return "";
                        }
                }
            }
            set
            {
                connString = value;
                //dcsBuilder = new DbConnectionStringBuilder();
                //dcsBuilder.ConnectionString = connString;
                //dbUser = dcsBuilder["User ID"].ToString();
                //dbPWD = dcsBuilder["Password"].ToString();
                pDbCommand.Connection = activeConn;
                pDbCommand.Transaction = pDbTransaction;
            }
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (ActiveConn == null)
            {
                try
                {
                    while (retryTimes >= 1)
                    {
                        InitEnvironment();
                        ActiveConn.ConnectionString = ConnectionString;
                        ActiveConn.Open();
                        if (ActiveConn.State == ConnectionState.Open)
                            return true;
                        retryTimes--;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;//连接失败
                }
            }
            if (ActiveConn.State == ConnectionState.Open) return true;
            if (ActiveConn.State != ConnectionState.Broken && ActiveConn.State != ConnectionState.Closed)
            {
                //数据库连接已经打开
                return true;
            }
            if (ActiveConn.State != ConnectionState.Broken)
            {
                ActiveConn.Close();
            }
            try
            {
                while (retryTimes >= 1)
                {
                    ActiveConn.ConnectionString = ConnectionString;
                    ActiveConn.Open();
                    if (ActiveConn.State == ConnectionState.Open)
                        return true;
                    retryTimes--;
                }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("连接失败:" + ex.Message);
                return false;//连接失败
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool DisConnect()
        {
            bool bFlag;
            try
            {
                if (ActiveConn == null)
                {
                    bFlag = false;
                }
                if (ActiveConn.State != ConnectionState.Broken &&
                    ActiveConn.State != ConnectionState.Closed)
                {
                    //关闭连接
                    ActiveConn.Close();
                    ActiveConn.Dispose();
                    bFlag = true;
                }
                else
                {
                    bFlag = true;
                }
            }
            catch (Exception ex)
            {
                bFlag = false;
            }
            return bFlag;
        }

        #endregion

        #region 事务成员

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>是否成功</returns>
        public bool BeginTransaction()
        {
            bool bFlag;
            try
            {
                StartTransaction();


                pDbTransaction = ActiveConn.BeginTransaction();

                pDbCommand.Transaction = pDbTransaction;

                bFlag = true;
            }
            catch (Exception ex)
            {
                bFlag = false;
            }

            return bFlag;
        }
        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <returns>是否成功</returns>
        public bool Rollback()
        {
            bool bFlag;
            try
            {
                pDbTransaction.Rollback();
                bFlag = true;
                //修改人：杨江川 修改时间：2010-01-15
                pDbTransaction = null;

            }
            catch (Exception ex)
            {
                bFlag = false;
            }

            return bFlag;
        }
        /// <summary>
        /// 事务提交
        /// </summary>
        /// <returns>是否成功</returns>
        public bool Commit()
        {
            bool bFlag;
            try
            {
                pDbTransaction.Commit();
                bFlag = true;
                //修改人：杨江川 修改时间：2010-01-15
                pDbTransaction = null;
            }
            catch (Exception ex)
            {
                bFlag = false;
            }
            return bFlag;
        }

        #endregion

        #region DataTable操作

        public DataTable GetTable(string tableName)
        {
            try
            {
                foreach (DataTable dt in activeDataSet.Tables)
                {
                    if (dt.TableName == tableName)
                        return dt;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private IList<string> dtNameList = null;

        public IList<string> GetTablesName()
        {
            try
            {
                //定义表名称集合
                if (dtNameList == null)
                    dtNameList = new List<string>();
                dtNameList.Clear();
                //加入表名称集合
                foreach (ItemInfo<IDataAdapter, string> item in pDataAdapterCollection)
                {
                    dtNameList.Add(item.DisplayValue);
                }
                return dtNameList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SaveTable(string tableName)
        {
            bool bFlag;
            try
            {
                pDataAdapter = CurrentDataAdapter(tableName);

                //根据连接类型保存
                switch (connDbType)
                {
                    case DatabaseType.MSSQL:
                        {

                            SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder((SqlDataAdapter)pDataAdapter);
                            ((SqlDataAdapter)pDataAdapter).Update(activeDataSet, tableName);
                        }
                        break;
                    case DatabaseType.MSAccess:
                        {
                            OleDbCommandBuilder commandBuilder = new OleDbCommandBuilder((OleDbDataAdapter)pDataAdapter);
                            ((OleDbDataAdapter)pDataAdapter).Update(activeDataSet, tableName);
                        }
                        break;
                    case DatabaseType.Oracle:
                        {
                            OracleCommandBuilder oraCommandBuilder = new OracleCommandBuilder((OracleDataAdapter)pDataAdapter);
                            ((OracleDataAdapter)pDataAdapter).Update(activeDataSet, tableName);
                        }
                        break;
                    //case DatabaseType.TekOracle:
                    //    {
                    //        DDTek.Oracle.OracleCommandBuilder TekOracleCommandBuilder = new DDTek.Oracle.OracleCommandBuilder((DDTek.Oracle.OracleDataAdapter)pDataAdapter);
                    //        ((DDTek.Oracle.OracleDataAdapter)pDataAdapter).Update(activeDataSet, tableName);
                    //    }
                    //    break;
                }
                bFlag = true;
            }
            catch (Exception ex)
            {
                bFlag = false;
            }
            return bFlag;
        }

        public bool SaveTable(string tableName, bool release)
        {
            if (release)
            {
                return ReleaseTable(tableName, true);
            }
            else
            {
                return SaveTable(tableName);
            }
        }

        public bool ReleaseTable(string tableName)
        {
            return ReleaseTable(tableName, false);
        }

        public bool ReleaseTable(string tableName, bool storage)
        {
            if (tableName == null && tableName == "") return false;
            bool bFlag = false;
            if (storage)
            {
                bFlag = this.SaveTable(tableName);
            }
            try
            {
                RemoveDataAdapter(tableName);
                activeDataSet.Tables.Remove(tableName);
            }
            catch (Exception ex)
            {
            }
            return bFlag;
        }

        public bool Clear()
        {
            bool bFlag = false;
            try
            {
                activeDataSet.Tables.Clear();
                pDataAdapterCollection.Clear();
                bFlag = true;
            }
            catch (Exception ex)
            {
                bFlag = false;
            }
            return bFlag;
        }

        #endregion

        #region Blob字段操作部份

        /// <summary>
        /// 读取Oracle数据blog字段到Byte[]对象
        /// </summary>
        /// <param name="tabname">表名字</param>
        /// <param name="fieldname">字段名字</param>
        /// <param name="wherestr">Where条件语句</param>
        /// <returns></returns>
        public byte[] ReadFromOracleBlog(string tabname, string fieldname, string wherestr)
        {
            String sSql = "SELECT " + fieldname + " FROM " + tabname + " " + wherestr;
            Byte[] blob = null;
            try
            {
                pDbCommand.Parameters.Clear();
                pDbCommand.CommandType = CommandType.Text;
                pDbCommand.CommandText = sSql;
                // 创建DataReader
                IDataReader dr = pDbCommand.ExecuteReader();
                // 只返回第一行，也是唯一行的数据
                if (dr.Read())
                {
                    // 将BLOB转Stream
                    if (dr[fieldname] == DBNull.Value)
                    {
                        return null;
                    }
                    blob = (Byte[])dr[fieldname];
                }
                else
                {
                    return null;
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                blob = null;
            }
            return blob;
        }

        public byte[] ReadBlobToBytes(string commandText)
        {
            return ReadBlobToBytes(commandText, (IDbDataParameter[])null);
        }

        public byte[] ReadBlobToBytes(string commandText, IDbDataParameter[] commandParameters)
        {
            Byte[] blob = null;
            try
            {
                object obj = ExecuteScalar(commandText, CommandType.Text, commandParameters);
                blob = obj as Byte[];
                return blob;
            }
            catch (Exception ex)
            {
            }
            return blob;
        }

        public bool ReadBlobToFile(string commandText, string filePath)
        {
            return ReadBlobToFile(commandText, (IDbDataParameter[])null, filePath);
        }

        public bool ReadBlobToFile(string commandText, IDbDataParameter[] commandParameters, string filePath)
        {
            try
            {
                //读取Blob字段
                Byte[] blob = ReadBlobToBytes(commandText, commandParameters);
                //从流创建文件
                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                fileStream.Write(blob, 0, blob.Length);
                fileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool WriteBytesToBlob(string tabname, string fieldname, string wherestr, ref Byte[] content)
        {
            if (content == null)
            {
                return false;
            }
            string sSQL = "";
            try
            {
                pDbCommand.Parameters.Clear();
                pDbCommand.Connection = ActiveConn;
                //事务
                if (pDbTransaction != null)
                {
                    if (pDbTransaction.Connection == null)
                    {
                        return false;
                    }
                    pDbCommand.Transaction = pDbTransaction;
                }
                //IDbDataParameter commandParameter = pAccessFactory.CreateDbDataParameter(connDbType);
                if (pDbCommand is OracleCommand)
                {
                    IDbDataParameter pOraDataParameter = new OracleParameter("blobval", Oracle.DataAccess.Client.OracleDbType.Blob, content.GetLength(0));
                    pOraDataParameter.Value = content;
                    pOraDataParameter.Direction = ParameterDirection.InputOutput;
                    sSQL = "update " + tabname + " set " + fieldname + " =:blobval " + wherestr;
                    pDbCommand.Parameters.Add(pOraDataParameter);
                }
                if (pDbCommand is OleDbCommand)
                {
                    ((OleDbCommand)pDbCommand).Parameters.Add("blobval", OleDbType.LongVarBinary, content.Length).Value = content;
                    sSQL = "update " + tabname + " set " + fieldname + "=@blobval " + wherestr;
                }
                if (pDbCommand is SqlCommand)
                {
                    ((SqlCommand)pDbCommand).Parameters.Add("blobval", SqlDbType.Image, content.Length).Value = content;
                    sSQL = "update " + tabname + " set " + fieldname + "=@blobval " + wherestr;
                }
                //if (pDbCommand is DDTek.Oracle.OracleCommand)
                //{
                //    IDbDataParameter pTekDataParameter = new DDTek.Oracle.OracleParameter("@blobval", DDTek.Oracle.OracleDbType.Blob, content.GetLength(0));
                //    pTekDataParameter.Value = content;
                //    pTekDataParameter.Direction = ParameterDirection.InputOutput;
                //    sSQL = "update " + tabname + " set " + fieldname + " =? " + wherestr;
                //    pDbCommand.Parameters.Add(pTekDataParameter);
                //}
                pDbCommand.CommandType = CommandType.Text;
                pDbCommand.CommandText = sSQL;
                pDbCommand.ExecuteNonQuery();
                pDbCommand.Parameters.Clear();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将文件存入到数据库BLOB字段中
        /// </summary>
        /// <param name="tabname">表名字</param>
        /// <param name="fieldname">字段名字</param>
        /// <param name="wherestr">条件语句，可为空，如果不为空，需要带where关键字传入</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否成功</returns>
        public bool WriteFileToBlob(string tabname, string fieldname, string wherestr, string filePath)
        {
            //读取文件到流
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] blob = new byte[Convert.ToInt32(fileStream.Length)];
            fileStream.Read(blob, 0, blob.Length);
            fileStream.Close();
            //若文件为空，返回false
            if (blob == null)
            {
                return false;
            }
            string sSQL = "";
            try
            {
                pDbCommand.Parameters.Clear();
                pDbCommand.Connection = ActiveConn;
                //事务
                if (pDbTransaction != null)
                {
                    if (pDbTransaction.Connection == null)
                    {
                        return false;
                    }
                    pDbCommand.Transaction = pDbTransaction;
                }
                //IDbDataParameter commandParameter = pAccessFactory.CreateDbDataParameter(connDbType);
                if (pDbCommand is OracleCommand)
                {
                    IDbDataParameter pOraDataParameter = new OracleParameter("blobval", Oracle.DataAccess.Client.OracleDbType.Blob, blob.GetLength(0));
                    pOraDataParameter.Value = blob;
                    pOraDataParameter.Direction = ParameterDirection.InputOutput;
                    sSQL = "update " + tabname + " set " + fieldname + " =:blobval " + wherestr;
                    pDbCommand.Parameters.Add(pOraDataParameter);
                }
                if (pDbCommand is SqlCommand)
                {
                    ((SqlCommand)pDbCommand).Parameters.Add("blobval", SqlDbType.Image, blob.Length).Value = blob;
                    sSQL = "update " + tabname + " set " + fieldname + "=@blobval " + wherestr;
                }
                if (pDbCommand is OleDbCommand)
                {
                    ((OleDbCommand)pDbCommand).Parameters.Add("blobval", OleDbType.LongVarBinary, blob.Length).Value = blob;
                    sSQL = "update " + tabname + " set " + fieldname + "=@blobval " + wherestr;
                }
                //if (pDbCommand is DDTek.Oracle.OracleCommand)
                //{
                //    IDbDataParameter pTekDataParameter = new DDTek.Oracle.OracleParameter("@blobval", DDTek.Oracle.OracleDbType.Blob, blob.GetLength(0));
                //    pTekDataParameter.Value = blob;
                //    pTekDataParameter.Direction = ParameterDirection.InputOutput;
                //    sSQL = "update " + tabname + " set " + fieldname + " =? " + wherestr;
                //    pDbCommand.Parameters.Add(pTekDataParameter);
                //}
                pDbCommand.CommandType = CommandType.Text;
                pDbCommand.CommandText = sSQL;
                pDbCommand.ExecuteNonQuery();
                pDbCommand.Parameters.Clear();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 其它操作

        /// <summary>
        /// 获取指定来源的下一标识号
        /// </summary>
        /// <param name="tableName">指定数据表名称</param>
        /// <param name="fieldName">指定的字段索引</param>
        /// <param name="startNumber">启始标识号</param>
        /// <param name="step">步长</param>
        /// <returns>下一标识号</returns>
        public long GetNextValidID(string tableName, string fieldName, long startNumber, long step)
        {
            long id = -1;
            try
            {
                switch (connDbType)
                {
                    case (DatabaseType.MSAccess):
                        {
                            StringBuilder sql = new StringBuilder("select count(*) as tempvalue from tbsys_idmanager where F_TableName = ");
                            sql.Append("'" + tableName.ToUpper() + "'");
                            sql.Append(" and F_FieldName =");
                            sql.Append("'" + fieldName.ToUpper() + "'");

                            long tempvalue = Convert.ToInt64(ExecuteScalar(sql.ToString(), CommandType.Text));
                            if (tempvalue == 0)
                            {

                                String sqlStr = "select count(*) as tempvalue from tbsys_idmanager where F_TableName='TBSYS_IDManager' and F_FieldName='F_ID'";

                                tempvalue = Convert.ToInt64(ExecuteScalar(sqlStr.ToString(), CommandType.Text));
                                if (tempvalue == 0)
                                {
                                    ExecuteSQL("Insert Into tbsys_idmanager (F_ID,F_TableName,F_FieldName,F_Current,F_Start,F_Step) values(10000,'TBSYS_IDManager','F_ID',10000,10000,1)");
                                }
                                ExecuteSQL("update tbsys_idmanager set F_Current= F_Current+F_Step where F_TableName='TBSYS_IDManager' and F_FieldName='F_ID'");
                                sqlStr = "select F_Current+F_Step as tempvalue from tbsys_idmanager where F_TableName='TBSYS_IDManager' and F_FieldName='F_ID'";
                                tempvalue = Convert.ToInt64(ExecuteScalar(sqlStr, CommandType.Text));
                                ExecuteSQL("Insert Into tbsys_idmanager (F_ID,F_TableName,F_FieldName,F_Current,F_Start,F_Step) values(" + tempvalue + ",'" + tableName + "','" + fieldName + "'," + startNumber + "," + startNumber + "," + step + ")");
                                tempvalue = startNumber;
                            }
                            else
                            {
                                String sqlStr = "select F_Current+F_Step as tempvalue from tbsys_idmanager where F_TableName='" + tableName + "' and  F_FieldName='" + fieldName + "'";
                                tempvalue = Convert.ToInt64(ExecuteScalar(sqlStr, CommandType.Text));
                                ExecuteSQL("update tbsys_idmanager set F_Current= F_Current+F_Step where F_TableName='" + tableName + "' and  F_FieldName='" + fieldName + "'");
                            }
                            id = tempvalue;
                        }
                        break;
                    case (DatabaseType.MSSQL):
                        {
                            pDbCommand.Parameters.Clear();
                            SqlCommand cmdSql = pDbCommand as SqlCommand;
                            cmdSql.CommandText = "dt_getnextvalue";
                            cmdSql.CommandType = CommandType.StoredProcedure;

                            SqlParameter RetVal = cmdSql.Parameters.Add("RetVal", SqlDbType.Int);
                            RetVal.Direction = ParameterDirection.ReturnValue;

                            SqlParameter TabNameIn = cmdSql.Parameters.Add("@tablename", SqlDbType.VarChar, 100);
                            TabNameIn.Direction = ParameterDirection.Input;

                            SqlParameter FieldNameIn = cmdSql.Parameters.Add("@fieldname", SqlDbType.VarChar, 100);
                            FieldNameIn.Direction = ParameterDirection.Input;

                            SqlParameter StartValueIn = cmdSql.Parameters.Add("@startvalue", SqlDbType.Int);
                            StartValueIn.Direction = ParameterDirection.Input;

                            SqlParameter StepValueIn = cmdSql.Parameters.Add("@stepvalue", SqlDbType.Int);
                            StepValueIn.Direction = ParameterDirection.Input;

                            SqlParameter ValueOut = cmdSql.Parameters.Add("@value", SqlDbType.Int);
                            ValueOut.Direction = ParameterDirection.Output;

                            TabNameIn.Value = tableName;
                            FieldNameIn.Value = fieldName;
                            StartValueIn.Value = startNumber;
                            StepValueIn.Value = step;

                            IDbDataParameter[] Params = new IDbDataParameter[] { TabNameIn, FieldNameIn, StartValueIn, StepValueIn, ValueOut };
                            ExecuteNonQuery("dt_getnextvalue", CommandType.StoredProcedure, Params);

                            id = Convert.ToInt64(ValueOut.Value);
                            cmdSql.Parameters.Clear();
                        }
                        break;
                    case (DatabaseType.Oracle):
                        {
                            pDbCommand.Parameters.Clear();
                            OracleCommand cmdOra = pDbCommand as OracleCommand;
                            cmdOra.CommandText = "dt_getnextvalue";
                            cmdOra.CommandType = CommandType.StoredProcedure;


                            Oracle.DataAccess.Client.OracleParameter TabNameIn = cmdOra.Parameters.Add("tablename", Oracle.DataAccess.Client.OracleDbType.NVarchar2, 100);
                            TabNameIn.Direction = ParameterDirection.Input;

                            Oracle.DataAccess.Client.OracleParameter FieldNameIn = cmdOra.Parameters.Add("fieldname", Oracle.DataAccess.Client.OracleDbType.NVarchar2, 100);
                            FieldNameIn.Direction = ParameterDirection.Input;

                            Oracle.DataAccess.Client.OracleParameter StartValueIn = cmdOra.Parameters.Add("startvalue", Oracle.DataAccess.Client.OracleDbType.Int32);
                            StartValueIn.Direction = ParameterDirection.Input;

                            Oracle.DataAccess.Client.OracleParameter StepValueIn = cmdOra.Parameters.Add("stepvalue", Oracle.DataAccess.Client.OracleDbType.Int32);
                            StepValueIn.Direction = ParameterDirection.Input;

                            Oracle.DataAccess.Client.OracleParameter ValueOut = cmdOra.Parameters.Add("tempvalue", Oracle.DataAccess.Client.OracleDbType.Int32);
                            ValueOut.Direction = ParameterDirection.Output;

                            TabNameIn.Value = tableName;
                            FieldNameIn.Value = fieldName;
                            StartValueIn.Value = startNumber;
                            StepValueIn.Value = step;


                            IDbDataParameter[] Params = new IDbDataParameter[] { TabNameIn, FieldNameIn, StartValueIn, StepValueIn, ValueOut };
                            ExecuteNonQuery("dt_getnextvalue", CommandType.StoredProcedure, Params);

                            id = Convert.ToInt64(ValueOut.Value);
                            cmdOra.Parameters.Clear();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
            }

            return id;
        }
        /// <summary>
        /// 获取指定来源的下一标识号
        /// </summary>
        /// <param name="tableName">指定数据表名称</param>
        /// <param name="fieldName">指定的字段索引</param>
        /// <returns>下一标识号</returns>
        public long GetNextValidID(string tableName, string fieldName)
        {
            return GetNextValidID(tableName, fieldName, 10000, 1);
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        public DateTime SystemDataTime
        {
            get
            {
                string sSQL = "";
                DateTime sTime = new DateTime();
                Object res;
                res = "";
                try
                {
                    switch (connDbType)
                    {
                        case DatabaseType.Unknown:
                            res = DateTime.Now.ToLongDateString();
                            break;
                        case DatabaseType.MSSQL:
                            sSQL = "select getdate()";
                            res = this.ExecuteScalar(sSQL, CommandType.Text);
                            break;
                        case DatabaseType.MSAccess:
                            res = DateTime.Now.ToLongDateString();
                            break;
                        case DatabaseType.Oracle:
                        case DatabaseType.TekOracle:
                            sSQL = "select to_char(sysdate,'YYYY-MM-DD hh24:mi:ss')  from  dual";
                            res = this.ExecuteScalar(sSQL, CommandType.Text);
                            break;
                        default:
                            res = DateTime.Now.ToLongDateString();
                            break;
                    }
                    sTime = Convert.ToDateTime(res);
                }
                catch (Exception ex)
                {
                }
                return sTime;
            }
        }

        #endregion

        #region 应用扩展部分


        public DataTable ExecuteDatatable(string tableName, string commandText, bool release)
        {
            try
            {
                DataSet ds = ExecuteDataset(commandText, CommandType.Text, release, tableName);//得到数据集
                if (ds == null) return null;
                return ds.Tables[tableName];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable ExecuteDatatableBySdoGeometry(string tableName, string commandText, bool release)
        {
            try
            {
                if (ActiveConn == null)
                {
                    return null;
                }
                if (commandText == null || commandText.Length == 0)
                {
                    return null;
                }

                try
                {
                    OracleDataAdapter mAdp = new OracleDataAdapter(commandText, (Oracle.DataAccess.Client.OracleConnection)ActiveConn);

                    DataTable mDst = new DataTable();

                    mAdp.Fill(mDst);
                    return mDst;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int ExecuteSQL(string sqlText)
        {
            return ExecuteNonQuery(sqlText, CommandType.Text);
        }

        #endregion

        #region ExecuteDataset部份

        public DataSet ExecuteDataset(string commandText, CommandType commandType)
        {
            return ExecuteDataset(commandText, commandType, true);
        }

        public DataSet ExecuteDataset(string commandText, CommandType commandType, IDbDataParameter[] commandParameters)
        {

            return ExecuteProcedureDataset(commandText, commandType, commandParameters, true);
            //return ExecuteDataset(commandText, commandType, commandParameters, true);
        }

        public DataSet ExecuteDataset(string storedprocedureName, params object[] parameterValues)
        {
            return ExecuteDataset(storedprocedureName, true, parameterValues);
        }

        public DataSet ExecuteDataset(string commandText, CommandType commandType, bool release, string dsName)
        {
            return ExecuteDataset(commandText, commandType, (IDbDataParameter[])null, release, dsName);
        }

        public DataSet ExecuteDataset(string commandText, CommandType commandType, IDbDataParameter[] commandParameters, bool release, string dsName)
        {
            OracleSpatial.Geometry.SdoGeometry SDO = new OracleSpatial.Geometry.SdoGeometry();
            if (ActiveConn == null)
            {
                return null;
            }
            if (commandText == null || commandText.Length == 0)
            {
                return null;
            }

            try
            {
                bool mustCloseConnection = false;
                PrepareCommand(pDbCommand, ActiveConn, pDbTransaction, commandType, commandText, commandParameters, out mustCloseConnection);
                //创建适配器
                IDataAdapter newDataAdapter = pAccessFactory.CreateDbDataAdapter(connDbType, commandText, ActiveConn);
                DataSet ds = null;//= new DataSet();
                string displayValue = dsName;
                if (!release)
                {
                    //若不释放,将其加入适配器集合中
                    if (CurrentDataAdapter(displayValue) != null)
                    {
                    }
                    else
                    {
                        ItemInfo<IDataAdapter, string> itemInfo = new ItemInfo<IDataAdapter, string>(newDataAdapter, displayValue);
                        pDataAdapterCollection.Add(itemInfo);
                        //根据数据连接类型创建DataAdaper
                        switch (connDbType)
                        {
                            case DatabaseType.MSSQL:
                                {
                                    SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder((SqlDataAdapter)newDataAdapter);
                                    ((SqlDataAdapter)newDataAdapter).Fill(activeDataSet, displayValue);
                                }
                                break;
                            case DatabaseType.MSAccess:
                                {
                                    OleDbCommandBuilder commandBuilder = new OleDbCommandBuilder((OleDbDataAdapter)newDataAdapter);
                                    ((OleDbDataAdapter)newDataAdapter).Fill(activeDataSet, displayValue);
                                }
                                break;
                            case DatabaseType.Oracle:
                                {
                                    OracleCommandBuilder oraCommandBuilder = new OracleCommandBuilder((OracleDataAdapter)newDataAdapter);
                                    ((OracleDataAdapter)newDataAdapter).Fill(activeDataSet, displayValue);
                                }
                                break;
                            //case DatabaseType.TekOracle:
                            //    {
                            //        DDTek.Oracle.OracleCommandBuilder TekOracleCommandBuilder = new DDTek.Oracle.OracleCommandBuilder((DDTek.Oracle.OracleDataAdapter)newDataAdapter);
                            //        ((DDTek.Oracle.OracleDataAdapter)newDataAdapter).Fill(activeDataSet, displayValue);
                            //    }
                            //    break;
                            default:
                                return null;
                        }
                        ds = activeDataSet;
                    }
                }
                else
                {
                    ds = new DataSet();
                    newDataAdapter.Fill(ds);
                    ds.Tables[0].TableName = displayValue;
                }
                pDbCommand.Parameters.Clear();
                if (mustCloseConnection)
                {//关闭连接
                    ActiveConn.Close();
                }
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet ExecuteProcedureDataset(string commandText, CommandType commandType, IDbDataParameter[] commandParameters, bool release)
        {
            try
            {
                DataSet ds = null;
                OracleCommand cmd = new OracleCommand(commandText, activeConn as OracleConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                if (activeConn.State == ConnectionState.Broken)
                {
                    activeConn.Close();
                    activeConn.Open();
                }
                else if (activeConn.State == ConnectionState.Closed)
                {
                    activeConn.Open();
                }
                if (commandParameters != null && commandParameters.Length > 0)
                    adapter.SelectCommand.Parameters.AddRange(commandParameters);

                OracleDataReader read = cmd.ExecuteReader(CommandBehavior.SchemaOnly);//ODP驱动时如果查询的字段有参数则会报ORA-01036: 非法的变量名/编号错误,先执行以下ExecuteReader可避免
                read.Close();
                read.Dispose();
                ds = activeDataSet;
                adapter.Fill(ds);

                cmd.Parameters.Clear();
                activeConn.Close();
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region ExecuteNonQuery部份
        public int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, commandType, (IDbDataParameter[])null);
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, IDbDataParameter[] commandParameters)
        {
            try
            {
                if (ActiveConn == null)
                {
                    return -1;
                }

                bool mustCloseConnection = false;

                PrepareCommand(pDbCommand, ActiveConn, pDbTransaction, commandType, commandText, commandParameters, out mustCloseConnection);

                int retval = pDbCommand.ExecuteNonQuery();

                //zhouzh20101101将此句改放到Finally块中
                //pDbCommand.Parameters.Clear();
                if (mustCloseConnection)
                {
                    ActiveConn.Close();//执行完后关闭连接
                }
                return retval;
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                pDbCommand.Parameters.Clear();
            }
        }


        public int ExecuteNonQueryByBuffer(string commandText, CommandType commandType, byte[] buffer)
        {
            try
            {
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = new OracleParameter(":document", OracleDbType.Blob);
                parameters[0].Value = buffer;
                return ExecuteNonQuery(commandText, commandType, parameters);
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
               
            }
        }


        #region 为项目改编的ExecuteNonQuery函数
        /// <summary>
        /// zhouzh200910 执行非查询语句，有异常则直接抛出
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public int ExecuteNonQueryWithException(string commandText, CommandType commandType, IDbDataParameter[] commandParameters)
        {
            try
            {
                if (ActiveConn == null)
                {
                    return -1;
                }

                bool mustCloseConnection = false;

                PrepareCommand(pDbCommand, ActiveConn, pDbTransaction, commandType, commandText, commandParameters, out mustCloseConnection);

                int retval = pDbCommand.ExecuteNonQuery();

                //zhouzh20101101将此句改放到Finally块中
                //pDbCommand.Parameters.Clear();
                if (mustCloseConnection)
                {
                    ActiveConn.Close();//执行完后关闭连接
                }
                return retval;
            }
            catch (Exception ex)
            {
                throw ex;
                return -1;
            }
            finally
            {
                pDbCommand.Parameters.Clear();
            }
        }

        /// <summary>
        /// zhouzh200910 执行非查询语句，有异常则直接抛出
        /// </summary>
        /// <param name="storedprocedureName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public int ExecuteNonQueryWithException(string storedprocedureName, params object[] parameterValues)
        {
            if (ActiveConn == null)
            {
                return -1;
            }
            if (storedprocedureName == null || storedprocedureName.Length == 0)
            {
                return -1;
            }

            //if ((parameterValues != null) && (parameterValues.Length > 0))
            //{
            //    IDbDataParameter[] commandParameters = pDbHelperParameterCache.GetSpParameterSet(ActiveConn, storedprocedureName);

            //    AssignParameterValues(commandParameters, parameterValues);

            //    return ExecuteNonQueryWithException(storedprocedureName, CommandType.StoredProcedure, commandParameters);
            //}
            //else
            //{
            return ExecuteNonQueryWithException(storedprocedureName, CommandType.StoredProcedure);
            //}
        }

        /// <summary>
        /// zhouzh200910 执行非查询语句，有异常则直接抛出
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public int ExecuteNonQueryWithException(string commandText, CommandType commandType)
        {
            return ExecuteNonQueryWithException(commandText, commandType, (IDbDataParameter[])null);
        }


        #endregion

        public int ExecuteNonQuery(string storedprocedureName, params object[] parameterValues)
        {
            if (ActiveConn == null)
            {
                return -1;
            }
            if (storedprocedureName == null || storedprocedureName.Length == 0)
            {
                return -1;
            }

            //if ((parameterValues != null) && (parameterValues.Length > 0))
            //{
            //    IDbDataParameter[] commandParameters = pDbHelperParameterCache.GetSpParameterSet(ActiveConn, storedprocedureName);

            //    AssignParameterValues(commandParameters, parameterValues);

            //    return ExecuteNonQuery(storedprocedureName, CommandType.StoredProcedure, commandParameters);
            //}
            //else
            //{
            return ExecuteNonQuery(storedprocedureName, CommandType.StoredProcedure);
            //}
        }

        #endregion

        #region ExecuteReader部份

        public IDataReader ExecuteReader(string commandText, CommandType commandType)
        {
            return ExecuteReader(commandText, commandType, (IDbDataParameter[])null);
        }

        IDataReader pDataReader;
        public IDataReader ExecuteReader(string commandText, CommandType commandType, IDbDataParameter[] commandParameters)
        {
            if (ActiveConn == null)
            {
                return null;
            }
            if (commandText == null || commandText.Length == 0)
            {
                return null;
            }

            try
            {
                bool mustCloseConnection = false;
                PrepareCommand(pDbCommand, ActiveConn, pDbTransaction, commandType, commandText, commandParameters, out mustCloseConnection);

                if (pDataReader != null && !pDataReader.IsClosed)
                    pDataReader.Close();

                pDataReader = pDbCommand.ExecuteReader();
                bool canClear = true;
                foreach (IDbDataParameter commandParameter in pDbCommand.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                    {
                        canClear = false;
                        break;
                    }
                }
                if (canClear)
                {
                    pDbCommand.Parameters.Clear();
                }
                if (mustCloseConnection)
                    ActiveConn.Close();//执行完后关闭连接

                return pDataReader;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IDataReader ExecuteReader(string storedprocedureName, params object[] parameterValues)
        {
            try
            {
                if (ActiveConn == null)
                {
                    return null;
                }

                if (storedprocedureName == null || storedprocedureName.Length == 0)
                {
                    return null;
                }

                //if ((parameterValues != null) && (parameterValues.Length > 0))
                //{
                //    IDbDataParameter[] commandParameters = pDbHelperParameterCache.GetSpParameterSet(ConnectionString, storedprocedureName);

                //    AssignParameterValues(commandParameters, parameterValues);

                //    return ExecuteReader(storedprocedureName, CommandType.StoredProcedure, commandParameters);
                //}
                //else
                //{
                return ExecuteReader(storedprocedureName, CommandType.StoredProcedure);
                //}
            }
            catch (Exception ex)
            {
                //异常处理
                return null;
            }
        }

        #endregion

        #region ExecuteScalar部份

        public object ExecuteScalar(string commandText, CommandType commandType)
        {
            return ExecuteScalar(commandText, commandType, (IDbDataParameter[])null);
        }

        public object ExecuteScalar(string commandText, CommandType commandType, IDbDataParameter[] commandParameters)
        {


            if (ActiveConn == null)
            {
                return null;
            }

            bool mustCloseConnection = false;
            PrepareCommand(pDbCommand, ActiveConn, (IDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            object retval = pDbCommand.ExecuteScalar();

            pDbCommand.Parameters.Clear();

            if (mustCloseConnection)
                ActiveConn.Close();

            return retval;
        }

        public object ExecuteScalar(string storedprocedureName, params object[] parameterValues)
        {
            if (ActiveConn == null)
            {
                return null;
            }

            if (storedprocedureName == null || storedprocedureName.Length == 0)
            {
                return null;
            }

            //if ((parameterValues != null) && (parameterValues.Length > 0))
            //{
            //    IDbDataParameter[] commandParameters = pDbHelperParameterCache.GetSpParameterSet(ConnectionString, storedprocedureName);

            //    AssignParameterValues(commandParameters, parameterValues);//绑定参数

            //    return ExecuteScalar(storedprocedureName, CommandType.StoredProcedure, commandParameters);
            //}
            //else
            //{
            return ExecuteScalar(storedprocedureName, CommandType.StoredProcedure);
            //}
        }

        #endregion

        #endregion

    }
}
