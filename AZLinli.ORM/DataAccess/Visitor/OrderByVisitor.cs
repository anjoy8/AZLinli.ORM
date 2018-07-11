using AZLinli.ORM.Cache;
using AZLinli.ORM.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess
{
    public class OrderByVisitor<T> : ExpressionVisitor, IVisitor
    {
        /// <summary>
        /// 反射缓存介质类
        /// </summary>
        protected EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public List<string> OrderByFieldsStack { get; set; }

        public StringBuilder OrderByFields { get; set; }

        public string OrderBy { get; set; }

        /// <summary>
        /// 表的别名
        /// </summary>
        public string Alias { get; set; }

        public OrderByVisitor()
        {
            OrderByFieldsStack = new List<string>();
            OrderByFields = new StringBuilder();
            this.EntityInfo = Caches.EntityInfoCache.Get(typeof(T));
        }

        public string Translate(Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> exp)
        {
            this.Visit(exp.Body);
            return string.Join(",", OrderByFieldsStack);
        }

        public string Translate(Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> exp, string alias)
        {
            Alias = alias;
            this.Visit(exp.Body);
            return string.Join(",", OrderByFieldsStack);
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodExp)
        {
            if (methodExp.Method.Name == "OrderByDescending" || methodExp.Method.Name == "ThenByDescending")
            {
                OrderByFields.Clear();
                OrderBy = "DESC";
                Expression obj = this.Visit(((LambdaExpression)methodExp.Arguments[1]).Body);
                OrderByFieldsStack.Insert(0, OrderByFields.ToString().TrimEnd(','));
                OrderByFields.Clear();
                if (methodExp.Arguments[0].NodeType == ExpressionType.Call)
                {
                    this.Visit(methodExp.Arguments[0]);
                }
                return methodExp;
            }
            else if (methodExp.Method.Name == "OrderBy" || methodExp.Method.Name == "ThenBy")
            {
                OrderByFields.Clear();
                OrderBy = "ASC";
                Expression obj = this.Visit(((LambdaExpression)methodExp.Arguments[1]).Body);
                OrderByFieldsStack.Insert(0, OrderByFields.ToString().TrimEnd(','));
                OrderByFields.Clear();
                if (methodExp.Arguments[0].NodeType == ExpressionType.Call)
                {
                    this.Visit(methodExp.Arguments[0]);
                }
                return methodExp;
            }
            else
            {
                Expression obj = this.Visit(methodExp.Object);
                object result = Expression.Lambda(methodExp).Compile().DynamicInvoke();
                var r = Expression.Constant(result, methodExp.Method.ReturnType);
                this.Visit(r);
                return r;
            }

            throw new NotSupportedException(string.Format("lambda表达式不支持使用此'{0}'方法", methodExp.Method.Name));
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                this.Visit(node.Arguments[i]);
            }
            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = this.Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }
            return assignment;
        }

        protected override Expression VisitMember(MemberExpression memberExp)
        {
            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                var typeName = memberExp.Expression.Type.Name;
                string fieldName = GetFieldName(typeName,memberExp.Member.Name);
                if (!string.IsNullOrEmpty(Alias))
                {
                    fieldName = Alias + "." + GetFieldName(typeName,memberExp.Member.Name);
                }
                OrderByFields.Append(string.Format("{0} {1},", fieldName, OrderBy));
                return memberExp;
            }
            else if (memberExp.NodeType == ExpressionType.MemberAccess)
            {
                object result = Expression.Lambda(memberExp).Compile().DynamicInvoke();
                var r = Expression.Constant(result, result.GetType());
                this.Visit(r);
                return r;
            }
            throw new NotSupportedException(string.Format("成员 '{0}’不支持", memberExp.Member.Name));
        }

        public string GetFieldName(string typeName, string memberName)
        {
            if (this.EntityInfo.Columns.Keys.Contains(memberName))
            {
                return this.EntityInfo.Columns[memberName];
            }
            else
            {
                return memberName;
            }
        }

        public DbParameter GetDbParameter(string parameterName, object value)
        {
            throw new NotImplementedException();
        }

        public string ParameterPrefix
        {
            get { throw new NotImplementedException(); }
        }
    }
}
