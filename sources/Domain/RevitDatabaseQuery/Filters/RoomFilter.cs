using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class RoomFilter : Filter
    {
        private readonly RoomCmdArgument roomMatch;
        private readonly Room room;

        public RoomFilter(RoomCmdArgument roomMatch, Room room)
        {
            this.roomMatch = roomMatch;
            var solid = room.ClosedShell.OfType<Solid>().FirstOrDefault();
            if (solid?.Volume > 0)
            {
                var orgin = solid.ComputeCentroid();
                var tranTranslation = Transform.CreateTranslation(orgin.Negate());
                var tranScale = Transform.Identity.ScaleBasis(1.05);
                var solidWithDelta = SolidUtils.CreateTransformed(solid, tranTranslation.Inverse * tranScale * tranTranslation);              
                Filter = new ElementIntersectsSolidFilter(solidWithDelta);
                FilterSyntax = $"new ElementIntersectsSolidFilter({roomMatch.Name})";
            }
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands, Document document)
        {
            var rooms = commands.OfType<RoomCmd>().SelectMany(x => x.Arguments).OfType<RoomCmdArgument>().ToList();
            if (rooms.Count == 1)
            {
                yield return new RoomFilter(rooms.First(), document.GetElement(rooms.First().Value) as Room);
            }
            if (rooms.Count > 1)
            {
                yield return new Group(rooms.Select(x => new RoomFilter(x, document.GetElement(x.Value) as Room)).Where(x => x.Filter != null).ToList());
            }
        }
    }    
}