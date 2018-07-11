using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AZLinli.ORM.DataAccess
{
    public interface IDbContext: IDisposable
    {
        IDatabase DataBase { get; set; }
    }
}
