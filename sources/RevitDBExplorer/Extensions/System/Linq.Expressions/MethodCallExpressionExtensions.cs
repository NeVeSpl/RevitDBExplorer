using Autodesk.Revit.DB;
using ExpressionTreeToString;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System.Linq.Expressions
{
    internal static class LambdaExpressionExtensions
    {
        public static string ToCeSharp(this LambdaExpression lambdaExpression)
        {
            var body = new ParameterReplacer(lambdaExpression.Parameters[0], Expression.Parameter(lambdaExpression.Parameters[0].Type, "document")).Visit(lambdaExpression.Body);
           
            var isDocument = lambdaExpression.Parameters[1].Type == typeof(Document);
            var secondParamName = isDocument ? "document" : "item";
            body = new ParameterReplacer(lambdaExpression.Parameters[1], Expression.Parameter(lambdaExpression.Parameters[1].Type, secondParamName)).Visit(body);
           
            var methodCallExpression = body as MethodCallExpression;        
            var syntax = methodCallExpression.ToString("C#");
        
            return syntax;
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression oldParameter;
            private readonly ParameterExpression newParameter;

            public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                this.oldParameter = oldParameter;
                this.newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == oldParameter ? newParameter : node;
            }
        }
    }
}