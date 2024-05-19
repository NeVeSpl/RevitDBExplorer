using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Category_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
#if R2022_MIN
            MemberTemplate<Category>.Create((doc, category) => ParameterFilterUtilities.GetFilterableParametersInCommon(doc, new[] { category.Id } )),
#endif        
            MemberTemplate<Category>.Create((doc, category) => doc.ActiveView.CanCategoryBeHidden(category.Id), kind: MemberKind.AsArgument),
            MemberTemplate<Category>.Create((doc, category) => doc.ActiveView.CanCategoryBeHiddenTemporary(category.Id), kind: MemberKind.AsArgument),
            MemberTemplate<Category>.Create((doc, category) => doc.ActiveView.GetCategoryHidden(category.Id), kind: MemberKind.AsArgument),
            MemberTemplate<Category>.Create((doc, category) => doc.ActiveView.GetCategoryOverrides(category.Id), kind: MemberKind.AsArgument),
            MemberTemplate<Category>.Create((doc, category) => doc.ActiveView.IsCategoryOverridable(category.Id), kind: MemberKind.AsArgument),
#if R2022_MIN
            MemberTemplate<Category>.Create((doc, category) => doc.ActiveView.GetColorFillSchemeId(category.Id), kind: MemberKind.AsArgument),
#endif 


            MemberTemplate<Category>.Create((doc, category) => ViewSchedule.IsValidCategoryForKeySchedule(category.Id)),
            MemberTemplate<Category>.Create((doc, category) => ViewSchedule.IsValidCategoryForKeySchedule(category.Id)),
            MemberTemplate<Category>.Create((doc, category) => ViewSchedule.IsValidCategoryForMaterialTakeoff(category.Id)),
            MemberTemplate<Category>.Create((doc, category) => ViewSchedule.IsValidFamilyForNoteBlock(doc, category.Id)),
        ];
    }
}