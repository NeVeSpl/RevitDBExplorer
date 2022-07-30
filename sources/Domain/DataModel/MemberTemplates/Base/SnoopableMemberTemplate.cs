using System;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates.Base
{
    internal interface ISnoopableMemberTemplate
    {
        Type ForType { get; }     
        bool CanBeUsedWith(object @object);
        SnoopableMember SnoopableMember { get; init; }
    }


    internal sealed class SnoopableMemberTemplate<TSnoopedObjectType> : ISnoopableMemberTemplate
    {
        private Func<TSnoopedObjectType, bool> CanBeUsedTyped { get; init; }
        public SnoopableMember SnoopableMember { get; init; }
        public Type ForType => typeof(TSnoopedObjectType);
        public bool CanBeUsedWith(object @object)
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
                SnoopableMember = new SnoopableMember(null, kind, memberName, declaringType, memberAccessor, null),             
                CanBeUsedTyped = canBeUsed,              
            };
        }
    }
}