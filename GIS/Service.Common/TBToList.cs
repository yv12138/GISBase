using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Data;

namespace Service.Common
{

    public class TBToList
    {
        /// <summary>
        /// 获取列名集合
        /// </summary>
        private static IList<string> GetColumnNames(DataColumnCollection dcc)
        {
            IList<string> list = new List<string>();
            foreach (DataColumn dc in dcc)
            {
                list.Add(dc.ColumnName);
            }
            return list;
        }

        /// <summary>
        ///属性名称和类型名的键值对集合
        /// </summary>
        private static Hashtable GetColumnType<T>(DataColumnCollection dcc)
        {
            if (dcc == null || dcc.Count == 0)
            {
                return null;
            }
            IList<string> colNameList = GetColumnNames(dcc);

            Type t = typeof(T);
            PropertyInfo[] properties = t.GetProperties();
            Hashtable hashtable = new Hashtable();
            int i = 0;
            foreach (PropertyInfo p in properties)
            {
                foreach (string col in colNameList)
                {
                    if (col.ToLower().Equals(p.Name.ToLower()))
                    {
                        hashtable.Add(col, p.PropertyType.ToString() + i++);
                        break;
                    }
                }
            }

            return hashtable;
        }

        /// <summary>
        /// DataTable转换成IList
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return new List<T>();
            }

            PropertyInfo[] properties = typeof(T).GetProperties();//获取实体类型的属性集合
            List<T> list = new List<T>();
            T model = default(T);
            foreach (DataRow dr in dt.Rows)
            {
                model = Activator.CreateInstance<T>();//创建实体
                foreach (PropertyInfo p in properties)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName.ToLower().Equals(p.Name.ToLower()))
                        {
                            object obj = dr[col.ColumnName];
                            if (obj == null || obj is DBNull)
                                continue;
                            if (p.PropertyType == typeof(string))
                            {
                                p.SetValue(model, obj.ToString(), null);
                            }
                            else if (p.PropertyType == typeof(int) || p.PropertyType == typeof(int?))
                            {
                                p.SetValue(model, int.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                            {
                                p.SetValue(model, DateTime.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(float) || p.PropertyType == typeof(float?))
                            {
                                p.SetValue(model, float.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(double) || p.PropertyType == typeof(double?))
                            {
                                p.SetValue(model, double.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?))
                            {
                                p.SetValue(model, decimal.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
                            {
                                bool bVal = obj == null || string.IsNullOrWhiteSpace(obj.ToString()) || obj.ToString().Trim() != "0" || obj.ToString().Trim().ToLower() == "false";
                                p.SetValue(model, bVal, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(model);
            }

            return list;
        }

        #region DataRow转换为model
        public static T DataRowToEntity<T>(DataRow dr)
        {

            PropertyInfo[] properties = typeof(T).GetProperties();//获取实体类型的属性集合
            T model = default(T);
            try
            {
                model = Activator.CreateInstance<T>();//创建实体
                foreach (PropertyInfo p in properties)
                {
                    foreach (DataColumn col in dr.Table.Columns)
                    {
                        if (col.ColumnName.ToLower().Equals(p.Name.ToLower()))
                        {
                            object obj = dr[col.ColumnName];
                            if (obj == null || obj is DBNull)
                                continue;
                            if (p.PropertyType == typeof(string))
                            {
                                p.SetValue(model, obj.ToString(), null);
                            }
                            else if (p.PropertyType == typeof(int) || p.PropertyType == typeof(int?))
                            {
                                p.SetValue(model, int.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                            {
                                p.SetValue(model, DateTime.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(float) || p.PropertyType == typeof(float?))
                            {
                                p.SetValue(model, float.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(double) || p.PropertyType == typeof(double?))
                            {
                                p.SetValue(model, double.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?))
                            {
                                p.SetValue(model, decimal.Parse(obj.ToString()), null);
                            }
                            else if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
                            {
                                bool bVal = obj == null || string.IsNullOrWhiteSpace(obj.ToString()) || obj.ToString().Trim() != "0" || obj.ToString().Trim().ToLower() == "false";
                                p.SetValue(model, bVal, null);
                            }
                            break;
                        }
                    }
                }

            }
            catch (Exception)
            {

            }
            return model;
        }

        #endregion

        public static T ConvertEntity<T>(object obj)
        {
            try
            {
                if (obj == null) return default(T);
                PropertyInfo[] properties = typeof(T).GetProperties();
                PropertyInfo[] propertiesobj = obj.GetType().GetProperties();
                T model = Activator.CreateInstance<T>();//创建实体
                foreach (PropertyInfo p in properties)
                {
                    try
                    {
                        if (p.Name.ToUpper() == "BSM")
                        {
                            continue;
                        }
                        PropertyInfo resDefault = propertiesobj.FirstOrDefault(f => f.Name.ToLower().Equals(p.Name.ToLower()));
                        if (resDefault != null)
                        {
                            if (p.PropertyType == resDefault.PropertyType)
                            {
                                p.SetValue(model, resDefault.GetValue(obj, null), null);
                            }
                            else if (p.PropertyType == typeof(string) && resDefault.GetValue(obj, null) != null)
                            {
                                p.SetValue(model, resDefault.GetValue(obj, null).ToString(), null);
                            }
                            else if ((p.PropertyType == typeof(int) || p.PropertyType == typeof(int?)) && resDefault.GetValue(obj, null) != null)
                            {
                                p.SetValue(model, resDefault.GetValue(obj, null).ToString(), null);
                            }
                            else if ((p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)) && resDefault.GetValue(obj, null) != null)
                            {
                                p.SetValue(model, DateTime.Parse(resDefault.GetValue(obj, null).ToString()), null);
                            }
                            else if ((p.PropertyType == typeof(float) || p.PropertyType == typeof(float?)) && resDefault.GetValue(obj, null) != null)
                            {
                                p.SetValue(model, float.Parse(resDefault.GetValue(obj, null).ToString()), null);
                            }
                            else if ((p.PropertyType == typeof(double) || p.PropertyType == typeof(double?)) && resDefault.GetValue(obj, null) != null)
                            {
                                p.SetValue(model, double.Parse(resDefault.GetValue(obj, null).ToString()), null);
                            }
                            else if ((p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?)) && resDefault.GetValue(obj, null) != null)
                            {
                                p.SetValue(model, decimal.Parse(resDefault.GetValue(obj, null).ToString()), null);
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }

                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 按照属性顺序的列名集合
        /// </summary>
        private static IList<string> GetColumnNames<T>(Hashtable hh)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();//获取实体类型的属性集合
            IList<string> ilist = new List<string>();
            int i = 0;
            foreach (PropertyInfo p in properties)
            {
                ilist.Add(GetKey(p.PropertyType.ToString() + i++, hh));
            }
            return ilist;
        }

        /// <summary>
        /// 根据Value查找Key
        /// </summary>
        private static string GetKey(string val, Hashtable tb)
        {
            foreach (DictionaryEntry de in tb)
            {
                if (de.Value.ToString() == val)
                {
                    return de.Key.ToString();
                }
            }
            return null;
        }

    }
}
