using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Service.DBOperator
{       
    /// <summary>
    /// Ŀ    �ģ���ϵ���ݿ���ʹ���
    /// �� �� �ˣ�
    /// ����ʱ�䣺2007-10-22
    /// ��    ע��
    ///	�޸�������
    /// �� �� �ˣ�
    /// �޸����ڣ�
    /// </summary>
    public static class RDBFactory
    {
        static RDBFactory()
        {
        }

        /// <summary>
        /// ����IDbHelper�ӿ�ʵ��
        /// </summary>
        /// <param name="DbConnection">System.Data.IDbConnection���Ӷ���</param>
        /// <returns>IDbHelper�ӿ�ʵ��</returns>
        public static IRDBHelper CreateDbHelper(System.Data.IDbConnection DbConnection)
        {
            RDBHelper pDbHelper = new RDBHelper(DbConnection);
            return pDbHelper;
        }

        /// <summary>
        /// ����IDbHelper�ӿ�ʵ��
        /// </summary>
        /// <param name="strCnn">���ݿ����Ӵ�</param>
        /// <param name="dataBaseType">���ݿ�����</param>
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
        /// ����IDbHelper�ӿ�ʵ��
        /// </summary>
        /// <param name="instance">ʵ�����ƣ������ӱ�ʶ</param>
        /// <param name="server">���ݿ����ڷ�����</param>
        /// <param name="port">�˿�</param>
        /// <param name="database">���ݿ�����(���ݿ�ʵ������)</param>
        /// <param name="userid">�������ݿ���ʻ�</param>
        /// <param name="password">�������ݿ��ʻ�������</param>
        /// <param name="dataBaseType">��������</param>
        /// <returns>IDbHelper�ӿ�</returns>
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
        /// ��������
        /// </summary>
        /// <param name="instance">ʵ�����ƣ������ӱ�ʶ</param>
        /// <param name="server">���ݿ����ڷ�����</param>
        /// <param name="port">�˿�</param>
        /// <param name="database">���ݿ�����(���ݿ�ʵ������)</param>
        /// <param name="userid">�������ݿ���ʻ�</param>
        /// <param name="password">�������ݿ��ʻ�������</param>
        /// <param name="dataBaseType">��������</param>
        /// <returns>�Ƿ�ɹ�</returns>
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
        /// ����IDbHelper�ӿ�ʵ��
        /// </summary>
        /// <param name="instance">ʵ�����ƣ������ӱ�ʶ</param>
        /// <param name="datasource">����Դ(�ļ���ַ)</param>
        /// <param name="userid">�������ݿ���ʻ�</param>
        /// <param name="password">�������ݿ��ʻ�������</param>
        /// <returns>IDbHelper�ӿ�</returns>
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
        /// ��������
        /// </summary>
        /// <param name="instance">ʵ�����ƣ������ӱ�ʶ</param>
        /// <param name="datasource">����Դ(�ļ���ַ)</param>
        /// <param name="userid">�������ݿ���ʻ�</param>
        /// <param name="password">�������ݿ��ʻ�������</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public static Boolean TestConnect(string instance, string datasource, string userid, string password)
        {
            RDBHelper pDbHelper = new RDBHelper(DatabaseType.MSAccess);
            pDbHelper.User = userid;
            pDbHelper.dbFileName = datasource;
            pDbHelper.PWD = password;
            return pDbHelper.Connect();
        }

        /// <summary>
        /// �ж��Ƿ�Ϊĳ����������
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
