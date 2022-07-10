using System.Collections.ObjectModel;
using RevitDBExplorer.WPF;

namespace RevitDBExplorer.ViewModels
{
    internal class TreeViewItemVM : BaseViewModel
    {
        private bool isSelected = false;
        private bool isExpanded = false;
        private bool isEnabled  = true;
        private ObservableCollection<TreeViewItemVM> items = null;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;
                OnPropertyChanged();
            }
        }
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        
        public ObservableCollection<TreeViewItemVM> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }
    }
}