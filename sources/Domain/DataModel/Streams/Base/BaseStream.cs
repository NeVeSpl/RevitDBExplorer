using System;
using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams.Base
{
    internal abstract class BaseStream
    {
        private readonly Dictionary<Type, List<ISnoopableMemberTemplate>> templates = new();


        protected void RegisterTemplates(Type type, IEnumerable<ISnoopableMemberTemplate> templatesToRegister)
        {
            List<ISnoopableMemberTemplate> list;
            if (!templates.TryGetValue(type, out  list))
            {
                list = new List<ISnoopableMemberTemplate>();
                templates[type] = list;
            }
            list.AddRange(templatesToRegister);
        }


        public virtual IEnumerable<SnoopableMember> Stream(SnoopableObject snoopableObject)
        {
            foreach (var keyValue in templates)
            {
                if (keyValue.Key.IsAssignableFrom(snoopableObject.Object.GetType()))
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
        public virtual bool ShouldEndAllStreaming() => false;
    }
}