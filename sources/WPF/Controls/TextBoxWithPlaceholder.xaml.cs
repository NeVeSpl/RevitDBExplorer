using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.WPF.Controls
{
    public interface IAutocompleteItem
    {
        string TextToInsert { get; }
        string Label { get; }
        string Description { get; }
    }
    public class AutocompleteItem : IAutocompleteItem
    {
        public string Label { get; init; }
        public string TextToInsert { get; init; }
        public string Description { get; init; }
        

        public AutocompleteItem(string textToInsert, string label, string description)
        {
            Label = label;
            TextToInsert = textToInsert;
            Description = description;
        }
    }
    public interface IAutocompleteItemProvider
    {
        (IEnumerable<IAutocompleteItem> items, int prefixLength) GetAutocompleteItems(string fullText, int caretPosition);
    }


    public partial class TextBoxWithPlaceholder : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata("Placeholder"));

        public static readonly DependencyProperty AutocompleteItemProviderProperty = DependencyProperty.Register(nameof(AutocompleteItemProvider), typeof(IAutocompleteItemProvider), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata(null));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set 
            { 
                SetValue(TextProperty, value); 
            }
        }
        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set
            {
                SetValue(IsPopupOpenProperty, value);
            }
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

        Binding myBinding;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Window parentWindow = this.GetParentWindow();
            if (parentWindow != null)
            {
                parentWindow.Deactivated += ParentWindow_Deactivated;
                parentWindow.Activated += ParentWindow_Activated;
                parentWindow.MouseDown += ParentWindow_MouseDown;
            }

            myBinding = BindingOperations.GetBinding(cTextBox, TextBox.TextProperty);
        }

       

        private void ParentWindow_Deactivated(object sender, EventArgs e)
        {            
            cPopup.IsOpen = false;
        }
        private void ParentWindow_Activated(object sender, EventArgs e)
        {
           
        }
        private void ParentWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cPopup.IsOpen = false;
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
            if (cTextBox.IsFocused)
            {
                TryOpen();
            }
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
                    TryOpen(true);
                    
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
                TryOpen(true);               
            }
            if (e.Key== Key.Escape) 
            {
                cPopup.IsOpen = false;
                e.Handled = true;
            }
            if (e.Handled)
            {                
                Keyboard.Focus(cTextBox);               
            }
        }

        int prefixLength;
        private bool TryOpen(bool again = false)
        {
            if (AutocompleteItemProvider == null) return false;

            (var autocompleteItems, prefixLength) = AutocompleteItemProvider.GetAutocompleteItems(cTextBox.Text, cTextBox.CaretIndex);
        
            bool shouldBeOpened = autocompleteItems?.Count() > 0;

            if (shouldBeOpened && again)
            {
                shouldBeOpened = autocompleteItems.Count() != AutocompleteItems.Count();
            }

            if (shouldBeOpened)
            {
                AutocompleteItems = new ObservableCollection<IAutocompleteItem>(autocompleteItems);
            }
          
            cPopup.IsOpen = shouldBeOpened;
            return shouldBeOpened;
        }
        private void InserText(string text)
        {
            internalChange = true;
            int indx = cTextBox.CaretIndex;
            cTextBox.Select(indx - prefixLength, prefixLength);
            cTextBox.SelectedText = text;
            cTextBox.Select(indx + text.Length- prefixLength, 0);
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

        private void cPopup_Closed(object sender, EventArgs e)
        {
            IsPopupOpen = false;
            //internalChange = true;
            //Text = cTextBox.Text;
            //BindingOperations.SetBinding(cTextBox, TextBox.TextProperty, myBinding);
            //internalChange = false;
        }

        private void cPopup_Opened(object sender, EventArgs e)
        {
            IsPopupOpen = true; ;
            //BindingOperations.ClearBinding(cTextBox, TextBox.TextProperty);
        }
    }
}