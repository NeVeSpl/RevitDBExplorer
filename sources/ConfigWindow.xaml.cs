using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using RevitDBExplorer.Properties;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    public partial class ConfigWindow : Window, INotifyPropertyChanged
    {
        private bool _isEventMonitorEnabled;
        private ObservableCollection<Theme> _themes = new() 
        { 
            new Theme("Default", "Light - default"),
            new Theme("Dark", "Dark - be ready"),
            new Theme("DarkR2024", "Dark - R2024") 
        };
        private Theme _selectedTheme;

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




        public ConfigWindow()
        {
            InitializeComponent();
            IsEventMonitorEnabled = AppSettings.Default.IsEventMonitorEnabled;
            SelectedTheme = _themes.Where(x => x.Id == AppSettings.Default.Theme).FirstOrDefault() ?? _themes.First();
            this.DataContext = this;
        }



        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Default.IsEventMonitorEnabled = IsEventMonitorEnabled;
            AppSettings.Default.Theme = SelectedTheme.Id;
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }

    public record Theme(string Id, string Title);
}
