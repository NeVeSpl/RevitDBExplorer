using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class ViewCropRegionShapeManager_GetSplitRegionOffset : MemberAccessorByType<ViewCropRegionShapeManager>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(ViewCropRegionShapeManager.GetSplitRegionOffset);
        IMemberAccessor IHaveFactoryMethod.Create() => new ViewCropRegionShapeManager_GetSplitRegionOffset();


        protected override bool CanBeSnoooped(Document document, ViewCropRegionShapeManager manager) => manager.NumberOfSplitRegions > 1;
        protected override string GetLabel(Document document, ViewCropRegionShapeManager manager) => "[XYZ]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ViewCropRegionShapeManager manager)
        {            
            for (var i = 0; i < manager.NumberOfSplitRegions; i++)
            {
                yield return new SnoopableObject(manager.GetSplitRegionOffset(i), document, $"[{i}]");
            }    
        }
    }
}
