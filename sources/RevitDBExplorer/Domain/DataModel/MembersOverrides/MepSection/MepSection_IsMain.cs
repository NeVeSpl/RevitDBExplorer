using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides.MepSection
{

    internal class MepSection_IsMain : MemberAccessorByType<MEPSection>, ICanCreateMemberAccessor
    {
        public IEnumerable<LambdaExpression> GetHandledMembers() => [ (MEPSection x) => x.IsMain(null) ];


        protected override ReadResult Read(SnoopableContext context, MEPSection value) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Boolean), null),
            CanBeSnooped = value.GetElementIds().Count > 0
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, MEPSection value)
        {
            foreach (var id in value.GetElementIds())
            {
                var element = context.Document.GetElement(id);
                if (element is not FamilyInstance) continue;
                yield return SnoopableObject.CreateKeyValuePair(context.Document, id, value.IsMain(id));
            }
        }
    }
}
