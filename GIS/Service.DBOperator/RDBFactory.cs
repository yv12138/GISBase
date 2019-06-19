using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Service.DBOperator
{       
    /// <summary>
    /// 目    的：关系数据库访问工厂
    /// 创 建 人：
    /// 创建时间：2007-10-22
    /// 备    注：
    ///	修改描述：
    /// 修 改 人：
    /// 修改日期：
    /// </summary>
    public static class RDBFactory
    {
        static RDBFactory()
        {
        }

        /// <summary>
        /// 创建IDbHelper接口实例
        /// </summary>
        /// <param name="DbConnection">System.Data.IDbConnection连接对象</param>
        /// <returns>IDbHelper接口实例</returns>
        public static IRDBHelper CreateDbHelper(System.Data.IDbConnection DbConnection)
        {
            RDBHelper pDbHelper = new RDBHelper(DbConnection);
            return pDbHelper;
        }

        /// <summary>
        /// 创建IDbHelper接口实例
        /// </summary>
        /// <param name="strCnn">数据库连接串</param>
        /// <param name="dataBaseType">数据库类型</param>
        /// <returns></returns>
        public static IRDBHelper CreateDbHelper(string strCnn, DatabaseType dataBaseType)
        {
            if (checkRegion("CX"))
            {
                strCnn = Service.Common.Md5Helper.DESDeCode(strCnn);
            }
            AccessFactory pAccessFactory = new AccessFactory();
            System.Data.IDbConnection pDbConnection = pAccessFactory.CreateDbConnection(dataBaseType);
            pDbConnection.ConnectionString = strCnn;
            try
            {
                pDbConnection.Open();
            }
            catch (Exception ex)
            {
                //LogAPI.debug(ex);
            }
            RDBHelper pDbHelper = new RDBHelper(pDbConnection);

            return pDbHelper;
        }


        /// <summary>
        /// 创建IDbHelper接口实例
        /// </summary>
        /// <param name="instance">实例名称－－连接标识</param>
        /// <param name="server">数据库所在服务器</param>
        /// <param name="port">端口</param>
        /// <param name="database">数据库名称(数据库实例名称)</param>
        /// <param name="userid">访问数据库的帐户</param>
        /// <param name="password">访问数据库帐户的密码</param>
        /// <param name="dataBaseType">连接类型</param>
        /// <returns>IDbHelper接口</returns>
        public static IRDBHelper CreateDbHelper(string instance, string server,string port, string database, string userid, string password, DatabaseType dataBaseType)
        {
            RDBHelper pDbHelper = new RDBHelper(dataBaseType);
            pDbHelper.Server = server;
            pDbHelper.Port = port;
            pDbHelper.Service = database;
            pDbHelper.User = userid;
            pDbHelper.PWD = password;

            //DCCP.ProductStorage.RDBHelperStorage.Add(new ItemInfo<string, IRDBHelper>(instance, pDbHelper));

            return ((IRDBHelper)pDbHelper);
        }

        /// <summary>
        /// 测试链接
        /// </summary>
        /// <param name="instance">实例名称－－连接标识</param>
        /// <param name="server">数据库所在服务器</param>
        /// <param name="port">端口</param>
        /// <param name="database">数据库名称(数据库实例名称)</param>
        /// <param name="userid">访问数据库的帐户</param>
        /// <param name="password">访问数据库帐户的密码</param>
        /// <param name="dataBaseType">连接类型</param>
        /// <returns>是否成功</returns>
        public static Boolean TestConnect(string instance, string server,string port, string database, string userid, string password, DatabaseType dataBaseType)
        {
            RDBHelper pDbHelper = new RDBHelper(dataBaseType);
            pDbHelper.Server = server;
            pDbHelper.Service = database;
            pDbHelper.Port = port;
            pDbHelper.User = userid;
            pDbHelper.PWD = password;
            return pDbHelper.Connect();
        }

        /// <summary>
        /// 创建IDbHelper接口实例
        /// </summary>
        /// <param name="instance">实例名称－－连接标识</param>
        /// <param name="datasource">数据源(文件地址)</param>
        /// <param name="userid">访问数据库的帐户</param>
        /// <param name="password">访问数据库帐户的密码</param>
        /// <returns>IDbHelper接口</returns>
        public static IRDBHelper CreateDbHelper(string instance, string datasource, string userid, string password)
        {
            RDBHelper pDbHelper = new RDBHelper(DatabaseType.MSAccess);
            pDbHelper.User= userid;
            pDbHelper.dbFileName = datasource;
            pDbHelper.PWD = password;

            //DCCP.ProductStorage.RDBHelperStorage.Add(new ItemInfo<string, IRDBHelper>(instance, pDbHelper));

            return ((IRDBHelper)pDbHelper);
        }

        /// <summary>
        /// 测试链接
        /// </summary>
        /// <param name="instance">实例名称－－连接标识</param>
        /// <param name="datasource">数据源(文件地址)</param>
        /// <param name="userid">访问数据库的帐户</param>
        /// <param name="password">访问数据库帐户的密码</param>
        /// <returns>是否成功</returns>
        public static Boolean TestConnect(string instance, string datasource, string userid, string password)
        {
            RDBHelper pDbHelper = new RDBHelper(DatabaseType.MSAccess);
            pDbHelper.User = userid;
            pDbHelper.dbFileName = datasource;
            pDbHelper.PWD = password;
            return pDbHelper.Connect();
        }

        /// <summary>
        /// 判断是否为某地区服务器
        /// </summary>
        /// <returns></returns>
        private static bool checkRegion(string city)
        {
            string ywh = Common.ConfigurationManager.ManageConfig.instance.GetappSettings("YWH");
            if (!string.IsNullOrEmpty(ywh) && ywh.ToUpper() == city.ToUpper())
            {
                return true;
            }
            return false;
        }
    }
}
