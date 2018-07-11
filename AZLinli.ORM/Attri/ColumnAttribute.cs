using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        {

        }
        public ColumnAttribute(string columnName)
        {
            this.Name = columnName;
        }

        public string Name { get; set; }
        
    }
}
