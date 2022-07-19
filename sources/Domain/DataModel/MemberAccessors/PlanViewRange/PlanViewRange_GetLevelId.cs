using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class PlanViewRange_GetLevelId : MemberAccessorByType<PlanViewRange>, IHaveFactoryMethod
    {       
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (PlanViewRange x, PlanViewPlane p) => x.GetLevelId(p); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new PlanViewRange_GetLevelId();


        protected override bool CanBeSnoooped(Document document, PlanViewRange viewRange) => true;
        protected override string GetLabel(Document document, PlanViewRange viewRange) => $"[{nameof(Level)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, PlanViewRange viewRange)
        {
            foreach (PlanViewPlane type in Enum.GetValues(typeof(PlanViewPlane)))
            {
                var levelId = viewRange.GetLevelId(type);
                if (levelId is not null && levelId > ElementId.InvalidElementId)
                {
                    var level = document.GetElement(levelId);
                    yield return SnoopableObject.CreateInOutPair(document, type, level);                  
                }
                else
                {
                    yield return SnoopableObject.CreateInOutPair(document, type, levelId);                   
                }
            }
        }
    }
}