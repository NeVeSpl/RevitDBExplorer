using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.RevitDatabaseScripting;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Scripting
{
    internal class RDScriptingVM : BaseViewModel
    {       
        private bool isPanelOpen = false;
        private GridLength height;      
        private IEnumerable<Inline> output;
        private FlowDocument outputDocument;
        private IEnumerable<Inline> input;
        private int selectedTabIndex;
        private IEnumerable<object> inputs = Enumerable.Empty<object>();

        public bool IsOpen
        {
            get
            {
                return isPanelOpen;
            }
            set
            {
                isPanelOpen = value;
                OnPropertyChanged();
            }
        }
        public GridLength Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }
   

        public RelayCommand CloseCommand { get; }
        public RelayCommand RunCommand { get; }       
        public IEnumerable<Inline> Output
        {
            get
            {
                return output;
            }
            set
            {
                output = value;
                OnPropertyChanged();
            }
        }
        public FlowDocument OutputDocument
        {
            get
            {
                return outputDocument;
            }
            set
            {
                outputDocument = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<Inline> Input
        {
            get
            {
                return input;
            }
            set
            {
                input = value;
                OnPropertyChanged();
            }
        }
        public int SelectedTabIndex
        {
            get
            {
                return selectedTabIndex;
            }
            set
            {
                selectedTabIndex = value;
                OnPropertyChanged();
            }
        }
        public int InputsCount
        {
            get => inputs.Count();
        }


        public RDScriptingVM()
        {
            CloseCommand = new RelayCommand(Close);
            RunCommand = new RelayCommand(Run);          
        }


        public void Open()
        {           
            IsOpen = true;
            Height = new GridLength(Math.Max(Height.Value, 198));
            SelectedTabIndex = 1;
            PrintInputs(); 
        }
        private void Close(object parameter)
        {
            IsOpen = false;
            Height = new GridLength(0, GridUnitType.Auto);
        }
        private async void Run(object parameter)
        {            
          

        }

        public void SetScript(string scriptText)
        {
            
        }
        public void SetInput(IEnumerable<object> inputs)
        {
            this.inputs = inputs;
            //SelectedTabIndex = 0;
            OnPropertyChanged(nameof(InputsCount));
            PrintInputs();
        }

        private readonly SolidColorBrush typeBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#008080");
        private void PrintInputs()
        {
            var input = new List<Inline>()
            {
                new Run("Parameters that can be used in the method header:"),
                new LineBreak(),
                new Run("    Document ") {Foreground=typeBrush }, new Run("document"),
                new LineBreak(),
                new Run("    UIApplication ") {Foreground=typeBrush }, new Run("uia"),
                new LineBreak(),
                new Run("    IEnumerable") {Foreground=typeBrush }, new Run("<object>") {Foreground=typeBrush }, new Run(" objects - which contains curently "), new Run($"{inputs.Count()}"){Foreground=Brushes.Blue, FontWeight=FontWeights.Bold }, new Run(" objects"),
            };
            Input = input;
        }
    }  
}