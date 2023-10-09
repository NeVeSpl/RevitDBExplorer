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
        private bool isHighlighted;

        public string SortingKey { get; init; }
        public string GroupingKey { get; init; }
        public virtual string Name { get; }
        public bool IsHighlighted
        {
            get
            {
                return isHighlighted;
            }
            set
            {
                isHighlighted = value;
                OnPropertyChanged();
            }
        }


        public virtual bool Filter(string filterPhrase) => true;
        public virtual void Read() 
        {
        }
    }
    internal class ListItemForSM : ListItem, IListItem
    {
        private readonly SnoopableMember leftMember;
        private readonly SnoopableMember rightMember;
        private readonly bool doCompare;

        private SnoopableMember Member => leftMember ?? rightMember;
        public override string Name => Member.Name;
        public string Icon => $"Icon{Member.MemberKind}";
        public DeclaringType DeclaringType => Member.DeclaringType;
        public RevitDBExplorer.Domain.DocXml Documentation => Member.Documentation;
        public SnoopableMember this[int i]
        {
            get 
            {
                return i switch { 0 => leftMember, _ => rightMember };
            }            
        }


        public ListItemForSM(SnoopableMember left, SnoopableMember right, Action askForReload, bool doCompare = false)
        {
            leftMember = left;
            rightMember = right;
            this.doCompare = doCompare;
            if (leftMember != null)
            {
                leftMember.SnoopableObjectChanged += () => askForReload();
            }
            if (rightMember != null)
            {
                rightMember.SnoopableObjectChanged += () => askForReload();
            }

            Compare();
            
            SortingKey = $"{Member.DeclaringType.InheritanceLevel:000}_{(int)Member.MemberKind}_{Member.Name}";
            GroupingKey = Member.DeclaringType.Name;
        }


        private void Compare()
        {
            if (doCompare)
            {
                if ((leftMember?.ValueViewModel is IValuePresenter leftVP) && (rightMember?.ValueViewModel is IValuePresenter rightVp))
                {
                    IsHighlighted = !leftVP.Label.Equals(rightVp.Label);
                }
                else
                {
                    IsHighlighted = true;
                }
                if ((leftMember?.ValueViewModel is IValueEditor) && (rightMember?.ValueViewModel is IValueEditor))
                {
                    IsHighlighted = false;
                }
            }
        }



        public override bool Filter(string filterPhrase)
        {
            bool left = false;
            bool right = false;
            if (leftMember != null)
            {
                bool inName = leftMember.Name.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                bool inValue = leftMember.ValueViewModel is IValuePresenter valuePresenter && valuePresenter.Label.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                left = inName || inValue;
            }
            if (rightMember != null)
            {
                bool inName = rightMember.Name.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                bool inValue = rightMember.ValueViewModel is IValuePresenter valuePresenter && valuePresenter.Label.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                right = inName || inValue;
            }
            return left || right;
        }
        public override void Read()
        {
            leftMember?.Read();
            rightMember?.Read();
            Compare();
        }
    }

    internal class ListItemForSP : ListItem, IListItem
    {
        private readonly SnoopableParameter snoopableParameter;

        public override string Name => snoopableParameter.Name;
        public SnoopableParameter this[int i]
        {
            get
            {
                return i switch {  _ => snoopableParameter };
            }
        }


        public ListItemForSP(SnoopableParameter x, SnoopableParameter y, Action askForReload)
        {
            this.snoopableParameter = x;
        }


        public override void Read()
        {
            snoopableParameter.Read();
        }
    }
}
