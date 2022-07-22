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
        SnoopableMember.Kind Kind { get; }
        bool CanBeUsed(object @object);
    }

    internal sealed class SnoopableMemberTemplate<TSnoopedObjectType> : ISnoopableMemberTemplate
    {
        public Func<TSnoopedObjectType, bool> CanBeUsedTyped { get; init; }
        public Type DeclaringType { get; init; }
        public string MemberName { get; init; }
        public IMemberAccessor MemberAccessor { get; init; }
        public SnoopableMember.Kind Kind { get; init; }
        public bool CanBeUsed(object @object)
        {
            if (CanBeUsedTyped != null)
            {
                Guard.IsAssignableToType<TSnoopedObjectType>(@object);
                return CanBeUsedTyped((TSnoopedObjectType)@object);
            }
            return true;
        }


        public static ISnoopableMemberTemplate Create<TReturnType>(Expression<Func<Document, TSnoopedObjectType, TReturnType>> getter, Func<TSnoopedObjectType, bool> canBeUsed = null)
        {
            var compiledGetter = getter.Compile();
            var methodCallExpression = (getter.Body as MethodCallExpression);           
            var memberAccessor = new MemberAccessorByFunc<TSnoopedObjectType, TReturnType>(compiledGetter);  
            return Create(methodCallExpression.Method.DeclaringType, methodCallExpression.Method.Name, memberAccessor, canBeUsed);
        } 
        public static ISnoopableMemberTemplate Create(Type declaringType, string memberName, IMemberAccessor memberAccessor, Func<TSnoopedObjectType, bool> canBeUsed = null, SnoopableMember.Kind kind = SnoopableMember.Kind.StaticMethod ) 
        {
            return new SnoopableMemberTemplate<TSnoopedObjectType>()
            {
                DeclaringType = declaringType,
                MemberName = memberName,
                MemberAccessor = memberAccessor,
                CanBeUsedTyped = canBeUsed,
                Kind = kind,
            };
        }
    }
}