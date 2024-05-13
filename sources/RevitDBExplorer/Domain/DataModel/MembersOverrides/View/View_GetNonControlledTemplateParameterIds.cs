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
    internal class View_GetNonControlledTemplateParameterIds : MemberAccessorByType<View>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (View x) => x.GetNonControlledTemplateParameterIds() ];


        protected override ReadResult Read(SnoopableContext context, View view) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Parameter), null),
            CanBeSnooped = !view.Document.IsFamilyDocument && view.IsTemplate && view.GetNonControlledTemplateParameterIds().Count > 0
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, View view)
        {
            var templateParameterIds = view.GetNonControlledTemplateParameterIds().ToLookup(x => x);
            var templateParameters = view.Parameters.OfType<Parameter>().Where(x => templateParameterIds.Contains(x.Id)).ToList();

            return templateParameters.Select(x => new SnoopableObject(context.Document, x));
        }
    }
}