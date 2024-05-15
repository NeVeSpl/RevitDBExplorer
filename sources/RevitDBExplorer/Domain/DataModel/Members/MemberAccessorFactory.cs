using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Base;
using RevitDBExplorer.Domain.DataModel.Members.Internals;


// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members
{
    internal static class MemberAccessorFactory
    {
        private static readonly Dictionary<string, Func<IAccessor>> memberAccessorOverrides = new();
        

        public static void Init()
        {

        }
        static MemberAccessorFactory()
        {
            var accessors = GetAllInstancesThatImplement<ICanCreateMemberAccessor>();
            foreach (var accessor in accessors)
            {
                foreach (var handledMember in accessor.GetHandledMembers())
                {
                    var memberId = handledMember.GetUniqueId();
                    memberAccessorOverrides[memberId] = accessor.GetType().CompileFactoryMethod<IAccessor>();
                }
            }

            var overridesCollections = GetAllInstancesThatImplement<IHaveMembersOverrides>();

            foreach (var collection in overridesCollections)
            {
                foreach (var memberOverride in collection.GetOverrides())
                {
                    memberAccessorOverrides[memberOverride.UniqueId] = memberOverride.MemberAccessorFactory;
                }
            }
        }
        private static IEnumerable<T> GetAllInstancesThatImplement<T>() where T : class
        {
            var type = typeof(T);
            var types = type.Assembly.GetTypes().Where(p => type.IsAssignableFrom(p) && !p.IsInterface).ToList();
            var instances = types.Select(x => Activator.CreateInstance(x) as T);
            return instances;
        }



        public static IAccessor CreateMemberAccessor(MethodInfo getMethod, MethodInfo setMethod)
        {
            var memberAccessor = CreateMemberAccessor(getMethod);

            memberAccessor.UniqueId = getMethod.GetUniqueId();
            if (string.IsNullOrEmpty(memberAccessor.DefaultInvocation.Syntax))
            {
                memberAccessor.DefaultInvocation.Syntax = "item." + getMethod.GenerateInvocation();
            }

            return memberAccessor;
        }

        private static IAccessor CreateMemberAccessor(MethodInfo getMethod)
        {
            if (memberAccessorOverrides.TryGetValue(getMethod.GetUniqueId(), out Func<IAccessor> factory))
            {
                return factory();
            }            

            if (getMethod.IsStatic)
            {
                return new MemberAccessorForStatic(getMethod);
            }

            if (getMethod.ReturnType == typeof(void))
            {
                return new MemberAccessorForNotExposed(getMethod);
            }

            var @params = getMethod.GetParameters();

            if (@params.Length == 0)
            {
                var genericFactory = GenericFactory.GetInstance(getMethod.DeclaringType, getMethod.ReturnType);
                var accessor = genericFactory.CreateMemberAccessorByRefCompiled(getMethod);
                return accessor;
            }

            if (@params.Length == 1)
            {
                if (MemberAccessorByIteration.HandledParameterTypes.Contains(@params[0].ParameterType) || @params[0].ParameterType.IsEnum)
                {
                    var genericFactory = GenericFactory.GetInstance(getMethod.DeclaringType, getMethod.ReturnType);
                    var accessor = genericFactory.CreateMemberAccessorByIteration(getMethod);
                    return accessor;
                }
            }
            if (@params.All(x => MemberAccessorByRef.HandledParameterTypes.Contains(x.ParameterType)))
            {
                return new MemberAccessorByRef(getMethod);
            }

            return new MemberAccessorForNotExposed(getMethod);
        }
    }

    internal interface ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> GetHandledMembers();       
    }

    internal interface IHaveMembersOverrides
    {
        IEnumerable<IMemberOverride> GetOverrides();
    }
}