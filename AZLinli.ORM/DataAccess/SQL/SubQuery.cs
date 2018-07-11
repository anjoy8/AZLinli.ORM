using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AZLinli.ORM.ORM
{
    /// <summary>
    /// 子查询
    /// </summary>
    public class SubQuery
    {
        /// <summary>
        /// 子查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static int? GetSingle<T>(Expression<Func<T, bool>> exp, Expression<Func<T, int?>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static int GetSingle<T>(Expression<Func<T, bool>> exp, Expression<Func<T, int>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static string GetSingle<T>(Expression<Func<T, bool>> exp, Expression<Func<T, string>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static double GetSingle<T>(Expression<Func<T, bool>> exp, Expression<Func<T, double>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static double? GetSingle<T>(Expression<Func<T, bool>> exp, Expression<Func<T, double?>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static DateTime GetSingle<T>(Expression<Func<T, bool>> exp, Expression<Func<T, DateTime>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static DateTime? GetSingle<T>(Expression<Func<T, bool>> exp, Expression<Func<T, DateTime?>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static TResult GetSingle<T,TResult>(Expression<Func<T, bool>> exp, Expression<Func<T, TResult>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static List<int> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, int>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static List<int?> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, int?>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static List<string> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, string>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static List<double> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, double>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static List<double?> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, double?>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static List<DateTime> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, DateTime>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static List<DateTime?> GetList<T>(Expression<Func<T, bool>> exp, Expression<Func<T, DateTime?>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }

        public static List<TResult> GetList<T,TResult>(Expression<Func<T, bool>> exp, Expression<Func<T, TResult>> select) where T : class, new()
        {
            throw new NotImplementedException("SQL子查询解析函数无需实现");
        }
    }
}
