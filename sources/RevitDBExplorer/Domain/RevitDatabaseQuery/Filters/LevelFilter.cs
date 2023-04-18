using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class LevelFilter : Filter
    {
        private readonly LevelCmdArgument levelArg;


        public LevelFilter(LevelCmdArgument level)
        {
            this.levelArg = level;  
            FilterSyntax = $"new ElementLevelFilter({level.Name})";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var levels = commands.OfType<LevelCmd>().SelectMany(x => x.Arguments).OfType<LevelCmdArgument>().ToList();
            if (levels.Count == 1)
            {
                yield return new LevelFilter(levels.First());
            }
            if (levels.Count > 1)
            {
                yield return new Group(levels.Select(x => new LevelFilter(x)).ToList());
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementLevelFilter(levelArg.Value);
        }
    }   
}