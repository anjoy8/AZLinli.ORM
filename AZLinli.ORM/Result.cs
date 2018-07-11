using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AZLinli.ORM
{
    public enum Flag
    {
        Warning,
        OK,
        Error
    }

    /// <summary>
    /// 操作结果
    /// </summary>
    public class Result  
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Result()
        {
            this.Flag = Flag.OK;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bFlag"></param>
        public Result(Flag bFlag)
        {
            this.Flag = bFlag;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bFlag"></param>
        /// <param name="sMessage"></param>
        public Result(Flag bFlag, string sMessage)
        {
            this.Flag = bFlag;
            this.Message = sMessage;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bFlag"></param>
        /// <param name="sMessage"></param>
        /// <param name="sCode"></param>
        public Result(Flag bFlag, string sMessage, string sCode)
        {
            this.Flag = bFlag;
            this.Message = sMessage;
            this.Code = sCode;
        }

        /// <summary>
        /// 操作编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 操作成功失败标识
        /// </summary>
        public Flag Flag { get; set; }

        /// <summary>
        /// 操作信息
        /// </summary>
        public string Message { get; set; }

    }


    /// <summary>
    /// 查询结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Result()
            : base()
        {
            ResultObj=default(T);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Result(Flag bFlag)
            : base(bFlag)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Result(Flag bFlag, T oObject)
        {
            this.Flag = bFlag;
            this.ResultObj = oObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Result(Flag bFlag, string sMessage)
            : base(bFlag, sMessage)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Result(Flag bFlag, string sMessage, T oObject)
        {
            this.Flag = bFlag;
            this.Message = sMessage;
            this.ResultObj = oObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Result(Flag bFlag, string sMessage, string sCode)
            : base(bFlag, sMessage, sCode)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Result(Flag bFlag, string sMessage, string sCode, T oObject)
        {
            this.Flag = bFlag;
            this.Message = sMessage;
            this.Code = sCode;
            this.ResultObj = oObject;
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        public T ResultObj { get; set; }
    }


    /// <summary>
    /// 具有多个结果集的反馈类型 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Results<T> : Result
    {
        public Results()
            : base()
        {
            this.ResultsObj = new List<Result<T>>();
            FailCount = 0;
            SuccessCount = 0;
        }

        /// <summary>
        /// 失败条目
        /// </summary>
        public int FailCount { get; set; }

        /// <summary>
        /// 成功条目
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 查询结果
        /// </summary>
        public List<Result<T>> ResultsObj { get; set; }

        /// <summary>
        /// 判断是否完全处理正确
        /// </summary>
        /// <param name="successMessage"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        public Results<T> Self(string successMessage, string failMessage)
        {
            if (FailCount == 0)
            {
                this.Message = successMessage;
                this.Flag = Flag.OK;
            }
            else
            {
                this.Message = failMessage;
                this.Flag = Flag.Warning;
            }
            return this;
        }

        /// <summary>
        /// 判断是否完全处理正确
        /// </summary>
        /// <param name="successMessage"></param>
        /// <returns></returns>
        public Results<T> Self(string successMessage)
        {
            if (FailCount == 0)
            {
                this.Message = successMessage;
                this.Flag = Flag.OK;
            }
            else
            {
                this.Flag = Flag.Warning;
            }
            return this;
        }
    }
}
