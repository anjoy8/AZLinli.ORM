using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using AZLinli.ORM.Cache;
using AZLinli.ORM.DataAccess;

namespace AZLinli.ORM.DataAccess
{
    public sealed class DaoFactory
    {
        private static T LoadDao<T>(Assembly assembly, string className)
        {
            try
            {
                object obj = Caches.DaoCache.Get(className);
                if (obj == null)
                {

                    T dao = (T)assembly.CreateInstance(className);
                    Caches.DaoCache.Insert(className, dao);
                    return dao;
                }

                return (T)obj;
            }
            catch
            {
                throw new Exception(string.Format("DAO对象[{0}]创建失败！", className));
            }
        }


        public static T CreateDao<T>()
           where T : class, IBaseDao
        {
            string typeName = typeof(T).FullName;
            string daoClassName = String.Empty;

            // DAO类名
            daoClassName = typeName;

            // 得到DAO对象
            T dao = LoadDao<T>(typeof(T).Assembly, daoClassName) as T;

            // 第一次加载初始化读写数据库
            if (dao.DbContext == null)
            {
                dao.DbContext = new AZLinContext();
            }
            return dao;
        }

        public static T CreateDao<T>(ConnectionStringSettings settings)
          where T : class, IBaseDao
        {
            string typeName = typeof(T).FullName;
            string daoClassName = String.Empty;

            // DAO类名
            daoClassName = typeName;

            // 得到DAO对
            T dao = LoadDao<T>(typeof(T).Assembly, daoClassName) as T;

            // 第一次加载初始化读写数据库
            if (dao.DbContext == null || dao.DbContext.DataBase.ConnectionString != settings.ConnectionString)
            {
                dao.DbContext = new AZLinContext(settings);
            }
            return dao;
        }

    }
}
