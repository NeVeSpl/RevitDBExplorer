using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.RevitDatabaseScripting;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Base
{
    internal class BaseTreeViewModel : BaseViewModel
    {
        protected readonly TreeItemsCommands TreeItemsCommands;
        private ObservableCollection<TreeItem> treeItems = new();
        private TreeItem selectedItem;
        private bool allowToFrezeeItem;
        private bool enrichWithVisibilityData;

        public event Action<TreeSelectedItemChangedEventArgs> SelectedItemChanged;
     

        public ObservableCollection<TreeItem> TreeItems
        {
            get
            {
                return treeItems;
            }
            set
            {
                treeItems = value;
                OnPropertyChanged();
            }
        }
        public TreeItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        public bool AllowToFrezeeItem
        {
            get
            {
                return allowToFrezeeItem;
            }
            set
            {
                allowToFrezeeItem = value;
                OnPropertyChanged();
            }
        }
        public bool EnrichWithVisibilityData
        {
            get
            {
                return enrichWithVisibilityData;
            }
            set
            {
                enrichWithVisibilityData = value;
                OnPropertyChanged();
            }
        }


        public BaseTreeViewModel()
        {           
            TreeItemsCommands = new TreeItemsCommands();
        }
        

        public void RaiseSelectedItemChanged(TreeItem item)
        {
            var oldOne = SelectedItem;
            SelectedItem = item;            
            SelectedItemChanged?.Invoke(new TreeSelectedItemChangedEventArgs(this, oldOne, item));
        }
        

        public IEnumerable<SnoopableObjectTreeItem> StreamSnoopableObjectTreeItems()
        {
            foreach (var item in TreeItems)
            {
                foreach (var item2 in item.GetAllSnoopableObjectTreeItems())
                {
                    yield return item2;
                }
            }
        }


        public static IEnumerable<object> GetObjectsForTransfer(TreeItem treeViewItem)
        {
            return treeViewItem.GetAllSnoopableObjects().Where(x => x.Object != null).Select(x => x.Object).ToArray();
        }
    }

    internal record class TreeSelectedItemChangedEventArgs(BaseTreeViewModel Sender, TreeItem OldOne, TreeItem NewOne);
}