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
            return $"{elementName} ({element.Id.IntegerValue})";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Element element)
        {
            var freshElement = document.GetElement(element.Id);
            yield return new SnoopableObject(freshElement, document);
        }
    }
}