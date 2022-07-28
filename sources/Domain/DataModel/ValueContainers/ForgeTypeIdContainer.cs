using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ForgeTypeIdContainer : Base.ValueContainer<ForgeTypeId>
    {
        protected override bool CanBeSnoooped(ForgeTypeId id) => id is not null;
        protected override string ToLabel(ForgeTypeId id)
        {           
            return $"{id.TypeId}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ForgeTypeId id)
        {
            yield return new SnoopableObject(document, id);
        }
    }
}