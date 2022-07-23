using System.Windows.Data;

namespace RevitDBExplorer.WPF.MarkupExtensions
{
    /// <summary>
    /// source : https://thomaslevesque.com/2008/11/18/wpf-binding-to-application-settings-using-a-markup-extension/
    /// </summary>
    public class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = Properties.AppSettings.Default;
            this.Mode = BindingMode.OneTime;
        }
    }
}