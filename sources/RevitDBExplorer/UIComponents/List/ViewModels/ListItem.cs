using System;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.Streams.Base;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List.ViewModels
{

    internal interface IListItem
    {
       
    }

    internal class ListItem : BaseViewModel
    {
        public string SortingKey { get; init; }
        public string GroupingKey { get; init; }
        public virtual string Name { get; }

        public virtual bool Filter(string filterPhrase) => true;
        public virtual void Read() 
        {
        }
    }
    internal class ListItemForSM : ListItem, IListItem
    {
        private readonly SnoopableMember leftMember;
        private readonly SnoopableMember rightMember;

        public override string Name => leftMember.Name;
        public string Icon => $"Icon{leftMember.MemberKind}";
        public DeclaringType DeclaringType => leftMember.DeclaringType;
        public RevitDBExplorer.Domain.DocXml Documentation => leftMember.Documentation;
        public SnoopableMember this[int i]
        {
            get 
            {
                return i switch { 0 => leftMember, _ => rightMember };
            }            
        }


        public ListItemForSM(SnoopableMember left, SnoopableMember right, Action askForReload)
        {
            leftMember = left;
            rightMember = right;
            leftMember.SnoopableObjectChanged += () => askForReload();
            SortingKey = $"{left.DeclaringType.InheritanceLevel:000}_{(int)left.MemberKind}_{left.Name}";
            GroupingKey = left.DeclaringType.Name;
        }



        public override bool Filter(string filterPhrase)
        {
            bool inName = leftMember.Name.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            bool inValue = leftMember.ValueViewModel is IValuePresenter valuePresenter && valuePresenter.Label.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            return inName || inValue;
        }
        public override void Read()
        {
            leftMember.Read();            
        }
    }

    internal class ListItemForSP : ListItem, IListItem
    {
        private readonly SnoopableParameter snoopableParameter;

        public ListItemForSP(SnoopableParameter x)
        {
            this.snoopableParameter = x;
        }
    }
}
