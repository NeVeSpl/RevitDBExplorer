using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class SpatialElementBoundaryOptionsHandler : TypeHandler<SpatialElementBoundaryOptions>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, SpatialElementBoundaryOptions value) => true;
        protected override string ToLabel(SnoopableContext context, SpatialElementBoundaryOptions value)
        {
            if (value.StoreFreeBoundaryFaces == true)
            {
                return $"{value.SpatialElementBoundaryLocation},  store free boundary faces";
            }

            return $"{value.SpatialElementBoundaryLocation}";
        }
    }
}