using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM
{

    /// <summary>
    /// 查询类型
    /// </summary>
    public enum SelectType
    {
        /// <summary>
        /// 唯一
        /// </summary>
        Distinct,
        /// <summary>
        /// 最大
        /// </summary>
        Max,
        /// <summary>
        /// 最小
        /// </summary>
        Min,
        /// <summary>
        /// 数量
        /// </summary>
        Count
    }
}
