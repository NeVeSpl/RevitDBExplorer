using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface IMemberAccessor
    {
        ReadResult Read(Document document, object @object);      
        IEnumerable<SnoopableObject> Snoop(Document document, object @object);
    }


    internal abstract class MemberAccessorTyped<TSnoopedObjectType> : IMemberAccessor
    {
        ReadResult IMemberAccessor.Read(Document document, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);      
            var typedObject = (TSnoopedObjectType) @object;          
            return Read(document, typedObject);
        }
        public abstract ReadResult Read(Document document, TSnoopedObjectType @object);

        IEnumerable<SnoopableObject> IMemberAccessor.Snoop(Document document, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType) @object;            
            return Snoop(document, typedObject);
        }
        public abstract IEnumerable<SnoopableObject> Snoop(Document document, TSnoopedObjectType @object);
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