using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class View_GetFilterVisibility : MemberAccessorByType<View>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (View x, ElementId i) => x.GetFilterVisibility(i) ];


        protected override ReadResult Read(SnoopableContext context, View view) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Boolean), null),
            CanBeSnooped = !view.Document.IsFamilyDocument && view.AreGraphicsOverridesAllowed() && view.GetFilters().Count > 0
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, View view)
        {
            var filters = new FilteredElementCollector(context.Document).WherePasses(new ElementIdSetFilter(view.GetFilters())).ToElements();

            return filters.Select(x => SnoopableObject.CreateInOutPair(context.Document, x, view.GetFilterVisibility(x.Id)));
        }
    }
}