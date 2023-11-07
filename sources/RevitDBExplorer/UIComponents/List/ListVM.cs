using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.UIComponents.List.ViewModels;
using RevitDBExplorer.UIComponents.List.WPF;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List
{
    internal class ListVM : BaseViewModel
    {
        private readonly ObservableCollection<DynamicGridViewColumn> columnsFor1;
        private readonly ObservableCollection<DynamicGridViewColumn> columnsFor2;
        private readonly IAmWindowOpener windowOpener;
        private readonly IAmQueryExecutor queryExecutor;
        private readonly IAmScriptOpener scriptOpener;
        private ObservableCollection<IListItem> listItems = new();
        private ObservableCollection<DynamicGridViewColumn> columns = new();
        private IListItem listSelectedItem = null;
        private TreeItem treeSelectedItem = null;
        private string filterPhrase = "";
        private bool isMemberViewVisible = true;
        private bool hasParameters;
        private bool isComparisonActive;

        public ObservableCollection<IListItem> ListItems
        {
            get
            {
                return listItems;
            }
            set
            {
                listItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<DynamicGridViewColumn> Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
                OnPropertyChanged();
            }
        }
        public IListItem ListSelectedItem
        {
            get
            {
                return listSelectedItem;
            }
            set
            {                
                if (listSelectedItem != value)
                {
                    var oldOne = listSelectedItem;
                    listSelectedItem = value;
                    SelectedItemChanged?.Invoke(new ListSelectedItemChangedEventArgs(oldOne, listSelectedItem));
                }
                OnPropertyChanged();
            }
        }
        public event Action<ListSelectedItemChangedEventArgs> SelectedItemChanged;
        public RelayCommand KeyDown_Enter_Command { get; }
        public RelayCommand ListItem_Click_Command { get; }
        public RelayCommand ReloadCommand { get; }
        public RelayCommand CopyNameCommand { get; }
        public RelayCommand CopyValueCommand { get; }
        public RelayCommand OpenCHMCommand { get; }
        public RelayCommand SnoopParamInNewWindowCommand { get; }
        public RelayCommand SearchForParameterValueCommand { get; }
        public RelayCommand GenerateCSharpCodeCommand { get; }
        public string FilterPhrase
        {
            get
            {
                return filterPhrase;
            }
            set
            {
                filterPhrase = value;
                FilterListView();
                OnPropertyChanged();
            }
        }
        public bool IsMemberViewVisible
        {
            get
            {
                return isMemberViewVisible;
            }
            set
            {
                isMemberViewVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsParameterViewVisible));

                PopulateListView();
            }
        }
        public bool IsParameterViewVisible
        {
            get
            {
                return !IsMemberViewVisible;
            }
            set
            {
                IsMemberViewVisible = !value;
            }
        }
        public bool HasParameters
        {
            get
            {
                return hasParameters;
            }
            set
            {
                hasParameters = value;
                OnPropertyChanged();
                if (hasParameters == false)
                {
                    isMemberViewVisible = true;
                    OnPropertyChanged(nameof(IsMemberViewVisible));
                    OnPropertyChanged(nameof(IsParameterViewVisible));
                }
            }
        }
        public bool IsComparisonActive
        {
            get
            {
                return isComparisonActive;
            }
            set
            {
                isComparisonActive = value;
                OnPropertyChanged();
            }
        }


        public ListVM(IAmWindowOpener windowOpener, IAmQueryExecutor queryExecutor, IAmScriptOpener scriptOpener)
        {
            this.windowOpener = windowOpener;
            this.queryExecutor = queryExecutor;
            this.scriptOpener = scriptOpener;
            columnsFor1 = new ObservableCollection<DynamicGridViewColumn>()
            {
                new DynamicGridViewColumn("Name", 38) { Binding ="."},
                new DynamicGridViewColumn("Value", 62){ Binding ="[0]" },
            };
            columnsFor2 = new ObservableCollection<DynamicGridViewColumn>()
            {
                new DynamicGridViewColumn("Name", 30) { Binding ="."},
                new DynamicGridViewColumn("left Value", 35){ Binding ="[0]" },
                new DynamicGridViewColumn("right Value", 35){ Binding ="[1]" },
            };
            KeyDown_Enter_Command = new RelayCommand(KeyDown_Enter);
            ListItem_Click_Command = new RelayCommand(ListItem_Click);
            ReloadCommand = new RelayCommand(Reload);
            CopyNameCommand = new RelayCommand(CopyName);
            CopyValueCommand = new RelayCommand(CopyValue);
            OpenCHMCommand = new RelayCommand(OpenCHM);
            SnoopParamInNewWindowCommand = new RelayCommand(SnoopParamInNewWindow);
            SearchForParameterValueCommand = new RelayCommand(SearchForParameterValue);
            GenerateCSharpCodeCommand = new RelayCommand(GenerateCSharpCode);
        }


        public void ClearItems()
        {
            ListItems = new();
        }
       

        public async Task PopulateListView(SnoopableObjectTreeItem snoopableObjectTreeItem)
        {
            treeSelectedItem = snoopableObjectTreeItem;   
            await PopulateListView();
        }
        public async Task<bool> PopulateListView(UtilityGroupTreeItem utilityGroupTreeItem)
        {
            treeSelectedItem = utilityGroupTreeItem;
            return await PopulateListView();
        }
        private async Task<bool> PopulateListView()
        {
            //HasParameters = false;
            if (treeSelectedItem is SnoopableObjectTreeItem snoopableObjectTreeItem)
            {
                HasParameters = snoopableObjectTreeItem.Object.HasParameters;
                Columns = columnsFor1;
                IsComparisonActive = false;

                var items = await GetSnoopableItemsFromRevit(snoopableObjectTreeItem.Object);
                ListItems = new(items.Select(x => ListItemFactory.Create(x, null, Reload)));  
                SetupListView();

                return true;
            }
            if (treeSelectedItem is UtilityGroupTreeItem utilityGroupTreeItem)
            {
                return await PopulateListViewForComparison(utilityGroupTreeItem);
            }
            return false;
        }
        private async Task<IList<SnoopableItem>> GetSnoopableItemsFromRevit(SnoopableObject snoopableObject)
        {
            if (IsMemberViewVisible)
            {
                var members = await ExternalExecutor.ExecuteInRevitContextAsync(x => snoopableObject.GetMembers(x).OrderBy(x => x).OfType<SnoopableItem>().ToList());
                return members;
            }
            var parameters = await ExternalExecutor.ExecuteInRevitContextAsync(x => snoopableObject.GetParameters(x).OrderBy(x => x).OfType<SnoopableItem>().ToList());
            return parameters;
        }


        private async Task<bool> PopulateListViewForComparison(UtilityGroupTreeItem utilityGroupTreeItem)
        {           
            if (utilityGroupTreeItem.Items?.Count < 2)
            {
                return false;
            }
            Columns = columnsFor2;
            IsComparisonActive = true;
            var leftItem = utilityGroupTreeItem.Items[0] as SnoopableObjectTreeItem;
            var rightItem = utilityGroupTreeItem.Items[1] as SnoopableObjectTreeItem;

            if (leftItem.Object?.Object?.GetType() != rightItem.Object?.Object?.GetType())
            {
                return false;
            }

            HasParameters = leftItem.Object.HasParameters;

            var leftItems = await GetSnoopableItemsFromRevit(leftItem.Object);
            var rightItems = await GetSnoopableItemsFromRevit(rightItem.Object);
            var mergedItems = MergeTwoLists(leftItems, rightItems).Select(x => ListItemFactory.Create(x.first, x.second, Reload, true));

            ListItems = new(mergedItems);
            SetupListView();
            return true;
        }
        private IEnumerable<(SnoopableItem first, SnoopableItem second)> MergeTwoLists(IList<SnoopableItem> leftItems, IList<SnoopableItem> rightItems)
        {
            int i = 0;
            int j = 0;
            while (i < leftItems.Count && j < rightItems.Count)
            {
                var left = leftItems[i];
                var right = rightItems[j];

                if (left.Equals(right))
                {
                    yield return (left, right);
                    ++i;
                    ++j;
                    continue;
                }
                if (left.CompareTo(right) < 0)
                {
                    yield return (left, null);
                    ++i;
                    continue;
                }

                yield return (null, right);
                ++j;
            }

            while (i < leftItems.Count)
            {
                yield return (leftItems[i], null);
                ++i;
            }
            while (j < rightItems.Count)
            {
                yield return (null, rightItems[j]);
                ++j;
            }
        }
        private void SetupListView()
        {
            var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(ListItems);

            if (ListItems.Count < 666)
            {
                lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(IListItem.GroupingKey)));
                lcv.SortDescriptions.Add(new SortDescription(nameof(IListItem.SortingKey), ListSortDirection.Ascending));
            }
            lcv.Filter = ListViewFilter;
        }
        private bool ListViewFilter(object item)
        {
            if (item is IListItem listItem)
            {
                return listItem.Filter(FilterPhrase);
            }
            return true;
        }
        private void ReloadItems()
        {
            foreach (var item in ListItems)
            {
                item.Read();
            }
        }
        private void FilterListView()
        {
            if (ListItems != null)
            {
                CollectionViewSource.GetDefaultView(ListItems).Refresh();
            }
        }


        private void KeyDown_Enter(object obj)
        {
            if (ListSelectedItem is ListItemForMember ListItemForMember)
            {
                ListItem_Click(ListItemForMember[0]);
            }
        }
        private async void ListItem_Click(object obj)
        {
            if (obj is SnoopableItem snoopableItem)
            {
                if (snoopableItem?.CanBeSnooped == true)
                {
                    var sourceOfObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x =>
                    {
                        var source = snoopableItem.Snoop();
                        source.ReadFromTheSource(x);
                        return source;
                    });

                    windowOpener?.Open(sourceOfObjects);
                }
            }
        }
       

        private void CopyName(object obj)
        {
            if (obj is IListItem listItem)
            {
                Clipboard.SetDataObject($"{listItem.Name}");
            }
        }
        private void OpenCHM(object obj)
        {
            if (obj is ListItemForMember listItem)
            {
                CHMService.OpenCHM(listItem[0] ?? listItem[1]);
            }
        }
        private void SnoopParamInNewWindow(object obj)
        {
            if (obj is ListItemForParameter listItem)
            {
                var source = listItem.CreateSnoopParameter();               
                windowOpener?.Open(source);
            }
        }
        private void SearchForParameterValue(object obj)
        {
            if (obj is SnoopableParameter snoopableParameter)
            {
                queryExecutor.Query(snoopableParameter.GenerateQueryForForParameterValue());
            }
        }
        private void GenerateCSharpCode(object obj)
        {
            if (obj is SnoopableItem snoopableItem)
            {
                scriptOpener.Open(snoopableItem.GenerateScript());
            }
        }
        private void CopyValue(object obj)
        {
            if (obj is SnoopableItem snoopableItem)
            {
                if (snoopableItem.ValueViewModel is IValuePresenter presenter)
                {
                    Clipboard.SetDataObject($"{presenter.Label}");
                }
            }
        }


       
        private async void Reload()
        {
            await ExternalExecutor.ExecuteInRevitContextAsync(uiApp => ReloadItems());
        }       
    }

    internal record class ListSelectedItemChangedEventArgs(IListItem OldOne, IListItem NewOne);
    internal interface IAmWindowOpener
    {
        void Open(SourceOfObjects sourceOfObjects);
    }
    internal interface IAmQueryExecutor
    {
        void Query(string query);
    }
    internal interface IAmScriptOpener
    {
        void Open(string script);
    }
}