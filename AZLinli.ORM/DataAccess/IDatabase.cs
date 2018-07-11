using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;

namespace AZLinli.ORM.DataAccess
{
    public interface IDatabase
    {
        string ConnectionString { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        DataBaseType DBType { get; }

        #region <<实体生成工具>>

        int GenerateEntities(string nameSpace, Type baseClass = null);

        #endregion

        #region  << 执行SQL语句 >>

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>影响的记录数</returns>
        int ExecuteSql(string strSql);

        int ExecuteSqlIdentity(string strSql);

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParms">查询参数</param>
        /// <returns>影响的记录数</returns>
        int ExecuteSql(string strSql, params DbParameter[] cmdParms);

        int ExecuteSqlIdentity(string strSql, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行SQL语句，返回影响的记录数(对于长时间查询的语句，设置等待时间避免查询超时)
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="times">SQL执行过期时间</param>
        /// <returns></returns>
        int ExecuteSqlByTime(string strSql, int times);

        /// <summary>
        /// 执行SQL语句，返回影响的记录数(对于长时间查询的语句，设置等待时间避免查询超时)
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="times">SQL执行过期时间</param>
        /// <param name="cmdParms">查询参数</param>
        /// <returns></returns>
        int ExecuteSqlByTime(string strSql, int times, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果
        /// </summary>
        /// <param name="strSql">计算查询结果语句</param>
        /// <returns>查询结果</returns>
        T GetSingle<T>(string strSql);

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParms">查询参数</param>
        /// <returns>查询结果</returns>
        T GetSingle<T>(string strSql, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：使用后一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <returns>SqlDataReader</returns>
        IDataReader ExecuteReader(string strSql);

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：使用后一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParms">查询参数</param>
        /// <returns>SqlDataReader</returns>
        IDataReader ExecuteReader(string strSql, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <returns>DataSet</returns>
        DataSet Query(string strSql);

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <param name="cmdParms">查询参数</param>
        /// <returns>DataSet</returns>
        DataSet Query(string strSql, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行按照一定顺序排列的查询语句，返回DataSet
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <param name="orderText">排序语句，不包含ORDER BY</param>
        /// <param name="cmdParms">查询参数</param>
        /// <returns>DataSet</returns>
        DataSet Query(string strSql, string orderText, params DbParameter[] cmdParms);

        /// <summary>
        /// (对于长时间查询的语句，设置等待时间避免查询超时)
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <param name="times"></param>
        DataSet Query(string strSql, int times);

        /// <summary>
        /// (对于长时间查询的语句，设置等待时间避免查询超时)
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <param name="cmdParms">查询参数</param>
        /// <param name="times"></param>
        DataSet Query(string strSql, int times, params DbParameter[] cmdParms);

        #endregion

        #region << 存储过程操作 >>

        /// <summary>
        /// 执行存储过程，返回影响的行数       
        /// </summary>       
        int RunProcedure(string storedProcName);

        /// <summary>
        /// 执行存储过程带参数，返回影响的行数       
        /// </summary>       
        int RunProcedure(string storedProcName, params DbParameter[] parameters);

        /// <summary>
        /// 执行存储过程，返回输出参数的值和影响的行数       
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="OutParameter">输出参数名称</param>
        /// <returns></returns>
        object RunProcedure(string storedProcName, DbParameter OutParameter, params DbParameter[] InParameters);

        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：使用后一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        IDataReader RunProcedureReader(string storedProcName, params DbParameter[] parameters);

        #endregion

        #region << 分页数据操作 >>

        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <param name="sqlText">SQL语句</param>
        /// <param name="orderText"></param>
        /// <param name="pageIndex">当前页的页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="recordCount">输出参数，返回查询的总记录条数</param>
        /// <param name="pageCount">输出参数，返回查询的总页数</param>
        /// <param name="currentPageIndex">输出参数，返回当前页面索引</param>
        /// <param name="commandParameters">查询参数</param>
        /// <returns>返回查询结果</returns>
        DataTable ExecutePage(string sqlText, string orderText, int PageIndex, int PageSize, out int RecordCount, out int PageCount, out int currentPageIndex, params DbParameter[] commandParameters);

        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <param name="sqlText">SQL语句</param>
        /// <param name="orderText"></param>
        /// <param name="pageIndex">当前页的页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="recordCount">输出参数，返回查询的总记录条数</param>
        /// <param name="pageCount">输出参数，返回查询的总页数</param>
        /// <param name="currentPageIndex">输出参数，返回当前页面索引</param>
        /// <param name="commandParameters">查询参数</param>
        /// <returns>返回查询结果</returns>
        IDataReader ExecutePageReader(string sqlText, string orderText, int PageIndex, int PageSize, out int RecordCount, out int PageCount, out int currentPageIndex, params DbParameter[] commandParameters);

        IDataReader ExecutePageReader(string sqlText, string orderText, int pageIndex, int pageSize, out int recordCount, out int pageCount, out int currentPageIndex);

        #endregion

        #region << 参数化 >>

        /// <summary>
        /// 数据库参数连接符
        /// </summary>
        string ParameterPrefix { get; }

        /// <summary>
        /// 得到数据库SQL参数对象
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>返回SQL参数对象</returns>
        DbParameter GetDbParameter(string parameterName, object value);

        /// <summary>
        /// 得到数据库SQL参数对象
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="dbType">数据类型</param>
        /// <returns>返回SQL参数对象</returns>
        DbParameter GetDbParameter(string parameterName, object value, DbType dbType);

        #endregion

        #region<<事务>>
        void BeginTransaction();

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="iso">指定事务行为</param>
        void BeginTransaction(IsolationLevel level);

        /// <summary>
        /// 回滚事务
        /// </summary>
        void Rollback();

        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        #endregion

        void Dispose();

    }
}