using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Category_Overrides : IHaveMembersOverrides
    {
        public IEnumerable<IMemberOverride> GetOverrides() =>
        [
#if R2022_MIN && R2022_MAX
            MemberOverride<Category>.ByFunc((doc, category) => Category.GetBuiltInCategoryTypeId((BuiltInCategory)category.Id.IntegerValue)),            
#endif  
#if R2023_MIN
            MemberOverride<Category>.ByFunc((doc, category) => Category.GetBuiltInCategoryTypeId(category.BuiltInCategory)),
#endif  
        ];
    }
}