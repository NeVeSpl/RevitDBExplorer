using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class ElementType : Base.ValueType<Element>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new ElementType();
        }


        protected override bool CanBeSnoooped(Element element) => element is not null;
        protected override string ToLabel(Element element)
        {  
            var elementName = String.IsNullOrEmpty(element.Name) ? "<???>" : element.Name;
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
            return $"{elementName} ({element.Id.IntegerValue})";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Element element)
        {
            var freshElement = document.GetElement(element.Id);
            yield return new SnoopableObject(freshElement, document);
        }
    }
}