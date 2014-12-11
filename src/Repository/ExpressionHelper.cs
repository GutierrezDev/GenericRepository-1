using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Repository
{
    internal static class ExpressionHelper
    {
        internal static string ToIncludeString<T>(this Expression<Func<T, object>> selector)
        {
            var members = new List<PropertyInfo>();
            CollectRelationalMembers(selector, members);

            var sb = new StringBuilder();
            var separator = "";
            foreach (var member in members)
            {
                sb.Append(separator);
                sb.Append(member.Name);
                separator = ".";
            }

            return sb.ToString();
        }

        private static void CollectRelationalMembers(Expression exp, IList<PropertyInfo> members)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    CollectRelationalMembers(((LambdaExpression)exp).Body, members);
                    break;
                case ExpressionType.MemberAccess:
                    var mexp = (MemberExpression)exp;

                    CollectRelationalMembers(mexp.Expression, members);
                    members.Add((PropertyInfo)mexp.Member);

                    break;
                case ExpressionType.Call:
                    var cexp = (MethodCallExpression)exp;

                    if (cexp.Method.IsStatic == false)
                        throw new InvalidOperationException("Invalid type of expression.");

                    foreach (var arg in cexp.Arguments)
                        CollectRelationalMembers(arg, members);

                    break;
                case ExpressionType.Parameter:
                    return;

                case ExpressionType.Convert:
                    var conexp = (UnaryExpression)exp;
                    CollectRelationalMembers(conexp, members);
                    break;

                default:
                    throw new InvalidOperationException("Invalid type of expression.");
            }
        }
    }
}
