using System;
using System.Collections.Generic;
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


        public static ISnoopableMemberTemplate Create<TReturnType>(Expression<Func<Document, TForType, TReturnType>> getter,
                                                                   Func<TForType, bool> canBeUsed = null,
                                                                   MemberKind kind = MemberKind.StaticMethod)
        {
            var compiledGetter = getter.Compile();
            var methodCallExpression = getter.Body as MethodCallExpression;
            var memberAccessor = new MemberAccessorByFunc<TForType, TReturnType>(compiledGetter);
            
            memberAccessor.UniqueId = $"{getter.GetUniqueId()}";
            memberAccessor.DefaultInvocation.Syntax = getter.ToCeSharp();

            return WithCustomAC(methodCallExpression.Method.DeclaringType, methodCallExpression.Method.Name, memberAccessor, canBeUsed, kind, () => RevitDocumentationReader.GetMethodComments(methodCallExpression.Method));
        }

        public static ISnoopableMemberTemplate CreateWithParam<TParam0Type, TReturnType>(Expression<Func<Document, TForType, TParam0Type, TReturnType>> getter,
                                                                                Func<Document, TForType, IEnumerable<TParam0Type>> param_0_arguments,
                                                                                Func<TForType, bool> canBeUsed = null,
                                                                                MemberKind kind = MemberKind.StaticMethod)
        {
            var compiledGetter = getter.Compile();
            var methodCallExpression = getter.Body as MethodCallExpression;
            var param_0_name = getter.Parameters[2].Name;
            var memberAccessor = new MemberAccessorByFuncUltra<TForType, TParam0Type, TReturnType>(compiledGetter, param_0_arguments, param_0_name);

            memberAccessor.UniqueId = $"{getter.GetUniqueId()}";
            memberAccessor.DefaultInvocation.Syntax = getter.ToCeSharp();

            return WithCustomAC(methodCallExpression.Method.DeclaringType, methodCallExpression.Method.Name, memberAccessor, canBeUsed, kind, () => RevitDocumentationReader.GetMethodComments(methodCallExpression.Method));
        }


        public static ISnoopableMemberTemplate WithCustomAC(Type declaringType,
                                                            string memberName,
                                                            IAccessor memberAccessor,
                                                            Func<TForType, bool> canBeUsed = null,
                                                            MemberKind kind = MemberKind.StaticMethod,
                                                            Func<DocXml> documentationFactoryMethod = null)
        {
            if (string.IsNullOrEmpty(memberAccessor.UniqueId))
            {
                memberAccessor.UniqueId= $"{typeof(TForType).Name}_{memberAccessor.GetType().Name}.{memberName}";
            }
            if (string.IsNullOrEmpty(memberAccessor.DefaultInvocation.Syntax))
            {

            }

            return new MemberTemplate<TForType>()
            {
                Descriptor = new MemberDescriptor(typeof(TForType), kind, memberName, declaringType, memberAccessor, documentationFactoryMethod),
                CanBeUsedTyped = canBeUsed,
            };
        }
    }
}