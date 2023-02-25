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
    internal class SelectionProvider : Provider
    {


        public SelectionProvider()
        {
            Syntax = "elementIds.AddRange(uia.ActiveUIDocument.Selection.GetElementIds());";
        }

        public override IEnumerable<ElementId> GetIds(UIDocument uiDocument)
        {
            return uiDocument.Selection.GetElementIds();
        }


        public static IEnumerable<Provider> Create(IList<ICommand> commands)
        {
           
            if (commands.OfType<SelectionCmd>().Any())
            {
                yield return new SelectionProvider();
            }            
        }
    }
}