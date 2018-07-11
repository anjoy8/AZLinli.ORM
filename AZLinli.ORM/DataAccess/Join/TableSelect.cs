using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess
{
    /// <summary>
    /// Author AZLinli.COM
    /// </summary>
    public class TableSelect
    {
        public TableSelect()
        {
            Fields = new List<DynamicMember>();
        }
        public string Table { get; set; }

        /// <summary>
        /// field AS key
        /// </summary>
        public List<DynamicMember> Fields { get; set; }

        public string GetSelect()
        {
            StringBuilder cb = new StringBuilder();
            foreach (DynamicMember c in Fields)
            {
                if (!string.IsNullOrEmpty(c.Field))
                {
                    cb.Append(c.Field.TrimEnd(','));
                    if (!string.IsNullOrEmpty(c.Key) && c.Key != "*")
                    {
                        cb.Append(" AS ");
                        cb.Append(c.Key);
                    }
                    cb.Append(",");
                }
            }
            return cb.ToString();
        }
    }
}
