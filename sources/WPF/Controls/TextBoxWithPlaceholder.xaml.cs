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
        bool IsChosenOne { get; set; }
    }
    public class AutocompleteItem : IAutocompleteItem
    {
        public string Label { get; init; }
        public string TextToInsert { get; init; }
        public string Description { get; init; }
        public bool IsChosenOne { get; set; }

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
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public bool IsPopupOpen
        {
            get => (bool)GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty); 
            set => SetValue(PlaceholderProperty, value);
        }
        public IAutocompleteItemProvider AutocompleteItemProvider
        {
            get => (IAutocompleteItemProvider)GetValue(AutocompleteItemProviderProperty);
            set => SetValue(AutocompleteItemProviderProperty, value);
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
            Popup_SetIsOpen(false);
        }
        private void ParentWindow_Activated(object sender, EventArgs e)
        {
           
        }
        private void ParentWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Popup_SetIsOpen(false);
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Popup_SetIsOpen(false);
            }           
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (IsPopupOpen)
                {
                    var item = cListBox.SelectedItem as IAutocompleteItem;
                    Popup_SetIsOpen(false);

                    if (item != null)
                    {
                        InserText(item.TextToInsert);
                        TryOpen(true);
                    }
                    else
                    {
                        this.GetBindingExpression(TextBoxWithPlaceholder.TextProperty).UpdateSource();
                    }
                }
                else
                {
                    this.GetBindingExpression(TextBoxWithPlaceholder.TextProperty).UpdateSource();
                }
            }   
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (IsPopupOpen && Text.Length == 0)
                {
                    e.Handled = true;
                    Popup_SetIsOpen(false);
                    this.GetBindingExpression(TextBoxWithPlaceholder.TextProperty).UpdateSource();
                }
            }
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                if (IsPopupOpen)
                {
                    if (e.Key == Key.Up)
                    {
                        cListBox.SelectedIndex = Math.Max(0, cListBox.SelectedIndex - 1);
                    }
                    if (e.Key == Key.Down)
                    {
                        cListBox.SelectedIndex = Math.Min(cListBox.Items.Count, cListBox.SelectedIndex + 1);
                    }
                }
                else
                {
                    if (TryOpen())
                    {
                        //cListBox.Focus();
                        cListBox.SelectedIndex = 0;
                        cListBox.ScrollIntoView(cListBox.SelectedItem);
                        //var cfi = cListBox.ItemContainerGenerator.ContainerFromItem(cListBox.SelectedItem);
                        //var lbi = cfi as ListBoxItem;
                        //lbi.Focus();      
                    }
                }
                e.Handled = true;
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
     

        int prefixLength;
        private bool TryOpen(bool again = false)
        {
            if (AutocompleteItemProvider == null) return false;

            (var autocompleteItems, prefixLength) = AutocompleteItemProvider.GetAutocompleteItems(cTextBox.Text, cTextBox.CaretIndex);
        
            bool shouldBeOpened = autocompleteItems?.Count() > 0;

            if (shouldBeOpened && again)
            {
                shouldBeOpened = autocompleteItems.Count() != AutocompleteItems.Count();

                if (autocompleteItems.Count() == 1)
                {
                    if (autocompleteItems.First().TextToInsert.Length == prefixLength)
                    {
                        shouldBeOpened = false;
                    }
                }                    
            }
            
            if (shouldBeOpened)
            {
                AutocompleteItems = new ObservableCollection<IAutocompleteItem>(autocompleteItems);
            }

            Popup_SetIsOpen(shouldBeOpened);
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
                    Popup_SetIsOpen(false);
                }
            }
        }
        private CustomPopupPlacement[] CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            return new CustomPopupPlacement[] {new CustomPopupPlacement(new Point((0.01 - offset.X), (cMainGrid.ActualHeight - offset.Y -1)), PopupPrimaryAxis.None) };
        }

        private void Popup_SetIsOpen(bool state)
        {
            cPopup.IsOpen = state;
            IsPopupOpen = state;           
        }      
    }
}