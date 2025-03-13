using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class ElementType_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [   
         

#if R2026_MIN       
             MemberTemplate<ElementType>.Create((doc, target) =>  RebarCrankTypeUtils.GetCrankRatio(doc, target.Id), kind: MemberKind.StaticMethod),
             MemberTemplate<ElementType>.Create((doc, target) =>  RebarCrankTypeUtils.GetCrankLengthMultiplier(doc, target.Id), kind: MemberKind.StaticMethod),
             MemberTemplate<ElementType>.Create((doc, target) =>  RebarCrankTypeUtils.GetCrankOffsetMultiplier(doc, target.Id), kind: MemberKind.StaticMethod),
#endif
        ];
    }
}