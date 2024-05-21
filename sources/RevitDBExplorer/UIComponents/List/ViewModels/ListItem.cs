using System;
using System.Collections.Generic;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List.ViewModels
{
    internal interface IListItem
    {
        string SortingKey { get;  }
        string GroupingKey { get;  }
        public string Name { get; }
        public bool IsHighlighted { get; }
        public bool IsFavourite { get; }


        bool Filter(string filterPhrase);
        void Read();
    }

    internal class ListItem<T> : BaseViewModel, IListItem where T : SnoopableItem
    {
        private readonly T leftItem;
        private readonly T rightItem;
        private readonly bool doCompare;
        private bool isHighlighted;
        private bool isFavourite;

        protected T SnoopableItem => leftItem ?? rightItem;
        public string SortingKey { get; init; }
        public string GroupingKey { get; init; }
        public string Name => SnoopableItem.Name;
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
        public bool IsFavourite
        {
            get
            {
                return isFavourite;
            }
            set
            {
                isFavourite = value;
                OnPropertyChanged();
            }
        }
        public T this[int i]
        {
            get
            {
                return i switch { 0 => leftItem, _ => rightItem };
            }
        }


        public ListItem(T left, T right, Action askForReload, bool doCompare)
        {
            leftItem = left;
            rightItem = right;
            this.doCompare = doCompare;
            if (leftItem != null)
            {
                leftItem.ParentObjectChanged += () => askForReload();
            }
            if (rightItem != null)
            {
                rightItem.ParentObjectChanged += () => askForReload();
            }

            Compare();
            IsFavourite = FavoriteMembersManager.IsFavorite(SnoopableItem.GetUniqueId());
        }


        private void Compare()
        {
            if (doCompare)
            {
                if ((leftItem?.ValueViewModel is IValuePresenter leftVP) && (rightItem?.ValueViewModel is IValuePresenter rightVp))
                {
                    IsHighlighted = !leftVP.Label.Equals(rightVp.Label);
                }
                else
                {
                    IsHighlighted = true;
                }
                if ((leftItem?.ValueViewModel is IValueEditor) && (rightItem?.ValueViewModel is IValueEditor))
                {
                    IsHighlighted = false;
                }
            }
        }


        public bool Filter(string filterPhrase)
        {
            bool left = false;
            bool right = false;
            if (leftItem != null)
            {
                bool inName = leftItem.Name.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                bool inValue = leftItem.ValueViewModel is IValuePresenter valuePresenter && valuePresenter.Label.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                left = inName || inValue;
            }
            if (rightItem != null)
            {
                bool inName = rightItem.Name.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                bool inValue = rightItem.ValueViewModel is IValuePresenter valuePresenter && valuePresenter.Label.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                right = inName || inValue;
            }
            return left || right;
        }
        public void Read()
        {
            leftItem?.Read();
            rightItem?.Read();
            Compare();            
        }

        public IEnumerable<DrawingVisual> GetVisualization()
        {
            return SnoopableItem.GetVisualization();
        }
        public void OpenCHM()
        {
            if (SnoopableItem is SnoopableMember snoopableMember)
            {
                CHMService.OpenCHM(snoopableMember);
            }
        }
        public void ToogleFavourite()
        {
            IsFavourite = !IsFavourite;
            if (IsFavourite)
            {
                FavoriteMembersManager.AddFavorite(SnoopableItem.GetUniqueId());
            }
            else
            {
                FavoriteMembersManager.RemoveFavorite(SnoopableItem.GetUniqueId());
            }
        }
    }
}