using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class IListElementIdHandler : TypeHandler<IList<ElementId>>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, IList<ElementId> list)
        {
            return list.Count > 0;
        }
        protected override string ToLabel(SnoopableContext context, IList<ElementId> list)
        {
            return Labeler.GetLabelForCollection("ElementId", list.Count);
        }
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, IList<ElementId> ids)
        {
            if (ids.Any())
            {
                //var elements = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(ids)).ToElements();
                return ids.Select(x => new SnoopableObject(context.Document, x));
            }
            return Enumerable.Empty<SnoopableObject>();
        }
    }
}