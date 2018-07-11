using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess
{
    public class SqlConstructor
    {
        public SqlConstructor()
        {
            Sql = new StringBuilder();
            Parameters = new List<DbParameter>();
            HasSql = false;
        }
        public StringBuilder Sql { get; set; }

        public List<DbParameter> Parameters { get; set; }

        public bool HasSql { get; set; }

    }
}
