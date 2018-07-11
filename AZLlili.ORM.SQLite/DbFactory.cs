using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess
{
    public class DbFactory
    {
        private static string dbProviderName;
        /// <summary>
        /// 数据库类型
        /// </summary>
        private static string DBProviderName
        {
            get
            {
                if (String.IsNullOrEmpty(dbProviderName))
                {
                    string connectionName = ConfigurationManager.AppSettings["connectionName"];
                    if (string.IsNullOrEmpty(connectionName))
                    {
                        throw new Exception("未配置主数据库连接！");
                    }
                    ConnectionStringSettings connSetting = ConfigurationManager.ConnectionStrings[connectionName];
                    dbProviderName = connSetting.ProviderName;
                }
                return dbProviderName;
            }
        }

        private static string dbConnectionString;
        /// <summary>
        /// 主数据库连接字串
        /// </summary>
        private static string DBConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(dbConnectionString))
                {
                    string connectionName = ConfigurationManager.AppSettings["connectionName"];
                    if (string.IsNullOrEmpty(connectionName))
                    {
                        throw new Exception("未配置主数据库连接！");
                    }
                    ConnectionStringSettings connSetting = ConfigurationManager.ConnectionStrings[connectionName];

                    dbConnectionString = connSetting.ConnectionString;
                    dbProviderName = connSetting.ProviderName;
                }
                return dbConnectionString;
            }
        }

        /// <summary>
        /// 构建数据访问组件
        /// </summary>
        /// <returns></returns>
        public static IDatabase CreateDataBase()
        {
            try
            {
                IDatabase database = new SQLiteDataBase(DBConnectionString);
                return database;
            }
            catch(Exception ex)
            {
                throw new Exception("数据库连接字符串错误！");
            }
        }

        public static IDatabase CreateDataBase(ConnectionStringSettings settings)
        {
            try
            {
                IDatabase  database = new SQLiteDataBase(settings.ConnectionString);
                return database;
            }
            catch
            {
                throw new Exception("数据库连接字符串错误！");
            }
        }

    }
}
