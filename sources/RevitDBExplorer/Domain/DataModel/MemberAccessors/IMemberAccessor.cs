using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface IMemberAccessor
    { 
        IValueViewModel CreatePresenter(SnoopableContext context, object @object);  
    }
    internal interface IMemberAccessorWithSnoop
    {
        ReadResult Read(SnoopableContext context, object @object);
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object, IValueContainer state);
    }    
    internal interface IMemberAccessorWithWrite
    {
        bool CanBeWritten(SnoopableContext context, object @object);      
    }


    internal abstract class MemberAccessorTyped<TSnoopedObjectType> : IMemberAccessor, IMemberAccessorWithSnoop
    {
        IValueViewModel IMemberAccessor.CreatePresenter(SnoopableContext context, object @object)
        {            
            return new DefaultPresenter(this);
        }

        ReadResult IMemberAccessorWithSnoop.Read(SnoopableContext context, object @object)
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
        public abstract IValueEditor CreateEditor(SnoopableContext context, TSnoopedObjectType typedObject);
            

        bool IMemberAccessorWithWrite.CanBeWritten(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return CanBeWritten(context, typedObject);
        }
        public abstract bool CanBeWritten(SnoopableContext context, TSnoopedObjectType typedObject);
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