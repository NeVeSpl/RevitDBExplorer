using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.CodeAnalysis;
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
        private int selectedTabIndex;

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


        public RDScriptingVM(IScriptRunner scriptRunner)
        {
            CloseCommand = new RelayCommand(Close);
            RunCommand = new RelayCommand(Run);
            this.scriptRunner = scriptRunner;
        }


        public void Open(string databaseQueryToolTip)
        {
            IsOpen = true;
            Height = new GridLength(Math.Max(Height.Value, 100));

            var appArg = "";
            if (databaseQueryToolTip.Contains("uia."))
            {
                appArg = ", UIApplication uia";
            }

            var text = 
@$"IEnumerable<object> Select(Document document{appArg})
{{
    return {databaseQueryToolTip};    
}}";

            RoslynCodeEditor.SetText(text);
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

            var originalConsoleOut = Console.Out;
            var writer = new StringWriter();
            try
            {
                Console.SetOut(writer);

                if (result.Query != null)
                {
                    await scriptRunner.TryExecuteQuery(new SourceOfObjects(result.Query) { Title = "User query" });
                }
            }
            finally
            {
                Console.SetOut(originalConsoleOut);
            }

            var output = new List<Inline>();
            foreach (var diagnostic in result.Diagnostics)
            {
                output.Add(new Run() { Text = diagnostic, Foreground = Brushes.Red } );
                output.Add(new LineBreak());
                SelectedTabIndex = 1;
            }

            var consoleLines = writer.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in consoleLines)
            {
                output.Add(new Run() { Text = line });
                output.Add(new LineBreak());
            }

            Output = output;
        }
    }

    internal interface IScriptRunner
    {
        Task TryExecuteQuery(SourceOfObjects source);
    }
}