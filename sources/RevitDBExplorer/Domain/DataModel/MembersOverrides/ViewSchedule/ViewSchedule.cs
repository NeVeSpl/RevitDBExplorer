using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class ViewSchedule_Overrides : IHaveMembersOverrides
    {
        public IEnumerable<IMemberOverride> GetOverrides() =>
        [      
            MemberOverride<ViewSchedule>.ByFunc((document, target) => ViewSchedule.GetValidCategoriesForSchedule()),
            MemberOverride<ViewSchedule>.ByFunc((document, target) => ViewSchedule.GetValidCategoriesForKeySchedule()),
            MemberOverride<ViewSchedule>.ByFunc((document, target) => ViewSchedule.GetValidCategoriesForMaterialTakeoff()),
            MemberOverride<ViewSchedule>.ByFunc((document, target) => ViewSchedule.GetValidFamiliesForNoteBlock(document)),
        ];
    }
}