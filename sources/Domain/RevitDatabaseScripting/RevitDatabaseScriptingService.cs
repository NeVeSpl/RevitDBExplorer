using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class RevitDatabaseScriptingService
    {
        public static readonly Assembly[] AssemblyReferences = new[]
        {
            typeof(object).Assembly,
            typeof(Enumerable).Assembly,
            typeof(ISet<>).Assembly,
            typeof(Document).Assembly,
        };
        public static readonly string[] Imports = new[]
        {
            "System",
            "System.Threading",
            "System.Threading.Tasks",
            "System.Collections",
            "System.Collections.Generic",
            "System.Text",
            "System.Text.RegularExpressions",
            "System.Linq",
            "Autodesk.Revit.DB",
            "Autodesk.Revit.DB.Structure",
            "Autodesk.Revit.DB.Architecture"
        };


        public static async Task<ResultOfCompilation> Compile(string code, LambdaToBe lambdaToBe)
        {
            var options = ScriptOptions.Default.AddReferences(RevitDatabaseScriptingService.AssemblyReferences).AddImports(RevitDatabaseScriptingService.Imports);  
            var script = CSharpScript.Create<object>(code, options);

            var compilation = script.GetCompilation();
            var diagnostics = script.Compile();

            var id = lambdaToBe.ReturnType + "_" + string.Join(", ", lambdaToBe.Parameters);

            if (diagnostics.Any(x => x.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))
            {
                return new ResultOfCompilation(null, null);
            }

            Query query = null;
            switch (id)
            {
                case "Autodesk.Revit.DB.FilteredElementCollector_Autodesk.Revit.DB.Document":
                    {
                        var lambda = await script.ContinueWith<Func<Document, FilteredElementCollector>>($"(Document document) => {lambdaToBe.Name}(document)").RunAsync();
                        query = Query.Create(lambda.ReturnValue);
                        break;
                    }
                case "System.Collections.Generic.IEnumerable<Autodesk.Revit.DB.Element>_Autodesk.Revit.DB.Document":
                case "System.Collections.Generic.IEnumerable<System.Object>_Autodesk.Revit.DB.Document":
                    {
                        var lambda = await script.ContinueWith<Func<Document, IEnumerable<object>>>($"(Document document) => {lambdaToBe.Name}(document)").RunAsync();
                        query = Query.Create(lambda.ReturnValue);
                        break;
                    }
            }

            return new ResultOfCompilation(query, null);
        }

        internal class Query : IAmSourceOfEverything
        {
            private readonly Func<UIApplication, IEnumerable<SnoopableObject>> query;

            private Query(Func<UIApplication, IEnumerable<SnoopableObject>> query)
            {
                this.query = query;
            }


            public static Query Create(Func<Document, FilteredElementCollector> lambda)
            {
                if (lambda == null) return null;

                var query = new Query((UIApplication uia) => 
                {
                    var document = uia.ActiveUIDocument?.Document;
                    if (document == null) return null;

                    return lambda(document).ToElements().Select(x => new SnoopableObject(document, x));
                });               
                return query;
            }
            public static Query Create(Func<Document, IEnumerable<object>> lambda)
            {
                if (lambda == null) return null;

                var query = new Query((UIApplication uia) =>
                {
                    var document = uia.ActiveUIDocument?.Document;
                    if (document == null) return null;

                    return lambda(document).Select(x => new SnoopableObject(document, x));
                });
                return query;
            }

            public IEnumerable<SnoopableObject> Snoop(UIApplication app)
            {
                return query(app);
            }
        }
    }
    

    internal class ResultOfCompilation
    {        
        public IAmSourceOfEverything Query { get; }
        public ICommand Command { get; }


        public ResultOfCompilation(IAmSourceOfEverything query, ICommand command)
        {
            Query = query;
            Command = command;
        }
    }
}