using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ViewModels.Base;
using RevitDBExplorer.Properties;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List
{
    public partial class ListView : UserControl
    {
        public ListView()
        {
            InitializeComponent();
        }

        private void ListViewItem_MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var menu = menuItem.Parent as ContextMenu;
            var item = menu.PlacementTarget as ListViewItem;

            if (item.DataContext is SnoopableMember snoopableMember)
            {
                if (snoopableMember.ValueViewModel is IValuePresenter presenter)
                {
                    Clipboard.SetDataObject($"{snoopableMember.Name}= {presenter.Label}");
                }
            }
        }

        private void GroupItem_MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var menu = menuItem.Parent as ContextMenu;
            var item = menu.PlacementTarget as GroupItem;
            var content = item.Content as CollectionViewGroup;

            Clipboard.SetDataObject(content?.Name);
               
        }

        private void ListViewItem_MenuItemOpenCHM_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var menu = menuItem.Parent as ContextMenu;
            var item = menu.PlacementTarget as ListViewItem;

            if (item.DataContext is SnoopableMember snoopableMember)
            {
                string helpFileName = AppSettings.Default.RevitAPICHMFilePath;
                if (System.IO.File.Exists(helpFileName))
                {
                    string postfix = "";
                    switch(snoopableMember.MemberKind)
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
                    System.Windows.Forms.Help.ShowHelp(null, helpFileName, System.Windows.Forms.HelpNavigator.KeywordIndex, $"{snoopableMember.DeclaringType.BareName}.{snoopableMember.Name}{postfix}");
                }
                else
                {
                    MessageBox.Show($".chm file does not exist at the given location: {helpFileName}. Please set the correct location in the configuration.");
                }
            }
        }
    }
}