using System;
using System.Collections;
using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal sealed class SystemTypeStream : BaseStream
    {
        private bool shouldEndAllStreaming = false;
        public override bool ShouldEndAllStreaming() => shouldEndAllStreaming;


        public override IEnumerable<SnoopableMember> Stream(SnoopableObject snoopableObject)
        {
            var type = snoopableObject.Object.GetType();
            shouldEndAllStreaming = false;

            if ((type.IsEnum) || (type.IsPrimitive) || (type == typeof(string)))
            {
                var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.Property, "Value", type, new MemberAccessorForConstValue(type, snoopableObject.Document, snoopableObject.Object), null);                
                yield return member;
                shouldEndAllStreaming = true;                
            }

            if (snoopableObject.Object is IList list)
            {
                Type itemType = typeof(object);
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    itemType = genericArgs[0];
                }

                for (int i = 0; i < list.Count; i++)
                {                   
                    var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.Property, i.ToString(), type, new MemberAccessorForConstValue(itemType, snoopableObject.Document, list[i]), null);
                    yield return member;
                }
                shouldEndAllStreaming = true;
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
                    var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.Property, item.Key.ToString(), type, new MemberAccessorForConstValue(itemType, snoopableObject.Document, item.Value), null);                   
                    yield return member;
                }
                shouldEndAllStreaming = true;
            }
        }
    }
}