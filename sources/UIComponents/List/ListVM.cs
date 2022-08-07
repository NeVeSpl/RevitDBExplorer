using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List
{
    internal class ListVM : BaseViewModel
    {
        private ObservableCollection<SnoopableMember> listItems = new();
        private SnoopableMember listSelectedItem = null;

        public event Action MemberSnooped;
        public ObservableCollection<SnoopableMember> ListItems
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
        public SnoopableMember ListSelectedItem
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


        public ListVM()
        {
            KeyDown_Enter_Command = new RelayCommand(KeyDown_Enter);            
            ListItem_Click_Command = new RelayCommand(ListItem_Click);
        }
       

        public void PopulateListView(IList<SnoopableMember> members, Predicate<object> listViewFilter)
        {            
            ListItems = new(members);

            var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(ListItems);

            lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SnoopableMember.DeclaringTypeName)));
            lcv.SortDescriptions.Add(new SortDescription(nameof(SnoopableMember.DeclaringTypeLevel), ListSortDirection.Ascending));
            lcv.SortDescriptions.Add(new SortDescription(nameof(SnoopableMember.MemberKind), ListSortDirection.Ascending));
            lcv.SortDescriptions.Add(new SortDescription(nameof(SnoopableMember.Name), ListSortDirection.Ascending));
            lcv.Filter = listViewFilter;
        }
        public void ReloadItems()
        {
            foreach (var item in ListItems)
            {
                item.Read();
            }
        }
        public void FilterListView()
        {
            if (ListItems != null)
            {
                CollectionViewSource.GetDefaultView(ListItems).Refresh();
            }
        }
       

        private void KeyDown_Enter(object obj)
        {
            MemberSnooped?.Invoke();
        }
        private void ListItem_Click(object obj)
        {
            ListSelectedItem = obj as SnoopableMember;
            MemberSnooped?.Invoke();
        }      
    }
}