using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ElementHandler : TypeHandler<Element>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Element element) => element is not null;
        protected override string ToLabel(SnoopableContext context, Element element)
        {  
            var elementName = String.IsNullOrEmpty(element.Name) ? $"{element.GetType().GetCSharpName()} : <???>" : element.Name;
            if ((element is Wall) || (element is Floor) || (element is FamilyInstance))
            {
                var parameter = element.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM);
                if (parameter?.HasValue == true)
                {
                    elementName = parameter.AsValueString();
                }
            }
            if (element is FamilySymbol symbol)
            {
                elementName = $"{symbol.FamilyName}: {symbol.Name}";
            }
            return $"{elementName} ({element.Id})";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Element element)
        {
            var freshElement = document?.GetElement(element.Id) ?? element;
            yield return new SnoopableObject(document, freshElement);
        }
    }
}