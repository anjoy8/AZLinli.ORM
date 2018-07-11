using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess.Dialect
{
    public class SqlServerDialect : ISqlDialect
    {
        public string Contains()
        {
            return " LIKE '%'+{0}+'%' ";
        }

        public string DateTimeToChar()
        {
            return " ({0}) ";
        }

        public string EndsWith()
        {
            return " LIKE '%'+{0} ";
        }

        public string IndexOf()
        {
            return " CHARINDEX({0},{1})-1 ";
        }

        public string ParseTimeFormat(string clrFormat)
        {
            clrFormat = clrFormat
                  .Replace("yyyy", "'+CAST(DATEPART(yy,{0}) AS VARCHAR)+'")
                  .Replace("MM", "'+CAST(DATEPART(m,{0}) AS VARCHAR)+'")
                  .Replace("dd", "'+CAST(DATEPART(d,{0}) AS VARCHAR)+'")
                  .Replace("HH", "'+CAST(DATEPART(hh,{0}) AS VARCHAR)+'")
                  .Replace("mm", "'+CAST(DATEPART(mi,{0}) AS VARCHAR)+'")
                  .Replace("ss", "'+CAST(DATEPART(ss,{0}) AS VARCHAR)+'")
                  .TrimStart("'+".ToCharArray())
                  .TrimEnd("+'".ToCharArray());
            return clrFormat;
        }

        public string SelectIdentity()
        {
            return ";SELECT SCOPE_IDENTITY()";
        }

        public string StartsWith()
        {
            return " LIKE {0}+'%' ";
        }

        public string ToChar()
        {
            return " CAST({0} AS VARCHAR) ";
        }

        public string ToDateTime()
        {
            return " CAST({0} AS DATETIME) ";
        }

        public string ToNumber()
        {
            return " CAST({0} AS INT) ";
        }

        public string DateDiff(DateDiffType type)
        {
            switch(type)
            {
                case DateDiffType.Day:
                    return " DATEDIFF(DAY,{0},{1}) ";
                case DateDiffType.Hour:
                    return " DATEDIFF(HOUR,{0},{1}) ";
                case DateDiffType.Minute:
                    return " DATEDIFF(MINUTE,{0},{1}) ";
                default:
                    throw new NotImplementedException("DateDiff函数不支持此格式");
            }
            
        }
    }
}
