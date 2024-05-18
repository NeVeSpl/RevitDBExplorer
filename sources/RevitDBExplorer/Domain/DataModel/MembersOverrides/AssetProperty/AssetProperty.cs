using System.Collections.Generic;
using Autodesk.Revit.DB.Visual;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class AssetProperty_Overrides : IHaveMembersOverrides
    {
        public IEnumerable<IMemberOverride> GetOverrides() =>
        [
            MemberOverride<AssetProperty>.ByFunc((document, target) => AssetProperty.GetTypeName(target.Type)),
        ];
    }
}