using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.WPF.Controls
{
    public interface IAutocompleteItem
    {
        string Label { get; }
        string TextToInsert { get; }
    }
    public class AutocompleteItem : IAutocompleteItem
    {
        public string Label { get; init; }
        public string TextToInsert { get; init; }

        public AutocompleteItem(string label, string textToInsert)
        {
            Label = label;
            TextToInsert = textToInsert;
        }
    }
    public interface IAutocompleteItemProvider
    {
        ObservableCollection<IAutocompleteItem> GetAutocompleteItems(string fullText, string textOnTheLeftSideOfCaret);
    }


    public partial class TextBoxWithPlaceholder : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata("Placeholder"));

        public static readonly DependencyProperty AutocompleteItemProviderProperty = DependencyProperty.Register(nameof(AutocompleteItemProvider), typeof(IAutocompleteItemProvider), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata(null));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        public IAutocompleteItemProvider AutocompleteItemProvider
        {
            get { return (IAutocompleteItemProvider)GetValue(AutocompleteItemProviderProperty); }
            set { SetValue(AutocompleteItemProviderProperty, value); }
        }
       
        public ObservableCollection<IAutocompleteItem> AutocompleteItems
        {
            get
            {
                return cListBox.ItemsSource as ObservableCollection<IAutocompleteItem>;
            }
            set
            {
                cListBox.ItemsSource = value;
            }
        }

      
        public TextBoxWithPlaceholder()
        {            
            InitializeComponent();
            cMainGrid.DataContext = this;

                    
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Window parentWindow = this.GetParentWindow();
            if (parentWindow != null)
            {
                parentWindow.Deactivated += ParentWindow_Deactivated;
                parentWindow.Activated += ParentWindow_Activated;
            }
        }


        private void ParentWindow_Deactivated(object sender, EventArgs e)
        {            
            cPopup.IsOpen = false;
        }
        private void ParentWindow_Activated(object sender, EventArgs e)
        {
           
        }
                

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                cPopup.IsOpen = false;
            }
            if (e.Key == Key.Enter)
            {
                cPopup.IsOpen = false;
            }
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is ListBoxItem) return;

            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                case Key.Prior:
                case Key.Next:
                    if (TryOpen())
                    {
                        cListBox.Focus();
                        cListBox.SelectedIndex = 0;
                        cListBox.ScrollIntoView(cListBox.SelectedItem);
                        var cfi = cListBox.ItemContainerGenerator.ContainerFromItem(cListBox.SelectedItem);
                        var lbi = cfi as ListBoxItem;
                        lbi.Focus();                       
                    }
                    e.Handled = true;
                    break;
            }
        }
        bool internalChange = false;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {            
            if (internalChange) return;
            TryOpen();
        }
        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            { 
                TryOpen();
            }
        }

        private void ListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = (e.OriginalSource as FrameworkElement)?.DataContext as IAutocompleteItem;
                if (item != null)
                {
                    e.Handled = true;
                    InserText(item.TextToInsert);
                    cPopup.IsOpen = false;
                    Keyboard.Focus(cTextBox);                   
                }
            }            
        }
        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            var item = (e.OriginalSource as FrameworkElement)?.DataContext as IAutocompleteItem;
            if (item == null) return;  
            
            if (e.Key== Key.Enter) 
            {
                InserText(item.TextToInsert);
                e.Handled = true;
            }
            if (e.Key== Key.Escape) 
            {                
                e.Handled = true;
            }
            if (e.Handled)
            {
                cPopup.IsOpen = false;
                Keyboard.Focus(cTextBox);               
            }
        }   

        private bool TryOpen()
        {
            AutocompleteItems = AutocompleteItemProvider?.GetAutocompleteItems(cTextBox.Text, cTextBox.Text.Substring(0, cTextBox.CaretIndex));

            if (cListBox.Items.Count == 0) return false;
            cPopup.IsOpen = true;
            return true;
        }
        private void InserText(string text)
        {
            internalChange = true;
            int indx = cTextBox.CaretIndex;
            cTextBox.SelectedText = text;
            cTextBox.Select(indx + text.Length, 0);
            internalChange = false;
        }


        private void MainGrid_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!this.IsParentFor(e.NewFocus as DependencyObject) && !cPopup.Child.IsParentFor(e.NewFocus as DependencyObject))
            {
                if (e.NewFocus is not IAutocompleteItem)
                {
                    cPopup.IsOpen = false;
                }
            }
        }
        private CustomPopupPlacement[] CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            return new CustomPopupPlacement[] {new CustomPopupPlacement(new Point((0.01 - offset.X), (cMainGrid.ActualHeight - offset.Y)), PopupPrimaryAxis.None) };
        }

       
    
    }
}