using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Transactions;

namespace AZLinli.ORM.Transaction
{
    /// <summary>
    /// 事务接口
    /// </summary>
    public interface ITransaction
    {
        /// <summary>
        /// 开启事务
        /// </summary>
        void BeginTransaction();

        void BeginTransaction(IsolationLevel level);

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns></returns>
        void Commit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void Rollback();
    }
}
