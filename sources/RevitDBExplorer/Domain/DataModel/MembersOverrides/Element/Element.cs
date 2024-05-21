using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Element_Overrides : IHaveMembersOverrides
    {
        public IEnumerable<IMemberOverride> GetOverrides() =>
        [
            MemberOverride<Element>.ByFunc((document, element) => element.GetDependentElements(null)),           
        ];
    }
}