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
    /// AZLinContext
    /// Author AZLinli.COM
    /// </summary>
    public class AZLinContext : DbContext
    {
        /// <summary>
        ///  连接查询 不限制Lambda表达式树参数别名
        /// </summary>
        public JoinQuery JoinQuery
        {
            get
            {
                return new JoinQuery(this);
            }
        }

        /// <summary>
        /// 连接查询 遵循Lambda表达式树参数别名相同原则(m,n)=>
        /// </summary>
        public JoinQuery JoinQueryAlias
        {
            get
            {
                return new JoinQuery(this, true);
            }
        }
        public AZLinContext()
        {
        }

        public AZLinContext(ConnectionStringSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add<T>(T entity) where T : class, new()
        {
            if (entity != null)
            {
                return base.InsertT(entity);
            }
            return 0;
        }

        /// <summary>
        /// 插入批量数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Add<T>(List<T> entities) where T : class, new()
        {
            return base.InsertBatchT(entities);
        }

        /// <summary>
        /// 删除单一数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete<T>(T entity) where T : class, new()
        {
            if (entity != null)
            {
                return base.DeleteT<T>(entity);
            }
            return 0;
        }

        /// <summary>
        /// 根据lambda表达式条件删除操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int Delete<T>(Expression<Func<T, bool>> exp) where T : class, new()
        {
            return base.DeleteT(exp);
        }

        /// <summary>
        /// 删除批量数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Delete<T>(List<T> entities) where T : class, new()
        {
            return base.DeleteBatchT(entities);
        }

        /// <summary>
        /// 更新单一数据(实体类含有主键特性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update<T>(T entity) where T : class, new()
        {
            if (entity != null)
            {
                return base.UpdateT(entity);
            }
            return 0;
        }

        /// <summary>
        /// 根据lambda表达式更新实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int Update<T>(T entity, Expression<Func<T, bool>> exp) where T : class, new()
        {
            return base.UpdateT(entity, exp);
        }

        /// <summary>
        /// 依赖字段更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateExp"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int Update<T>(Expression<Func<T, T>> updateExp, Expression<Func<T, bool>> exp) where T : class, new()
        {
            return base.UpdateT(updateExp, exp);
        }

        /// <summary>
        /// 更新指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateAction"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int Update<T>(Action<T> updateAction, Expression<Func<T, bool>> exp) where T : class, new()
        {
            T entity = new T();
            List<string> infos = (from instruction in updateAction.Method.GetInstructions()
                                  where instruction.OpCode.OperandType == OperandType.InlineMethod && instruction.OpCode.Name == "callvirt" && ((MethodInfo)instruction.Operand).Name.StartsWith("set_")
                                  select ((MethodInfo)instruction.Operand).Name.Substring(4)).ToList();
            updateAction(entity);
            return base.UpdateT(entity, infos.Distinct().ToList(), exp);
        }


        /// <summary>
        /// 更新批量数据(实体类含有主键特性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Update<T>(List<T> entities) where T : class, new()
        {
            return base.UpdateBatchT(entities);
        }

        /// <summary>
        /// 忽略指定字段更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="ignoreFields"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int UpdateIgnoreFields<T>(T entity, Expression<Func<T, dynamic>> ignoreFields, Expression<Func<T, bool>> exp) where T : class, new()
        {
            return base.UpdateTIgnoreFields(entity, ignoreFields, exp);
        }

        /// <summary>
        /// 仅更新指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="onlyFields"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int UpdateOnlyFields<T>(T entity, Expression<Func<T, dynamic>> onlyFields, Expression<Func<T, bool>> exp) where T : class, new()
        {
            return base.UpdateTOnlyFields(entity, onlyFields, exp);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool IsExists<T>(Expression<Func<T, bool>> exp) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda<T, dynamic>(selectSql, parms, exp, null, SelectType.Count);
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                if (reader.Read() && reader[0] != DBNull.Value)
                {
                    return Convert.ToInt32(reader[0]) > 0;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetMax<T>(Expression<Func<T, dynamic>> key) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            GetSQLByLambda<T, dynamic>(selectSql, null, null, key, SelectType.Max);
            using (var reader = DataBase.ExecuteReader(selectSql.ToString()))
            {
                if (reader.Read() && reader[0] != DBNull.Value)
                {
                    return (dynamic)reader[0];
                }
                else
                {
                    return default(dynamic);
                }
            }
        }

        /// <summary>
        /// 获取最大值 根据条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetMax<T>(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> key) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda<T, dynamic>(selectSql, parms, exp, key, SelectType.Max);
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                if (reader.Read() && reader[0] != DBNull.Value)
                {
                    return (dynamic)reader[0];
                }
                else
                {
                    return default(dynamic);
                }
            }
        }

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetMin<T>(Expression<Func<T, dynamic>> key) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            GetSQLByLambda<T, dynamic>(selectSql, null, null, key, SelectType.Min);
            using (var reader = DataBase.ExecuteReader(selectSql.ToString()))
            {
                if (reader.Read() && reader[0] != DBNull.Value)
                {
                    return (dynamic)reader[0];
                }
                else
                {
                    return default(dynamic);
                }
            }
        }

        /// <summary>
        /// 获取最小值根据条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetMin<T>(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> key) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda<T, dynamic>(selectSql, parms, exp, key, SelectType.Min);
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                if (reader.Read() && reader[0] != DBNull.Value)
                {
                    return (dynamic)reader[0];
                }
                else
                {
                    return default(dynamic);
                }
            }
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int GetCount<T>(Expression<Func<T, bool>> exp) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetExistsSQLByLambda<T>(selectSql, parms, exp);
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                if (reader.Read() && reader[0] != DBNull.Value)
                {
                    return Convert.ToInt32(reader[0]);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 查询单一数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public T GetSingle<T>(Expression<Func<T, bool>> exp) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda(selectSql, parms, exp);
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                if (reader.Read())
                {
                    T entity = new T();
                    SetEntityMembers<T>(reader, entity);
                    return entity;
                }
                else
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// 根据排序获取单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public T GetSingle<T>(Expression<Func<T, bool>> exp, Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> orderby) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda(selectSql, parms, exp, orderby);
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                if (reader.Read())
                {
                    T entity = new T();
                    SetEntityMembers<T>(reader, entity);
                    return entity;
                }
                else
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// 获取动态类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public dynamic GetSingle<T>(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> select) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda<T, dynamic>(selectSql, parms, exp, select);
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                if (reader.Read())
                {
                    return GetDynamicObject<dynamic>(reader);
                }
                else
                {
                    return default(dynamic);
                }
            }
        }

        /// <summary>
        /// 查询批量数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> exp) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda(selectSql, parms, exp);
            List<T> entities = new List<T>();
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                while (reader.Read())
                {
                    T entityWhile = new T();
                    SetEntityMembers<T>(reader, entityWhile);
                    entities.Add(entityWhile);
                }
            }
            return entities;
        }

        /// <summary>
        /// 查询部分字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, T>> select) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda(selectSql, parms, exp, select);
            List<T> objs = new List<T>();
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                while (reader.Read())
                {
                    T entityWhile = new T();
                    SetEntityMembers<T>(reader, entityWhile);
                    objs.Add(entityWhile);
                }
            }
            return objs;
        }

        /// <summary>
        /// 根据排序获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> orderby) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda(selectSql, parms, exp, orderby);
            List<T> entities = new List<T>();
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                while (reader.Read())
                {
                    T entityWhile = new T();
                    SetEntityMembers<T>(reader, entityWhile);
                    entities.Add(entityWhile);
                }
            }
            return entities;
        }

        /// <summary>
        /// 获取TObject类型列表对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<TObject> GetList<T, TObject>(Expression<Func<T, bool>> exp, Expression<Func<T, TObject>> select) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda(selectSql, parms, exp, select);
            List<TObject> objs = new List<TObject>();
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                while (reader.Read())
                {
                    objs.Add(GetDynamicObject<TObject>(reader));
                }
            }
            return objs;
        }

        public List<dynamic> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> select) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda(selectSql, parms, exp, select);
            List<dynamic> objs = new List<dynamic>();
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                while (reader.Read())
                {
                    objs.Add(GetDynamicObject<dynamic>(reader));
                }
            }
            return objs;
        }

        /// <summary>
        /// 去重复获取TObject类型列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<TObject> GetDistinctList<T, TObject>(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> select) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda(selectSql, parms, exp, select, SelectType.Distinct);
            List<TObject> objs = new List<TObject>();
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                while (reader.Read())
                {
                    objs.Add(GetDynamicObject<TObject>(reader));
                }
            }
            return objs;
        }

        /// <summary>
        /// 去重复获取动态类型列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public List<dynamic> GetDistinctList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, dynamic>> select) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            GetSQLByLambda(selectSql, parms, exp, select, SelectType.Distinct);
            List<dynamic> objs = new List<dynamic>();
            using (var reader = DataBase.ExecuteReader(selectSql.ToString(), parms.ToArray()))
            {
                while (reader.Read())
                {
                    objs.Add(GetDynamicObject<dynamic>(reader));
                }
            }
            return objs;
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
            return this.JoinQuery.LeftJoin(on).Where(where).Fields(select).GetList<TObject>();
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
            return this.JoinQuery.LeftJoin(on).Where(where).Fields(select).GetList<dynamic>();
        }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageFilter"></param>
        /// <returns></returns>
        public PageDataSource<T> GetPage<T>(PageFilter pageFilter) where T : class, new()
        {
            StringBuilder selectSql = new StringBuilder();
            List<DbParameter> parms = new List<DbParameter>();
            if (pageFilter is PageFilter<T>)
            {
                GetSQLByLambda(selectSql, parms, (Expression<Func<T, bool>>)((PageFilter<T>)pageFilter).WhereExpression);
            }
            else
            {
                GetSQLByLambda<T>(selectSql, parms, null);
            }
            PageDataSource<T> dataSource = new PageDataSource<T>();
            int recordCount, pageCount, pageIndex;
            using (var reader = DataBase.ExecutePageReader(selectSql.ToString(), pageFilter.OrderText, pageFilter.PageIndex, pageFilter.PageSize, out recordCount, out pageCount, out pageIndex, parms.ToArray()))
            {
                dataSource.RecordCount = recordCount;
                dataSource.PageCount = pageCount;
                dataSource.PageIndex = pageIndex;
                dataSource.PageSize = pageFilter.PageSize;
                dataSource.DataSource = new List<T>();

                while (reader.Read())
                {
                    T entityWhile = new T();
                    SetEntityMembers<T>(reader, entityWhile);
                    dataSource.DataSource.Add(entityWhile);
                }
                return dataSource;
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
            PageDataSource<TObject> dataSource = new PageDataSource<TObject>();
            int recordCount, pageCount, pageIndex;
            using (var reader = DataBase.ExecutePageReader(sql, pageFilter.OrderText, pageFilter.PageIndex, pageFilter.PageSize, out recordCount, out pageCount, out pageIndex))
            {
                dataSource.RecordCount = recordCount;
                dataSource.PageCount = pageCount;
                dataSource.PageIndex = pageIndex;
                dataSource.PageSize = pageFilter.PageSize;
                dataSource.DataSource = new List<TObject>();

                while (reader.Read())
                {
                    dataSource.DataSource.Add(GetDynamicObject<TObject>(reader));
                }
                return dataSource;
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
            PageDataSource<TObject> dataSource = new PageDataSource<TObject>();
            int recordCount, pageCount, pageIndex;

            using (var reader = DataBase.ExecutePageReader(sql, pageFilter.OrderText, pageFilter.PageIndex, pageFilter.PageSize, out recordCount, out pageCount, out pageIndex, cmdParms))
            {
                dataSource.RecordCount = recordCount;
                dataSource.PageCount = pageCount;
                dataSource.PageIndex = pageIndex;
                dataSource.PageSize = pageFilter.PageSize;
                dataSource.DataSource = new List<TObject>();

                while (reader.Read())
                {
                    dataSource.DataSource.Add(GetDynamicObject<TObject>(reader));
                }
                return dataSource;
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
            List<TObject> dataSource = new List<TObject>();
            using (var reader = DataBase.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    dataSource.Add(GetDynamicObject<TObject>(reader));
                }
                return dataSource;
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
            List<TObject> dataSource = new List<TObject>();
            using (var reader = DataBase.ExecuteReader(sql, cmdParms.ToArray()))
            {
                while (reader.Read())
                {
                    dataSource.Add(GetDynamicObject<TObject>(reader));
                }
                return dataSource;
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
            using (var reader = DataBase.ExecuteReader(sql, cmdParms.ToArray()))
            {
                if (reader.Read())
                {
                    return GetDynamicObject<TObject>(reader);
                }
                else
                {
                    return default(TObject);
                }
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
            using (var reader = DataBase.ExecuteReader(sql))
            {
                if (reader.Read())
                {
                    return GetDynamicObject<TObject>(reader);
                }
                else
                {
                    return default(TObject);
                }
            }
        }

        #endregion

        #region<<生成实体工具>>

        public bool GenerateEntities(string nameSpace)
        {
            try
            {
                this.DataBase.GenerateEntities(nameSpace);
                return true;
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
            return this.DataBase.GetDbParameter(name, value);
        }

        /// <summary>
        /// 参数化前缀
        /// </summary>
        public string ParameterPrefix
        {
            get { return this.DataBase.ParameterPrefix; }
        }
    }
}
