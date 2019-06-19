using System;
using System.Xml.Linq;
using System.ServiceModel;

namespace Service.Common.ConfigurationManager
{
    public class connectionStringEntity
    {

        public string Name { get; set; }
        public string connectionString { get; set; }
        public string providerName { get; set; }

        public connectionStringEntity(XElement element)
        {
            this.Name = element.Attribute("name") == null ? "" : element.Attribute("name").Value;
            this.connectionString = element.Attribute("connectionString") == null ? "" : element.Attribute("connectionString").Value;
            this.providerName = element.Attribute("providerName") == null ? "" : element.Attribute("providerName").Value;
        }
    }
    public class appSettingsEntity
    {

        public string key { get; set; }
        public string value { get; set; }

        public appSettingsEntity(XElement element)
        {
            this.key = element.Attribute("key") == null ? "" : element.Attribute("key").Value;
            this.value = element.Attribute("value") == null ? "" : element.Attribute("value").Value;
        }
    }

    public class webServicesEntity
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public int MaxBufferSize { get; set; }
        public int MaxReceivedMessageSize { get; set; }
        public string SendTimeout { get; set; }
        public string ReceiveTimeout { get; set; }

        public WebServiceStruct ToWebServiceStruct()
        {
            WebServiceStruct pConfig = new WebServiceStruct();
            pConfig.Binding = new System.ServiceModel.BasicHttpBinding();
            pConfig.Binding.MaxBufferSize = MaxBufferSize;
            pConfig.Binding.MaxReceivedMessageSize = MaxReceivedMessageSize;
            TimeSpan pSend;
            TimeSpan.TryParse(SendTimeout, out pSend);
            pConfig.Binding.SendTimeout = pSend;
            TimeSpan pReceive;
            TimeSpan.TryParse(ReceiveTimeout, out pReceive);
            pConfig.Binding.ReceiveTimeout = pReceive;
            pConfig.Address = new System.ServiceModel.EndpointAddress(URL);
            return pConfig;
        }
    }

    public class WebServiceStruct
    {
        public BasicHttpBinding Binding { get; set; }
        public EndpointAddress Address { get; set; }
    }
}
