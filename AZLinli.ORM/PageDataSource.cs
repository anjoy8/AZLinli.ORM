using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AZLinli.ORM
{
    /// <summary>
    /// 分页数据源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageDataSource<T>
    {
        public PageDataSource()
        {
            this.PageIndex = 0;
            this.PageSize = 20;
        }

        public PageDataSource(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        /// <summary>
        /// 所有记录的数量（输出参数）
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 所有页面的数量（输出参数）
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// page index
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// data source
        /// </summary>
        public List<T> DataSource { get; set; }
    }
}
