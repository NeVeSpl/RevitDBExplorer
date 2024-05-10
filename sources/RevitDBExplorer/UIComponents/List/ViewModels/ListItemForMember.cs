using System;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List.ViewModels
{
    internal sealed class ListItemForMember : ListItem<SnoopableMember>
    {        
        public string Icon => $"Icon{SnoopableItem.MemberKind}";
        public DeclaringType DeclaringType => SnoopableItem.DeclaringType;
        public RevitDBExplorer.Domain.DocXml Documentation => SnoopableItem.Documentation;
        

        public ListItemForMember(SnoopableMember left, SnoopableMember right, Action askForReload, bool doCompare) : base(left, right, askForReload, doCompare) 
        { 
            SortingKey = $"{SnoopableItem.DeclaringType.InheritanceLevel:000}_{(int)SnoopableItem.MemberKind}_{SnoopableItem.Name}";
            GroupingKey = SnoopableItem.DeclaringType.Name;
        }
    }
}