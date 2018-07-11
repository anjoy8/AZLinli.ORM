using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess.Dialect
{
    public class SQLiteDialect : ISqlDialect
    {
        public string Contains()
        {
            return " LIKE ('%'||{0}||'%')";
        }

        public string DateTimeToChar()
        {
           return " strftime({0},'{1}','localtime') ";
        }

        public string EndsWith()
        {
            return " LIKE ('%'||{0})";
        }

        public string IndexOf()
        {
            throw new NotImplementedException("SQLite 不支持IndexOf等价的函数");
        }

        public string ParseTimeFormat(string clrFormat)
        {
            clrFormat = clrFormat
                   .Replace("yyyy", "%Y")
                   .Replace("MM", "%m")
                   .Replace("dd", "%d")
                   .Replace("HH", "%H")
                   .Replace("mm", "%M")
                   .Replace("ss", "%S");
            return clrFormat;
        }

        public string SelectIdentity()
        {
            return "; SELECT LAST_INSERT_ROWID()";
        }

        public string StartsWith()
        {
            return " LIKE ({0}||'%')";
        }

        public string ToChar()
        {
            return " CAST({0} AS CHAR) ";
        }

        public string ToDateTime()
        {
            return " DATETIME({0}) ";
        }

        public string ToNumber()
        {
            return " CAST({0} AS INT) ";
        }

        public string DateDiff(DateDiffType type)
        {
            switch (type)
            {
                case DateDiffType.Day:
                    return " DATEDIFF({1},{0}) ";
                case DateDiffType.Hour:
                case DateDiffType.Minute:
                default:
                    throw new NotImplementedException("SQLite DateDiff函数不支持Hour Minute此格式");
            }

        }
    }
}
