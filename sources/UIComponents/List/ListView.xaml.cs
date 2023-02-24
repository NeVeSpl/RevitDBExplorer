using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ViewModels.Base;
using RevitDBExplorer.Properties;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List
{
    public partial class ListView : UserControl
    {
        private ListVM listVM;


        public ListView()
        {
            InitializeComponent();
            this.DataContextChanged += ListView_DataContextChanged;
        }


        private void ListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is ListVM vm)
            {
                listVM = vm;
            }
        }

        private void ListViewItem_MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var menu = menuItem.Parent as ContextMenu;
            var item = menu.PlacementTarget as FrameworkElement;

            if (item.DataContext is SnoopableMember snoopableMember)
            {
                var isNameColumn = item.GetParent(x => string.Equals(x.Tag, "NameColumn")) != null;
                if (isNameColumn)
                {
                    Clipboard.SetDataObject($"{snoopableMember.Name}");
                }
                else
                {
                    if (snoopableMember.ValueViewModel is IValuePresenter presenter)
                    {
                        Clipboard.SetDataObject($"{presenter.Label}");
                    }
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
            var item = menu.PlacementTarget as FrameworkElement;

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

        private void ListViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var source = e.Source as FrameworkElement;
                if (source?.DataContext is SnoopableMember snoopableMember)
                {
                    listVM.ListItem_Click_Command.Execute(snoopableMember);
                    e.Handled= true;
                }
            }
        }

        private Point _initialMousePosition;
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            _initialMousePosition = e.GetPosition(this);
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            var item = Mouse.DirectlyOver as FrameworkElement;           

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var movedDistance = (_initialMousePosition - e.GetPosition(this)).Length;


                if ((movedDistance > 13) && (item?.DataContext is SnoopableMember snoopableMember))
                {
                    var isNameColumn = item.GetParent(x => string.Equals(x.Tag, "NameColumn")) != null;

                    string textValue = "";
                    if (isNameColumn)
                    {
                        textValue = snoopableMember.Name;
                    }
                    else
                    {
                        if (snoopableMember.ValueViewModel is IValuePresenter presenter)
                        {
                            textValue = presenter.Label;
                        }
                    }

                    var bracketIndex =  textValue.IndexOf('(');
                    if (bracketIndex > 0)
                    {
                        textValue = textValue.Substring(0, bracketIndex).Trim();
                    }

                    DataObject data = new DataObject();                 
                    data.SetData("Object", snoopableMember);
                    data.SetData(DataFormats.StringFormat, textValue ?? "");                  
                    DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
                    e.Handled = false;
                }
            }

            base.OnPreviewMouseMove(e);
        }  
        
    }
}