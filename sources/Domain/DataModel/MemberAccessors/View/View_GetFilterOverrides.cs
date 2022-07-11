using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md


namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class View_GetFilterOverrides : MemberAccessorByType<View>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(View.GetFilterOverrides);
        IMemberAccessor IHaveFactoryMethod.Create() => new View_GetFilterOverrides();


        protected override bool CanBeSnoooped(Document document, View view)
        {
            bool canBesnooped = !view.Document.IsFamilyDocument && view.AreGraphicsOverridesAllowed() && view.GetFilters().Count > 0;
            return canBesnooped;
        }
        protected override string GetLabel(Document document, View value) => "[OverrideGraphicSettings]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, View view)
        {
            var filters = new FilteredElementCollector(document, view.GetFilters()).WhereElementIsNotElementType().ToElements();

            return filters.Select(x => new SnoopableObject(view.GetFilterOverrides(x.Id), document, x.Name));
        }
    }
}
