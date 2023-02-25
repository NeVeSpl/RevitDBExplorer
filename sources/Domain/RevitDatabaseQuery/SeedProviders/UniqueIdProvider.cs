using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Providers.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Providers
{
    internal class UniqueIdProvider : Provider
    {
        private readonly List<UniqueIdCmdArgument> args;

        public UniqueIdProvider(List<UniqueIdCmdArgument> args)
        {
            this.args = args;
            foreach (var arg in args)
            {
                Syntax += $"elementIds.Add(document.GetElement({arg.Name}).Id);";
            }            
        }

        public override IEnumerable<ElementId> GetIds(UIDocument uiDocument)
        {
            foreach (var arg in args)
            {
                var element = uiDocument.Document.GetElement(arg.Value);
                if (element != null) 
                {
                    yield return element.Id;
                }
            }
        }


        public static IEnumerable<Provider> Create(IList<ICommand> commands)
        {
            var ids = commands.OfType<UniqueIdCmd>().SelectMany(x => x.Arguments).OfType<UniqueIdCmdArgument>().ToList();
            if (ids.Any())
            {
                yield return new UniqueIdProvider(ids);
            }
        }
    }
}