using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ViewModels;
using RevitDBExplorer.Domain.DataModel.ViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface IMemberAccessor
    { 
        IValueViewModel CreatePresenter(SnoopableContext context, object @object);          
        ReadResult Read(SnoopableContext context, object @object, IValueViewModel presenter);   
    }
    internal interface IMemberAccessorWithSnoop
    {
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object, IValueContainer state);
    }
    
    internal interface IMemberAccessorWithWrite
    {
        bool CanBeWritten(SnoopableContext context, object @object);
        void Write(SnoopableContext context, object @object, IValueViewModel presenter);    
    }


    internal abstract class MemberAccessorTyped<TSnoopedObjectType> : IMemberAccessor, IMemberAccessorWithSnoop
    {
        IValueViewModel IMemberAccessor.CreatePresenter(SnoopableContext context, object @object)
        {            
            return new DefaultPresenterVM();
        }

        ReadResult IMemberAccessor.Read(SnoopableContext context, object @object, IValueViewModel presenter)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);      
            var typedObject = (TSnoopedObjectType) @object;          
            return Read(context, typedObject);
        }
        public abstract ReadResult Read(SnoopableContext context, TSnoopedObjectType typedObject);

        IEnumerable<SnoopableObject> IMemberAccessorWithSnoop.Snoop(SnoopableContext context, object @object, IValueContainer state)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType) @object;            
            return Snoop(context, typedObject, state) ?? Enumerable.Empty<SnoopableObject>();
        }
        public virtual IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType typedObject, IValueContainer state) => null;
    }

    internal abstract class MemberAccessorTypedWithWrite<TSnoopedObjectType> : IMemberAccessor,  IMemberAccessorWithWrite
    {
        IValueViewModel IMemberAccessor.CreatePresenter(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return CreateEditor(context, typedObject);
        }
        public virtual IValueEditor CreateEditor(SnoopableContext context, TSnoopedObjectType typedObject) => new ExecuteEditorVM();

        ReadResult IMemberAccessor.Read(SnoopableContext context, object @object, IValueViewModel presenter)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            Read(context, typedObject, presenter as IValueEditor);
            return new ReadResult("", this.GetType().GetCSharpName(), false);
        }
        public abstract void Read(SnoopableContext context, TSnoopedObjectType typedObject, IValueEditor valueEditor);

        bool IMemberAccessorWithWrite.CanBeWritten(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return CanBeWritten(context, typedObject);
        }
        public abstract bool CanBeWritten(SnoopableContext context, TSnoopedObjectType typedObject);

        void IMemberAccessorWithWrite.Write(SnoopableContext context, object @object, IValueViewModel presenter)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            Write(context, typedObject, presenter as IValueEditor);
        }
        public abstract void Write(SnoopableContext context, TSnoopedObjectType typedObject, IValueEditor valueEditor); 
    }


    internal readonly ref struct ReadResult
    {
        public string Label { get; init; }
        public string AccessorName { get; init; }
        public bool CanBeSnooped { get; init; }
        public IValueContainer State { get; init; }


        public ReadResult(string value, string accessorName, bool canBeSnooped, IValueContainer state = null)
        {
            Label = value;
            AccessorName = accessorName;
            CanBeSnooped = canBeSnooped; 
            State = state;
        }
    }
}