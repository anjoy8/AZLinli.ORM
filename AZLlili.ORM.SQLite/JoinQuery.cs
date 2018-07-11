using AZLinli.ORM.Cache;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace AZLinli.ORM.DataAccess
{
    /// <summary>
    /// 连接查询辅助类
    /// Author AZLinli.COM
    /// </summary>
    public class JoinQuery
    {

        private int callIndex = 0;
        /// <summary>
        /// SQL
        /// </summary>
        protected StringBuilder SqlBuilder { get; set; }

        /// <summary>
        /// 连接条件
        /// </summary>
        protected List<JoinRelation> JoinRelations { get; set; }

        /// <summary>
        /// where条件
        /// 最后拼接SQL的时候要Trim掉'AND'
        /// </summary>
        protected StringBuilder WhereClause { get; set; }

        /// <summary>
        /// 条件参数
        /// </summary>
        protected List<DbParameter> Parameters { get; set; }

        /// <summary>
        /// 排序
        /// 最后拼接SQL的时候要Trim掉','
        /// </summary>
        protected StringBuilder OrderByFields { get; set; }

        /// <summary>
        /// 分组
        /// 最后拼接SQL的时候要Trim掉','
        /// </summary>
        protected StringBuilder GroupByFields { get; set; }

        protected StringBuilder SelectFields { get; set; }

        protected AZLinContext DbContext { get; set; }

        protected bool WithAlias { get; set; }

        /// <summary>
        /// 是否自动Dispose DbContext
        /// </summary>
        public bool IsAutoDisposeDbContext { get; set; }

        public JoinQuery()
        {
            SqlBuilder = new StringBuilder();
            JoinRelations = new List<JoinRelation>();
            WhereClause = new StringBuilder();
            Parameters = new List<DbParameter>();
            OrderByFields = new StringBuilder();
            GroupByFields = new StringBuilder();
            SelectFields = new StringBuilder();
        }

        private void Clear()
        {
            SqlBuilder.Clear();
            JoinRelations.Clear();
            WhereClause.Clear();
            Parameters.Clear();
            OrderByFields.Clear();
            GroupByFields.Clear();
            SelectFields.Clear();
        }

        public JoinQuery(AZLinContext db) : this()
        {
            DbContext = db;
        }

        public JoinQuery(AZLinContext db, bool withAlias) : this(db)
        {
            this.WithAlias = withAlias;
        }

        /// <summary>
        /// 左外连接 参数决定顺序
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="on"></param>
        /// <returns></returns>
        public JoinQuery LeftJoin<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> on) where TLeft : class, new() where TRight : class, new()
        {
            JoinRelation link = new JoinRelation { JoinType = JoinType.Outer };
            SqlVisitor visitor = new SqlVisitor(DbContext.DataBase.DBType, callIndex++, WithAlias);
            link.LeftTable = Caches.EntityInfoCache.Get(typeof(TLeft)).TableName;
            if (WithAlias)
            {
                link.LeftTableAlias = on.Parameters[0].Name;
            }
            link.RightTable = Caches.EntityInfoCache.Get(typeof(TRight)).TableName;
            if (WithAlias)
            {
                link.RightTableAlias = on.Parameters[1].Name;
            }
            link.OnSql = visitor.Translate(on);
            Parameters.AddRange(visitor.Parameters);
            JoinRelations.Add(link);
            return this;
        }

        /// <summary>
        /// 内连接 参数决定顺序
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="on"></param>
        /// <returns></returns>
        public JoinQuery InnerJoin<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> on) where TLeft : class, new() where TRight : class, new()
        {
            JoinRelation link = new JoinRelation { JoinType = JoinType.Inner };
            SqlVisitor visitor = new SqlVisitor(DbContext.DataBase.DBType, callIndex++, WithAlias);
            link.LeftTable = Caches.EntityInfoCache.Get(typeof(TLeft)).TableName;
            if (WithAlias)
            {
                link.LeftTableAlias = on.Parameters[0].Name;
            }
            link.RightTable = Caches.EntityInfoCache.Get(typeof(TRight)).TableName;
            if (WithAlias)
            {
                link.RightTableAlias = on.Parameters[1].Name;
            }
            link.OnSql = visitor.Translate(on);
            Parameters.AddRange(visitor.Parameters);
            JoinRelations.Add(link);
            return this;
        }

        /// <summary>
        /// 添加where查询条件
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public JoinQuery Where<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class, new()
        {
            SqlVisitor visitor = new SqlVisitor(DbContext.DataBase.DBType, callIndex++, WithAlias);
            visitor.Translate(where);
            WhereClause.AppendFormat("{0} AND ", visitor.SqlBuilder.ToString());
            Parameters.AddRange(visitor.Parameters);
            return this;
        }

        public JoinQuery Where<T1, T2>(Expression<Func<T1, T2, bool>> where) where T1 : class, new() where T2 : class, new()
        {
            SqlVisitor visitor = new SqlVisitor(DbContext.DataBase.DBType, callIndex++, WithAlias);
            visitor.Translate(where);
            WhereClause.AppendFormat("{0} AND ", visitor.SqlBuilder.ToString());
            Parameters.AddRange(visitor.Parameters);
            return this;
        }

        public JoinQuery Where<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> where) where T1 : class, new() where T2 : class, new() where T3 : class, new()
        {
            SqlVisitor visitor = new SqlVisitor(DbContext.DataBase.DBType, callIndex++, WithAlias);
            visitor.Translate(where);
            WhereClause.AppendFormat("{0} AND ", visitor.SqlBuilder.ToString()); ;
            Parameters.AddRange(visitor.Parameters);
            return this;
        }

        /// <summary>
        /// 添加升序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public JoinQuery OrderByAsc<TEntity>(Expression<Func<TEntity, dynamic>> orderBy)
        {
            DynamicVisitor visitor = new DynamicVisitor(WithAlias);
            visitor.Translate(orderBy);
            foreach (DynamicMember c in visitor.DynamicMembers)
            {
                OrderByFields.AppendFormat("{0} ASC,", c.Field);
            }
            return this;
        }

        /// <summary>
        /// 添加降序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public JoinQuery OrderByDesc<TEntity>(Expression<Func<TEntity, dynamic>> orderBy)
        {
            DynamicVisitor visitor = new DynamicVisitor(WithAlias);
            visitor.Translate(orderBy);
            foreach (DynamicMember c in visitor.DynamicMembers)
            {
                OrderByFields.AppendFormat("{0} DESC,", c.Field);
            }
            return this;
        }

        /// <summary>
        /// 添加查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public JoinQuery Fields<TEntity>(Expression<Func<TEntity, dynamic>> select = null)
        {
            EntityInfo entityInfo = Caches.EntityInfoCache.Get(typeof(TEntity));
            if (select == null)
            {
                SelectFields.AppendFormat("{0},", entityInfo.SelectFields);
            }
            else
            {
                SqlVisitor visitor = new SqlVisitor(DbContext.DataBase.DBType, callIndex++, WithAlias);
                string fields = visitor.Translate(select);
                SelectFields.Append(fields);
                Parameters.AddRange(visitor.Parameters);
            }
            return this;
        }

        public JoinQuery Fields<T1, T2>(Expression<Func<T1, T2, dynamic>> select = null)
        {
            EntityInfo e1 = Caches.EntityInfoCache.Get(typeof(T1));
            EntityInfo e2 = Caches.EntityInfoCache.Get(typeof(T2));
            if (select == null)
            {
                SelectFields.AppendFormat("{0},{1},", e1.SelectFields, e2.SelectFields);
            }
            else
            {
                SqlVisitor visitor = new SqlVisitor(DbContext.DataBase.DBType, callIndex++, WithAlias);
                string fields = visitor.Translate(select);
                SelectFields.Append(fields);
                Parameters.AddRange(visitor.Parameters);
            }
            return this;
        }

        public JoinQuery Fields<T1, T2, T3>(Expression<Func<T1, T2, T3, dynamic>> select = null)
        {
            EntityInfo e1 = Caches.EntityInfoCache.Get(typeof(T1));
            EntityInfo e2 = Caches.EntityInfoCache.Get(typeof(T2));
            EntityInfo e3 = Caches.EntityInfoCache.Get(typeof(T3));
            if (select == null)
            {
                SelectFields.AppendFormat("{0},{1},{2},", e1.SelectFields, e2.SelectFields, e3.SelectFields);
            }
            else
            {
                SqlVisitor visitor = new SqlVisitor(DbContext.DataBase.DBType, callIndex++, WithAlias);
                string fields = visitor.Translate(select);
                SelectFields.Append(fields);
                Parameters.AddRange(visitor.Parameters);
            }
            return this;
        }

        public JoinQuery GroupBy<TEntity>(Expression<Func<TEntity, dynamic>> select = null)
        {
            DynamicVisitor visitor = new DynamicVisitor(WithAlias);
            visitor.Translate(select);
            foreach (DynamicMember c in visitor.DynamicMembers)
            {
                GroupByFields.AppendFormat("{0},", c.Field);
            }
            return this;
        }

        public JoinQuery GroupBy<T1, T2>(Expression<Func<T1, T2, dynamic>> select = null)
        {
            DynamicVisitor visitor = new DynamicVisitor(WithAlias);
            visitor.Translate(select);
            foreach (DynamicMember c in visitor.DynamicMembers)
            {
                GroupByFields.AppendFormat("{0},", c.Field);
            }
            return this;
        }

        public JoinQuery GroupBy<T1, T2, T3>(Expression<Func<T1, T2, T3, dynamic>> select = null)
        {
            DynamicVisitor visitor = new DynamicVisitor(WithAlias);
            visitor.Translate(select);
            foreach (DynamicMember c in visitor.DynamicMembers)
            {
                GroupByFields.AppendFormat("{0},", c.Field);
            }
            return this;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public List<TModel> GetList<TModel>()
        {
            List<JoinRelation> tables = new List<JoinRelation>();
            SqlBuilder.AppendFormat(" SELECT {0} FROM {1}", SelectFields.ToString().TrimEnd(','), JoinRelations[0].LeftTable);
            if (WithAlias)
            {
                SqlBuilder.AppendFormat(" AS {0}", JoinRelations[0].LeftTableAlias);
            }
            foreach (JoinRelation j in JoinRelations)
            {
                if (j.JoinType == JoinType.Outer)
                {
                    SqlBuilder.Append(" LEFT OUTER JOIN ");
                }
                else
                {
                    SqlBuilder.Append(" INNER JOIN ");
                }
                if (WithAlias)
                {
                    if (tables.Count(m => m.LeftTableAlias == j.RightTableAlias || m.RightTableAlias == j.RightTableAlias) == 0)
                    {
                        SqlBuilder.AppendFormat("{0} AS {1}", j.RightTable, j.RightTableAlias);
                        tables.Add(j);
                    }
                    else
                    {
                        SqlBuilder.AppendFormat("{0} AS {1}", j.LeftTable, j.LeftTableAlias);
                        tables.Add(j);
                    }
                }
                else
                {
                    if (tables.Count(m => m.LeftTable == j.RightTable || m.RightTable == j.RightTable) == 0)
                    {
                        SqlBuilder.Append(j.RightTable);
                        tables.Add(j);
                    }
                    else
                    {
                        SqlBuilder.Append(j.LeftTable);
                        tables.Add(j);
                    }
                }
                SqlBuilder.AppendFormat(" ON {0}", j.OnSql.TrimEnd("AND".ToCharArray()));
            }
            if (WhereClause.Length > 0)
            {
                SqlBuilder.AppendFormat(" WHERE {0}", WhereClause.ToString().Trim().TrimEnd("AND".ToCharArray()));
            }
            if (GroupByFields.Length > 0)
            {
                SqlBuilder.AppendFormat(" GROUP BY {0}", GroupByFields.ToString().Trim().TrimEnd(','));
            }
            if (OrderByFields.Length > 0)
            {
                SqlBuilder.AppendFormat(" ORDER BY {0}", OrderByFields.ToString().Trim().TrimEnd(','));
            }
            //开始组装sql
            List<TModel> result = DbContext.GetList<TModel>(SqlBuilder.ToString(), Parameters.ToArray());
            if (this.IsAutoDisposeDbContext)
            {
                DbContext.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 返回数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            List<JoinRelation> tables = new List<JoinRelation>();
            SqlBuilder.AppendFormat(" SELECT COUNT(1) AS CT FROM {0}", JoinRelations[0].LeftTable);
            if (WithAlias)
            {
                SqlBuilder.AppendFormat(" AS {0}", JoinRelations[0].LeftTableAlias);
            }
            foreach (JoinRelation j in JoinRelations)
            {
                if (j.JoinType == JoinType.Outer)
                {
                    SqlBuilder.Append(" LEFT OUTER JOIN ");
                }
                else
                {
                    SqlBuilder.Append(" INNER JOIN ");
                }
                if (WithAlias)
                {
                    if (tables.Count(m => m.LeftTableAlias == j.RightTableAlias || m.RightTableAlias == j.RightTableAlias) == 0)
                    {
                        SqlBuilder.AppendFormat("{0} AS {1}", j.RightTable, j.RightTableAlias);
                        tables.Add(j);
                    }
                    else
                    {
                        SqlBuilder.AppendFormat("{0} AS {1}", j.LeftTable, j.LeftTableAlias);
                        tables.Add(j);
                    }
                }
                else
                {
                    if (tables.Count(m => m.LeftTable == j.RightTable || m.RightTable == j.RightTable) == 0)
                    {
                        SqlBuilder.Append(j.RightTable);
                        tables.Add(j);
                    }
                    else
                    {
                        SqlBuilder.Append(j.LeftTable);
                        tables.Add(j);
                    }
                }
                SqlBuilder.AppendFormat(" ON {0}", j.OnSql.TrimEnd("AND".ToCharArray()));
            }
            if (WhereClause.Length > 0)
            {
                SqlBuilder.AppendFormat(" WHERE {0}", WhereClause.ToString().Trim().TrimEnd("AND".ToCharArray()));
            }
            if (GroupByFields.Length > 0)
            {
                SqlBuilder.AppendFormat(" GROUP BY {0}", GroupByFields.ToString().Trim().TrimEnd(','));
            }
            if (OrderByFields.Length > 0)
            {
                SqlBuilder.AppendFormat(" ORDER BY {0}", OrderByFields.ToString().Trim().TrimEnd(','));
            }
            //开始组装sql
            int result = DbContext.GetSingle<int>(SqlBuilder.ToString(), Parameters.ToArray());
            if (this.IsAutoDisposeDbContext)
            {
                DbContext.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 多表连查分页
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public PageDataSource<TModel> GetPage<TModel>(PageFilter page)
        {
            List<JoinRelation> tables = new List<JoinRelation>();
            SqlBuilder.AppendFormat(" SELECT {0} FROM {1}", SelectFields.ToString().TrimEnd(','), JoinRelations[0].LeftTable);
            if (WithAlias)
            {
                SqlBuilder.AppendFormat(" AS {0}", JoinRelations[0].LeftTableAlias);
            }
            foreach (JoinRelation j in JoinRelations)
            {
                if (j.JoinType == JoinType.Outer)
                {
                    SqlBuilder.Append(" LEFT OUTER JOIN ");
                }
                else
                {
                    SqlBuilder.Append(" INNER JOIN ");
                }
                if (WithAlias)
                {
                    if (tables.Count(m => m.LeftTableAlias == j.RightTableAlias || m.RightTableAlias == j.RightTableAlias) == 0)
                    {
                        SqlBuilder.AppendFormat("{0} AS {1}", j.RightTable, j.RightTableAlias);
                        tables.Add(j);
                    }
                    else
                    {
                        SqlBuilder.AppendFormat("{0} AS {1}", j.LeftTable, j.LeftTableAlias);
                        tables.Add(j);
                    }
                }
                else
                {
                    if (tables.Count(m => m.LeftTable == j.RightTable || m.RightTable == j.RightTable) == 0)
                    {
                        SqlBuilder.Append(j.RightTable);
                        tables.Add(j);
                    }
                    else
                    {
                        SqlBuilder.Append(j.LeftTable);
                        tables.Add(j);
                    }
                }
                SqlBuilder.AppendFormat(" ON {0}", j.OnSql.TrimEnd("AND".ToCharArray()));
            }
            if (WhereClause.Length > 0)
            {
                SqlBuilder.AppendFormat(" WHERE {0}", WhereClause.ToString().Trim().TrimEnd("AND".ToCharArray()));
            }
            if (GroupByFields.Length > 0)
            {
                SqlBuilder.AppendFormat(" GROUP BY {0}", GroupByFields.ToString().Trim().TrimEnd(','));
            }
            if (OrderByFields.Length > 0)
            {
                page.OrderText = OrderByFields.ToString().Trim().TrimEnd(',');
            }
            //开始组装sql
            //开始组装sql
            PageDataSource<TModel> result = DbContext.GetPage<TModel>(SqlBuilder.ToString(), page, Parameters.ToArray());
            if (this.IsAutoDisposeDbContext)
            {
                DbContext.Dispose();
            }
            return result;
        }
    }
}
