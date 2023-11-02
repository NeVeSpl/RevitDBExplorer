using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace RevitDBExplorer.WPF.Controls
{
    /// <summary>
    /// source: https://stackoverflow.com/questions/9794151/stop-tabcontrol-from-recreating-its-children
    /// </summary>
    [TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
    public class TabControlEx : TabControl
    {
        private Panel ItemsHolderPanel = null;

        public TabControlEx()
            : base()
        {
            // This is necessary so that we get the initial databound selected item
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        /// <summary>
        /// If containers are done, generate the selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                this.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                UpdateSelectedItem();
            }
        }

        /// <summary>
        /// Get the ItemsHolder and generate any children
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemsHolderPanel = GetTemplateChild("PART_ItemsHolder") as Panel;
            UpdateSelectedItem();
        }



        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateSelectedItem();
        }

        private void UpdateSelectedItem()
        {
            if (ItemsHolderPanel == null)
                return;

            // Generate a ContentPresenter if necessary
            TabItem item = GetSelectedTabItem();
            if (item != null)
                CreateChildContentPresenter(item);

            // show the right child
            foreach (ContentPresenter child in ItemsHolderPanel.Children)
                child.Visibility = ((child.Tag as TabItem).IsSelected) ? Visibility.Visible : Visibility.Collapsed;
        }

        private ContentPresenter CreateChildContentPresenter(object item)
        {
            if (item == null)
                return null;

            ContentPresenter cp = FindChildContentPresenter(item);

            if (cp != null)
                return cp;

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            cp = new ContentPresenter();
            cp.Content = (item is TabItem) ? (item as TabItem).Content : item;
            cp.ContentTemplate = this.SelectedContentTemplate ?? this.ContentTemplate;
            cp.ContentTemplateSelector = this.SelectedContentTemplateSelector;
            cp.ContentStringFormat = this.SelectedContentStringFormat ?? this.ContentStringFormat;
            cp.Visibility = Visibility.Collapsed;
            cp.Tag = (item is TabItem) ? item : (this.ItemContainerGenerator.ContainerFromItem(item));
            ItemsHolderPanel.Children.Add(cp);
            return cp;
        }

        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data is TabItem)
                data = (data as TabItem).Content;

            if (data == null)
                return null;

            if (ItemsHolderPanel == null)
                return null;

            foreach (ContentPresenter cp in ItemsHolderPanel.Children)
            {
                if (cp.Content == data)
                    return cp;
            }

            return null;
        }

        protected TabItem GetSelectedTabItem()
        {
            object selectedItem = base.SelectedItem;
            if (selectedItem == null)
                return null;

            TabItem item = selectedItem as TabItem;
            if (item == null)
                item = base.ItemContainerGenerator.ContainerFromIndex(base.SelectedIndex) as TabItem;

            return item;
        }
    }
}
