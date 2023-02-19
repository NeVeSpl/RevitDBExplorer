using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RevitDBExplorer.Domain.DataModel;
using static RevitDBExplorer.Domain.RevitDatabaseQuery.RevitDatabaseQueryService;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;

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
            typeof(UIApplication).Assembly,
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
            "Autodesk.Revit.DB.Architecture",
            "Autodesk.Revit.UI",
        };


        public static async Task<ResultOfCompilation> Compile(string code, LambdaToBe lambdaToBe)
        {
            var errors = new List<string>();

            if (lambdaToBe == null)
            {
                errors.Add("Could not find any suitable method to run.");
                return new ResultOfCompilation(null, null, errors);
            }

            var options = ScriptOptions.Default.AddReferences(RevitDatabaseScriptingService.AssemblyReferences).AddImports(RevitDatabaseScriptingService.Imports);  
            var script = CSharpScript.Create<object>(code, options);

            var compilation = script.GetCompilation();
            var diagnostics = script.Compile();       

            if (diagnostics.Any(x => x.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))
            {
                foreach (var diagnostic in diagnostics.Where(x => x.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))
                {
                    errors.Add(diagnostic.ToString());
                }

                return new ResultOfCompilation(null, null, errors);
            }           

            Query query = null;

            var (invocation, isResolved) = PrepereInvocation(lambdaToBe);

            if (isResolved == false)
            {
                errors.Add("Could not resolve method arguments");
                return new ResultOfCompilation(null, null, errors);
            }

            if (lambdaToBe.IsReturnTypeEnumerable)
            {
                // query, returns collection
                var lambda = await script.ContinueWith<Func<UIApplication, IEnumerable<object>>>($"(UIApplication uia) => {lambdaToBe.Name}({invocation})").RunAsync();
                query = Query.Create(lambda.ReturnValue);
            }
            else
            {
                if (!lambdaToBe.IsReturnTypeVoid)
                {
                    // query, retruns single object
                    var lambda = await script.ContinueWith<Func<UIApplication, object>>($"(UIApplication uia) => {lambdaToBe.Name}({invocation})").RunAsync();
                    query = Query.Create(lambda.ReturnValue);
                }
                else
                {
                    // command
                    errors.Add("Command are not supported (yet)");
                }
            }

            return new ResultOfCompilation(query, null, errors);
        }

        public static (string invocation, bool isResolved) PrepereInvocation(LambdaToBe lambdaToBe)
        {
            List<string> arguments = new List<string>();
            foreach (var parameter in lambdaToBe.Parameters)
            {
                switch(parameter)
                {
                    case "Autodesk.Revit.DB.Document":
                        arguments.Add("uia?.ActiveUIDocument?.Document");
                        break;
                    case "Autodesk.Revit.UI.UIApplication":
                        arguments.Add("uia");
                        break;
                    default:
                        return ("", false);
                }
            }

            var invocation = String.Join(", ", arguments);
            return (invocation, true);
        }


        internal class Query : IAmSourceOfEverything
        {
            private readonly Func<UIApplication, IEnumerable<SnoopableObject>> query;

            private Query(Func<UIApplication, IEnumerable<SnoopableObject>> query)
            {
                this.query = query;
            }


            public static Query Create(Func<UIApplication, object> lambda)
            {
                if (lambda == null) return null;

                var query = new Query((UIApplication uia) => 
                {
                    var document = uia.ActiveUIDocument?.Document;
                    if (document == null) return null;

                    var returnValue = lambda(uia);

                    if (returnValue is FilteredElementCollector collector)
                    {
                        return collector.ToElements().Select(x => new SnoopableObject(document, x));
                    }

                    return new[] { new SnoopableObject(document, returnValue) };
                });               
                return query;
            }
            public static Query Create(Func<UIApplication, IEnumerable<object>> lambda)
            {
                if (lambda == null) return null;

                var query = new Query((UIApplication uia) =>
                {
                    var document = uia.ActiveUIDocument?.Document;
                    if (document == null) return null;

                    return lambda(uia)?.Select(x => new SnoopableObject(document, x));
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
        public IEnumerable<string> Diagnostics { get; }
     

        public ResultOfCompilation(IAmSourceOfEverything query, ICommand command, IEnumerable<string> diagnostics)
        {
            Query = query;
            Command = command;
            Diagnostics = diagnostics;
        }
    }
}