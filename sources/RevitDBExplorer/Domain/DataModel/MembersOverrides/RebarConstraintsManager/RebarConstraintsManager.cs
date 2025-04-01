using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class RebarConstraintsManager_Overrides : IHaveMembersOverrides
    {
        public IEnumerable<IMemberOverride> GetOverrides() =>
        [
         

        #if R2021_MIN
            MemberOverride<RebarConstraintsManager>.ByFuncWithParam((document, target, handle) => target.GetPreferredConstraintOnHandle(handle), (document, target) => target.GetAllHandles()),
            MemberOverride<RebarConstraintsManager>.ByFuncWithParam((document, target, handle) => target.GetCurrentConstraintOnHandle(handle), (document, target) => target.GetAllHandles()),
        #endif
        ];

    }
}
