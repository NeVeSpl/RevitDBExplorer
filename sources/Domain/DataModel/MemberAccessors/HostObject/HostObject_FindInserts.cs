using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class HostObject_FindInserts : MemberAccessorByType<HostObject>, ICanCreateMemberAccessor
    {
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (HostObject x) => x.FindInserts(true, true, true, true); } }
        IMemberAccessor ICanCreateMemberAccessor.Create() => new HostObject_FindInserts();


        protected override bool CanBeSnoooped(Document document, HostObject hostObject)
        {
            var ids = hostObject.FindInserts(true, true, true, true);
            return ids.Any();
        }
        protected override string GetLabel(Document document, HostObject hostObject) => $"[{nameof(ElementId)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, HostObject hostObject)
        {
            var ids = hostObject.FindInserts(true, true, true, true);

            if (ids.Any())
            {
                var insertedElements = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(ids));
                return insertedElements.Select(x => new SnoopableObject(document, x));
            }
            return Enumerable.Empty<SnoopableObject>();
        }
    }
}