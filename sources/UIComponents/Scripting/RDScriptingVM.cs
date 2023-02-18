using System;
using System.Threading.Tasks;
using System.Windows;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.RevitDatabaseScripting;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Scripting
{
    internal class RDScriptingVM : BaseViewModel
    {
        private IScriptRunner scriptRunner;
        private bool isOpen = false;
        private GridLength height;
        private IRoslynCodeEditor roslynCodeEditor;


        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
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

            if (result.Query != null)
            {
                await scriptRunner.TryExecuteScript(new SourceOfObjects(result.Query) { Title = "User query" });
            }
        }
    }

    internal interface IScriptRunner
    {
        Task TryExecuteScript(SourceOfObjects source);
    }
}