using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Service.Common
{
    public class ConvertData
    {
        /// <summary>
        /// 将指定类型数据转换为T类型的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(object obj)
        {
            List<T> result = new List<T>();
            try
            {
                string exceptProperty = string.Empty;
                if (typeof(T).Name != string.Format("DJB_{0}", obj.GetType().Name) || string.Format("DJB_{0}", typeof(T).Name) != obj.GetType().Name)
                {
                    exceptProperty += "YSDM";
                }
                if (obj is IList)
                {
                    foreach (var item in obj as IList)
                    {
                        T model = default(T);
                        model = Activator.CreateInstance<T>();
                        foreach (var prop in item.GetType().GetProperties())
                        {
                            if (exceptProperty.Contains(prop.Name))
                            {
                                continue;
                            }
                            PropertyInfo tProperty = model.GetType().GetProperty(prop.Name);
                            if (tProperty != null && tProperty.CanWrite)
                            {
                                if (tProperty.PropertyType == prop.PropertyType)
                                    tProperty.SetValue(model, prop.GetValue(item, null), null);
                            }
                        }
                        result.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 将指定类型的数据转换为T类型的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(object obj)
        {
            T result = default(T);
            try
            {
                result = Activator.CreateInstance<T>();
                foreach (var prop in obj.GetType().GetProperties())
                {
                    PropertyInfo tProperty = result.GetType().GetProperty(prop.Name);                   
                    if (tProperty != null && tProperty.GetType() == prop.GetType() && tProperty.PropertyType == prop.PropertyType)
                    {
                        if (tProperty.CanWrite)
                        {
                            tProperty.SetValue(result, prop.GetValue(obj, null), null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
