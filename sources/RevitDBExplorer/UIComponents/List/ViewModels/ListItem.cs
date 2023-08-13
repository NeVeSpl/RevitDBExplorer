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
        private readonly SnoopableMember snoopableMember;
           

        public override string Name => snoopableMember.Name;
        public string Icon => $"Icon{snoopableMember.MemberKind}";
        public DeclaringType DeclaringType => snoopableMember.DeclaringType;
        public RevitDBExplorer.Domain.DocXml Documentation => snoopableMember.Documentation;
        public SnoopableMember this[int i]
        {
            get { return snoopableMember; }            
        }


        public ListItemForSM(SnoopableMember x)
        {
            this.snoopableMember = x;
            SortingKey = $"{x.DeclaringType.InheritanceLevel:000}_{(int)x.MemberKind}_{x.Name}";
            GroupingKey = x.DeclaringType.Name;
        }



        public override bool Filter(string filterPhrase)
        {
            bool inName = snoopableMember.Name.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            bool inValue = snoopableMember.ValueViewModel is IValuePresenter valuePresenter && valuePresenter.Label.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            return inName || inValue;
        }
        public override void Read()
        {
            snoopableMember.Read();            
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
