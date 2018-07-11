using AZLinli.ORM.Cache;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace AZLinli.ORM.DataAccess
{
    /// <summary>
    /// Lambda转动态对象
    /// Author AZLinli.COM
    /// </summary>
    public class DynamicVisitor : ExpressionVisitor, IVisitor
    {
        protected bool WithAlias { get; set; }

        /// <summary>
        /// 动态属性
        /// </summary>
        public List<DynamicMember> DynamicMembers { get; set; }

        public DynamicVisitor()
        {
            DynamicMembers = new List<DynamicMember>();
        }

        public DynamicVisitor(bool withAlias):this()
        {
            WithAlias = withAlias;
        }

        public void Translate(Expression exp)
        {
            this.DynamicMembers.Clear();
            if (exp != null)
            {
                this.Visit(((LambdaExpression)exp).Body);
            }
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var entityInfo = Caches.EntityInfoCache.Get(node.Type);
            foreach (string key in entityInfo.Columns.Keys)
            {
                if (WithAlias)
                {
                    DynamicMembers.Add(new DynamicMember { Key = key, Field = node.Name + "." + entityInfo.Columns[key] });
                }
                else
                {
                    DynamicMembers.Add(new DynamicMember { Key = key, Field = entityInfo.TableName + "." + entityInfo.Columns[key] });
                }
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                if (DynamicMembers.Count(m => m.Key == node.Members[i].Name) == 0)
                {
                    DynamicMembers.Add(new DynamicMember { Key = node.Members[i].Name });
                }
                this.Visit(node.Arguments[i]);

            }
            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            if (DynamicMembers.Count(m => m.Key == assignment.Member.Name) == 0)
            {
                DynamicMembers.Add(new DynamicMember { Key = assignment.Member.Name });
            }
            Expression e = this.Visit(assignment.Expression);
            return assignment;
        }

        protected override Expression VisitMember(MemberExpression memberExp)
        {
            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                string alias = string.Empty;
                if (WithAlias)
                {
                    alias = ((ParameterExpression)(memberExp.Expression)).Name;
                }
                else
                {
                    var entityInfo = Caches.EntityInfoCache.Get(memberExp.Expression.Type);
                    alias = entityInfo.TableName;
                }
                string fieldName = alias + "." + GetFieldName(memberExp.Expression.Type, memberExp.Member.Name);
                if (DynamicMembers.Count > 0)
                {
                    DynamicMembers.Last().Field = fieldName;
                }
                else
                {
                    DynamicMembers.Add(new DynamicMember { Key = memberExp.Member.Name, Field = fieldName });
                }
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


        protected override Expression VisitMethodCall(MethodCallExpression methodExp)
        {
            if (methodExp.Object != null && methodExp.Object.NodeType == ExpressionType.MemberAccess)
            {
                Expression obj = this.Visit(methodExp.Object);
                return obj;
            }
            else if (methodExp.Arguments.Count > 0)
            {
                for (int i = 0; i < methodExp.Arguments.Count; i++)
                {
                    Expression obj = this.Visit(methodExp.Arguments[i]);
                    return obj;
                }
            }
            throw new NotSupportedException(string.Format("lambda表达式不支持使用此'{0}'方法", methodExp.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression unaryExp)
        {
            try
            {
                if (unaryExp.NodeType == ExpressionType.Convert)
                {
                    this.Visit(unaryExp.Operand);
                    return unaryExp;
                }
                else
                {
                    object result = Expression.Lambda(unaryExp).Compile().DynamicInvoke();
                    var r = Expression.Constant(result, result.GetType());
                    this.Visit(r);
                    return r;
                }
            }
            catch
            {
                throw new NotSupportedException(string.Format("一元运算符 '{0}’不支持", unaryExp.NodeType));
            }
        }

        protected override Expression VisitBinary(BinaryExpression binaryExp)
        {
            this.Visit(binaryExp.Left);
            this.Visit(binaryExp.Right);
            return binaryExp;
        }

        public string ParameterPrefix
        {
            get { throw new NotImplementedException(); }
        }

        public DbParameter GetDbParameter(string parameterName, object value)
        {
            throw new NotImplementedException();
        }

        public string GetFieldName(Type tableType, string memberName)
        {
            var entityInfo = Caches.EntityInfoCache.Get(tableType);
            if (entityInfo.Columns.Keys.Contains(memberName))
            {
                return entityInfo.Columns[memberName];
            }
            else
            {
                return memberName;
            }
        }
    }
}
