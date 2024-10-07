using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Accessors
{
    internal abstract class MemberAccessorByType<TSnoopedObjectType> : MemberAccessorTypedWithDefaultPresenter<TSnoopedObjectType> where TSnoopedObjectType : class
    {
        protected sealed override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType @object, IValueContainer state)
        {
            return Snoop(context, @object);
        }
        protected virtual IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType value) => null;

        protected sealed override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, TSnoopedObjectType @object, IValueContainer state)
        {
            return GetVisualization(context, @object);
        }
        protected virtual IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, TSnoopedObjectType @object) => null;
    }

    /*
    internal abstract class MemberAccessorByTypeLambda<TSnoopedObjectType> : MemberAccessorByType<TSnoopedObjectType> where TSnoopedObjectType : class
    {
        public Overrides Override { get; } = new Overrides();

        protected override bool CanBeSnoooped(Document document, TSnoopedObjectType value)
        {
#if NET
            ArgumentNullException.ThrowIfNull(Override.CanBeSnooped);
#endif
            return Override.CanBeSnooped(document, value);
        }
        protected override string GetLabel(Document document, TSnoopedObjectType value)
        {
#if NET
            ArgumentNullException.ThrowIfNull(Override.GetLabel);
#endif
            return Override.GetLabel(document, value);
        }
        protected override IEnumerable<SnoopableObject> Snoop(Document document, TSnoopedObjectType value)
        {
            if (Override.Snoop == null) return null;
            return Override.Snoop(document, value);
        }


        public class Overrides
        {
            public Func<Document, TSnoopedObjectType, bool> CanBeSnooped { get; set; }
            public Func<Document, TSnoopedObjectType, string> GetLabel { get; set; }            
            public Func<Document, TSnoopedObjectType, IEnumerable<SnoopableObject>> Snoop { get; set; }
        }
    }
    */
}