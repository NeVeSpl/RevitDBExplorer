using System;
using System.Collections.Generic;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal interface IValueContainer
    {
        Type Type { get; }
        Type TypeHandlerType { get; }
        string TypeHandlerName { get; }
        IValueContainer SetValue(SnoopableContext context, object value);


        string ValueAsString { get; }
        bool CanBeSnooped { get; }
        bool CanBeVisualized { get; }
        string ToolTip { get;  }
        IEnumerable<SnoopableObject> Snoop();
        IEnumerable<DrawingVisual> GetVisualization();
    } 
}