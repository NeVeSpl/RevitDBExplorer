using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion;
using RevitDBExplorer.Properties;
using RevitDBExplorer.Utils;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    internal partial class ConfigWindow : Window, INotifyPropertyChanged
    {
        private bool _isEventMonitorEnabled;
        private ObservableCollection<Theme> _themes = new() 
        { 
            new Theme("Default", "Light - default"),
            new Theme("Dark", "Dark - be ready"),
            new Theme("DarkR2024", "Dark - R2024") 
        };
        private Theme _selectedTheme;
        private bool _featureFlag;
        private string _revitAPICHMFilePath;
        private ObservableCollection<FavoriteQueryDTO> favoriteQueries;
        private bool addRDBECmdToModifyTab;

        public bool IsEventMonitorEnabled
        {
            get
            {
                return _isEventMonitorEnabled;
            }
            set
            {
                _isEventMonitorEnabled = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Theme> Themes
        {
            get
            {
                return _themes;
            }
            set
            {
                _themes = value;
                OnPropertyChanged();
            }
        }
        public Theme SelectedTheme
        {
            get
            {
                return _selectedTheme;
            }
            set
            {
                _selectedTheme = value;
                OnPropertyChanged();
            }
        }
        public bool OpenLinksInNewWindow
        {
            get
            {
                return _featureFlag;
            }
            set
            {
                _featureFlag = value;
                OnPropertyChanged();
            }
        }
        public string RevitAPICHMFilePath
        {
            get
            {
                return _revitAPICHMFilePath;
            }
            set
            {
                _revitAPICHMFilePath = value;
                OnPropertyChanged();
            }
        }
        public bool AddRDBECmdToModifyTab
        {
            get
            {
                return addRDBECmdToModifyTab;
            }
            set
            {
                addRDBECmdToModifyTab = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FavoriteQueryDTO> FavoriteQueries
        {
            get
            {
                return favoriteQueries;
            }
            set
            {
                favoriteQueries = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand DeleteQueryCommand { get; }


        public ConfigWindow()
        {
            InitializeComponent();
            IsEventMonitorEnabled = AppSettings.Default.IsEventMonitorEnabled;
            SelectedTheme = _themes.Where(x => x.Id == AppSettings.Default.Theme).FirstOrDefault() ?? _themes.First();
            OpenLinksInNewWindow = AppSettings.Default.OpenLinksInNewWindow;
            RevitAPICHMFilePath = AppSettings.Default.RevitAPICHMFilePath;
            AddRDBECmdToModifyTab = AppSettings.Default.AddRDBECmdToModifyTab;            
            FavoriteQueries = new ObservableCollection<FavoriteQueryDTO>(FavoritesManager.FavoriteQueries);
            DeleteQueryCommand = new RelayCommand(DeleteQuery);
            this.DataContext = this;
        }

        private void DeleteQuery(object obj)
        {
            FavoriteQueries.Remove(obj as FavoriteQueryDTO);
        }        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Default.IsEventMonitorEnabled = IsEventMonitorEnabled;
            AppSettings.Default.Theme = SelectedTheme.Id;
            AppSettings.Default.OpenLinksInNewWindow = OpenLinksInNewWindow;
            AppSettings.Default.RevitAPICHMFilePath = RevitAPICHMFilePath;
            AppSettings.Default.AddRDBECmdToModifyTab = AddRDBECmdToModifyTab;
            AppSettings.Default.Save();
            FavoritesManager.FavoriteQueries = FavoriteQueries.ToList();
            FavoritesManager.Save();
            ApplicationModifyTab.Update(AppSettings.Default.AddRDBECmdToModifyTab);
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void DataGrid_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        private void BrowseOnClick(object sender, RoutedEventArgs e)
        {
            // open dialog and select file 
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "CHM files (*.chm)|*.chm";
            if (dialog.ShowDialog() == true)
            {
                RevitAPICHMFilePath = dialog.FileName;
            }
        }
    }

    public record Theme(string Id, string Title);
}
