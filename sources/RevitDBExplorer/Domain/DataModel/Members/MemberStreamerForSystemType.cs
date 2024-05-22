using System;
using System.Collections;
using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.MembersOverrides;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members
{
    internal static class MemberStreamerForSystemType
    {
        public static bool IsSystemType(object target)
        {
            var type = target?.GetType();

            if (target is Type { IsEnum: true } runtimeType)
            {
                return true;
            }

            return false;
        }




        public static IEnumerable<MemberDescriptor> Stream(SnoopableContext context, object target)
        {
            var type = target.GetType();

            if (target is Type { IsEnum : true } runtimeType)
            {                
                var values = Enum.GetValues(runtimeType);
                var isLong = Enum.GetUnderlyingType(runtimeType) == typeof(long);

                foreach (var item in values)
                {
                    var enumLabel = item.ToString();
                    var value =  Convert.ToInt64(item);
                    var valueLabel = value.ToString();

                    var member = new MemberDescriptor(runtimeType, MemberKind.Property, valueLabel, runtimeType, new MemberAccessorForConstValue(typeof(string), context, enumLabel), null);
                    yield return member;
                }
            }
         

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