using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess.Dialect
{
    public class SqlDialectFactory
    {
        public static ISqlDialect CreateSqlDialect()
        {
            try
            {
                return new SQLiteDialect();
            }
            catch
            {
                throw new Exception("SqlDialect错误！");
            }
        }
    }
}
