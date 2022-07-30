using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface IMemberAccessor
    {
        ReadResult Read(SnoopableContext context, object @object);      
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object);
    }


    internal interface IMemberAccessorWithValue
    {
        IValueContainer Value { get; }
    }


    internal abstract class MemberAccessorTyped<TSnoopedObjectType> : IMemberAccessor
    {
        ReadResult IMemberAccessor.Read(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);      
            var typedObject = (TSnoopedObjectType) @object;          
            return Read(context, typedObject);
        }
        public abstract ReadResult Read(SnoopableContext context, TSnoopedObjectType @object);

        IEnumerable<SnoopableObject> IMemberAccessor.Snoop(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType) @object;            
            return Snoop(context, typedObject) ?? Enumerable.Empty<SnoopableObject>();
        }
        public abstract IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType @object);
    }


    readonly ref struct ReadResult
    {
        public string Label { get; init; }
        public string AccessorName { get; init; }
        public bool CanBeSnooped { get; init; }
     

        public ReadResult(string value, string sccessorName, bool canBeSnooped)
        {
            Label = value;
            AccessorName = sccessorName;
            CanBeSnooped = canBeSnooped;          
        }
    }
}