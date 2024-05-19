using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Parameter_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
#if R2022_MIN
            MemberTemplate<Parameter>.Create((doc, parameter) => UnitFormatUtils.Format(doc.GetUnits(), parameter.Definition.GetDataType(), parameter.AsDouble(), false), x => UnitUtils.IsMeasurableSpec(x.Definition?.GetDataType())),
#endif
            MemberTemplate<Parameter>.Create((doc, target) => GlobalParametersManager.IsValidGlobalParameter(doc, target.Id)),
            MemberTemplate<Parameter>.Create((doc, target) => doc.FamilyManager.GetAssociatedFamilyParameter(target), kind: MemberKind.AsArgument),
        ];
    }
}