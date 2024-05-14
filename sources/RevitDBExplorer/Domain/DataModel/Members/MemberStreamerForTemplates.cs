using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members
{
    internal static class MemberStreamerForTemplates
    {
        private static readonly Dictionary<Type, List<ISnoopableMemberTemplate>> forTypes = new();

        public static void Init()
        {

        }
        static MemberStreamerForTemplates()
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
            if (!forTypes.TryGetValue(template.Descriptor.ForType, out List<ISnoopableMemberTemplate> list))
            {
                list = new List<ISnoopableMemberTemplate>();
                forTypes[template.Descriptor.ForType] = list;
            }
            list.Add(template);
        }


        private static readonly Dictionary<Type, IReadOnlyList<ISnoopableMemberTemplate>> Cache_Templates = new();
        public static IEnumerable<MemberDescriptor> Stream(object snoopableObject)
        {
            var objectType = snoopableObject.GetType();
            var templates = Cache_Templates.GetOrCreate(objectType, x => StreamTemplates(x).ToArray());
            foreach (var template in templates)
            {
                if (template.CanBeUsedWith(snoopableObject))
                {                            
                    yield return template.Descriptor;
                }
            }
        }
        private static IEnumerable<ISnoopableMemberTemplate> StreamTemplates(Type objectType)
        {         
            foreach (var keyValue in forTypes)
            {
                if (keyValue.Key.IsAssignableFrom(objectType))
                {
                    foreach (var template in keyValue.Value)
                    {
                        yield return template;
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