using System;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Base
{
    internal interface ISnoopableMemberTemplate
    {
        bool CanBeUsedWith(object @object);
        MemberDescriptor Descriptor { get; }
    }


    internal sealed class MemberTemplate<TForType> : ISnoopableMemberTemplate
    {
        private Func<TForType, bool> CanBeUsedTyped { get; init; }
        public MemberDescriptor Descriptor { get; init; }

        public bool CanBeUsedWith(object @object)
        {
            if (CanBeUsedTyped != null)
            {
                Guard.IsAssignableToType<TForType>(@object);
                return CanBeUsedTyped((TForType)@object);
            }
            return true;
        }


        public static ISnoopableMemberTemplate Create<TReturnType>(Expression<Func<Document, TForType, TReturnType>> getter, Func<TForType, bool> canBeUsed = null, MemberKind kind = MemberKind.StaticMethod)
        {
            var compiledGetter = getter.Compile();
            var methodCallExpression = getter.Body as MethodCallExpression;
            var memberAccessor = new MemberAccessorByFunc<TForType, TReturnType>(compiledGetter);

            return Create(methodCallExpression.Method.DeclaringType, methodCallExpression.Method.Name, memberAccessor, canBeUsed, kind, () => RevitDocumentationReader.GetMethodComments(methodCallExpression.Method));
        }
        public static ISnoopableMemberTemplate Create(Type declaringType, string memberName, IAccessor memberAccessor, Func<TForType, bool> canBeUsed = null, MemberKind kind = MemberKind.StaticMethod, Func<DocXml> documentationFactoryMethod = null)
        {
            return new MemberTemplate<TForType>()
            {
                Descriptor = new MemberDescriptor(typeof(TForType), kind, memberName, declaringType, memberAccessor, documentationFactoryMethod),
                CanBeUsedTyped = canBeUsed,
            };
        }
    }
}