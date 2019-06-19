using Business.Public;
using Entity.Web;
using Newtonsoft.Json.Linq;
using NHibernate;
using Service.Common;
using Service.Common.ConfigurationManager;
using Service.DBOperator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebGIS.Controllers
{
    [RoutePrefix("api/Web")]
    public class HumiController : ApiController
    {
        [HttpPost]
        [Route("SaveData")]
        public IList<Humi> Post([FromBody]JObject value)
        {
            string jsondata = HttpContext.Current.Request.Form["jsondata"].ToString();
            var pageSize = int.Parse(string.IsNullOrEmpty(HttpContext.Current.Request["rows"]) ? "10" : HttpContext.Current.Request["rows"]);
            var pageIndex = int.Parse(string.IsNullOrEmpty(HttpContext.Current.Request["page"]) ? "1" : HttpContext.Current.Request["page"]);
            int total = 0, start = 0, limit = 0;
            string json = value["JsonData"].ToString();
            string hql = "from Humi where 1=1 ";
            var list = PublicBusiness.GetEntitiesByHQL<Humi>(hql).ToList();
            return list;
        }  
    }
}