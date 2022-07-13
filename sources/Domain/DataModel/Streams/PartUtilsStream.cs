using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal sealed class PartUtilsStream
    {
        private static readonly (string, IMemberAccessor)[] partUtilsForElement = new (string, IMemberAccessor)[0];
        private static readonly (string, IMemberAccessor)[] partUtilsForPart = new (string, IMemberAccessor)[0];

        static PartUtilsStream()
        {
            partUtilsForElement = new (string, IMemberAccessor)[]
                {
                    (nameof(PartUtils.AreElementsValidForCreateParts), new MemberAccessorByFunc<Element, bool>((doc, target) => PartUtils.AreElementsValidForCreateParts(doc, new[] { target.Id }))),
                    (nameof(PartUtils.GetAssociatedPartMaker), new MemberAccessorByFunc<Element, PartMaker>((doc, target) => PartUtils.GetAssociatedPartMaker(doc,  target.Id))),
                    (nameof(PartUtils.HasAssociatedParts), new MemberAccessorByFunc<Element, bool>((doc, target) => PartUtils.HasAssociatedParts(doc, target.Id))),
                    (nameof(PartUtils.IsValidForCreateParts), new MemberAccessorByFunc<Element, bool>((doc, target) => PartUtils.IsValidForCreateParts(doc, new LinkElementId(target.Id)))),
                };
            partUtilsForPart = new (string, IMemberAccessor)[]
                {
                    (nameof(PartUtils.ArePartsValidForDivide), new MemberAccessorByFunc<Part, bool>((doc, target) => PartUtils.ArePartsValidForDivide(doc, new[] { target.Id }))),
                    (nameof(PartUtils.ArePartsValidForMerge), new MemberAccessorByFunc<Part, bool>((doc, target) => PartUtils.ArePartsValidForMerge(doc, new[] { target.Id }))),
                    (nameof(PartUtils.GetChainLengthToOriginal), new MemberAccessorByFunc<Part, int>((doc, target) => PartUtils.GetChainLengthToOriginal(target))),

                    (nameof(PartUtils.GetMergedParts), new MemberAccessorByFunc<Part, IEnumerable<ElementId>>((doc, target) => PartUtils.GetMergedParts(target))),
                    (nameof(PartUtils.IsMergedPart), new MemberAccessorByFunc<Part, bool>((doc, target) => PartUtils.IsMergedPart(target))),
                    (nameof(PartUtils.IsPartDerivedFromLink), new MemberAccessorByFunc<Part, bool>((doc, target) => PartUtils.IsPartDerivedFromLink(target))),
                };
        }

        public IEnumerable<SnoopableMember> Stream(SnoopableObject snoopableObject)
        {
            if (snoopableObject.Object is Element)
            {
                foreach (var item in partUtilsForElement)
                {
                    yield return new SnoopableMember(snoopableObject, SnoopableMember.Kind.StaticMethod, item.Item1, typeof(PartUtils), item.Item2, null);
                }
            }

            if (snoopableObject.Object is Part)
            {
                foreach (var item in partUtilsForPart)
                {
                    yield return new SnoopableMember(snoopableObject, SnoopableMember.Kind.StaticMethod, item.Item1, typeof(PartUtils), item.Item2, null);
                }
            }
        }
    }
}
