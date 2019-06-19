using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common
{
   public class PublicTools
    {
        /// <summary>
        /// 方法返回数值
        /// </summary>
        /// <param name="code">运行结果 0：正常，1：存在错误</param>
        /// <param name="msg">提示信息</param>
        /// <param name="jsondata">返回数据</param>
        /// <returns></returns>
        public static JsonObject GetReturnData(int code = 0, string msg = null, object jsondata = null)
        {
            return GetReturnLayuiData(jsondata, 0, code, msg);
        }


        /// <summary>
        /// 方法返回数值
        /// </summary>
        /// <param name="jsonData">返回数据</param>
        /// <param name="count">行数</param>
        /// <param name="code">运行结果 0：正常，1：存在错误</param>
        /// <param name="msg">提示信息</param>
        /// <returns></returns>
        public static JsonObject GetReturnLayuiData(object jsonData, int count = 0, int code = 0, string msg = null)
        {
            JsonObject obj = new JsonObject();
            obj["code"] = code.ToString();
            obj["msg"] = msg;
            obj["count"] = count.ToString();
            obj["data"] = jsonData;
            return obj;
        }

    }
}
