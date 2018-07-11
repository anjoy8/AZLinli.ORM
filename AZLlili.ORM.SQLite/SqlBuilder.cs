using AZLinli.ORM.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess
{
    public class SqlBuilder
    {
        /// <summary>
        /// 获取查询字段
        /// Author AZLinli.COM
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetSelectAllFields<T>()
        {
            string selectFields = Caches.EntityInfoCache.Get(typeof(T)).SelectFields;
            return selectFields;
        }

        /// <summary>
        /// 获取查询字段带别名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableAlias"></param>
        /// <returns></returns>
        public static string GetSelectAllFields<T>(string tableAlias)
        {
            var entityInfo = Caches.EntityInfoCache.Get(typeof(T));
            string selectFields = Caches.EntityInfoCache.Get(typeof(T)).SelectFields;
            selectFields = selectFields.Replace(entityInfo.TableName + ".",tableAlias + ".");
            return selectFields;
        }

        public static string GetTableName(Type tableType)
        {
            var entityInfo = Caches.EntityInfoCache.Get(tableType);
            return entityInfo.TableName;
        }

        public static string GetFieldName(Type tableType, string memberName)
        {
            var entityInfo = Caches.EntityInfoCache.Get(tableType);
            if (entityInfo.Columns.Keys.Contains(memberName))
            {
                return entityInfo.Columns[memberName];
            }
            else
            {
                return memberName;
            }
        }

        /// <summary>
        /// 获取in的sql
        /// </summary>
        /// <param name="tableNameOrAlias"></param>
        /// <param name="columnName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static SqlConstructor GetInSql(string tableNameOrAlias, string columnName, IEnumerable<int> values)
        {
            SqlConstructor result = new SqlConstructor();
            if (values != null && values.Any())
            {
                result.Sql.AppendFormat(" {0}.{1} IN (", tableNameOrAlias, columnName);
                int i = 100;
                foreach (var v in values)
                {
                    i++;
                    result.Sql.AppendFormat("@{0}{1},", columnName, i);
                    result.Parameters.Add(DbParameterFactory.CreateDbParameter(columnName + i.ToString(), v));
                }
                result.Sql.Remove(result.Sql.Length - 1, 1);
                result.Sql.Append(") ");
                result.HasSql = true;
            }
            return result;
        }

        /// <summary>
        /// 获取in的sql
        /// </summary>
        /// <param name="tableNameOrAlias"></param>
        /// <param name="columnName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static SqlConstructor GetInSql(string tableNameOrAlias, string columnName, IEnumerable<string> values)
        {
            SqlConstructor result = new SqlConstructor();
            if (values != null && values.Any())
            {
                result.Sql.AppendFormat(" {0}.{1} IN (", tableNameOrAlias, columnName);
                int i = 100;
                foreach (var v in values)
                {
                    i++;
                    result.Sql.AppendFormat("@{0}{1},", columnName, i);
                    result.Parameters.Add(DbParameterFactory.CreateDbParameter(columnName + i.ToString(), v));
                }
                result.Sql.Remove(result.Sql.Length - 1, 1);
                result.Sql.Append(") ");
                result.HasSql = true;
            }
            return result;
        }

    }
}
