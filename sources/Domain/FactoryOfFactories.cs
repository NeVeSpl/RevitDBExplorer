using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly Dictionary<string, ICanCreateMemberAccessor> forTypeMembers = new();


        public static void Init()
        {

        }


        static FactoryOfFactories()
        {
            var memberTemplateFactories = GetFactoriesOfType<IHaveMemberTemplates>();

            foreach (var factory in memberTemplateFactories)
            {
                foreach (var template in factory.GetTemplates())
                {
                    RegisterTemplate(template);
                }    
            }

            var accessorsFactories = GetFactoriesOfType<ICanCreateMemberAccessor>();
            forTypeMembers = accessorsFactories.SelectMany(x => x.GetHandledMembers(), (factory, key) => (factory, key)).ToDictionary(x => x.key, x => x.factory);
        }


        private static IEnumerable<T> GetFactoriesOfType<T>() where T : class
        {
            var type = typeof(T);
            var factoryTypes = type.Assembly.GetTypes().Where(p => type.IsAssignableFrom(p) && !p.IsInterface).ToList();
            var factoryinstances = factoryTypes.Select(x => Activator.CreateInstance(x) as T);
            return factoryinstances;
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
                        if (template.CanBeUsed(snoopableObject.Object))
                        {
                            var member = new SnoopableMember(snoopableObject, template.Kind, template.MemberName, template.DeclaringType, template.MemberAccessor, null);
                            yield return member;
                        }
                    }
                }
            }
        }
        public static IMemberAccessor CreateMemberAccessor(MethodInfo getMethod, MethodInfo setMethod)
        {
            if (forTypeMembers.TryGetValue(getMethod.GetUniqueId(), out ICanCreateMemberAccessor factory))
            {
                return factory.Create();
            }

            return new MemberAccessorByRef(getMethod, setMethod);
        }
    }


    internal interface IHaveMemberTemplates
    {
        IEnumerable<ISnoopableMemberTemplate> GetTemplates();
    }
}