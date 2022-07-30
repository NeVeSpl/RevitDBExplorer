using System;
using System.Windows;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

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


        public void UpdateSource()
        {
            var dictionaryName = String.IsNullOrEmpty(name) ? "Default" : name;
            var themeName = String.IsNullOrEmpty(Properties.AppSettings.Default.Theme) ? "Default" : Properties.AppSettings.Default.Theme;
            var path = new Uri($"pack://application:,,,/RevitDBExplorer;component/Resources/Themes/{themeName}.{dictionaryName}.xaml", UriKind.RelativeOrAbsolute);
            base.Source = path;
        }
    }
}