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
        public bool FeatureFlag
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
            FeatureFlag = AppSettings.Default.FeatureFlag;
            RevitAPICHMFilePath = AppSettings.Default.RevitAPICHMFilePath;
            this.DataContext = this;
            FavoriteQueries = new ObservableCollection<FavoriteQueryDTO>(FavoritesManager.FavoriteQueries);
            DeleteQueryCommand = new RelayCommand(DeleteQuery);
        }

        private void DeleteQuery(object obj)
        {
            FavoriteQueries.Remove(obj as FavoriteQueryDTO);
        }        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Default.IsEventMonitorEnabled = IsEventMonitorEnabled;
            AppSettings.Default.Theme = SelectedTheme.Id;
            AppSettings.Default.FeatureFlag = FeatureFlag;
            AppSettings.Default.RevitAPICHMFilePath = RevitAPICHMFilePath;
            FavoritesManager.FavoriteQueries = FavoriteQueries.ToList();
            FavoritesManager.Save();
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
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
    }

    public record Theme(string Id, string Title);
}
