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
        private ObservableCollection<ListItem> listItems = new();
        private ObservableCollection<DynamicGridViewColumn> columns = new();
        private ListItem listSelectedItem = null;
        private TreeItem treeSelectedItem = null;
        private string filterPhrase = "";
        private bool isMemberViewVisible = true;
        private bool hasParameters;


        public ObservableCollection<ListItem> ListItems
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
        public ListItem ListSelectedItem
        {
            get
            {
                return listSelectedItem;
            }
            set
            {
                listSelectedItem = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand KeyDown_Enter_Command { get; }
        public RelayCommand ListItem_Click_Command { get; }
        public RelayCommand ReloadCommand { get; }
        public RelayCommand CopyNameCommand { get; }
        public RelayCommand CopyValueCommand { get; }
        public RelayCommand OpenCHMCommand { get; }
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
            }
        }


        public ListVM(IAmWindowOpener windowOpener)
        {
            this.windowOpener = windowOpener;
            columnsFor1 = new ObservableCollection<DynamicGridViewColumn>()
            {
                new DynamicGridViewColumn("Name", 38) { Binding ="."},
                new DynamicGridViewColumn("Value", 62){ Binding ="[0]" },
            };
            columnsFor2 = new ObservableCollection<DynamicGridViewColumn>()
            {
                new DynamicGridViewColumn("Name", 30) { Binding ="."},
                new DynamicGridViewColumn("left Value", 40){ Binding ="[0]" },
                new DynamicGridViewColumn("right Value", 40){ Binding ="[1]" },
            };
            KeyDown_Enter_Command = new RelayCommand(KeyDown_Enter);
            ListItem_Click_Command = new RelayCommand(ListItem_Click);
            ReloadCommand = new RelayCommand(Reload);
            CopyNameCommand = new RelayCommand(CopyName);
            CopyValueCommand = new RelayCommand(CopyValue);
            OpenCHMCommand = new RelayCommand(OpenCHM);            
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
        private async Task PopulateListView()
        {
            HasParameters = false;
            if (treeSelectedItem is SnoopableObjectTreeItem snoopableObjectTreeItem)
            {
                HasParameters = snoopableObjectTreeItem.Object.HasParameters;
                Columns = columnsFor1;
                if (IsMemberViewVisible)
                {
                    var members = await ExternalExecutor.ExecuteInRevitContextAsync(x => snoopableObjectTreeItem.Object.GetMembers(x).ToList());
                    ListItems = new(members.Select(x => new ListItemForSM(x, null, Reload)));
                    SetupListView();
                }
                else
                {
                    var parameters = await ExternalExecutor.ExecuteInRevitContextAsync(x => snoopableObjectTreeItem.Object.GetParameters(x).ToList());
                    ListItems = new(parameters.Select(x => new ListItemForSP(x, null, Reload)));
                }
            }
        }
        public async Task<bool> PopulateListView(UtilityGroupTreeItem utilityGroupTreeItem)
        {
            HasParameters = false;
            if (utilityGroupTreeItem.Items?.Count < 2)
            {
                return false;
            }
            Columns = columnsFor2;
            var leftItem = utilityGroupTreeItem.Items[0] as SnoopableObjectTreeItem;
            var rightItem = utilityGroupTreeItem.Items[1] as SnoopableObjectTreeItem;

            if (leftItem.Object?.Object?.GetType() != rightItem.Object?.Object?.GetType())
            {
                return false;
            }

            var leftMembers = await ExternalExecutor.ExecuteInRevitContextAsync(x => leftItem.Object.GetMembers(x).OrderBy(x => x).ToList());
            var rightMembers = await ExternalExecutor.ExecuteInRevitContextAsync(x => rightItem.Object.GetMembers(x).OrderBy(x => x).ToList());

            var members = new List<ListItemForSM>();
            int i = 0;
            int j = 0;
            while (i < leftMembers.Count && j < rightMembers.Count)
            {
                var left = leftMembers[i];
                var right = rightMembers[j];

                if (left.Equals(right))
                {
                    members.Add(new ListItemForSM(left, right, Reload, true));
                    ++i;
                    ++j;
                    continue;
                }
                if (left.CompareTo(right) < 0)
                {
                    members.Add(new ListItemForSM(left, null, Reload, true));
                    ++i;                  
                    continue;
                }
               
                members.Add(new ListItemForSM(null, right, Reload, true));               
                ++j;                              
            }

            while (i < leftMembers.Count)
            {
                members.Add(new ListItemForSM(leftMembers[i], null, Reload, true));
                ++i;
            }
            while (j < rightMembers.Count)
            {
                members.Add(new ListItemForSM(null, rightMembers[j], Reload, true));
                ++j;
            }

            ListItems = new(members);
            SetupListView();
            return true;
        }
        private void SetupListView()
        {
            var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(ListItems);

            if (ListItems.Count < 666)
            {
                lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ListItem.GroupingKey)));
                lcv.SortDescriptions.Add(new SortDescription(nameof(ListItem.SortingKey), ListSortDirection.Ascending));
            }
            lcv.Filter = ListViewFilter;
        }
        private bool ListViewFilter(object item)
        {
            if (item is ListItem listItem)
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
            if (ListSelectedItem is ListItemForSM listItemForSM)
            {
                ListItem_Click(listItemForSM[0]);
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
            if (obj is ListItem listItem)
            {
                Clipboard.SetDataObject($"{listItem.Name}");
            }
        }
        private void OpenCHM(object obj)
        {
            if (obj is ListItemForSM listItem)
            {
                CHMService.OpenCHM(listItem[0]);
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


        private void Reload()
        {
            Reload(null);
        }
        private async void Reload(object parameter)
        {
            await ExternalExecutor.ExecuteInRevitContextAsync(uiApp => ReloadItems());
        }       
    }

    internal interface IAmWindowOpener
    {
        void Open(SourceOfObjects sourceOfObjects);
    }
}