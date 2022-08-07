using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.UIComponents.List.ValueEditors;
using RevitDBExplorer.UIComponents.List.ValuePresenters;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface IMemberAccessor
    {
        IValuePresenter GetPresenter(SnoopableContext context, object @object);
        ReadResult Read(SnoopableContext context, object @object);   
    }

    internal interface IMemberAccessorWithSnoop
    {
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object);
    }
    internal interface IMemberAccessorWithValue
    {
        IValueContainer Value { get; }
    }
    internal interface IMemberAccessorWithWrite
    {
        bool CanBeWritten(SnoopableContext context, object @object);
        void Write(SnoopableContext context, object @object);    
    }


    internal abstract class MemberAccessorTyped<TSnoopedObjectType> : IMemberAccessor, IMemberAccessorWithSnoop
    {
        IValuePresenter IMemberAccessor.GetPresenter(SnoopableContext context, object @object)
        { 
            if (this is IMemberAccessorWithValue reader)
            {
                return new DefaultPresenterVM() { ValueContainer = reader.Value };
            }
            return new DefaultPresenterVM();
        }

        ReadResult IMemberAccessor.Read(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);      
            var typedObject = (TSnoopedObjectType) @object;          
            return Read(context, typedObject);
        }
        public abstract ReadResult Read(SnoopableContext context, TSnoopedObjectType typedObject);

        IEnumerable<SnoopableObject> IMemberAccessorWithSnoop.Snoop(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType) @object;            
            return Snoop(context, typedObject) ?? Enumerable.Empty<SnoopableObject>();
        }
        public virtual IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType typedObject) => null;
    }

    internal abstract class MemberAccessorTypedWithWrite<TSnoopedObjectType> : IMemberAccessor,  IMemberAccessorWithWrite
    {
        IValuePresenter IMemberAccessor.GetPresenter(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return GetEditor(context, typedObject);
        }
        public virtual IValueEditor GetEditor(SnoopableContext context, TSnoopedObjectType typedObject) => new ExecuteEditorVM();

        ReadResult IMemberAccessor.Read(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            Read(context, typedObject);
            return new ReadResult("", this.GetType().GetCSharpName(), false);
        }
        public abstract void Read(SnoopableContext context, TSnoopedObjectType typedObject);

        bool IMemberAccessorWithWrite.CanBeWritten(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return CanBeWritten(context, typedObject);
        }
        public abstract bool CanBeWritten(SnoopableContext context, TSnoopedObjectType typedObject);

        void IMemberAccessorWithWrite.Write(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            Write(context, typedObject);
        }
        public abstract void Write(SnoopableContext context, TSnoopedObjectType typedObject); 
    }


    internal readonly ref struct ReadResult
    {
        public string Label { get; init; }
        public string AccessorName { get; init; }
        public bool CanBeSnooped { get; init; }
     

        public ReadResult(string value, string accessorName, bool canBeSnooped)
        {
            Label = value;
            AccessorName = accessorName;
            CanBeSnooped = canBeSnooped;          
        }
    }

    internal interface IValuePresenter
    {
    }
    internal interface IValueEditor : IValuePresenter
    {
    }    
}