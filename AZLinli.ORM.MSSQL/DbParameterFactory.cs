
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess
{
    public class DbParameterFactory
    {

        /// <summary>
        /// 创建DbParameter
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(string parameterName, object value)
        {
            try
            {

                return new SqlParameter("@" + parameterName, value);

            }
            catch
            {
                throw new Exception("DbParameter错误！");
            }
        }

        public static string GetParameterPrefix()
        {
            return "@";
        }
    }
}

