using AZLinli.ORM;
using AZLinli.ORM.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DEMO
{
    class Program
    {
        static void Main(string[] args)
        {
            AZLinContext db = new AZLinContext();
            StringBuilder sql = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();

            //sql.AppendFormat(@"SELECT {0} from User WHERE ", "*");
            //sql.Append(" uID>@uID ");
            //parameters.Add(db.GetDbParameter("uID", 0));

            //PageFilter pageFilter = new PageFilter { PageIndex = 1, PageSize = 5 };
            //pageFilter.OrderText = "uID ASC";
            //PageDataSource<User> books = db.GetPage<User>(sql.ToString(), pageFilter, parameters.ToArray());


            //Model
            var model = db.GetSingle<User>(m => m.uID == 3);
            if (model!=null)
            {
                var item = model;
                Console.WriteLine($"name:uID,value:{item.uID} \t name:uLoginName,value:{item.uLoginName} \t name:uLoginPWD,value:{item.uLoginPWD} \t name:uRealName,value:{item.uRealName} \t name:uUpdateTime,value:{item.uUpdateTime} \t name:uCreateTime,value:{item.uCreateTime}");
            }

            //Select
            List<User> bks = db.GetList<User>(m => true, q => q.OrderBy(m => m.uID));
            foreach (var item in bks)
            {
                Console.WriteLine($"name:uID,value:{item.uID} \t name:uLoginName,value:{item.uLoginName} \t name:uLoginPWD,value:{item.uLoginPWD} \t name:uRealName,value:{item.uRealName} \t name:uUpdateTime,value:{item.uUpdateTime} \t name:uCreateTime,value:{item.uCreateTime}");
             
            }

            //Update
            var author = db.GetSingle<User>(m => true, q => q.OrderBy(m => m.uID));
            if (author != null)
            {
                author.uLoginName = "jim";
                var effect = db.Update(author);
            }

            //ADD
            User user = new User()
            {
                uLoginName="zhagnsan"+Guid.NewGuid().ToString(),
                uLoginPWD="ddddd154sdfsf",
                uRealName="no tell uuuuu",
                uStatus=1,
                uRemark="",
                uCreateTime=DateTime.Now,
                uUpdateTime=DateTime.Now,
            };
            var data = db.Add(user);

            Console.WriteLine($"name:uID,value:{data}");

            Console.ReadKey();
        }
    }
}
