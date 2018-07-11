
using AZLinli.ORM.Cache;
using AZLinli.ORM.DataAccess.Dialect;
using AZLinli.ORM.ORM;
using System;
using System.Collections;
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
    /// Lambda转where SQL
    /// Author AZLinli.COM
    /// </summary>
    public class SqlVisitor : ExpressionVisitor, IVisitor
    {
        private DataBaseType DbType;

        private List<DbParameter> parameters;

        /// <summary>
        /// 条件参数
        /// </summary>
        public List<DbParameter> Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        public string ParameterPrefix
        {
            get
            {
                return DbParameterFactory.GetParameterPrefix();
            }
        }

        public StringBuilder SqlBuilder
        {
            get;
            set;
        }

        public int CallIndex { get; set; }

        public int ParameterIndex { get; set; }

        /// <summary>
        /// 是否预处理二元运算 列和value都在左边的 譬如Compare
        /// </summary>
        public bool IsPreTreatBinary { get; set; }

        public Type MemberType { get; set; }

        public ISqlDialect SqlDialect { get; set; }

        public VisitorType SqlVisitorType { get; set; }

        protected bool WithAlias { get; set; }

        public SqlVisitor(DataBaseType dbType)
        {
            this.parameters = new List<DbParameter>();
            this.DbType = dbType;
            CallIndex = 0;
            ParameterIndex = 0;
            SqlDialect = SqlDialectFactory.CreateSqlDialect();
        }

        public SqlVisitor(DataBaseType dbType, int callIndex) : this(dbType)
        {
            CallIndex = callIndex;
        }

        public SqlVisitor(DataBaseType dbType, int callIndex, VisitorType visitorType) : this(dbType, callIndex)
        {
            this.SqlVisitorType = visitorType;
        }

        public SqlVisitor(DataBaseType dbType, int callIndex, bool withAlias) : this(dbType, callIndex)
        {
            this.WithAlias = withAlias;
        }

        public string TranslateClause(Expression exp)
        {
            int position = SqlBuilder.Length;
            this.Visit(exp);
            string clause = SqlBuilder.ToString().Substring(position);
            SqlBuilder.Remove(position, clause.Length);
            return clause;
        }

        public string Translate(Expression exp)
        {
            this.SqlBuilder = new StringBuilder();
            //m=>true
            if (((LambdaExpression)exp).Body.NodeType == ExpressionType.Constant)
            {
                if (Convert.ToBoolean((((LambdaExpression)exp).Body as ConstantExpression).Value) == true)
                {
                    return " 1=1 ";
                }
            }
            this.Visit(((LambdaExpression)exp).Body);
            return this.SqlBuilder.ToString();
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodExp)
        {
            switch (methodExp.Method.Name)
            {
                case "Contains":
                    if (methodExp.Method.DeclaringType == typeof(string))
                    {
                        this.Visit(methodExp.Object);
                        SqlBuilder.AppendFormat(SqlDialect.Contains(), TranslateClause(methodExp.Arguments[0]));
                        return methodExp;
                    }
                    else
                    {
                        Expression mex;
                        if (methodExp.Object != null)
                        {
                            this.Visit(methodExp.Arguments[0]);
                            SqlBuilder.Append(" IN (");
                            mex = methodExp.Object;
                        }
                        else
                        {
                            this.Visit(methodExp.Arguments[1]);
                            SqlBuilder.Append(" IN (");
                            mex = methodExp.Arguments[0];
                        }
                        if (mex.NodeType == ExpressionType.Call && mex is MethodCallExpression && ((MethodCallExpression)mex).Method.DeclaringType == typeof(SubQuery))
                        {
                            this.Visit(mex);
                        }
                        else
                        {
                            IEnumerable result = Expression.Lambda(mex).Compile().DynamicInvoke() as IEnumerable;
                            int length = 0;
                            foreach (var v in result)
                            {
                                length++;
                                this.Visit(Expression.Constant(v));
                                SqlBuilder.Append(",");
                            }
                            if (length == 0)
                            {
                                SqlBuilder.Append("NULL,");
                            }
                            SqlBuilder.Remove(SqlBuilder.Length - 1, 1);
                        }
                        SqlBuilder.Append(")");
                        return methodExp;
                    }
                case "ToUpper":
                    SqlBuilder.Append(" UPPER(");
                    this.Visit(methodExp.Object);
                    SqlBuilder.Append(")");
                    return methodExp;
                case "ToLower":
                    SqlBuilder.Append(" LOWER(");
                    this.Visit(methodExp.Object);
                    SqlBuilder.Append(")");
                    return methodExp;
                case "CompareTo":
                    IsPreTreatBinary = true;
                    this.Visit(methodExp.Object);
                    SqlBuilder.Append("{0}");
                    this.Visit(methodExp.Arguments[0]);
                    return methodExp;
                case "Equals":
                    this.Visit(methodExp.Object);
                    SqlBuilder.Append("=");
                    this.Visit(methodExp.Arguments[0]);
                    return methodExp;
                case "IndexOf":
                    SqlBuilder.AppendFormat(SqlDialect.IndexOf(), TranslateClause(methodExp.Arguments[0]), TranslateClause(methodExp.Object));
                    return methodExp;
                case "IsNullOrEmpty":
                    string template = " ({0} IS NULL OR {0}='') ";
                    string[] sections = template.Split(new string[] { "{0}" }, StringSplitOptions.None);
                    for (int i = 0; i < sections.Length - 1; i++)
                    {
                        if (i == 0)
                        {
                            SqlBuilder.Append(sections[i]);
                        }
                        this.Visit(methodExp.Arguments[0]);
                        SqlBuilder.Append(sections[i + 1]);
                    }
                    return methodExp;
                case "ToString":
                    //Convert.ToString()
                    if (methodExp.Method.DeclaringType == typeof(System.Convert))
                    {
                        MemberTypeVisitor mtVisitor = new MemberTypeVisitor();
                        MemberType = mtVisitor.Translate(methodExp.Arguments[0]);
                        if (MemberType != null)
                        {
                            SqlBuilder.AppendFormat(SqlDialect.ToChar(), TranslateClause(methodExp.Arguments[0]));
                            return methodExp;
                        }
                    }
                    else if (methodExp.Method.DeclaringType == typeof(System.Object))
                    {
                        MemberTypeVisitor mtVisitor = new MemberTypeVisitor();
                        MemberType = mtVisitor.Translate(methodExp.Object);
                        if (MemberType != null)
                        {
                            SqlBuilder.AppendFormat(SqlDialect.ToChar(), TranslateClause(methodExp.Object));
                            return methodExp;
                        }
                    }
                    else if (methodExp.Method.DeclaringType == typeof(Nullable<DateTime>) || methodExp.Method.DeclaringType == typeof(DateTime))
                    {
                        if (methodExp.Arguments[0] is ConstantExpression)
                        {
                            MemberTypeVisitor mtVisitor = new MemberTypeVisitor();
                            MemberType = mtVisitor.Translate(methodExp.Object);
                            if (MemberType != null)
                            {
                                string timeFormat = ((ConstantExpression)methodExp.Arguments[0]).Value as string;
                                SqlBuilder.AppendFormat(SqlDialect.DateTimeToChar(), TranslateClause(methodExp.Object), SqlDialect.ParseTimeFormat(timeFormat));
                                return methodExp;
                            }
                        }
                    }
                    goto default;
                case "ToInt32":
                    //Convert.ToInt32()
                    if (methodExp.Method.DeclaringType == typeof(System.Convert))
                    {
                        MemberTypeVisitor mtVisitor = new MemberTypeVisitor();
                        MemberType = mtVisitor.Translate(methodExp.Arguments[0]);
                        if (MemberType != null)
                        {
                            SqlBuilder.AppendFormat(SqlDialect.ToNumber(), TranslateClause(methodExp.Arguments[0]));
                            return methodExp;
                        }
                    }
                    goto default;
                case "ToDateTime":
                    if (methodExp.Method.DeclaringType == typeof(System.Convert))
                    {
                        MemberTypeVisitor mtVisitor = new MemberTypeVisitor();
                        MemberType = mtVisitor.Translate(methodExp.Arguments[0]);
                        if (MemberType == typeof(Nullable<DateTime>) || MemberType == typeof(DateTime))
                        {
                            this.Visit(methodExp.Arguments[0]);
                            return methodExp;
                        }
                        else if (MemberType == typeof(string))
                        {
                            SqlBuilder.AppendFormat(SqlDialect.ToDateTime(), TranslateClause(methodExp.Arguments[0]));
                            return methodExp;
                        }
                    }
                    goto default;
                case "StartsWith":
                    this.Visit(methodExp.Object);
                    SqlBuilder.AppendFormat(SqlDialect.StartsWith(), TranslateClause(methodExp.Arguments[0]));
                    return methodExp;
                case "EndsWith":
                    this.Visit(methodExp.Object);
                    SqlBuilder.AppendFormat(SqlDialect.EndsWith(), TranslateClause(methodExp.Arguments[0]));
                    return methodExp;
                case "TrimStart":
                    SqlBuilder.Append(" LTRIM(");
                    this.Visit(methodExp.Object);
                    SqlBuilder.Append(")");
                    return methodExp;
                case "TrimEnd":
                    SqlBuilder.Append(" RTRIM(");
                    this.Visit(methodExp.Object);
                    SqlBuilder.Append(")");
                    return methodExp;
                case "Trim":
                    SqlBuilder.Append(" RTRIM(LTRIM(");
                    this.Visit(methodExp.Object);
                    SqlBuilder.Append("))");
                    return methodExp;
                case "Count":
                    if (methodExp.Method.DeclaringType == typeof(SqlFunctions))
                    {
                        SqlBuilder.AppendFormat("COUNT({0})", TranslateClause(methodExp.Arguments[0]));
                        return methodExp;
                    }
                    goto default;
                case "CountDistinct":
                    if (methodExp.Method.DeclaringType == typeof(SqlFunctions))
                    {
                        SqlBuilder.AppendFormat("COUNT(DISTINCT {0})", TranslateClause(methodExp.Arguments[0]));
                        return methodExp;
                    }
                    goto default;
                case "Max":
                case "Min":
                case "Avg":
                    if (methodExp.Method.DeclaringType == typeof(SqlFunctions))
                    {
                        SqlBuilder.AppendFormat("{0}({1})", methodExp.Method.Name, TranslateClause(methodExp.Arguments[0]));
                        return methodExp;
                    }
                    goto default;
                case "GetSingle":
                    if (methodExp.Method.DeclaringType == typeof(SubQuery))
                    {
                        Expression opd1 = methodExp.Arguments[0];
                        Expression opd2 = methodExp.Arguments[1];
                        if (methodExp.Arguments[0].NodeType == ExpressionType.Quote)
                        {
                            opd1 = ((UnaryExpression)(methodExp.Arguments[0])).Operand;
                        }
                        if (methodExp.Arguments[1].NodeType == ExpressionType.Quote)
                        {
                            opd2 = ((UnaryExpression)(methodExp.Arguments[1])).Operand;
                        }
                        if (WithAlias)
                        {
                            SqlBuilder.AppendFormat("(SELECT {0} FROM {1} AS {3} WHERE {2})", TranslateClause(((LambdaExpression)opd2).Body), GetTableName(((LambdaExpression)opd1).Parameters[0].Type), TranslateClause(((LambdaExpression)opd1).Body), ((LambdaExpression)opd1).Parameters[0].Name);
                        }
                        else
                        {
                            SqlBuilder.AppendFormat("(SELECT {0} FROM {1} WHERE {2})", TranslateClause(((LambdaExpression)opd2).Body), GetTableName(((LambdaExpression)opd1).Parameters[0].Type), TranslateClause(((LambdaExpression)opd1).Body));
                        }
                        return methodExp;
                    }
                    goto default;
                case "GetList":
                    if (methodExp.Method.DeclaringType == typeof(SubQuery))
                    {
                        Expression opd1 = methodExp.Arguments[0];
                        Expression opd2 = methodExp.Arguments[1];
                        if (methodExp.Arguments[0].NodeType == ExpressionType.Quote)
                        {
                            opd1 = ((UnaryExpression)(methodExp.Arguments[0])).Operand;
                        }
                        if (methodExp.Arguments[1].NodeType == ExpressionType.Quote)
                        {
                            opd2 = ((UnaryExpression)(methodExp.Arguments[1])).Operand;
                        }
                        if (WithAlias)
                        {
                            SqlBuilder.AppendFormat("SELECT {0} FROM {1} AS {3} WHERE {2}", TranslateClause(((LambdaExpression)opd2).Body), GetTableName(((LambdaExpression)opd1).Parameters[0].Type), TranslateClause(((LambdaExpression)opd1).Body), ((LambdaExpression)opd1).Parameters[0].Name);
                        }
                        else
                        {
                            SqlBuilder.AppendFormat("SELECT {0} FROM {1} WHERE {2}", TranslateClause(((LambdaExpression)opd2).Body), GetTableName(((LambdaExpression)opd1).Parameters[0].Type), TranslateClause(((LambdaExpression)opd1).Body));
                        }
                        return methodExp;
                    }
                    goto default;
                default:
                    try
                    {
                        object result = Expression.Lambda(methodExp).Compile().DynamicInvoke();
                        var r = Expression.Constant(result, methodExp.Method.ReturnType);
                        this.Visit(r);
                        return r;
                    }
                    catch
                    {
                        if (methodExp.Object != null)
                        {
                            Expression obj = this.Visit(methodExp.Object);
                            return obj;
                        }
                        else
                        {
                            Expression obj = this.Visit(methodExp.Arguments[0]);
                            return obj;
                        }
                    }
                    throw new NotSupportedException(string.Format("lambda表达式不支持使用此'{0}'方法，请自行扩展", methodExp.Method.Name));

            }
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
                    SqlBuilder.Append(" NOT ");
                    this.Visit(unaryExp.Operand);
                    return unaryExp;
                }
                else if (unaryExp.NodeType == ExpressionType.Quote)
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
            SqlBuilder.Append("(");
            switch (binaryExp.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    this.Visit(binaryExp.Left);
                    SqlBuilder.Append(" AND ");
                    this.Visit(binaryExp.Right);
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    this.Visit(binaryExp.Left);
                    SqlBuilder.Append(" OR ");
                    this.Visit(binaryExp.Right);
                    break;
                case ExpressionType.Equal:
                    this.Visit(binaryExp.Left);
                    if (IsNullValue(binaryExp.Right))
                    {
                        SqlBuilder.Append(" IS ");
                    }
                    else
                    {
                        SqlBuilder.Append("=");
                    }
                    this.Visit(binaryExp.Right);
                    break;
                case ExpressionType.NotEqual:
                    this.Visit(binaryExp.Left);
                    if (IsNullValue(binaryExp.Right))
                    {
                        SqlBuilder.Append(" IS NOT ");
                    }
                    else
                    {
                        SqlBuilder.Append("<>");
                    }
                    this.Visit(binaryExp.Right);
                    break;
                case ExpressionType.LessThan:
                    this.Visit(binaryExp.Left);
                    if (IsPreTreatBinary)
                    {
                        SqlBuilder.Replace("{0}", "<");
                        IsPreTreatBinary = false;
                    }
                    else
                    {
                        SqlBuilder.Append("<");
                        this.Visit(binaryExp.Right);
                    }
                    break;
                case ExpressionType.LessThanOrEqual:
                    this.Visit(binaryExp.Left);
                    if (IsPreTreatBinary)
                    {
                        SqlBuilder.Replace("{0}", "<=");
                        IsPreTreatBinary = false;
                    }
                    else
                    {
                        SqlBuilder.Append("<=");
                        this.Visit(binaryExp.Right);
                    }
                    break;
                case ExpressionType.GreaterThan:
                    this.Visit(binaryExp.Left);
                    if (IsPreTreatBinary)
                    {
                        SqlBuilder.Replace("{0}", ">");
                        IsPreTreatBinary = false;
                    }
                    else
                    {
                        SqlBuilder.Append(">");
                        this.Visit(binaryExp.Right);
                    }
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this.Visit(binaryExp.Left);
                    if (IsPreTreatBinary)
                    {
                        SqlBuilder.Replace("{0}", ">=");
                        IsPreTreatBinary = false;
                    }
                    else
                    {
                        SqlBuilder.Append(">=");
                        this.Visit(binaryExp.Right);
                    }
                    break;
                case ExpressionType.Add:
                    if (binaryExp.Type == typeof(string))
                    {
                        SqlBuilder.Append("(");
                        this.Visit(binaryExp.Left);
                        SqlBuilder.Append("||");
                        this.Visit(binaryExp.Right);
                        SqlBuilder.Append(")");
                    }
                    else
                    {
                        this.Visit(binaryExp.Left);
                        SqlBuilder.Append("+");
                        this.Visit(binaryExp.Right);
                    }
                    break;
                case ExpressionType.Subtract:
                    this.Visit(binaryExp.Left);
                    SqlBuilder.Append("-");
                    this.Visit(binaryExp.Right);
                    break;
                case ExpressionType.Multiply:
                    this.Visit(binaryExp.Left);
                    SqlBuilder.Append("*");
                    this.Visit(binaryExp.Right);
                    break;
                case ExpressionType.Divide:
                    this.Visit(binaryExp.Left);
                    SqlBuilder.Append("/");
                    this.Visit(binaryExp.Right);
                    break;
                default:
                    throw new NotSupportedException(string.Format("二元运算符 '{0}'不支持", binaryExp.NodeType));

            }
            SqlBuilder.Append(")");
            return binaryExp;
        }

        protected bool IsNullValue(Expression exp)
        {
            try
            {
                if (exp.NodeType == ExpressionType.MemberAccess && ((MemberExpression)exp).Expression.NodeType == ExpressionType.Parameter)
                {
                    return false;
                }
                object result = Expression.Lambda(exp).Compile().DynamicInvoke();
                return result == null;
            }
            catch
            {
                return false;
            }
        }

        protected override Expression VisitConstant(ConstantExpression constantExp)
        {
            if (constantExp.Value == null)
            {
                SqlBuilder.Append("NULL");
            }
            else
            {
                string paramName = string.Format("{0}_{1}", CallIndex, ParameterIndex++);
                this.parameters.Add(GetDbParameter(paramName, constantExp.Value));
                SqlBuilder.Append(ParameterPrefix + paramName);
            }
            return constantExp;
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
                    alias = GetTableName(memberExp.Expression.Type);
                }
                string fieldName =string.Format("{0}.{1}", alias, GetFieldName(memberExp.Expression.Type, memberExp.Member.Name));
                if (!string.IsNullOrEmpty(fieldName))
                {
                    SqlBuilder.Append(fieldName);
                    MemberType = memberExp.Type;
                }
                return memberExp;
            }
            else if (memberExp.NodeType == ExpressionType.MemberAccess)
            {
                object result = Expression.Lambda(memberExp).Compile().DynamicInvoke();
                if (result != null)
                {
                    var r = Expression.Constant(result, result.GetType());
                    this.Visit(r);
                    return r;
                }
                else
                {
                    var r = Expression.Constant(result);
                    this.Visit(r);
                    return r;
                }
            }
            throw new NotSupportedException(string.Format("成员 '{0}’不支持", memberExp.Member.Name));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var entityInfo = Caches.EntityInfoCache.Get(node.Type);
            if (WithAlias)
            {
                SqlBuilder.AppendFormat("{0},", entityInfo.SelectFields.Replace(GetTableName(node.Type) + ".", node.Name + "."));
            }
            else
            {
                SqlBuilder.AppendFormat("{0},", entityInfo.SelectFields);
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                this.Visit(node.Arguments[i]);
                if (node.Arguments[i].NodeType != ExpressionType.Parameter)
                {
                    SqlBuilder.AppendFormat(" AS {0},", node.Members[i].Name);
                }
            }
            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            if (this.SqlVisitorType == VisitorType.UpdateSet)
            {
                SqlBuilder.AppendFormat(" {0}=", GetFieldName(assignment.Member.DeclaringType, assignment.Member.Name));
                this.Visit(assignment.Expression);
                SqlBuilder.Append(",");
            }
            else
            {
                this.Visit(assignment.Expression);
                SqlBuilder.AppendFormat(" AS {0},", assignment.Member.Name);
            }
            return assignment;
        }

        public string GetTableName(Type tableType)
        {
            var entityInfo = Caches.EntityInfoCache.Get(tableType);
            return entityInfo.TableName;
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

        public DbParameter GetDbParameter(string parameterName, object value)
        {
            return DbParameterFactory.CreateDbParameter(parameterName, value);
        }
    }
}
