using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace AZLinli.ORM.Cache
{
    public class DaoCache : BaseCache<string,object>
    {
       
        /// <summary>
        /// 当缓存没有时从外部加载数据 true要重写LoadData
        /// </summary>
        public override bool IsGetExternalData
        {
            get
            {
                return false;
            }
        }

        public override bool IsExpireRemove
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 重写过期时间
        /// </summary>
        public override int ExpireTime
        {
            get
            {
                return 60 * 60 * 1000;
            }
        }

        public override int PeriodTime
        {
            get
            {
                return 60 * 30 * 1000;
            }
        }
    }
}
