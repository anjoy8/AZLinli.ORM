using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM
{
    /// <summary>
    /// 实体缺失主键特性异常
    /// </summary>
    public class LambdaLossException : Exception
    {
        public LambdaLossException()
            : base("Lambda表达式为null")
        {
        }

        public LambdaLossException(string message)
            : base(message)
        {
        }
    }
}
