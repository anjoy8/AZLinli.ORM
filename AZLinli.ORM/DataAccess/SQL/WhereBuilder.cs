using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM
{
    public class WhereBuilder<T>
    {
        public Expression<Func<T, bool>> WhereExpression { get; set; }

        /// <summary>
        /// Add表达式条件
        /// </summary>
        public void And(Expression<Func<T, bool>> value)
        {
            if (WhereExpression != null)
            {
                Expression<Func<T, bool>> exp = (Expression<Func<T, bool>>)WhereExpression;
                WhereExpression = exp.And<T>(value);
            }
            else
            {
                WhereExpression = value;
            }
        }

        /// <summary>
        /// Or表达式条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Or(Expression<Func<T, bool>> value)
        {
            if (WhereExpression != null)
            {
                Expression<Func<T, bool>> exp = (Expression<Func<T, bool>>)WhereExpression;
                WhereExpression = exp.Or<T>(value);
            }
            else
            {
                WhereExpression = value;
            }
        }
    }
}
