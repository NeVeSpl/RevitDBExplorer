using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class View_GetFilterOverrides : MemberAccessorByType<View>, IHaveFactoryMethod
    {     
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (View x, ElementId i) => x.GetFilterOverrides(i); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new View_GetFilterOverrides();


        protected override bool CanBeSnoooped(Document document, View view)
        {
            bool canBesnooped = !view.Document.IsFamilyDocument && view.AreGraphicsOverridesAllowed() && view.GetFilters().Count > 0;
            return canBesnooped;
        }
        protected override string GetLabel(Document document, View value) => $"[{nameof(OverrideGraphicSettings)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, View view)
        {
            var filters = new FilteredElementCollector(document, view.GetFilters()).WhereElementIsNotElementType().ToElements();

            return filters.Select(x => SnoopableObject.CreateInOutPair(document, x, view.GetFilterOverrides(x.Id)));
        }
    }
}
