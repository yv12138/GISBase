//using NHibernate.Mapping.Attributes;
using System;


namespace Entity.Web
{
    //[Serializable]
    //[Class(Table = "Humi")]
   public class Humi
    {
        //[Id(0, Name = "ID", Column = "ID", UnsavedValue = "0")]
        //[Generator(1, Class = "native")]
        public virtual int ID { get; set; }
        //[Property]
        public virtual DateTime? Date { get; set; }
        //[Property]
        public virtual string Title { get; set; }
        //[Property]
        public virtual string Data { get; set; }
    }
}
