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
        private IScriptRunner scriptRunner;
        private bool isPanelOpen = false;
        private GridLength height;
        private IRoslynCodeEditor roslynCodeEditor;
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
        public IRoslynCodeEditor RoslynCodeEditor
        {
            get
            {
                return roslynCodeEditor;
            }
            set
            {
                roslynCodeEditor = value;
                OnPropertyChanged();
            }
        }
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


        public RDScriptingVM(IScriptRunner scriptRunner)
        {
            CloseCommand = new RelayCommand(Close);
            RunCommand = new RelayCommand(Run);
            this.scriptRunner = scriptRunner;
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
            var code = RoslynCodeEditor.GetText();
            var lambdaToBe = await roslynCodeEditor.GetTypeOfLambda();

            var result = await RevitDatabaseScriptingService.Compile(code, lambdaToBe);
            result.SetInputObjects(inputs);

            var originalConsoleOut = Console.Out;
            var writer = new StringWriter();
            try
            {
                Console.SetOut(writer);

                if (result.SelectQuery != null)
                {                    
                    await scriptRunner.TryExecuteQuery(new SourceOfObjects(result.SelectQuery) { Title = "User query" }); 
                    writer.WriteLine($"{DateTimeOffset.Now.ToString("hh:mm:ss")} : query completed");
                }
                if (result.UpdateQuery != null)
                {
                    await ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync(x => result.UpdateQuery.Execute(x), null, "RDS update command");
                    writer.WriteLine($"{DateTimeOffset.Now.ToString("HH:mm:ss")} : query completed");
                }
            }
            finally
            {
                Console.SetOut(originalConsoleOut);
            }

            var output = new List<Inline>();
            var outputDocument = new FlowDocument() { PagePadding = new Thickness(0), FontFamily= Application.DefaultFontFamily };
            foreach (var diagnostic in result.Diagnostics)
            {
                output.Add(new Run() { Text = diagnostic, Foreground = Brushes.Red } );
                output.Add(new LineBreak());
                SelectedTabIndex = 2;
            }

            var consoleLines = writer.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in consoleLines)
            {
                output.Add(new Run() { Text = line });
                output.Add(new LineBreak());
            }

            var paragraph = new Paragraph() { FontSize = 12, };
            paragraph.Inlines.AddRange(output);
            outputDocument.Blocks.Add(paragraph);

            Output = output;
            OutputDocument = outputDocument;
        }

        public void SetScript(string scriptText)
        {
            RoslynCodeEditor.SetText(scriptText);
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

    internal interface IScriptRunner
    {
        Task TryExecuteQuery(SourceOfObjects source);
    }
}