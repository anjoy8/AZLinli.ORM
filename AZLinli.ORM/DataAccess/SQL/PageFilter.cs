using AZLinli.ORM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace AZLinli.ORM
{
    /// <summary>
    /// 分页条件类 
    /// OrderText如果不设置则为数据库默认排序
    /// 如果不需要构造where条件 WhereExpression不需要构造
    /// Author: AZLinli.COM
    /// </summary>
    [DataContract]
    public class PageFilter
    {
        private string orderText;
        private int pageIndex;
        private int pageSize;

        public PageFilter()
        {
            this.pageSize = 20;
            this.pageIndex = 1;
            this.orderText = " (SELECT 0) ";
        }

        public PageFilter(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.orderText = " (SELECT 0) ";
        }

        public PageFilter(int pageIndex, int pageSize, string orderText)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.OrderText = orderText;
        }

        /// <summary>
        /// 排序字符串 ID ASC,NAME DESC
        /// </summary>
        public string OrderText
        {
            get
            {
                if (string.IsNullOrEmpty(this.orderText))
                {
                    //throw new Exception("排序条件不能为空！");
                    return " (SELECT 0) ";
                }
                return this.orderText;
            }
            set
            {
                this.orderText = value;
            }
        }

        /// <summary>
        /// 要查询的页码从1开始
        /// </summary>
        [DataMember]
        public int PageIndex
        {
            get
            {
                return this.pageIndex;
            }
            set
            {
                if (value < 1)
                {
                    this.pageIndex = 1;
                }
                else
                {
                    this.pageIndex = value;
                }
            }
        }

        /// <summary>
        /// 每页大小
        /// </summary>
        [DataMember]
        public int PageSize
        {
            get
            {
                return this.pageSize;
            }
            set
            {
                if (value < 1)
                {
                    this.pageSize = 20;
                }
                else
                {
                    this.pageSize = value;
                }
            }
        }
    }

    public class PageFilter<T>:PageFilter
    {
        public Expression WhereExpression { get; set; }

        /// <summary>
        /// 添加AND条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
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
        /// 添加OR条件
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

        /// <summary>
        /// 添加OrderBy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void OrderBy(Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> value)
        {
            if (value != null)
            {
                OrderByVisitor<T> orderByVisitor = new OrderByVisitor<T>();
                OrderText = orderByVisitor.Translate(value);
            }
        }

        /// <summary>
        /// 添加OrderBy 别名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="alias"></param>
        public void OrderBy(Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> value, string alias)
        {
            if (value != null)
            {
                OrderByVisitor<T> orderByVisitor = new OrderByVisitor<T>();
                OrderText = orderByVisitor.Translate(value, alias);
            }
        }

        public PageFilter():base()
        {
        }

        public PageFilter(int pageIndex, int pageSize):base(pageIndex, pageSize)
        {
        }

        public PageFilter(int pageIndex, int pageSize, string orderText):base(pageIndex, pageSize, orderText)
        {
        }
    }
}
