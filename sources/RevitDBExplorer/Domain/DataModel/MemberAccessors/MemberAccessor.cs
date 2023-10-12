using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{

    internal abstract class MemberAccessor<TSnoopedObjectType> : IAccessor
    {
        IValueViewModel IAccessor.CreatePresenter(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return CreatePresenter(context, typedObject);
        }
        public virtual IValueViewModel CreatePresenter(SnoopableContext context, TSnoopedObjectType typedObject) => null;
    }

    internal abstract class MemberAccessorTyped<TSnoopedObjectType> : MemberAccessor<TSnoopedObjectType>, IAccessorWithSnoop
    {
        public override IValueViewModel CreatePresenter(SnoopableContext context, TSnoopedObjectType @object)
        {            
            return new DefaultPresenter(this);
        }

        ReadResult IAccessorWithSnoop.Read(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);      
            var typedObject = (TSnoopedObjectType) @object;          
            return Read(context, typedObject);
        }
        public abstract ReadResult Read(SnoopableContext context, TSnoopedObjectType typedObject);

        IEnumerable<SnoopableObject> IAccessorWithSnoop.Snoop(SnoopableContext context, object @object, IValueContainer state)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType) @object;            
            return Snoop(context, typedObject, state) ?? Enumerable.Empty<SnoopableObject>();
        }
        public virtual IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType typedObject, IValueContainer state) => null;
    }
}