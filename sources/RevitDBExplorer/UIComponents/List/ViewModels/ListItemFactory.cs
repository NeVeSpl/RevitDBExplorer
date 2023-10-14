using System;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List.ViewModels
{
    internal static class ListItemFactory
    {
        public static IListItem Create(SnoopableItem left, SnoopableItem right, Action askForReload, bool doCompare = false)
        {
            if (left is SnoopableMember)
            {
                return new ListItemForMember(left as SnoopableMember, right as SnoopableMember, askForReload, doCompare);
            }
            return new ListItemForParameter(left as SnoopableParameter, right as SnoopableParameter, askForReload, doCompare);
        }
    }
}