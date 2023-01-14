using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ForgeTypeIdHandler : TypeHandler<ForgeTypeId>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, ForgeTypeId id) => id is not null;
        protected override string ToLabel(SnoopableContext context, ForgeTypeId id)
        {           
            return $"{id.TypeId}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ForgeTypeId id)
        {
            yield return new SnoopableObject(document, id);
        }
    }
}