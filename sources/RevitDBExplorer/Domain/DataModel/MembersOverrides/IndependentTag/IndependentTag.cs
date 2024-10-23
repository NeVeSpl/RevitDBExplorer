using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class IndependentTag_Overrides : IHaveMembersOverrides
    {
        public IEnumerable<IMemberOverride> GetOverrides() =>
        [
            MemberOverride<IndependentTag>.ByFuncWithParam((document, element, referenceTagged) => element.GetLeaderElbow(referenceTagged), (document, element) => element.GetTaggedReferences()),
            MemberOverride<IndependentTag>.ByFuncWithParam((document, element, referenceTagged) => element.GetLeaderEnd(referenceTagged), (document, element) => element.GetTaggedReferences()),
            MemberOverride<IndependentTag>.ByFuncWithParam((document, element, referenceTagged) => element.HasLeaderElbow(referenceTagged), (document, element) => element.GetTaggedReferences()),

#if R2023_MIN
            MemberOverride<IndependentTag>.ByFuncWithParam((document, element, referenceTagged) => element.IsLeaderVisible(referenceTagged), (document, element) => element.GetTaggedReferences()),
#endif

        ];
    }
}
