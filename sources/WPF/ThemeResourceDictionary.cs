using System;
using System.Windows;

namespace RevitDBExplorer.WPF
{
    internal class ThemeResourceDictionary : ResourceDictionary
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                UpdateSource();
            }
        }


        private void UpdateSource()
        {
            var category = String.IsNullOrEmpty(name) ? "Default" : name;
            var style = String.IsNullOrEmpty(Properties.AppSettings.Default.Theme) ? "Default" : Properties.AppSettings.Default.Theme;
            var path = new Uri($"pack://application:,,,/RevitDBExplorer;component/Resources/Themes/{style}.{category}.xaml", UriKind.RelativeOrAbsolute);
            base.Source = path;
        }
    }
}
