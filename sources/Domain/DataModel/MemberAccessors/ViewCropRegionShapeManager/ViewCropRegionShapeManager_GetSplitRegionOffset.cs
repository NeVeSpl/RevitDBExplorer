using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class ViewCropRegionShapeManager_GetSplitRegionOffset : MemberAccessorByType<ViewCropRegionShapeManager>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (ViewCropRegionShapeManager x) => x.GetSplitRegionOffset(69); }         


        protected override bool CanBeSnoooped(Document document, ViewCropRegionShapeManager manager) => manager.NumberOfSplitRegions > 1;
        protected override string GetLabel(Document document, ViewCropRegionShapeManager manager) => $"[{nameof(XYZ)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ViewCropRegionShapeManager manager)
        {            
            for (var i = 0; i < manager.NumberOfSplitRegions; i++)
            {
                yield return new SnoopableObject(document, manager.GetSplitRegionOffset(i)) { Index = i };
            }    
        }
    }
}