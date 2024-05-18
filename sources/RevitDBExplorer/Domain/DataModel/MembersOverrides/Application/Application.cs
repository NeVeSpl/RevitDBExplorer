using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Application_Overrides : IHaveMembersOverrides
    {
        public IEnumerable<IMemberOverride> GetOverrides() =>
        [
            MemberOverride<Autodesk.Revit.ApplicationServices.Application>.ByFunc((document, element) => Autodesk.Revit.ApplicationServices.Application.GetFailureDefinitionRegistry()),
        ];
    }
}