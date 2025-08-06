using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Explorer
{
    internal class ExplorerTreeViewModel : BaseTreeViewModel
    {
        private SourceOfObjects sourceOfObjects;
        private GroupBy groupBy = GroupBy.TypeName;
        private string filterPhrase = string.Empty;
        private bool isExpanded = true;
        private bool treeNotForEvents;
        

        public RelayCommand SwitchViewCommand { get; }
        public RelayCommand ReloadCommand { get; }
        public RelayCommand CollapseCommand { get; }        
        public RelayCommand ToggleHideCommand { get; }


        public string FilterPhrase
        {
            get
            {
                return filterPhrase;
            }
            set
            {
                filterPhrase = value;
                FilterTreeView();
                OnPropertyChanged();
            }
        }
        public bool TreeNotForEvents
        {
            get
            {
                return treeNotForEvents;
            }
            set
            {
                treeNotForEvents = value;
                OnPropertyChanged();
            }
        }


        public ExplorerTreeViewModel() 
        {
            SwitchViewCommand = new RelayCommand(SwitchView);
            ReloadCommand = new RelayCommand(Reload);
            CollapseCommand = new RelayCommand(Collapse);
            ToggleHideCommand = new RelayCommand(ToggleHide);
        }


        public void ClearItems()
        {
            PopulateTreeView(null);
            FilterPhrase = "";
            isExpanded = true;
        }
        public void PopulateTreeView(SourceOfObjects sourceOfObjects)
        {
            EnrichWithVisibilityData = sourceOfObjects?.Info?.EnrichWithVisibilityData == true;
            TreeNotForEvents = true;
            FilterPhrase = "";
            this.sourceOfObjects = sourceOfObjects;

            if (sourceOfObjects == null)
            {
                TreeItems = new();
                return;
            }

            GroupTreeItem groupTreeVM = new GroupTreeItem(sourceOfObjects, TreeViewFilter, groupBy, TreeItemsCommands);
            if (isExpanded)
            {
                groupTreeVM.Expand(true);
            }
            else
            {
                groupTreeVM.IsExpanded = true;
            }
            groupTreeVM.SelectFirstDeepestVisibleItem();

            if (groupTreeVM.Items.Count == 1)
            {
                var firstChild = groupTreeVM.Items[0];
                if ((firstChild is GroupTreeItem group) && (groupTreeVM.Name != null) && false) // todo
                {
                    group.Name = groupTreeVM.Name;
                    group.GroupedBy = GroupBy.Source;
                }
                TreeItems = new(new[] { firstChild });
            }
            else
            {
                TreeItems = new(new[] { groupTreeVM });
            }

            PopulateElementIdTreeItemMap();
            SynchronizeSelectionWithRevit();
        }
        public void PopulateWithEvents(SourceOfObjects sourceOfObjects)
        {
            EnrichWithVisibilityData = false;
            TreeNotForEvents = false;
            var snoopableTreeObjects = sourceOfObjects.Objects.Select(x => new SnoopableObjectTreeItem(x, TreeItemsCommands) { IsExpanded = true }).ToList();
            TreeItems = new(snoopableTreeObjects);
        }
        private bool TreeViewFilter(object item)
        {
            if (item is SnoopableObjectTreeItem snoopableObjectVM)
            {
                return snoopableObjectVM.Object.Name.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return true;
        }
        private void FilterTreeView()
        {
            if (TreeItems != null)
            {
                foreach (var item in TreeItems.OfType<GroupTreeItem>())
                {
                    item.Refresh();
                }
            }
        }



        private void SwitchView(object parameter)
        {
            groupBy = groupBy == GroupBy.TypeName ? GroupBy.Category : GroupBy.TypeName;
            PopulateTreeView(sourceOfObjects);
        }
        private async void Reload(object parameter)
        {
            await ExternalExecutor.ExecuteInRevitContextAsync(x => sourceOfObjects.ReadFromTheSource(x));
            PopulateTreeView(sourceOfObjects);
        }
        private void Collapse(object parameter)
        {
            var first = TreeItems.FirstOrDefault();
            if (first == null) return;

            if (isExpanded)
            {
                first.Collapse();
                first.IsExpanded = true;
            }
            else
            {
                if (first is GroupTreeItem { Count: < 666 } group)
                {
                    first.Expand(true, 666);
                }
            }
            isExpanded = !isExpanded;
        }
        private void ToggleHide(object parameter)
        {
            if (SelectedItem is SnoopableObjectTreeItem soti)
            {
                
            }
        }

        public void BindEvents()
        {
            EventListener.SelectionChanged += EventListener_SelectionChanged;
        }
        public void UnbindEvents()
        {
            EventListener.SelectionChanged -= EventListener_SelectionChanged;
        }


        private void EventListener_SelectionChanged(object sender, Autodesk.Revit.UI.Events.SelectionChangedEventArgs e)
        {
            SynchronizeSelectionWithRevit();
        }

        Dictionary<ElementId, SnoopableObjectTreeItem> elementIdTreeItemMap = new ();
        List<SnoopableObjectTreeItem> selectedTreeItemsInRevit = new ();
        private void PopulateElementIdTreeItemMap()
        {
            elementIdTreeItemMap = new Dictionary<ElementId, SnoopableObjectTreeItem>();

            foreach (var treeItem in StreamSnoopableObjectTreeItems())
            {
                if (treeItem.Object?.Object is Element element)
                {
                    if (element.Id != ElementId.InvalidElementId)
                    {
                        elementIdTreeItemMap[element.Id] = treeItem;
                    }
                }
            }            
        }
        private void SynchronizeSelectionWithRevit()
        {
            var uiDocument = new UIDocument(sourceOfObjects.RevitDocument);
            selectedTreeItemsInRevit.ForEach(x => x.IsSelectedInRevit = false);
            selectedTreeItemsInRevit.Clear();

            foreach (var id in  uiDocument.Selection.GetElementIds())
            {
                if (elementIdTreeItemMap.TryGetValue(id, out var treeItem))
                {
                    treeItem.IsSelectedInRevit = true;  
                    selectedTreeItemsInRevit.Add(treeItem);
                }
            }
        }
    }
}