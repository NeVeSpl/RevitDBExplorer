using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class PlanViewRange_GetOffset : MemberAccessorByType<PlanViewRange>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(PlanViewRange.GetOffset);
        IMemberAccessor IHaveFactoryMethod.Create() => new PlanViewRange_GetOffset();


        protected override bool CanBeSnoooped(Document document, PlanViewRange viewRange) => true;
        protected override string GetLabel(Document document, PlanViewRange viewRange) => "[double]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, PlanViewRange viewRange)
        {
            foreach (PlanViewPlane type in Enum.GetValues(typeof(PlanViewPlane)))
            {
                var offset = viewRange.GetOffset(type);               
                yield return new SnoopableObject(offset, document, type.ToString());               
            }
        }
    }
}