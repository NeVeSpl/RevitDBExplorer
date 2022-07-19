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
                    new SnoopableMemberTemplate<Element, bool>((doc, target) => PartUtils.AreElementsValidForCreateParts(doc, new[] { target.Id })),
                    new SnoopableMemberTemplate<Element, PartMaker>((doc, target) => PartUtils.GetAssociatedPartMaker(doc,  target.Id)),
                    new SnoopableMemberTemplate<Element, bool>((doc, target) => PartUtils.HasAssociatedParts(doc, target.Id)),
                    new SnoopableMemberTemplate<Element, bool>((doc, target) => PartUtils.IsValidForCreateParts(doc, new LinkElementId(target.Id))),
                };
            partUtilsForPart = new ISnoopableMemberTemplate[]
                {
                    new SnoopableMemberTemplate<Part, bool>((doc, target) => PartUtils.ArePartsValidForDivide(doc, new[] { target.Id })),
                    new SnoopableMemberTemplate<Part, bool>((doc, target) => PartUtils.ArePartsValidForMerge(doc, new[] { target.Id })),
                    new SnoopableMemberTemplate<Part, int>((doc, target) => PartUtils.GetChainLengthToOriginal(target)),

                    new SnoopableMemberTemplate<Part, IEnumerable<ElementId>>((doc, target) => PartUtils.GetMergedParts(target)),
                    new SnoopableMemberTemplate<Part, bool>((doc, target) => PartUtils.IsMergedPart(target)),
                    new SnoopableMemberTemplate<Part, bool>((doc, target) => PartUtils.IsPartDerivedFromLink(target)),
                };            
        }


        public PartUtilsStream()
        {
            RegisterTemplates(typeof(Element), partUtilsForElement);
            RegisterTemplates(typeof(Part), partUtilsForPart);
        }
    }
}