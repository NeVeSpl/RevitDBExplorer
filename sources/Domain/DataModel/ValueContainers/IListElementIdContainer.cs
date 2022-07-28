using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class IListElementIdContainer : Base.ValueContainer<IList<ElementId>>
    {
        protected override bool CanBeSnoooped(IList<ElementId> list)
        {
            return list.Count > 0;
        }
        protected override string ToLabel(IList<ElementId> list)
        {            
            return $" [ElementId : {list.Count}]";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, IList<ElementId> ids)
        {
            if (ids.Any())
            {
                var elements = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(ids)).ToElements();
                return elements.Select(x => new SnoopableObject(document, x));
            }
            return Enumerable.Empty<SnoopableObject>();
        }
    }
}