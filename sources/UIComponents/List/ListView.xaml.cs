using System.Windows;
using System.Windows.Controls;
using RevitDBExplorer.Domain.DataModel;

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
                Clipboard.SetDataObject($"{snoopableMember.Name}= {snoopableMember.Label.Text}");
            }
        }
    }
}