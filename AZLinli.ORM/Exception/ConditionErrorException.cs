using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM
{
    /// <summary>
    /// 查询条件与属性名不一致异常
    /// </summary>
    public class ConditionErrorException : Exception
    {
         public ConditionErrorException()
            : base("查询条件与实体属性名不一致")
        {
        }

         public ConditionErrorException(string message)
            : base(message)
        {
        }
    }
}
