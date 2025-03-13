using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Solid_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Solid>.Create((document, target) => SolidUtils.SplitVolumes(target)),


            MemberTemplate<Solid>.Create((document, target) => SolidUtils.IsValidForTessellation(target)),
            MemberTemplate<Solid>.Create((document, target) => SolidUtils.TessellateSolidOrShell(target, new SolidOrShellTessellationControls()), canBeUsed: x => SolidUtils.IsValidForTessellation(x)),

#if R2026_MIN
            MemberTemplate<Solid>.Create((document, target) => SolidUtils.ComputeIsTopologicallyClosed(target)),
            MemberTemplate<Solid>.Create((document, target) => SolidUtils.ComputeIsGeometricallyClosed(target)),
#endif
        ];       
    }
}