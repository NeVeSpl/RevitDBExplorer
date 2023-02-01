using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class CategoryCmdFactory : CommandFactory<CategoryMatch>
    {
        public override IEnumerable<string> GetClassifiers()
        {
            yield return "c";
            yield return "cat";
            yield return "category";
        }
        public override IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public override bool CanRecognizeArgument(string argument)
        {
            if (argument.StartsWith(nameof(BuiltInCategory), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }


        public override ICommand Create(string cmdText, IList<ILookupResult> arguments)
        {
            return new Command(CmdType.Category, cmdText, arguments, null);
        }
        public override IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            var arg = argument.RemovePrefix(nameof(BuiltInCategory));
            return FuzzySearchEngine.Lookup(arg, FuzzySearchEngine.LookFor.Category);
        }
    }
}
