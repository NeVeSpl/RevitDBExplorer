using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Accessors
{
    internal abstract class MemberAccessorTyped<TSnoopedObjectType> : IAccessor
    {
        public string UniqueId { get; set; }
        public Invocation DefaultInvocation { get; } = new Invocation();

        IValueViewModel IAccessor.CreatePresenter(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return CreatePresenter(context, typedObject);
        }
        protected virtual IValueViewModel CreatePresenter(SnoopableContext context, TSnoopedObjectType typedObject) => null;
    }


    internal abstract class MemberAccessorTypedWithDefaultPresenter<TSnoopedObjectType> : MemberAccessorTyped<TSnoopedObjectType>, IAccessorForDefaultPresenter
    {
        protected override IValueViewModel CreatePresenter(SnoopableContext context, TSnoopedObjectType @object)
        {
            return new DefaultPresenter(this);
        }

        ReadResult IAccessorForDefaultPresenter.Read(SnoopableContext context, object @object)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            var readResult = Read(context, typedObject);
            if (string.IsNullOrEmpty(readResult.AccessorName))
            {
                readResult.AccessorName = $"[{this.GetType().Name}]";
            }

            return readResult;
        }
        protected abstract ReadResult Read(SnoopableContext context, TSnoopedObjectType typedObject);

        IEnumerable<SnoopableObject> IAccessorForDefaultPresenter.Snoop(SnoopableContext context, object @object, IValueContainer state)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return Snoop(context, typedObject, state) ?? Enumerable.Empty<SnoopableObject>();
        }
        protected virtual IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType typedObject, IValueContainer state)
        {
            return state?.Snoop();
        }

        IEnumerable<VisualizationItem> IAccessorForDefaultPresenter.GetVisualization(SnoopableContext context, object @object, IValueContainer state)
        {
            Guard.IsAssignableToType<TSnoopedObjectType>(@object);
            var typedObject = (TSnoopedObjectType)@object;
            return GetVisualization(context, typedObject, state) ?? Enumerable.Empty<VisualizationItem>();
        }
        protected virtual IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, TSnoopedObjectType typedObject, IValueContainer state)
        {
            return state?.GetVisualization();
        }
    }
}