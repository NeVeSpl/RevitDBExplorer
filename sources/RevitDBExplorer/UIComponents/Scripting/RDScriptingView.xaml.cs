using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RevitDBExplorer.Domain.RevitDatabaseScripting;
using RoslynPad.Editor;
using RoslynPad.Roslyn;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

internal interface IRoslynCodeEditor
{
    string GetText();
    void SetText(string text);
    Task<LambdaToBe> GetTypeOfLambda();
}

namespace RevitDBExplorer.UIComponents.Scripting
{
    public partial class RDScriptingView : UserControl, IRoslynCodeEditor
    {
        private RDScriptingVM scriptingVM;
        private RoslynHost roslynHost = null;
        private bool isRoslynCodeEditorInitialized = false;
        private Microsoft.CodeAnalysis.DocumentId documentId;


        public RDScriptingView()
        {
            InitializeComponent();
            this.DataContextChanged += RDScriptingView_DataContextChanged;
        }

        private void RDScriptingView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is RDScriptingVM vm)
            {
                vm.RoslynCodeEditor = this;
                scriptingVM = vm;
            }
        }


        public string GetText()
        {
            InitializeRoslynCodeEditor();
            return cRoslynCodeEditor.Text;
        }
        public void SetText(string text)
        {
            InitializeRoslynCodeEditor();
            cRoslynCodeEditor.Clear();
            cRoslynCodeEditor.AppendText(text);
        }


        private void InitializeRoslynCodeEditor()
        {
            if (isRoslynCodeEditorInitialized) return;
            isRoslynCodeEditorInitialized = true;

            var workingDirectory = Directory.GetCurrentDirectory();
            documentId = cRoslynCodeEditor.Initialize(InitializeRoslynHost(), new ClassificationHighlightColors(), workingDirectory, string.Empty);
        }


        private RoslynHost InitializeRoslynHost()
        {
            if (roslynHost == null)
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                var additionalAssemblies = new[]
                {
                    typeof(GlyphExtensions).Assembly,
                    typeof(RoslynCodeEditor).Assembly,
                };

                var references = RoslynHostReferences.NamespaceDefault.With(assemblyReferences: RevitDatabaseScriptingService.AssemblyReferences, imports: RevitDatabaseScriptingService.Imports);

                roslynHost = new RoslynHost(additionalAssemblies, references);
       
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            }
            return roslynHost;
        }
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            if (name.Name == "Microsoft.Bcl.AsyncInterfaces")
            {
                return typeof(IAsyncDisposable).Assembly;
            }
            return null;
        }

        async Task<LambdaToBe> IRoslynCodeEditor.GetTypeOfLambda()
        {
            var document = roslynHost.GetDocument(documentId);
            var model = await document.GetSemanticModelAsync();
            if (model != null)
            {
                var symbolFinder = new SymbolFinder(model.Compilation.AssemblyName);
                model.Compilation.GlobalNamespace.Accept(symbolFinder);
                return symbolFinder.Result;
            }
            return null;
        }

        private void TabControl_DragOver(object sender, DragEventArgs e)
        {

        }
        private void TabControl_Drop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);
            
            if (e.Data.GetDataPresent("Inputs"))
            {
                var inputs = e.Data.GetData("Inputs") as IEnumerable<object>;

                scriptingVM?.SetInput(inputs);
            }
            e.Handled = true;
        }
    }
}