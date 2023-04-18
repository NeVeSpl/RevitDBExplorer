using System;
using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal interface IValueContainer
    {
        Type Type { get; }
        Type TypeHandlerType { get; }
        string TypeName { get; }
        IValueContainer SetValue(SnoopableContext context, object value);


        string ValueAsString { get; }
        bool CanBeSnooped { get; }
        string ToolTip { get;  }
        IEnumerable<SnoopableObject> Snoop();
    } 
}