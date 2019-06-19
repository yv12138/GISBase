using System;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using System.IO;
using System.Collections.Generic;
/**********************************************************************
* 
* 戴强 2012 年7月7日创建数据目录底层操作类
* 
* *******************************************************************/
namespace Service.DBOperator
{
    /// <summary>
    ///NHibernate 数据库底层操作
    /// </summary>
    public class BasicDataManipulation
    {
        private static object factoryLock = new object();
        //优化Factory创建 增加连接池机制，随用随取，不重复创建
        private static Dictionary<string, ISessionFactory> _sessionFactoriesPool;

        private static ISessionFactory sessionFactory = null;

        #region NHibernate
        /// <summary>
        /// 返回一个session,前提条件是需要启用创建工厂方法
        /// </summary>
        /// <param name="path">cfg.xml文件路径</param>
        /// <param name="name">配置文件中对应的Name属性</param>
        /// <returns></returns>
        public ISession GetSession(string strConn)
        {
            ISessionFactory sessionFactory = null;
            lock (factoryLock)
            {
                //初始化连接池
                if (_sessionFactoriesPool == null)
                {
                    _sessionFactoriesPool = new Dictionary<string, ISessionFactory>();
                }
                //池中不存在所需连接即创建
                if (!_sessionFactoriesPool.ContainsKey(strConn))
                {
                    string bdckConn = Service.Common.ConfigurationManager.ManageConfig.instance.GetappSettings("YG_BDCK");
                    System.IO.StringReader stringR = null;
                    try
                    {
                        string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "hibernate.cfg.xml");
                        string cfg = string.Format(System.IO.File.ReadAllText(path), strConn);
                        stringR = new System.IO.StringReader(cfg);
                        System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringR);
                        Configuration config = new Configuration().Configure(xmlReader);
                        sessionFactory = config.BuildSessionFactory();
                        //创建完毕加入连接池
                        _sessionFactoriesPool.Add(strConn, sessionFactory);
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                    finally
                    {
                        if (stringR != null)
                        {
                            stringR.Dispose();
                            stringR = null;
                        }
                    }
                }
                //池中存在可直接取走
                else
                {
                    sessionFactory = _sessionFactoriesPool[strConn];
                }
            }
            ISession session = null;
            try
            {
                //返回会话
                session = sessionFactory.OpenSession();
                return session;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 老方法
        //public ISession GetSession(string strConn)
        //{
        //    string bdckConn = Service.Common.ConfigurationManager.ManageConfig.instance.GetappSettings("YG_BDCK");
        //    if (checkRegion("CX"))
        //    {
        //        strConn = Service.Common.Md5Helper.DESDeCode(strConn);
        //    }
        //    ISessionFactory sessionFactory = null;
        //    System.IO.StringReader stringR = null;
        //    try
        //    {
        //        string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "hibernate.cfg.xml");
        //        string cfg = string.Format(System.IO.File.ReadAllText(path), strConn);
        //        stringR = new System.IO.StringReader(cfg);
        //        System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringR);
        //        Configuration config = new Configuration().Configure(xmlReader);
        //        //独立加载户的hbm.xml;
        //        string hxmlpath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Bin\\XmlMapping\\H.hbm.xml");
        //        string xml = string.Format(File.ReadAllText(hxmlpath), bdckConn);
        //        config.AddXmlString(xml);
        //        sessionFactory = config.BuildSessionFactory();

        //        ISession session = sessionFactory.OpenSession();
        //        //DisposeFactory(sessionFactory);
        //        return session;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (sessionFactory != null)
        //        {
        //            sessionFactory.Dispose();
        //            sessionFactory = null;
        //        }
        //        if (stringR != null)
        //        {
        //            stringR.Dispose();
        //            stringR = null;
        //        }
        //    }
        //}
        #endregion

        public ISession GetSession()
        {
            ISession session = null;
            lock (factoryLock)
            {
                try
                {
                    if (sessionFactory == null)
                    {
                        string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "hibernate.cfg.xml");
                        string str = File.ReadAllText(path);
                        using (StringReader sr = new System.IO.StringReader(str))
                        {
                            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr);
                            Configuration config = new Configuration().Configure(xmlReader);
                            sessionFactory = config.BuildSessionFactory();
                        }
                    }
                    session = sessionFactory.OpenSession();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return session;
        }

        /// <summary>
        /// 开启事务并
        /// 返回一个事务对象
        /// </summary>
        /// <param name="session">需求事务的session</param>
        /// <param name="level">IsolationLevel 确认等级</param>
        /// <returns>事务对象</returns>
        public ITransaction BeginTransaction(ISession session, System.Data.IsolationLevel level)
        {
            ITransaction transaction = null;
            if (session == null)
                return null;
            if (!session.Transaction.IsActive)
                try
                {
                    transaction = session.BeginTransaction(level);
                }
                catch (HibernateException ex)
                {
                    DisposeTransaction(transaction);
                    Ex(ex);
                }

            return transaction;
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="transaction"></param>
        public void Rollback(ITransaction transaction)
        {
            try
            {
                if (transaction != null)
                    if (!transaction.WasRolledBack)
                        transaction.Rollback();
            }
            catch (HibernateException ex)
            {
                Ex(ex);
            }
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="transaction"></param>
        public void Commit(ITransaction transaction)
        {
            try
            {
                if (transaction != null)
                    if (!transaction.WasCommitted)
                        transaction.Commit();
            }
            catch (HibernateException ex)
            {
                Ex(ex);
            }
        }

        /// <summary>
        /// 工厂资源释放
        /// </summary>
        /// <param name="isf">需要释放的对象</param>
        public void DisposeFactory(ISessionFactory isf)
        {
            if (isf != null)
            {
                try
                {
                    if (!isf.IsClosed)
                        isf.Close();
                    isf.Dispose();
                }
                catch (HibernateException ex)
                {
                    Ex(ex);
                }

            }
        }
        /// <summary>
        /// session 释放
        /// </summary>
        /// <param name="session"></param>
        public void DisposeSession(ISession session)
        {
            if (session != null)
            {
                DisposeFactory(session.SessionFactory);
                try
                {
                    if (session.IsConnected)
                        session.Close();
                    session.Dispose();
                }
                catch (HibernateException ex)
                {
                    Ex(ex);
                }

            }
        }

        /// <summary>
        /// ITransaction 释放
        /// </summary>
        /// <param name="transaction"></param>
        public void DisposeTransaction(ITransaction transaction)
        {
            if (transaction != null)
            {
                try
                {
                    transaction.Dispose();
                }
                catch (HibernateException ex)
                {
                    Ex(ex);
                }
            }
        }

        #endregion

        private static void Ex(Exception ex)
        {
            throw ex;
        }

        /// <summary>
        /// 判断是否为某地区服务器
        /// </summary>
        /// <param name="city">泰安：TA;  新泰:XT;  肥城：FC;  曹县：CX</param>
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