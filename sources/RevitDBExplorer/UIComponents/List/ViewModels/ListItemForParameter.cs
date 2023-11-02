using System;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List.ViewModels
{
    internal sealed class ListItemForParameter : ListItem<SnoopableParameter>
    {
        public ListItemForParameter(SnoopableParameter left, SnoopableParameter right, Action askForReload, bool doCompare) : base(left, right, askForReload, doCompare)
        {
            SortingKey = $"{(int)SnoopableItem.Orgin}_{SnoopableItem.Name}";
            GroupingKey = SnoopableItem.Orgin.ToString();
        }


        public SourceOfObjects CreateSnoopParameter()
        {
            return SnoopableItem.SnoopParameter();
        }
    }
}