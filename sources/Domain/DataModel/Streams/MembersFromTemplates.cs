using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal static class MembersFromTemplates
    {
        private static readonly Dictionary<Type, List<ISnoopableMemberTemplate>> forTypes = new();

        public static void Init()
        {

        }


        static MembersFromTemplates()
        {
            var memberTemplateFactories = GetAllInstancesThatImplement<IHaveMemberTemplates>();

            foreach (var factory in memberTemplateFactories)
            {
                foreach (var template in factory.GetTemplates())
                {
                    RegisterTemplate(template);
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


        public static IEnumerable<SnoopableMember> Stream(SnoopableObject snoopableObject)
        {
            foreach (var member in CreateSnoopableMembersFor(snoopableObject))
            {
                yield return member;
            }
        }

        private static IEnumerable<SnoopableMember> CreateSnoopableMembersFor(SnoopableObject snoopableObject)
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
                            var member = new SnoopableMember(snoopableObject, template.Data.MemberKind, template.Data.Name, template.Data.DeclaringType, template.Data.MemberAccessor, template.Data.DocumentationFactoryMethod);
                            yield return member;
                        }
                    }
                }
            }
        }
    }

    internal interface IHaveMemberTemplates
    {
        IEnumerable<ISnoopableMemberTemplate> GetTemplates();
    }
}