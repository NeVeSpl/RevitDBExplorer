using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            typeof(UIApplication).Assembly,
            typeof(RevitDatabaseScriptingService).Assembly,
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
            "RevitDBExplorer.Domain.RevitDatabaseScripting"
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
            Query command = null;
            var (invocation, isResolved) = PrepereInvocation(lambdaToBe);

            if (isResolved == false)
            {
                errors.Add("Could not resolve method arguments");
                return new ResultOfCompilation(null, null, errors);
            }
          
            if (!lambdaToBe.ReturnType.IsVoid)
            {
                // query
                var lambda = await script.ContinueWith<Func<LambdaParams, object>>($"(LambdaParams parameters) => {lambdaToBe.Name}({invocation})").RunAsync();
                query = Query.Create(lambda.ReturnValue);
            }
            else
            {
                // command
                var lambda = await script.ContinueWith<Func<LambdaParams, object>>($"(LambdaParams parameters) => {{{lambdaToBe.Name}({invocation}); return null;}}").RunAsync();
                command = Query.Create(lambda.ReturnValue);               
            }            

            return new ResultOfCompilation(query, command, errors);
        }

        public static (string invocation, bool isResolved) PrepereInvocation(LambdaToBe lambdaToBe)
        {
            List<string> arguments = new List<string>();
            foreach (var parameter in lambdaToBe.Parameters)
            {
                if (parameter.Name == "Autodesk.Revit.DB.Document")
                {
                    arguments.Add("parameters.uia?.ActiveUIDocument?.Document");
                    continue;
                }
                if (parameter.Name == "Autodesk.Revit.UI.UIApplication")
                {
                    arguments.Add("parameters.uia");
                    continue;
                }
                if (parameter.IsEnumerable)
                {
                    arguments.Add($"parameters.objects.OfType<{parameter.FirstTypeArgumentName}>()");
                    continue;
                }              
                  
                return ("", false);                
            }

            var invocation = String.Join(", ", arguments);
            return (invocation, true);
        }

        

        internal class Query : IAmSourceOfEverything, IAcceptInput, IAmCommand
        {
            private readonly Func<LambdaParams, IEnumerable<SnoopableObject>> query;
            private IEnumerable<object> inputObjects = Enumerable.Empty<object>();

            private Query(Func<LambdaParams, IEnumerable<SnoopableObject>> query)
            {
                this.query = query;
            }


            public static Query Create(Func<LambdaParams, object> lambda)
            {
                if (lambda == null) return null;

                var query = new Query((LambdaParams parameters) => 
                {
                    var document = parameters.uia.ActiveUIDocument?.Document;
                    if (document == null) return null;

                    var returnValue = lambda(parameters);

                    if (returnValue is FilteredElementCollector collector)
                    {
                        return collector.ToElements().Select(x => new SnoopableObject(document, x));
                    }
                    if (returnValue is IEnumerable<object> enumerable)
                    {
                        return enumerable.Select(x => new SnoopableObject(document, x));
                    }

                    return new[] { new SnoopableObject(document, returnValue) };
                });               
                return query;
            }

            public void Execute(UIApplication app)
            {
                query(new LambdaParams(app, inputObjects));
            }

            public IEnumerable<SnoopableObject> Snoop(UIApplication app)
            {
                return query(new LambdaParams(app, inputObjects));
            }

            void IAcceptInput.SetInput(IEnumerable<object> inputs)
            {
                inputObjects = inputs;
            }
        }
    }

    public interface IAcceptInput
    {
        void SetInput(IEnumerable<object> inputs);
    }

    public class LambdaParams
    {
        public readonly UIApplication uia;
        public readonly IEnumerable<object> objects; 


        public LambdaParams(UIApplication app, IEnumerable<object> inputObjects)
        {
            uia = app;
            objects = inputObjects;
        }
    }

    internal class ResultOfCompilation
    {        
        public IAmSourceOfEverything SelectQuery { get; }
        public IAmCommand UpdateQuery { get; }
        public IEnumerable<string> Diagnostics { get; }
     

        public ResultOfCompilation(IAmSourceOfEverything selectQuery, IAmCommand updateQuery, IEnumerable<string> diagnostics)
        {
            SelectQuery = selectQuery;
            UpdateQuery = updateQuery;
            Diagnostics = diagnostics;
        }

        public void SetInputObjects(IEnumerable<object> inputs)
        {
            if (SelectQuery is IAcceptInput acceptInput)
            {
                acceptInput.SetInput(inputs);
            }
            if (UpdateQuery is IAcceptInput acceptInputToo)
            {
                acceptInputToo.SetInput(inputs);
            }
        }
    }
}