using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class ViewCropRegionShapeManager_GetSplitRegionOffset : MemberAccessorByType<ViewCropRegionShapeManager>, IHaveFactoryMethod
    {    
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (ViewCropRegionShapeManager x) => x.GetSplitRegionOffset(69); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new ViewCropRegionShapeManager_GetSplitRegionOffset();


        protected override bool CanBeSnoooped(Document document, ViewCropRegionShapeManager manager) => manager.NumberOfSplitRegions > 1;
        protected override string GetLabel(Document document, ViewCropRegionShapeManager manager) => $"[{nameof(XYZ)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ViewCropRegionShapeManager manager)
        {            
            for (var i = 0; i < manager.NumberOfSplitRegions; i++)
            {
                yield return new SnoopableObject(manager.GetSplitRegionOffset(i), document) { Index = i };
            }    
        }
    }
}