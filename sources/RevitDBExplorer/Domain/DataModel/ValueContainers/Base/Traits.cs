using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal interface ISnoop
    {
        bool CanBeSnooped(SnoopableContext context, object value);
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object value);
    }
    internal interface ISnoop<in T>
    {
        bool CanBeSnooped(SnoopableContext context, T value);
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, T value);
    }


    internal interface IHaveLabel
    {
        string ToLabel(SnoopableContext context, object value);
    }
    internal interface IHaveLabel<in T>
    {
        string ToLabel(SnoopableContext context, T value);
    }  


    internal interface IHaveToolTip<in T>
    {
        string GetToolTip(SnoopableContext context, T value);
    }


    internal interface IHaveVisualization
    {
        bool CanBeVisualized(SnoopableContext context, object value);
        IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, object value);
    }
    internal interface IHaveVisualization<in T>
    {
        bool CanBeVisualized(SnoopableContext context, T value);
        IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, T value);
    }
}