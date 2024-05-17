using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;
using ExpressionTreeToString;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System.Linq.Expressions
{
    internal static class MethodCallExpressionExtensions
    {
        public static string ToCeSharp(this MethodCallExpression methodCallExpression)
        {
            string syntax = null;
            if (methodCallExpression.Object is ParameterExpression)
            {
                var uniformMethodCallExpression = methodCallExpression.Update(Expression.Parameter(methodCallExpression.Object.Type, "item"), methodCallExpression.Arguments);
                syntax = uniformMethodCallExpression.ToString("C#");
            }
            if (methodCallExpression.Object == null)
            {
                var arguments = new List<Expression>();
                foreach (var arg in methodCallExpression.Arguments) 
                {
                    if (arg is ParameterExpression)
                    {
                        var isDocument = arg.Type == typeof(Document);                        
                        arguments.Add(Expression.Parameter(arg.Type, isDocument ? "document" : "item"));
                        continue;
                        
                    }
                    if (arg is MemberExpression member)
                    {
                        var isDocument = member.Expression.Type == typeof(Document);
                        arguments.Add(Expression.Property(Expression.Parameter(member.Expression.Type, isDocument ? "document" : "item"), member.Member as PropertyInfo));
                        continue;
                    }
                    arguments.Add(arg);
                }



                var uniformMethodCallExpression = methodCallExpression.Update(null, arguments);
                //var syntaxb = methodCallExpression.ToString("C#");
                syntax = uniformMethodCallExpression.ToString("C#");
            }
            return syntax;
        }
    }
}