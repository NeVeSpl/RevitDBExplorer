using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors.MepSection
{

    internal class MepSection_IsMain : MemberAccessorByType<MEPSection>, ICanCreateMemberAccessor
    {
        public IEnumerable<LambdaExpression> GetHandledMembers() { yield return (MEPSection x) => x.IsMain(null); }


        protected override bool CanBeSnoooped(Document document, MEPSection value) => value.GetElementIds().Count > 0;

        protected override string GetLabel(Document document, MEPSection value)
        {
            return Labeler.GetLabelForCollection("bool", null);
        }

        protected override IEnumerable<SnoopableObject> Snooop(Document document, MEPSection value)
        {
            foreach (var id in value.GetElementIds())
            {
                var element = document.GetElement(id);
                if (element is not FamilyInstance) continue;
                yield return SnoopableObject.CreateKeyValuePair(document, id, value.IsMain(id));
            }
        }
    }
}
