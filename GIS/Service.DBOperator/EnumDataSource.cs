
namespace Service.DBOperator
{
    /// <summary>
    /// 枚举数据源
    /// </summary>
    public enum EnumDataSource
    {
        /// <summary>
        /// 未知的数据库类型
        /// </summary>
        Unknown,
        /// <summary>
        /// 关系数据系统
        /// </summary>
        RDS,
        /// <summary>
        /// Microsoft Office Access
        /// </summary>
        MDB,
        /// <summary>
        /// SDE数据库
        /// </summary>
        SDE,
        /// <summary>
        /// 个人空间数据库
        /// </summary>
        PGDB,
         /// <summary>
        /// FTP连接
        /// </summary>
        FTP
    } 
}
