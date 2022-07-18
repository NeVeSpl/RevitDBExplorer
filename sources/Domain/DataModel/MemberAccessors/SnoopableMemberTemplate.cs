using System;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface ISnoopableMemberTemplate
    {
        Type DeclaringType { get; }
        string MemberName { get; }
        IMemberAccessor MemberAccessor { get; }
        Func<object, bool> ShouldBeCreated { get; }
    }

    internal sealed class SnoopableMemberTemplate<TInput, TReturn> : ISnoopableMemberTemplate where TInput : class
    {
        public Type DeclaringType { get; }
        public string MemberName { get; }
        public IMemberAccessor MemberAccessor { get; }
        public Func<object, bool> ShouldBeCreated { get; }


        public SnoopableMemberTemplate(Expression<Func<Document, TInput, TReturn>> getter, Func<TInput, bool> shouldBeCreated = null)
        {
            var compiledGetter = getter.Compile();
            var methodCallExpression = (getter.Body as MethodCallExpression);
            MemberName = methodCallExpression.Method.Name;
            DeclaringType = methodCallExpression.Method.DeclaringType;
            MemberAccessor = new MemberAccessorByFunc<TInput, TReturn>(compiledGetter);
            if (shouldBeCreated != null)
            {
                ShouldBeCreated = (x) => shouldBeCreated(x as TInput);
            }
            else
            {
                ShouldBeCreated = (x) => true;
            }
        }
    }
}