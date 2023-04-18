using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class View_GetNonControlledTemplateParameterIds : MemberAccessorByType<View>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (View x) => x.GetNonControlledTemplateParameterIds(); }  


        protected override bool CanBeSnoooped(Document document, View view)
        {
            bool canBesnooped = !view.Document.IsFamilyDocument && view.IsTemplate && view.GetNonControlledTemplateParameterIds().Count > 0;
            return canBesnooped;
        }
        protected override string GetLabel(Document document, View value) => $"[{nameof(Parameter)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, View view)
        {
            var templateParameterIds = view.GetNonControlledTemplateParameterIds().ToLookup(x => x);
            var templateParameters = view.Parameters.OfType<Parameter>().Where(x => templateParameterIds.Contains(x.Id)).ToList();

            return templateParameters.Select(x => new SnoopableObject(document, x));
        }
    }
}