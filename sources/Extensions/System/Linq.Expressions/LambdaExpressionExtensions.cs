using System.Linq.Expressions;
using System.Reflection;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System.Linq.Expressions
{
    internal static class LambdaExpressionExtensions
    {
        public static string GetUniqueId(this LambdaExpression lambda)
        {
            if (lambda.Body is MethodCallExpression methodCall)
            {
                return methodCall.Method.GetUniqueId();
            }
            if (lambda.Body is MemberExpression memberExpression)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                return propertyInfo.GetGetGetMethod().GetUniqueId();
            }
            throw new NotImplementedException();
        }
    }
}