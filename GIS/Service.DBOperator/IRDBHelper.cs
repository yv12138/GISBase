using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Collections;

namespace Service.DBOperator
{
    /// <summary>
    /// 目    的：数据库操作接口
    /// 创 建 人：
    /// 创建时间：
    /// 备    注：
    /// 修改描述：
    /// 修 改 人：
    /// 修改日期：
    /// </summary>
    public interface IRDBHelper
    {
        #region ConnectionInfo

        /// <summary>
        /// 数据库类型
        /// </summary>
        DatabaseType DataBase { get;}

        /// <summary>
        /// 连接状态
        /// </summary>
        ConnectionState State { get;}

        /// <summary>
        /// 连接信息
        /// </summary>
        String ConnectionString { get;set;}

        /// <summary>
        /// 当前活动连接
        /// </summary>
        IDbConnection ActiveConn { get;set;}

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns>是否成功</returns>
        bool Connect();

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns>是否成功</returns>
        Boolean DisConnect();

        #endregion

        #region Transaction Processing

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>是否成功</returns>
        Boolean BeginTransaction();

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns>是否成功</returns>
        Boolean Rollback();

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns>是否成功</returns>
        Boolean Commit();

        #endregion

        #region MemoryDataSetOperate

        /// <summary>
        /// 根据表名获取内存中的表
        /// </summary>
        /// <param name="tableName">DataTable名称</param>
        /// <returns>内存中名称为tableName的表</returns>
        DataTable GetTable(string tableName);

        /// <summary>
        /// 获取内存中的所有表的名称集合
        /// </summary>
        /// <returns>内存中表集合的名称</returns>
        IList<string> GetTablesName();

        /// <summary>
        /// 只存储修改 不释放DataTable
        /// </summary>
        /// <param name="tableName">DataTable名称</param>
        /// <returns>是否成功</returns>
        Boolean SaveTable(string tableName);

        /// <summary>
        /// 存储修改 释放DataTable
        /// </summary>
        /// <param name="tableName">DataTable名称</param>
        /// <param name="release">释放</param>
        /// <returns>是否成功</returns>
        Boolean SaveTable(string tableName, bool release);

        /// <summary>
        /// 释放DataTable
        /// </summary>
        /// <param name="tableName">DataTable名称</param>
        /// <param name="storage">是否要存储:True保存并释放,False直接释放</param>
        /// <returns>是否成功</returns>
        Boolean ReleaseTable(string tableName, bool storage);

        /// <summary>
        /// 释放DataTable，保存修改
        /// </summary>
        /// <param name="tableName">DataTable名称</param>
        /// <returns>是否成功</returns>
        Boolean ReleaseTable(string tableName);

        /// <summary>
        /// 清除所有内存DataTable,此方法使用时要非常小心
        /// </summary>
        /// <returns>是否成功</returns>
        Boolean Clear();

        #endregion

        #region DataOperateTools

        /// <summary>
        /// 从数据库字段读取到字节数组
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>字节数组</returns>
        Byte[] ReadBlobToBytes(String commandText);

        /// <summary>
        /// 从数据库字段读取到字节数组
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>字节数组</returns>
        Byte[] ReadBlobToBytes(String commandText, IDbDataParameter[] commandParameters);

        /// <summary>
        /// 从数据库字段读取到文件
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="filePath">文件地址</param>
        /// <returns>是否成功</returns>
        Boolean ReadBlobToFile(String commandText, String filePath);

        /// <summary>
        /// 从数据库字段读取到文件
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="filePath">文件地址</param>
        /// <returns>是否成功</returns>
        Boolean ReadBlobToFile(String commandText, IDbDataParameter[] commandParameters, String filePath);

        /// <summary>
        /// 将字节数组存入到数据库BLOB字段中
        /// </summary>
        /// <param name="tabname">表名字</param>
        /// <param name="fieldname">blob或image字段名字</param>
        /// <param name="wherestr">条件语句，可为空，如果不为空，需要带where关键字传入</param>
        /// <param name="content">要存入的字节流</param>
        /// <returns>是否成功</returns>
        Boolean WriteBytesToBlob(string tabname, string fieldname, string wherestr, ref Byte[] content);

        /// <summary>
        /// 将文件存入到数据库BLOB字段中
        /// </summary>
        /// <param name="tabname">表名字</param>
        /// <param name="fieldname">blob或image字段名字</param>
        /// <param name="wherestr">条件语句，可为空，如果不为空，需要带where关键字传入</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否成功</returns>
        Boolean WriteFileToBlob(string tabname, string fieldname, string wherestr, string filePath);

        /// <summary>
        /// 获取指定来源的下一标识号
        /// </summary>
        /// <param name="tableName">指定数据表名称</param>
        /// <param name="fieldName">指定的字段索引</param>
        /// <param name="startNumber">启始标识号</param>
        /// <param name="step">步长</param>
        /// <returns>下一标识号</returns>
        Int64 GetNextValidID(String tableName, String fieldName, Int64 startNumber, Int64 step);

        /// <summary>
        /// 获取指定来源的下一标识号
        /// </summary>
        /// <param name="tableName">指定数据表名称</param>
        /// <param name="fieldName">指定的字段索引</param>
        /// <returns>下一标识号</returns>
        Int64 GetNextValidID(String tableName, String fieldName);

        /// <summary>
        /// 获取系统当前时间
        /// </summary>
        DateTime SystemDataTime { get;}

        #endregion

        #region 应用扩展
        
        /// <summary>
        /// 执行数据表查询
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="release">是否释放</param>
        /// <returns>查询出来的数据表</returns>
        DataTable ExecuteDatatable(string tableName, string commandText, bool release);

        /// <summary>
        /// 执行数据表查询
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="release">是否释放</param>
        /// <returns>查询出来的数据表</returns>
        DataTable ExecuteDatatableBySdoGeometry(string tableName, string commandText, bool release);

        /// <summary>
        /// 执行SQL语句 NonQuery
        /// </summary>
        /// <param name="sqlText">标准SQL语句</param>
        /// <returns>返回受影响的记录数</returns>
        int ExecuteSQL(string sqlText);

        #endregion

        #region ExecuteDataset

        /// <summary>
        /// 执行数据集
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <returns>数据集</returns>
        DataSet ExecuteDataset(String commandText, CommandType commandType);

        /// <summary>
        /// 执行数据集
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>数据集</returns>
        DataSet ExecuteDataset(String commandText, CommandType commandType, IDbDataParameter[] commandParameters);

        /// <summary>
        /// 执行数据集
        /// </summary>
        /// <param name="storedprocedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数值</param>
        /// <returns>数据集</returns>
        DataSet ExecuteDataset(String storedprocedureName, params Object[] parameterValues);

        /// <summary>
        /// 执行数据集
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="release">是否释放数据集</param>
        /// <param name="dsName">保存在内存中的数据集名称</param>
        /// <returns>数据集</returns>
        DataSet ExecuteDataset(String commandText, CommandType commandType, Boolean release, string dsName);

        /// <summary>
        /// 执行数据集
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="release">是否释放数据集</param>
        /// <param name="dsName">保存在内存中的数据集名称</param>
        /// <returns>数据集</returns>
        DataSet ExecuteDataset(String commandText, CommandType commandType, IDbDataParameter[] commandParameters, Boolean release, string dsName);

        /// <summary>
        /// 执行数据集
        /// </summary>
        /// <param name="storedprocedureName">存储过程名称</param>
        /// <param name="release">是否释放数据集</param>
        /// <param name="parameterValues">存储过程参数值</param>
        /// <returns>数据集</returns>
        //DataSet ExecuteDataset(String storedprocedureName, Boolean release, params Object[] parameterValues);

        #endregion

        #region ExecuteNonQuery


        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <returns>影响到此次命令的记录数</returns>
        int ExecuteNonQuery(String commandText, CommandType commandType);

        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命今参数</param>
        /// <returns>影响到此次命令的记录数</returns>
        int ExecuteNonQuery(String commandText, CommandType commandType, IDbDataParameter[] commandParameters);

        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int ExecuteNonQueryByBuffer(string commandText, CommandType commandType, byte[] buffer);

        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="storedprocedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数值</param>
        /// <returns>影响到此次命令的记录数</returns>
        int ExecuteNonQuery(String storedprocedureName, params Object[] parameterValues);

        #endregion

        #region ExecuteNonQueryWithException
        /// <summary>
        /// 执行查询语句命令
        /// zhouzh200910 执行非查询语句，有异常则直接抛出
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <returns>影响到此次命令的记录数</returns>
        int ExecuteNonQueryWithException(String commandText, CommandType commandType);

        /// <summary>
        /// 执行查询语句命令
        /// zhouzh200910 执行非查询语句，有异常则直接抛出
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命今参数</param>
        /// <returns>影响到此次命令的记录数</returns>
        int ExecuteNonQueryWithException(String commandText, CommandType commandType, IDbDataParameter[] commandParameters);

        /// <summary>
        /// 执行查询语句命令
        /// zhouzh200910 执行非查询语句，有异常则直接抛出
        /// </summary>
        /// <param name="storedprocedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数值</param>
        /// <returns>影响到此次命令的记录数</returns>
        int ExecuteNonQueryWithException(String storedprocedureName, params Object[] parameterValues);

        #endregion

        #region ExecuteReader

        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <returns>DataReader</returns>
        IDataReader ExecuteReader(String commandText, CommandType commandType);

        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命今参数</param>
        /// <returns>DataReader</returns>
        IDataReader ExecuteReader(String commandText, CommandType commandType, IDbDataParameter[] commandParameters);

        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="storedprocedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数值</param>
        /// <returns>DataReader</returns>
        IDataReader ExecuteReader(String storedprocedureName, params Object[] parameterValues);

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <returns>单一数据对象</returns>
        object ExecuteScalar(String commandText, CommandType commandType);

        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命今参数</param>
        /// <returns>单一数据对象</returns>
        object ExecuteScalar(String commandText, CommandType commandType, IDbDataParameter[] commandParameters);

        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <param name="storedprocedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数值</param>
        /// <returns>单一数据对象</returns>
        object ExecuteScalar(String storedprocedureName, params Object[] parameterValues);

        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public enum DbConnectionOwnership
    {
        /// <summary>Connection is owned and managed by IDbHelper</summary>
        Internal,
        /// <summary>Connection is owned and managed by the caller</summary>
        External
    }
    /// <summary>
    /// 关系数据系统类型
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// 未知的类型
        /// </summary>
        Unknown,
        /// <summary>
        /// Microsoft SQL Server
        /// </summary>
        MSSQL,
        /// <summary>
        /// Microsoft Office Access
        /// </summary>
        MSAccess,
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,
        /// <summary>
        /// TekOracle
        /// </summary>
        TekOracle
    }
}
