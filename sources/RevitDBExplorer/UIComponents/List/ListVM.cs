using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.Properties;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List
{
    internal class ListVM : BaseViewModel
    {
        private ObservableCollection<SnoopableMember> listItems = new();
        private SnoopableMember listSelectedItem = null;
        private string filterPhrase = "";

        public event Action<SnoopableMember> MemberSnooped;
        public event Action MemberValueHasChanged;
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


        public ListVM()
        {
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
            members.ForEach(x => x.SnoopableObjectChanged += RaiseMemberValueHasChanged);
            ListItems = new(members);

            var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(ListItems);

            if (members.Count < 666)
            {
                lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SnoopableMember.DeclaringTypeName)));
                lcv.SortDescriptions.Add(new SortDescription(nameof(SnoopableMember.DeclaringTypeLevel), ListSortDirection.Ascending));
                lcv.SortDescriptions.Add(new SortDescription(nameof(SnoopableMember.MemberKind), ListSortDirection.Ascending));
                lcv.SortDescriptions.Add(new SortDescription(nameof(SnoopableMember.Name), ListSortDirection.Ascending));
            }
            lcv.Filter = ListViewFilter;
        }
        private bool ListViewFilter(object item)
        {
            if (item is SnoopableMember snoopableMember)
            {
                bool inName = snoopableMember.Name.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                bool inValue = snoopableMember.ValueViewModel is IValuePresenter valuePresenter && valuePresenter.Label.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
                return inName || inValue;
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
            RaiseMemberSnoopedEvent();
        }
        private void ListItem_Click(object obj)
        {
            ListSelectedItem = obj as SnoopableMember;
            RaiseMemberSnoopedEvent();
        }     
        private void RaiseMemberSnoopedEvent()
        {
            if (ListSelectedItem?.CanBeSnooped == true)
            {
                MemberSnooped?.Invoke(ListSelectedItem);
            }
        }
        private void RaiseMemberValueHasChanged()
        {
            MemberValueHasChanged?.Invoke();
            Reload(null);
        }


        private void CopyName(object obj)
        {
            if (obj is SnoopableMember snoopableMember)
            {
                Clipboard.SetDataObject($"{snoopableMember.Name}");
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
        private void OpenCHM(object obj)
        {
            if (obj is SnoopableMember snoopableMember)
            {
                string helpFileName = AppSettings.Default.RevitAPICHMFilePath;
                helpFileName = helpFileName.Replace("\"", "");
                if (System.IO.File.Exists(helpFileName))
                {
                    string postfix = "";
                    switch (snoopableMember.MemberKind)
                    {
                        case Domain.DataModel.Streams.Base.MemberKind.Property:
                            postfix = " property";
                            break;
                        case Domain.DataModel.Streams.Base.MemberKind.Method:
                        case Domain.DataModel.Streams.Base.MemberKind.StaticMethod:
                        case Domain.DataModel.Streams.Base.MemberKind.AsArgument:
                            postfix = " method";
                            break;
                    }

                    System.Windows.Forms.Help.ShowHelp(null, helpFileName,
                        System.Windows.Forms.HelpNavigator.KeywordIndex,
                        $"{snoopableMember.DeclaringType.BareName}.{snoopableMember.Name}{postfix}");
                }
                else
                {
                    MessageBox.Show(
                        $".chm file does not exist at the given location: {helpFileName}. Please set the correct location in the configuration.");
                }
            }
        }

        private async void Reload(object parameter)
        {
            await ExternalExecutor.ExecuteInRevitContextAsync(uiApp => ReloadItems());
        }
    }
}