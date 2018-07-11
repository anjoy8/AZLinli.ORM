using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess
{
    public class SqlFunctions
    {
        /// <summary>
        /// COUNT()函数
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static dynamic Count(dynamic member)
        {
            throw new NotImplementedException("SQL解析函数无需实现");
        }

        /// <summary>
        /// COUNT(DISTINCT)函数
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static dynamic CountDistinct(dynamic member)
        {
            throw new NotImplementedException("SQL解析函数无需实现");
        }

        /// <summary>
        /// MAX()函数
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static dynamic Max(dynamic member)
        {
            throw new NotImplementedException("SQL解析函数无需实现");
        }

        /// <summary>
        /// MIN()函数
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static dynamic Min(dynamic member)
        {
            throw new NotImplementedException("SQL解析函数无需实现");
        }

        /// <summary>
        /// AVG()函数
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static dynamic Avg(dynamic member)
        {
            throw new NotImplementedException("SQL解析函数无需实现");
        }

        public static int DateDiff(DateDiffType type, dynamic startTime, dynamic endTime)
        {
            throw new NotImplementedException("SQL解析函数无需实现");
        }
     
    }
}
