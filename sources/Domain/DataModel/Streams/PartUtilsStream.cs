using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal sealed class PartUtilsStream : BaseStream
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> partUtilsForElement = Enumerable.Empty<ISnoopableMemberTemplate>();
        private static readonly IEnumerable<ISnoopableMemberTemplate> partUtilsForPart = Enumerable.Empty<ISnoopableMemberTemplate>();

        static PartUtilsStream()
        {
            partUtilsForElement = new ISnoopableMemberTemplate[]
            {
                SnoopableMemberTemplate<Element>.Create((doc, target) => PartUtils.AreElementsValidForCreateParts(doc, new[] { target.Id })),
                SnoopableMemberTemplate<Element>.Create((doc, target) => PartUtils.GetAssociatedPartMaker(doc,  target.Id)),
                SnoopableMemberTemplate<Element>.Create((doc, target) => PartUtils.HasAssociatedParts(doc, target.Id)),
                SnoopableMemberTemplate<Element>.Create((doc, target) => PartUtils.IsValidForCreateParts(doc, new LinkElementId(target.Id))),
            };
            partUtilsForPart = new ISnoopableMemberTemplate[]
            {
                SnoopableMemberTemplate<Part>.Create((doc, target) => PartUtils.ArePartsValidForDivide(doc, new[] { target.Id })),
                SnoopableMemberTemplate<Part>.Create((doc, target) => PartUtils.ArePartsValidForMerge(doc, new[] { target.Id })),
                SnoopableMemberTemplate<Part>.Create((doc, target) => PartUtils.GetChainLengthToOriginal(target)),

                SnoopableMemberTemplate<Part>.Create((doc, target) => PartUtils.GetMergedParts(target)),
                SnoopableMemberTemplate<Part>.Create((doc, target) => PartUtils.IsMergedPart(target)),
                SnoopableMemberTemplate<Part>.Create((doc, target) => PartUtils.IsPartDerivedFromLink(target)),
            };            
        }


        public PartUtilsStream()
        {
            RegisterTemplates(typeof(Element), partUtilsForElement);
            RegisterTemplates(typeof(Part), partUtilsForPart);
        }
    }
}