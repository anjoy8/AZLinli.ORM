using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AZLinli.ORM.DataAccess
{
    public interface IBaseDao
    {
        AZLinContext DbContext { get; set; }
    }
}
