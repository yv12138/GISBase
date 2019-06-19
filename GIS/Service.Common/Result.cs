using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.Common
{
    /// <summary>
    /// web服务返回值单值返回类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    [DataContract]
    public class SingleResult<T> 
    {
        /// <summary>
        /// 返回值
        /// </summary>
        [DataMember]
        public T Data { get; set; }
        /// <summary>
        /// 返回值一般是0和1
        /// </summary>
        [DataMember]
        public int Result { get; set; }
        /// <summary>
        /// 错误原因
        /// </summary>
        [DataMember]
        public string Error { get; set; }
    }
    /// <summary>
    /// web服务返回值批量值返回类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    [DataContract]
    public class BatchResult<T> 
    {
        /// <summary>
        /// 返回值
        /// </summary>
        [DataMember]
        public List<T> Data { get; set; }
        /// <summary>
        /// 返回值一般是0和1
        /// </summary>
        [DataMember]
        public int Result { get; set; }
        /// <summary>
        /// 错误原因
        /// </summary>
        [DataMember]
        public string Error { get; set; }
    }
    //public interface IResult
    //{
    //    /// <summary>
    //    /// 返回值一般是0和1
    //    /// </summary>
    //    [DataMember]
    //    int Result { get; set; }
    //    /// <summary>
    //    /// 错误原因
    //    /// </summary>
    //    [DataMember]
    //    string Error { get; set; }
    //}

}
