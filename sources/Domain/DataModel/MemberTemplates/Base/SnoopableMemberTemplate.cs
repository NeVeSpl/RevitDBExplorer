using System;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates.Base
{
    internal interface ISnoopableMemberTemplate
    {
        bool CanBeUsedWith(object @object);
        MemberDescriptor Descriptor { get;  }
    }


    internal sealed class SnoopableMemberTemplate<TSnoopedObjectType> : ISnoopableMemberTemplate
    {
        private Func<TSnoopedObjectType, bool> CanBeUsedTyped { get; init; }
        public MemberDescriptor Descriptor { get; init; }
       
        public bool CanBeUsedWith(object @object)
        {
            if (CanBeUsedTyped != null)
            {
                Guard.IsAssignableToType<TSnoopedObjectType>(@object);
                return CanBeUsedTyped((TSnoopedObjectType)@object);
            }
            return true;
        }


        public static ISnoopableMemberTemplate Create<TReturnType>(Expression<Func<Document, TSnoopedObjectType, TReturnType>> getter, Func<TSnoopedObjectType, bool> canBeUsed = null, MemberKind kind = MemberKind.StaticMethod)
        {
            var compiledGetter = getter.Compile();
            var methodCallExpression = (getter.Body as MethodCallExpression);           
            var memberAccessor = new MemberAccessorByFunc<TSnoopedObjectType, TReturnType>(compiledGetter);  
            return Create(methodCallExpression.Method.DeclaringType, methodCallExpression.Method.Name, memberAccessor, canBeUsed, kind, () => RevitDocumentationReader.GetMethodComments(methodCallExpression.Method));
        } 
        public static ISnoopableMemberTemplate Create(Type declaringType, string memberName, IMemberAccessor memberAccessor, Func<TSnoopedObjectType, bool> canBeUsed = null, MemberKind kind = MemberKind.StaticMethod, Func<DocXml> documentationFactoryMethod = null) 
        {
            return new SnoopableMemberTemplate<TSnoopedObjectType>()
            {
                Descriptor = new MemberDescriptor(typeof(TSnoopedObjectType), kind, memberName, declaringType, memberAccessor, documentationFactoryMethod),             
                CanBeUsedTyped = canBeUsed,              
            };
        }
    }
}