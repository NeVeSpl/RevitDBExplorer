using System;
using System.Collections.Generic;
using System.Linq;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface IMemberAccessor
    {
        ReadResult Read(SnoopableContext context, object @object);      
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object);
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
        public string Value { get; init; }
        public string ValueTypeName { get; init; }
        public bool CanBeSnooped { get; init; }
        public Exception Exception { get; init; }

        public ReadResult(string value, string valueTypeName, bool canBeSnooped, Exception exception = null)
        {
            Value = value;
            ValueTypeName = valueTypeName;
            CanBeSnooped = canBeSnooped;
            Exception = exception;
        }
    }
}