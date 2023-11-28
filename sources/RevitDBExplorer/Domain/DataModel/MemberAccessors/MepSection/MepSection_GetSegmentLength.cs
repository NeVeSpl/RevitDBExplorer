using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors.MepSection
{

    internal class MepSection_GetSegmentLength : MemberAccessorByType<MEPSection>, ICanCreateMemberAccessor
    {
        public IEnumerable<LambdaExpression> GetHandledMembers() { yield return (MEPSection x) => x.GetSegmentLength(null); }


        protected override bool CanBeSnoooped(Document document, MEPSection value) => value.GetElementIds().Count > 0;

        protected override string GetLabel(Document document, MEPSection value)
        {
            return Labeler.GetLabelForCollection("double", null);
        }

        protected override IEnumerable<SnoopableObject> Snooop(Document document, MEPSection value)
        {
            foreach (var id in value.GetElementIds())
            {
                var element = document.GetElement(id);
                if (element is FamilyInstance) continue;
                yield return SnoopableObject.CreateKeyValuePair(document, id, value.GetSegmentLength(id));
            }
        }
    }
}
