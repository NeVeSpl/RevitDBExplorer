using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class View_GetNonControlledTemplateParameterIds : MemberAccessorByType<View>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(View.GetNonControlledTemplateParameterIds);
        IMemberAccessor IHaveFactoryMethod.Create() => new View_GetNonControlledTemplateParameterIds();


        protected override bool CanBeSnoooped(Document document, View view)
        {
            bool canBesnooped = !view.Document.IsFamilyDocument && view.IsTemplate && view.GetNonControlledTemplateParameterIds().Count > 0;
            return canBesnooped;
        }
        protected override string GetLabel(Document document, View value) => "[Parameter]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, View view)
        {
            var templateParameterIds = view.GetNonControlledTemplateParameterIds().ToLookup(x => x);
            var templateParameters = view.Parameters.OfType<Parameter>().Where(x => templateParameterIds.Contains(x.Id)).ToList();

            return templateParameters.Select(x => new SnoopableObject(x, document));
        }
    }
}
