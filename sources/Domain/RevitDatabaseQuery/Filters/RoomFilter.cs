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
        private readonly RoomCmdArgument arg;
     

        public RoomFilter(RoomCmdArgument arg)
        {
            this.arg = arg;          
            FilterSyntax = $"new ElementIntersectsSolidFilter((document.GetElement({arg.Name}) as Room).ClosedShell.OfType<Solid>().FirstOrDefault())";            
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var rooms = commands.OfType<RoomCmd>().SelectMany(x => x.Arguments).OfType<RoomCmdArgument>().ToList();
            if (rooms.Count == 1)
            {
                yield return new RoomFilter(rooms.First());
            }
            if (rooms.Count > 1)
            {
                yield return new Group(rooms.Select(x => new RoomFilter(x)));
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            var room = document.GetElement(arg.Value) as Room;

            var solid = room.ClosedShell.OfType<Solid>().FirstOrDefault(x => x.Volume > 0) ?? room.ClosedShell.OfType<Solid>().FirstOrDefault();

            if (solid.Volume > 0)
            {
                var orgin = solid.ComputeCentroid();
                var tranTranslation = Transform.CreateTranslation(orgin.Negate());
                var tranScale = Transform.Identity.ScaleBasis(1.05);
                solid = SolidUtils.CreateTransformed(solid, tranTranslation.Inverse * tranScale * tranTranslation);
            }

            return new ElementIntersectsSolidFilter(solid);
        }
    }    
}