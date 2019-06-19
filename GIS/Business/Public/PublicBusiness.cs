using NHibernate;
using Service.Common;
using Service.Common.ConfigurationManager;
using Service.DBOperator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Public
{
    /// <summary>
    /// 数据库查询封装公用方法
    /// </summary>
    public class PublicBusiness
    {
        #region  数据库查询封装公用方法

        #region NHibernate 方法
        /// <summary>
        /// 普通查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="hql">查询语句</param>
        /// <returns></returns>
        public static List<T> GetEntitiesByHQL<T>(string hql)
        {
            BasicDataManipulation baseData = new BasicDataManipulation();
            ISession Session = baseData.GetSession();
            List<T> list = new List<T>();
            if (Session == null)
            {
                throw new Exception("打开数据库连接失败！");
            }
            try
            {
                NHibernate.IQuery query = Session.CreateQuery(hql);
                list = query.List<T>().ToList();
                return list;
            }
            catch (Exception ex)
            {
                LogAPI.Debug(ex);
                return list;
            }
            finally
            {
                if (Session != null)
                {
                    Session.Dispose();
                    Session = null;
                }
            }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="hql">查询语句</param>
        /// <param name="skip">页数</param>
        /// <param name="take">每页显示条数</param>
        /// <param name="count">总数</param>
        /// <returns></returns>
        public static List<T> GetEntitiesByHQL<T>(string hql, int skip, int take, out int count)
        {
            BasicDataManipulation baseData = new BasicDataManipulation();
            ISession Session = baseData.GetSession();
            List<T> list = new List<T>();
            if (Session == null)
            {
                throw new Exception("打开数据库连接失败！");
            }
            try
            {
                NHibernate.IQuery query1 = Session.CreateQuery(hql);
                string hqlcount = "";
                int fromIndex = hql.IndexOf("from");
                int toIndex = hql.IndexOf("order");
                int len = 0;
                if (toIndex == -1)
                {
                    hqlcount = "select count(*) " + hql.Substring(fromIndex);
                }
                else
                {
                    len = toIndex - fromIndex;
                    hqlcount = "select count(*) " + hql.Substring(fromIndex, len);
                }
                NHibernate.IQuery querycount = Session.CreateQuery(hqlcount);
                try
                {
                    count = int.Parse(querycount.UniqueResult().ToString());
                }
                catch
                {
                    count = 0;
                }
                NHibernate.IQuery query = query1.SetFirstResult(skip).SetMaxResults(take);
                list = query.List<T>().ToList();
                return list;
            }
            catch (Exception ex)
            {
                count = 0;
                LogAPI.Debug(ex);
                return list;
            }
            finally
            {
                if (Session != null)
                {
                    Session.Dispose();
                    Session = null;
                }
            }

        }
        #endregion

        #region RDBHelper 方法
        /// <summary>
        ///  RDBHelper 查询分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="skip">页数</param>
        /// <param name="take">每页显示条数</param>
        /// <param name="count">总数</param>
        /// <returns></returns>
        public static List<T> GetEntitiesListByDB<T>(string sql, int skip, int take, out int count)
        {
            IRDBHelper helper = null;
            List<T> list = new List<T>();
            try
            {
                string strCon = ManageConfig.instance.GetConnectionStrings("SHHConnection");
                helper = RDBFactory.CreateDbHelper(strCon, DatabaseType.MSSQL);
                if (helper != null)
                {
                    string countSQL = string.Format("select count(1) as NUM from({0})", sql);
                    DataTable dt = helper.ExecuteDatatable("table", countSQL, true);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        count = int.Parse(dt.Rows[0]["NUM"].ToString());
                    }
                    else
                    {
                        count = 0;
                        LogAPI.Debug("查询总数失败！");
                    }
                    //分页查询
                    string strBegin = "select * from (select a.* ,rownum rn from ( ";
                    string strEnd = string.Format(")a where rownum < {0} ) where rn>{1} ", take, skip);
                    string querySQL = strBegin + sql + strEnd;
                    DataTable dataTable = helper.ExecuteDatatable("table", querySQL, true);
                    list = TBToList.ToList<T>(dataTable);
                }
                else
                {
                    count = 0;
                    LogAPI.Debug("数据库打开失败！");
                }
                return list;
            }
            catch (Exception ex)
            {
                count = 0;
                LogAPI.Debug("查询受理列表异常：");
                LogAPI.Debug(ex);
                return list;
            }
            finally
            {
                if (helper != null)
                {
                    helper.DisConnect();
                    helper = null;
                }
            }

        }
        /// <summary>
        /// 普通无分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> GetEntitiesListByDB<T>(string sql)
        {
            IRDBHelper helper = null;
            List<T> list = new List<T>();
            try
            {
                string strCon = ManageConfig.instance.GetConnectionStrings("SHHConnection");
                helper = RDBFactory.CreateDbHelper(strCon, DatabaseType.MSSQL);
                DataTable dataTable = helper.ExecuteDatatable("table", sql, true);
                list = TBToList.ToList<T>(dataTable);
                return list;
            }
            catch (Exception ex)
            {
                LogAPI.Debug(ex);
                return list;
            }
            finally
            {
                if (helper != null)
                {
                    helper.DisConnect();
                    helper = null;
                }
            }

        }
        public static int ExecuteSQL(string sql)
        {
            IRDBHelper helper = null;
            try
            {
                string strCon = ManageConfig.instance.GetConnectionStrings("SHHConnection");
                helper = RDBFactory.CreateDbHelper(strCon, DatabaseType.Oracle);
                int count = helper.ExecuteNonQuery(sql, CommandType.Text);
                if (count > 0)
                {
                    helper.Commit();
                }
                return count;
            }
            catch (Exception ex)
            {
                LogAPI.Debug(ex);
                return 0;
            }
            finally
            {
                if (helper != null)
                {
                    helper.DisConnect();
                    helper = null;
                }
            }

        }
        #endregion

        #endregion
    }
}
