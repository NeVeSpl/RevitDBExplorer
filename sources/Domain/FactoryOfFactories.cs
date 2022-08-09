using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class FactoryOfFactories
    {
        private static readonly Dictionary<Type, List<ISnoopableMemberTemplate>> forTypes = new();
        private static readonly Dictionary<string, Func<IMemberAccessor>> forTypeMembers = new();


        public static void Init()
        {

        }


        static FactoryOfFactories()
        {
            var memberTemplateFactories = GetAllInstancesThatImplement<IHaveMemberTemplates>();

            foreach (var factory in memberTemplateFactories)
            {
                foreach (var template in factory.GetTemplates())
                {
                    RegisterTemplate(template);
                }    
            }

            var accessors = GetAllInstancesThatImplement<ICanCreateMemberAccessor>();
            foreach (var accessor in accessors)
            {
                foreach (var handledMember in accessor.GetHandledMembers())
                {
                    var memberId = handledMember.GetUniqueId();
                    forTypeMembers[memberId] = accessor.GetType().CompileFactoryMethod<IMemberAccessor>();
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
        private static void RegisterTemplate(ISnoopableMemberTemplate template)
        {
            if (!forTypes.TryGetValue(template.ForType, out List<ISnoopableMemberTemplate> list))
            {
                list = new List<ISnoopableMemberTemplate>();
                forTypes[template.ForType] = list;
            }
            list.Add(template);
        }

        public static IEnumerable<SnoopableMember> CreateSnoopableMembersFor(SnoopableObject snoopableObject)
        {
            var objectType = snoopableObject.Object.GetType();
            foreach (var keyValue in forTypes)
            {
                if (keyValue.Key.IsAssignableFrom(objectType))
                {
                    foreach (var template in keyValue.Value)
                    {
                        if (template.CanBeUsedWith(snoopableObject.Object))
                        {
                            var member = new SnoopableMember(snoopableObject, template.SnoopableMember);
                            yield return member;
                        }
                    }
                }
            }
        }
        public static IMemberAccessor CreateMemberAccessor(MethodInfo getMethod, MethodInfo setMethod)
        {
            if (forTypeMembers.TryGetValue(getMethod.GetUniqueId(), out Func<IMemberAccessor> factory))
            {
                return factory();
            }

            var @params = getMethod.GetParameters();
            if (@params.Length == 1)
            {
                if (MemberAccessorByIteration.HandledParameterTypes.Contains(@params[0].ParameterType) || @params[0].ParameterType.IsEnum)
                {
                    return new MemberAccessorByIteration(getMethod);
                }
            }
            if (@params.All(x => MemberAccessorByRef.HandledParameterTypes.Contains(x.ParameterType)))
            {
                return new MemberAccessorByRef(getMethod, setMethod);
            }
            
            return null;
        }
    }


    internal interface IHaveMemberTemplates
    {
        IEnumerable<ISnoopableMemberTemplate> GetTemplates();
    }
}