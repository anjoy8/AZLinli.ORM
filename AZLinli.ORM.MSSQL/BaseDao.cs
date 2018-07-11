using AZLinli.ORM.Cache;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AZLinli.ORM.ILReader;
using System.Reflection.Emit;
using System.Dynamic;
using System.Configuration;

namespace AZLinli.ORM.DataAccess
{
    /// <summary>
    /// BaseDao 
    /// 为DAL层提供继承基类
    /// Author AZLinli.COM
    /// </summary>
    public class BaseDao<T> :IBaseDao where T : class, new()
    {
        public AZLinContext DbContext { get; set; }

        /// <summary>
        ///  连接查询 不限制Lambda表达式树参数名作为表的别名
        /// </summary>
        public JoinQuery JoinQuery
        {
            get
            {
                var jq = new JoinQuery(DbContext);
                jq.IsAutoDisposeDbContext = true;
                return jq;
            }
        }

        /// <summary>
        /// 连接查询 遵循Lambda表达式树参数名作为表的别名，别名保持相同原则(m,n)=>
        /// </summary>
        public JoinQuery JoinQueryAlias
        {
            get
            {
                var jq = new JoinQuery(DbContext, true);
                jq.IsAutoDisposeDbContext = true;
                return jq;
            }
        }

        public BaseDao()
        {
            DbContext = new AZLinContext();
        }

        public BaseDao(ConnectionStringSettings settings)
        {
            DbContext = new AZLinContext(settings);
        }

        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(T entity)
        {
            using (DbContext)
            {
                return DbContext.Add(entity);
            }
        }

        /// <summary>
        /// 插入批量数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Add(List<T> entities)
        {
            using (DbContext)
            {
                return DbContext.Add(entities);
            }
        }

        /// <summary>
        /// 删除单一数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(T entity)
        {
            using (DbContext)
            {
                return DbContext.Delete(entity);
            }
        }

        /// <summary>
        /// 根据lambda表达式条件删除操作
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int Delete(Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.Delete(exp);
            }
        }

        /// <summary>
        /// 删除批量数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Delete(List<T> entities)
        {
            using (DbContext)
            {
                return DbContext.Delete(entities);
            }
        }

        /// <summary>
        /// 更新单一数据(实体类含有主键特性)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(T entity)
        {
            using (DbContext)
            {
                return DbContext.Update(entity);
            }
        }

        /// <summary>
        /// 根据lambda表达式更新实体类
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int Update(T entity, Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.Update(entity, exp);
            }
        }

        /// <summary>
        /// 依赖字段更新
        /// </summary>
        /// <param name="updateExp"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int Update(Expression<Func<T, T>> updateExp, Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.Update(updateExp, exp);
            }
        }

        /// <summary>
        /// 更新指定字段
        /// </summary>
        /// <param name="updateAction"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int Update(Action<T> updateAction, Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.Update(updateAction, exp);
            }
        }

        /// <summary>
        /// 更新批量数据(实体类含有主键特性)
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Update(List<T> entities)
        {
            using (DbContext)
            {
                return DbContext.Update(entities);
            }
        }

        /// <summary>
        /// 忽略指定字段更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignoreFields"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int UpdateIgnoreFields(T entity, Expression<Func<T, dynamic>> ignoreFields, Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.UpdateIgnoreFields(entity, ignoreFields, exp);
            }
        }

        /// <summary>
        /// 仅更新指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="onlyFields"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int UpdateOnlyFields(T entity, Expression<Func<T, dynamic>> onlyFields, Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.UpdateIgnoreFields(entity, onlyFields, exp);
            }
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool IsExists(Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.IsExists(exp);
            }
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetMax(Expression<Func<T, dynamic>> key)
        {
            using (DbContext)
            {
                return DbContext.GetMax(key);
            }
        }

        /// <summary>
        /// 获取最大值 根据条件
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetMax(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> key)
        {
            using (DbContext)
            {
                return DbContext.GetMax(exp, key);
            }
        }

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetMin(Expression<Func<T, dynamic>> key)
        {
            using (DbContext)
            {
                return DbContext.GetMin(key);
            }
        }

        /// <summary>
        /// 获取最小值根据条件
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetMin(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> key)
        {
            using (DbContext)
            {
                return DbContext.GetMin(exp, key);
            }
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int GetCount(Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.GetCount(exp);
            }
        }

        /// <summary>
        /// 查询单一数据
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public T GetSingle(Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.GetSingle(exp);
            }
        }

        /// <summary>
        /// 根据排序获取单条数据
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public T GetSingle(Expression<Func<T, bool>> exp, Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> orderby)
        {
            using (DbContext)
            {
                return DbContext.GetSingle(exp, orderby);
            }
        }

        /// <summary>
        /// 获取动态类型
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public dynamic GetSingle(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> select)
        {
            using (DbContext)
            {
                return DbContext.GetSingle(exp, select);
            }
        }

        /// <summary>
        /// 查询批量数据
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public List<T> GetList(Expression<Func<T, bool>> exp)
        {
            using (DbContext)
            {
                return DbContext.GetList(exp);
            }
        }

        /// <summary>
        /// 查询部分字段
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<T> GetList(Expression<Func<T, bool>> exp, Expression<Func<T, T>> select)
        {
            using (DbContext)
            {
                return DbContext.GetList<T>(exp, select);
            }
        }

        /// <summary>
        /// 根据排序获取数据
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public List<T> GetList(Expression<Func<T, bool>> exp, Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> orderby)
        {
            using (DbContext)
            {
                return DbContext.GetList(exp, orderby);
            }
        }

        /// <summary>
        /// 获取TObject类型列表对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<TObject> GetList<TObject>(Expression<Func<T, bool>> exp, Expression<Func<T, TObject>> select)
        {
            using (DbContext)
            {
                return DbContext.GetList(exp, select);
            }
        }

        public List<dynamic> GetList(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> select)
        {
            using (DbContext)
            {
                return DbContext.GetList(exp, select);
            }
        }

        /// <summary>
        /// 去重复获取TObject类型列表
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<TObject> GetDistinctList<TObject>(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> select)
        {
            using (DbContext)
            {
                return DbContext.GetDistinctList<T, TObject>(exp, select);
            }
        }

        /// <summary>
        /// 去重复获取动态类型列表
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<dynamic> GetDistinctList(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> select)
        {
            using (DbContext)
            {
                return DbContext.GetDistinctList(exp, select);
            }
        }

        /// <summary>
        /// 双表简单查询
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="on"></param>
        /// <param name="where"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<TObject> GetJoin<T1, T2, TObject>(Expression<Func<T1, T2, bool>> on, Expression<Func<T1, T2, bool>> where, Expression<Func<T1, T2, dynamic>> select) where T1 : class, new() where T2 : class, new()
        {
            using (DbContext)
            {
                return DbContext.JoinQuery.LeftJoin(on).Where(where).Fields(select).GetList<TObject>();
            }
        }

        /// <summary>
        /// 双表简单查询
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="on"></param>
        /// <param name="where"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<dynamic> GetJoin<T1, T2>(Expression<Func<T1, T2, bool>> on, Expression<Func<T1, T2, bool>> where, Expression<Func<T1, T2, dynamic>> select) where T1 : class, new() where T2 : class, new()
        {
            using (DbContext)
            {
                return DbContext.JoinQuery.LeftJoin(on).Where(where).Fields(select).GetList<dynamic>();
            }
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageFilter"></param>
        /// <returns></returns>
        public PageDataSource<T> GetPage(PageFilter pageFilter)
        {
            using (DbContext)
            {
                return DbContext.GetPage<T>(pageFilter);
            }
        }

        #region <<借助SQL语句查询ViewModel方法>>

        /// <summary>
        /// 查询分页数据
        /// 根据SQL查询 不设置排序则为默认排序 不需要设置LambdaExpression
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pageFilter"></param>
        /// <returns></returns>
        public PageDataSource<TObject> GetPage<TObject>(string sql, PageFilter pageFilter)
        {
            using (DbContext)
            {
                return DbContext.GetPage<TObject>(sql, pageFilter);
            }
        }

        /// <summary>
        /// 查询分页数据
        /// 根据SQL查询 不设置排序则为默认排序 不需要设置LambdaExpression
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pageFilter"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public PageDataSource<TObject> GetPage<TObject>(string sql, PageFilter pageFilter, params DbParameter[] cmdParms)
        {
            using (DbContext)
            {
                return DbContext.GetPage<TObject>(sql, pageFilter, cmdParms);
            }
        }

        /// <summary>
        /// 通过sql获取实体集合
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<TObject> GetList<TObject>(string sql)
        {
            using (DbContext)
            {
                return DbContext.GetList<TObject>(sql);
            }
        }

        /// <summary>
        /// 通过sql获取实体集合
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<TObject> GetList<TObject>(string sql, params DbParameter[] cmdParms)
        {
            using (DbContext)
            {
                return DbContext.GetList<TObject>(sql, cmdParms);
            }
        }

        /// <summary>
        /// 获取单个
        /// </summary>
        /// <typeparam name="TVlaue"></typeparam>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public TObject GetSingle<TObject>(string sql, params DbParameter[] cmdParms)
        {
            using (DbContext)
            {
                return DbContext.GetSingle<TObject>(sql, cmdParms);
            }
        }

        /// <summary>
        /// 获取单个
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public TObject GetSingle<TObject>(string sql)
        {
            using (DbContext)
            {
                return DbContext.GetSingle<TObject>(sql);
            }
        }

        #endregion

        #region<<生成实体工具>>

        public bool GenerateEntities(string nameSpace)
        {
            try
            {
                using (DbContext)
                {
                    DbContext.DataBase.GenerateEntities(nameSpace);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /// <summary>
        /// 参数准备
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbParameter GetDbParameter(string name, object value)
        {
            return DbParameterFactory.CreateDbParameter(name, value);
        }

        /// <summary>
        /// 参数化前缀
        /// </summary>
        public string ParameterPrefix
        {
            get { return DbParameterFactory.GetParameterPrefix(); }
        }
    }
}
