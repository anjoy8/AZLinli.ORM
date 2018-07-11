using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess.Dialect
{
    public interface ISqlDialect
    {
        string ParseTimeFormat(string clrFormat);

        string DateTimeToChar();

        string ToNumber();

        string ToChar();

        string ToDateTime();

        string Contains();

        string EndsWith();

        string StartsWith();

        string IndexOf();

        string SelectIdentity();

        string DateDiff(DateDiffType type);
    }
}
