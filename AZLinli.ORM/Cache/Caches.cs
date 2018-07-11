using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.Cache
{
    /// <summary>
    /// 缓存容器
    /// </summary>
    public  class Caches
    {

        private static object _lock = new object();

        private static DaoCache daoCache;

        /// <summary>
        /// Dao对象缓存
        /// </summary>
        public static DaoCache DaoCache
        {
            get
            {
                if (daoCache == null)
                {
                    lock (_lock)
                    {
                        if (daoCache == null)
                        {
                            daoCache = new DaoCache();
                        }
                    }
                }

                return daoCache;
            }
        }

        private static EntityInfoCache entityInfoCache;

        /// <summary>
        /// 实体反射缓存
        /// </summary>
        public static EntityInfoCache EntityInfoCache
        {
            get
            {
                if (entityInfoCache == null)
                {
                    lock (_lock)
                    {
                        if (entityInfoCache == null)
                        {
                            entityInfoCache = new EntityInfoCache();
                        }
                    }
                }

                return entityInfoCache;
            }
        }

        private static PropertyAccessorCache propertyAccessorCache;

        /// <summary>
        /// 实体反射缓存
        /// </summary>
        public static PropertyAccessorCache PropertyAccessorCache
        {
            get
            {
                if (propertyAccessorCache == null)
                {
                    lock (_lock)
                    {
                        if (propertyAccessorCache == null)
                        {
                            propertyAccessorCache = new PropertyAccessorCache();
                        }
                    }
                }

                return propertyAccessorCache;
            }
        }

        private static ConstructorCache constructorCache;

        /// <summary>
        /// 构造函数
        /// </summary>
        public static ConstructorCache ConstructorCache
        {
            get
            {
                if (constructorCache == null)
                {
                    lock (_lock)
                    {
                        if (constructorCache == null)
                        {
                            constructorCache = new ConstructorCache();
                        }
                    }
                }

                return constructorCache;
            }
        }
      
    }
}
