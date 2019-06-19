using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace Service.Common.ConfigurationManager
{
    public class ManageConfig
    {

        private XDocument pXDoc = null;
        private XmlDocument xmlDoc = new XmlDocument();
        private List<connectionStringEntity> connectionStringList = null;
        private List<appSettingsEntity> appSettingList = null;
        private List<webServicesEntity> webServicesList = null;
        public static readonly ManageConfig instance = new ManageConfig();
        public static ManageConfig instanceSystemConfig;

        public static ManageConfig GetInstanceForSystemConfig()
        {
            if (instanceSystemConfig == null)
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ServiceConfig.xml");
                instanceSystemConfig = new ManageConfig(path);
            }
            return instanceSystemConfig;
        }
        private ManageConfig(string path)
        {
            init(path);
        }
        void init(string path)
        {
            appSettingList = new List<appSettingsEntity>();
            webServicesList = new List<webServicesEntity>();
            try
            {
                pXDoc = XDocument.Load(path);
                xmlDoc.Load(path);
                appSettingList = (from f in pXDoc.Element("configuration").Element("AppSettings").Elements()
                                  select new appSettingsEntity(f)).ToList<appSettingsEntity>();
                webServicesList = (
                   from pServiceConfig in pXDoc.Element("configuration").Element("CustomConfig").Element("WebServices").Elements()
                   select new webServicesEntity()
                   {
                       Name = pServiceConfig.Element("Name").Value,
                       //URL = IPMappingConfig.GetNewUrl(pServiceConfig.Element("URL").Value),
                       URL = pServiceConfig.Element("URL").Value,
                       MaxBufferSize = Convert.ToInt32(pServiceConfig.Element("MaxBufferSize").Value),
                       MaxReceivedMessageSize = Convert.ToInt32(pServiceConfig.Element("MaxReceivedMessageSize").Value),
                       SendTimeout = pServiceConfig.Element("SendTimeout").Value,
                       ReceiveTimeout = pServiceConfig.Element("ReceiveTimeout").Value,
                   }
               ).ToList<webServicesEntity>();
            }
            catch
            {

            }
        }

        private ManageConfig()
        {
            init();
        }
        void init()
        {
            connectionStringList = new List<connectionStringEntity>();
            appSettingList = new List<appSettingsEntity>();
            webServicesList = new List<webServicesEntity>();
            try
            {
                string strPath = System.AppDomain.CurrentDomain.BaseDirectory;               
                if (!string.IsNullOrWhiteSpace(strPath))
                {
                    strPath += "\\ServiceConfig.xml";
                    InitConfig(strPath);
                }
            }
            catch
            {

            }
        }
        public bool InitConfig(string Path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Path)) return false;
                pXDoc = XDocument.Load(Path);
                xmlDoc.Load(Path);
                //加载字符串连接
                connectionStringList = (from f in pXDoc.Element("configuration").Element("connectionStrings").Elements()
                                        select new connectionStringEntity(f)).ToList<connectionStringEntity>();
                appSettingList = (from f in pXDoc.Element("configuration").Element("appSettings").Elements()
                                  select new appSettingsEntity(f)).ToList<appSettingsEntity>();
                webServicesList = (
                   from pServiceConfig in pXDoc.Descendants("WebService")
                   select new webServicesEntity()
                   {
                       Name = pServiceConfig.Element("Name").Value,
                       //URL = IPMappingConfig.GetNewUrl(pServiceConfig.Element("URL").Value),
                       URL = pServiceConfig.Element("URL").Value,
                       MaxBufferSize = Convert.ToInt32(pServiceConfig.Element("MaxBufferSize").Value),
                       MaxReceivedMessageSize = Convert.ToInt32(pServiceConfig.Element("MaxReceivedMessageSize").Value),
                       SendTimeout = pServiceConfig.Element("SendTimeout").Value,
                       ReceiveTimeout = pServiceConfig.Element("ReceiveTimeout").Value,
                   }
               ).ToList<webServicesEntity>();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConnectionStrings(string key)
        {
            try
            {
                if (connectionStringList == null || connectionStringList.Count == 0 || string.IsNullOrWhiteSpace(key)) return string.Empty;
                connectionStringEntity findEnity = connectionStringList.FirstOrDefault(p => p.Name.ToUpper() == key.ToUpper());
                if (findEnity != null)
                {
                    return findEnity.connectionString;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetappSettings(string key)
        {
            try
            {
                if (appSettingList == null || appSettingList.Count == 0 || string.IsNullOrWhiteSpace(key)) return string.Empty;
                appSettingsEntity findEnity = appSettingList.FirstOrDefault(p => p.key.ToUpper() == key.ToUpper());
                if (findEnity != null)
                {
                    return findEnity.value;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取服务配置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WebServiceStruct GetCustomWebServiceConfig(string name)
        {
            try
            {
                if (webServicesList == null || webServicesList.Count == 0 || string.IsNullOrWhiteSpace(name)) return null;
                webServicesEntity findEnity = webServicesList.FirstOrDefault(p => p.Name == name);
                if (findEnity != null)
                {
                    return findEnity.ToWebServiceStruct();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取配置文件的相应节点
        /// </summary>
        /// <param name="elementName">节点名</param>
        /// <returns></returns>
        public XElement GetxElementByConfig(string elementName)
        {
            if (pXDoc == null || string.IsNullOrWhiteSpace(elementName)) return null;
            return pXDoc.Element("configuration").Descendants(elementName).FirstOrDefault();
        }
        /// <summary>
        /// 根据节点名获取配置文件的相应节点
        /// </summary>
        /// <param name="elementName">节点名</param>
        /// <returns></returns>
        public XmlNode GetXmlElementByConfig(string elementName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(elementName))
                    throw new Exception("根据节点名获取相应的节点时，节点名不能为空");
                return xmlDoc.SelectSingleNode("//" + elementName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 将指定字符串中的格式项替换为指定数组中相应对象的字符串表示形式。
        /// </summary>
        /// <param name="format">复合格式字符串。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>format 的副本，其中的格式项已替换为 args 中相应对象的字符串表示形式。</returns>
        public string stringFormat(string format, string[] args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(format) || args == null || args.Length == 0)
                {
                    return string.Empty;
                }
                format = string.Format(format, args);
                return format;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 返回限制用户IP地址，如为空则代表不是限制用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public string GetLimitedUserIP(string user)
        {
            string result = "";
            string data = GetappSettings("UserIP");
            if (!string.IsNullOrEmpty(data))
            {
                List<string> userIplist = data.Split('|').ToList();
                foreach (string userIp in userIplist)
                {
                    List<string> list = userIp.Split(',').ToList();
                    if (list.Count > 1 && list[0] == user)
                    {
                        result = list[1];
                        break;
                    }
                }
            }
            return result;
        }

        public bool GetSingleLogin()
        {
            bool result = false;
            string data = GetappSettings("SingleLogin");
            if (data != null && data == "1")
            {
                result = true;
            }
            return result;
        }
    }
}
