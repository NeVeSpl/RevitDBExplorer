using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Accessors
{
    internal interface IAccessorForDefaultPresenter : IAccessor
    {
        ReadResult Read(SnoopableContext context, object @object);
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object, IValueContainer state);
        IEnumerable<DrawingVisual> GetVisualization(SnoopableContext context, object @object, IValueContainer state);
    }
}