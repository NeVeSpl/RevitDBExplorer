using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal interface ICanBeSnooped
    {
        bool CanBeSnooped(SnoopableContext context, object value);
    }
    internal interface ICanBeSnooped<in T>
    {
        bool CanBeSnooped(SnoopableContext context, T value);
    }

    internal interface IToLabel
    {
        string ToLabel(SnoopableContext context, object value);
    }
    internal interface IToLabel<in T>
    {
        string ToLabel(SnoopableContext context, T value);
    }

    internal interface ISnoop
    {
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object value);
    }
    internal interface ISnoop<in T>
    {
        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, T value);
    }


    internal interface IHaveToolTip<in T>
    {
        string GetToolTip(SnoopableContext context, T value);
    }
}