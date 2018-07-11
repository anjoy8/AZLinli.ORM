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
    public class KeyLossException : Exception
    {
        public KeyLossException()
            : base("实体主键特性没有声明")
        {
        }

        public KeyLossException(string message)
            : base(message)
        {
        }
    }
}
