using System;
using System.Collections;
using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal sealed class SystemTypeStream
    {
        public bool EndStream { get; private set; } = false;

        public IEnumerable<SnoopableMember> Stream(SnoopableObject snoopableObject)
        {
            var type = snoopableObject.Object.GetType();

            if ((type.IsEnum) || (type.IsPrimitive) || (type == typeof(string)))
            {
                var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.Property, "Value", type, new MemberAccessorForPrimitive(type), null);                
                yield return member;
                EndStream = true;                
            }

            if (snoopableObject.Object is IList list)
            {
                Type itemType = typeof(object);
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    itemType = genericArgs[1];
                }

                for (int i = 0; i < list.Count; i++)
                {                   
                    var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.Property, i.ToString(), type, new MemberAccessorWithConst(itemType, snoopableObject.Document, list[i]), null);
                    yield return member;
                }
                EndStream = true;
            }

            if (snoopableObject.Object is IDictionary dict)
            {
                Type itemType = typeof(object);
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 2)
                {
                    itemType = genericArgs[1];
                }

                foreach (DictionaryEntry item in dict)
                {
                    var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.Property, item.Key.ToString(), type, new MemberAccessorWithConst(itemType, snoopableObject.Document, item.Value), null);                   
                    yield return member;
                }
                EndStream = true;
            }
        }
    }
}