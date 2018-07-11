using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AZLinli.ORM.Transaction
{

    /// <summary>
    /// 分布式事务
    /// </summary>
    public class DNetTransaction : ITransaction
    {

        System.Transactions.Transaction originalTransaction = System.Transactions.Transaction.Current;

        CommittableTransaction transaction = null;


        /// <summary>
        /// 开启一个事务
        /// </summary>
        public void BeginTransaction()
        {
            if (originalTransaction == null)
            {
                transaction = new CommittableTransaction();
                System.Transactions.Transaction.Current = transaction;
            }
        }

        public void BeginTransaction(IsolationLevel level)
        {
            if (originalTransaction == null)
            {
                transaction = new CommittableTransaction(new TransactionOptions { IsolationLevel = level });
                System.Transactions.Transaction.Current = transaction;
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns></returns>
        public void Commit()
        {
            if (originalTransaction == null)
            {
                transaction.Commit();
                System.Transactions.Transaction.Current = originalTransaction;
                transaction.Dispose();
            }
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void Rollback()
        {
            if (originalTransaction == null)
            {
                transaction.Rollback();
                System.Transactions.Transaction.Current = originalTransaction;
                transaction.Dispose();
            }
            else
            {
                throw new Exception("分布式事务异常");
            }
        }
    }
}
