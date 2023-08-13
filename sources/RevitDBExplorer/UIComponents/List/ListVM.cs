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
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List
{
    internal class ListVM : BaseViewModel
    {
        private readonly IAmWindowOpener windowOpener;
        private ObservableCollection<ListItem> listItems = new();
        private ObservableCollection<DynamicGridViewColumn> columns = new();
        private ListItem listSelectedItem = null;
        private string filterPhrase = "";
        private bool isMemberViewVisible;
        
      
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
                OnPropertyChanged(nameof(IsProperyViewVisible));
            }
        }
        public bool IsProperyViewVisible
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


        public ListVM(IAmWindowOpener windowOpener)
        {
            this.windowOpener = windowOpener;
            Columns = new ObservableCollection<DynamicGridViewColumn>()
            {
                new DynamicGridViewColumn("Name", 38) { Binding ="."},
                new DynamicGridViewColumn("Value", 62){ Binding ="[0]" },
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
            PopulateListView(System.Array.Empty<SnoopableMember>());
        }
        public void PopulateListView(IList<SnoopableMember> members)
        {
            members.ForEach(x => x.SnoopableObjectChanged += ItemValueHasChanged);         
            ListItems = new(members.Select(x => new ListItemForSM(x)));

            var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(ListItems);

            if (members.Count < 666)
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
            if (obj is SnoopableMember snoopableMember)
            {
                if (snoopableMember?.CanBeSnooped == true)
                {
                    var sourceOfObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x =>
                    {
                        var source = snoopableMember.Snoop();
                        source.ReadFromTheSource(x);
                        return source;
                    });

                    windowOpener?.Open(sourceOfObjects);
                }
            }
        }
        private void ItemValueHasChanged()
        {
            Reload(null);
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
            if (obj is SnoopableMember snoopableMember)
            {
                if (snoopableMember.ValueViewModel is IValuePresenter presenter)
                {
                    Clipboard.SetDataObject($"{presenter.Label}");
                }
            }
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