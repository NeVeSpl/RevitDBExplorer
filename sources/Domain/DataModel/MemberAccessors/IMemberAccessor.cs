using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    internal interface IMemberAccessorWithWrite
    {
        void Write(SnoopableContext context, object @object);
        bool CanBeWritten(SnoopableContext context, object @object);
    }


    internal abstract class MemberAccessorTyped<TSnoopedObjectType> : IMemberAccessor
    {
        ReadResult IMemberAccessor.Read(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);      
            var typedObject = (TSnoopedObjectType) @object;          
            return Read(context, typedObject);
        }
        public abstract ReadResult Read(SnoopableContext context, TSnoopedObjectType typedObject);

        IEnumerable<SnoopableObject> IMemberAccessor.Snoop(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType) @object;            
            return Snoop(context, typedObject) ?? Enumerable.Empty<SnoopableObject>();
        }
        public virtual IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType typedObject) => null;
    }


    internal abstract class MemberAccessorTypedWithWrite<TSnoopedObjectType> : MemberAccessorTyped<TSnoopedObjectType>, IMemberAccessorWithWrite
    {
        void IMemberAccessorWithWrite.Write(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            Write(context, typedObject);
        }
        public abstract void Write(SnoopableContext context, TSnoopedObjectType typedObject);

        bool IMemberAccessorWithWrite.CanBeWritten(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return CanBeWritten(context, typedObject);
        }
        public abstract bool CanBeWritten(SnoopableContext context, TSnoopedObjectType typedObject);
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