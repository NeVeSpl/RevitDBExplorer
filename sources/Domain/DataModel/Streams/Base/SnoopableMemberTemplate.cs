using System;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams.Base
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
            ShouldBeCreated = Wrap(shouldBeCreated);
        }       
        public SnoopableMemberTemplate(Type declaringType, string memberName, IMemberAccessor memberAccessor, Func<TInput, bool> shouldBeCreated = null)
        {
            DeclaringType = declaringType;
            MemberName = memberName;
            MemberAccessor = memberAccessor;
            ShouldBeCreated = Wrap(shouldBeCreated);
        }


        private Func<object, bool> Wrap(Func<TInput, bool> shouldBeCreated)
        {
            if (shouldBeCreated != null)
            {
                return (x) => shouldBeCreated(x as TInput);
            }
            else
            {
                return (x) => true;
            }
        }
    }
}