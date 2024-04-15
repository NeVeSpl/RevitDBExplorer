using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class RebarBendingDetail_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();

        static RebarBendingDetail_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
              
#if R2025_MIN
                SnoopableMemberTemplate<IndependentTag>.Create((doc, target) =>  RebarBendingDetail.IsBendingDetail(target), kind: MemberKind.StaticMethod),
                SnoopableMemberTemplate<IndependentTag>.Create((doc, target) =>  RebarBendingDetail.GetHost(target), kind: MemberKind.StaticMethod, canBeUsed: x => RebarBendingDetail.IsBendingDetail(x)),
                SnoopableMemberTemplate<IndependentTag>.Create((doc, target) =>  RebarBendingDetail.GetHosts(target), kind: MemberKind.StaticMethod, canBeUsed: x => RebarBendingDetail.IsBendingDetail(x)),
                SnoopableMemberTemplate<IndependentTag>.Create((doc, target) =>  RebarBendingDetail.GetPosition(target), kind: MemberKind.StaticMethod, canBeUsed: x => RebarBendingDetail.IsBendingDetail(x)),
                SnoopableMemberTemplate<IndependentTag>.Create((doc, target) =>  RebarBendingDetail.GetRotation(target), kind: MemberKind.StaticMethod, canBeUsed: x => RebarBendingDetail.IsBendingDetail(x)),
                SnoopableMemberTemplate<IndependentTag>.Create((doc, target) =>  RebarBendingDetail.GetTagRelativeRotation(target), kind: MemberKind.StaticMethod, canBeUsed: x => RebarBendingDetail.IsBendingDetail(x)),
                SnoopableMemberTemplate<IndependentTag>.Create((doc, target) =>  RebarBendingDetail.IsRealisticBendingDetail(target), kind: MemberKind.StaticMethod, canBeUsed: x => RebarBendingDetail.IsBendingDetail(x)),
                SnoopableMemberTemplate<IndependentTag>.Create((doc, target) =>  RebarBendingDetail.GetTagRelativePosition(target), kind: MemberKind.StaticMethod, canBeUsed: x => RebarBendingDetail.IsBendingDetail(x)),
                SnoopableMemberTemplate<IndependentTag>.Create((doc, target) =>  RebarBendingDetail.IsSchematicBendingDetail(target), kind: MemberKind.StaticMethod, canBeUsed: x => RebarBendingDetail.IsBendingDetail(x)),
#endif
                
            };
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
