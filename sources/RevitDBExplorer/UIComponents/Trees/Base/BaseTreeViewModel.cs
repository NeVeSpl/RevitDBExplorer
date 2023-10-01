using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
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

        public event Action<SelectedItemChangedEventArgs> SelectedItemChanged;
        public event Action<string> ScriptForRDSHasChanged;

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
       

        public BaseTreeViewModel()
        {           
            TreeItemsCommands = new TreeItemsCommands(new RelayCommand(GenerateUpdateQueryRDS));
        }
        

        public void RaiseSelectedItemChanged(TreeItem item)
        {
            var oldOne = SelectedItem;
            SelectedItem = item;
            SelectedItemChanged?.Invoke(new SelectedItemChangedEventArgs(this, oldOne, item));
        }
        

        private void GenerateUpdateQueryRDS(object parameter)
        {
            string text = "";

            if (parameter is SnoopableObjectTreeItem snoopableObjectTreeItem)
            {
                if (snoopableObjectTreeItem.Object.Object is Parameter revitParameter)
                {
                    text = CodeGenerator.GenerateUpdateCommandForParameter(revitParameter);
                }
                else
                {
                    text = CodeGenerator.GenerateUpdateCommandForType(snoopableObjectTreeItem.Object.Object?.GetType());
                }
            }
            if (parameter is GroupTreeItem groupTreeItem)
            {
                text = CodeGenerator.GenerateUpdateCommandForType(typeof(object));

                var pointer = groupTreeItem;
                while (pointer != null)
                {
                    if (pointer is TypeGroupTreeItem typeGroupTreeItem)
                    {
                        text = CodeGenerator.GenerateUpdateCommandForType(typeGroupTreeItem.GetAllSnoopableObjects().FirstOrDefault()?.Object?.GetType());
                        break;
                    }

                    pointer = pointer.Parent;
                }
            }

            ScriptForRDSHasChanged?.Invoke(text);
        }


        public static IEnumerable<object> GetObjectsForTransfer(TreeItem treeViewItem)
        {
            return treeViewItem.GetAllSnoopableObjects().Where(x => x.Object != null).Select(x => x.Object).ToArray();
        }
    }

    internal record class SelectedItemChangedEventArgs(BaseTreeViewModel Sender, TreeItem OldOne, TreeItem NewOne);
}