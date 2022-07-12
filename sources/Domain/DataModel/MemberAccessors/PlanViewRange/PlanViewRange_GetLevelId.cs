using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class PlanViewRange_GetLevelId : MemberAccessorByType<PlanViewRange>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(PlanViewRange.GetLevelId);
        public override string MemberParams => typeof(PlanViewPlane).Name;
        IMemberAccessor IHaveFactoryMethod.Create() => new PlanViewRange_GetLevelId();


        protected override bool CanBeSnoooped(Document document, PlanViewRange viewRange) => true;
        protected override string GetLabel(Document document, PlanViewRange viewRange) => "[Level]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, PlanViewRange viewRange)
        {
            foreach (PlanViewPlane type in Enum.GetValues(typeof(PlanViewPlane)))
            {
                var levelId = viewRange.GetLevelId(type);
                if (levelId is not null && levelId != ElementId.InvalidElementId)
                {
                    var level = document.GetElement(levelId) as Level;
                    yield return new SnoopableObject(level, document, type.ToString());                  
                }
                else
                {
                    yield return new SnoopableObject(levelId, document, type.ToString());                   
                }
            }
        }
    }
}