using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess
{
    public interface IVisitor
    {
        string ParameterPrefix { get; }

        DbParameter GetDbParameter(string parameterName, object value);

        //string GetFieldName(string typeName,string memberName);

    }
}
