using System;
using System.Collections;
using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal static class MemberStreamerForSystemType
    {
        public static IEnumerable<MemberDescriptor> Stream(SnoopableContext context, object target)
        {
            var type = target.GetType();
         

            if ((type.IsEnum) || (type.IsPrimitive) || (type == typeof(string)))
            {
                var member = new MemberDescriptor(type, MemberKind.Property, "Value", type, new MemberAccessorForConstValue(type, context, target), null);                
                yield return member;                            
            }

            if (target is IList list)
            {
                Type itemType = typeof(object);
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    itemType = genericArgs[0];
                }

                for (int i = 0; i < Math.Min(list.Count, 9999); i++)
                {                   
                    var member = new MemberDescriptor(type, MemberKind.Property, i.ToString(), type, new MemberAccessorForConstValue(itemType, context, list[i]), null);
                    yield return member;
                }
                if (list.Count == 0)
                {
                    yield return new MemberDescriptor(type, MemberKind.Property, "<list is empty>", type, new MemberAccessorForConstValue(itemType, context, null), null);
                }
            }

            if (target is IDictionary dict)
            {
                Type itemType = typeof(object);
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 2)
                {
                    itemType = genericArgs[1];
                }

                foreach (DictionaryEntry item in dict)
                {
                    var member = new MemberDescriptor(type, MemberKind.Property, item.Key.ToString(), type, new MemberAccessorForConstValue(itemType, context, item.Value), null);                   
                    yield return member;
                }
                if (dict.Count == 0)
                {
                    yield return new MemberDescriptor(type, MemberKind.Property, "<dictionary is empty>", type, new MemberAccessorForConstValue(itemType, context, null), null);
                }
            }
        }
    }
}