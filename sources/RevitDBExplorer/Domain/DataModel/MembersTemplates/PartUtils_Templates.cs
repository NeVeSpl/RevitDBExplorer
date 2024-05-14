using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal sealed class PartUtils_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Element>.Create((doc, target) => PartUtils.AreElementsValidForCreateParts(doc, new[] { target.Id })),
            MemberTemplate<Element>.Create((doc, target) => PartUtils.GetAssociatedPartMaker(doc,  target.Id)),
            MemberTemplate<Element>.Create((doc, target) => PartUtils.HasAssociatedParts(doc, target.Id)),
            MemberTemplate<Element>.Create((doc, target) => PartUtils.IsValidForCreateParts(doc, new LinkElementId(target.Id))),
           
            MemberTemplate<Part>.Create((doc, target) => PartUtils.ArePartsValidForDivide(doc, new[] { target.Id })),
            MemberTemplate<Part>.Create((doc, target) => PartUtils.ArePartsValidForMerge(doc, new[] { target.Id })),
            MemberTemplate<Part>.Create((doc, target) => PartUtils.GetChainLengthToOriginal(target)),

            MemberTemplate<Part>.Create((doc, target) => PartUtils.GetMergedParts(target)),
            MemberTemplate<Part>.Create((doc, target) => PartUtils.IsMergedPart(target)),
            MemberTemplate<Part>.Create((doc, target) => PartUtils.IsPartDerivedFromLink(target)),
        ];  
    }
}