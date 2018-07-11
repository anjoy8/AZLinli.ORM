using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AZLinli.ORM.DataAccess
{
    public class MemberTypeVisitor:ExpressionVisitor
    {
        public Type MemberType { get; set; }

        public Type Translate(Expression exp) 
        {
            this.Visit(exp);
            return MemberType;
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
                else if (unaryExp.NodeType == ExpressionType.Not)
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
        

        protected override Expression VisitConstant(ConstantExpression constantExp)
        {
            return constantExp;
        }

        protected override Expression VisitMember(MemberExpression memberExp)
        {
            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                MemberType = memberExp.Type;
            }
            return memberExp;
        }
        
    }
}
