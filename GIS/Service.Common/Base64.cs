using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Common
{
    public class Base64
    {

        public string Base64Code(string Message)
        {
            byte[] bytes = Encoding.Default.GetBytes(Message);
            return Convert.ToBase64String(bytes); 
        }
        ///<summary>
        ///Base64解密
        ///</summary>
        ///<paramname="Message"></param>
        ///<returns></returns>
        public string Base64Decode(string Message)
        {
            byte[] bytes = Convert.FromBase64String(Message);
            return Encoding.Default.GetString(bytes);
        }
    }
}
