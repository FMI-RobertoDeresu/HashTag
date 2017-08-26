using System;
using System.Linq.Expressions;

namespace HashTag.Infrastructure.Extensions
{
    public static class ExpressionExtensions
    {
        public static string FullName<T, TP>(this Expression<Func<T, TP>> expression)
        {
            if (expression == null)
                throw new ArgumentException("expression");
            return expression.Body.FullName();
        }

        private static string FullName(this Expression expression)
        {
            if (expression == null || expression.NodeType != ExpressionType.MemberAccess)
                return "";

            var memberExpression = (MemberExpression) expression;
            var isRoot = memberExpression.Expression?.NodeType != ExpressionType.MemberAccess;
            return memberExpression.Expression.FullName() + (isRoot ? "" : ".") + memberExpression.Member.Name;
        }
    }
}