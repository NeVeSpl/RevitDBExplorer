using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class View_GetFilterVisibility : MemberAccessorByType<View>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(View.GetFilterVisibility);
        IMemberAccessor IHaveFactoryMethod.Create() => new View_GetFilterVisibility();


        protected override bool CanBeSnoooped(Document document, View view)
        {
            bool canBesnooped = !view.Document.IsFamilyDocument && view.AreGraphicsOverridesAllowed() && view.GetFilters().Count > 0;
            return canBesnooped;
        }
        protected override string GetLabel(Document document, View value) => "[Boolean]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, View view)
        {
            var filters = new FilteredElementCollector(document, view.GetFilters()).WhereElementIsNotElementType().ToElements();

            return filters.Select(x => new SnoopableObject(view.GetFilterVisibility(x.Id), document, x.Name));
        }
    }
}
