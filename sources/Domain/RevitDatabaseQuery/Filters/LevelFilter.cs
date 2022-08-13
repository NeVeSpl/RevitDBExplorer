using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class LevelFilter : Filter
    {
        private readonly LevelMatch levelMatch;


        public LevelFilter(LevelMatch level)
        {
            this.levelMatch = level;           
            Filter = new ElementLevelFilter(level.Value);
            FilterSyntax = $"new ElementLevelFilter({level.Name})";
        }


        public static IEnumerable<QueryItem> Create(List<Command> commands)
        {
            var levels = commands.Where(x => x.Type == CmdType.Level).SelectMany(x => x.MatchedArguments).OfType<LevelMatch>().ToList();
            if (levels.Count == 1)
            {
                yield return new LevelFilter(levels.First());
            }
            if (levels.Count > 1)
            {
                yield return new Group(levels.Select(x => new LevelFilter(x)).ToList());
            }
        }
    }
}