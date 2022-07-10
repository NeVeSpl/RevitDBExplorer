using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class ElementIdType : Base.ValueType<ElementId>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new ElementIdType();
        }


        private Element element;

        public override IValueType SetValue(Document document, object value)
        {
            base.SetValue(document, value);
            if (value is ElementId id)
            {
                element = document.GetElement(id);
            }
            return this;
        }
        protected override bool CanBeSnoooped(ElementId id) => element is not null;
        protected override string ToLabel(ElementId id)
        {
            if (id == ElementId.InvalidElementId) return ElementId.InvalidElementId.ToString();
            if (id.IntegerValue < -1)
            {                
                var parName = Enum.GetName(typeof(BuiltInParameter), id.IntegerValue);
                var catName = Enum.GetName(typeof(BuiltInCategory), id.IntegerValue);
                if (parName != null) return $"BuiltInParameter.{parName} ({id})";
                if (catName != null) return $"BuiltInCategory.{catName} ({id})";
                return $"{id}";
            }
            return new ElementType().SetValue(null, element).ValueAsString;
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ElementId id)
        {
            var freshElement = document.GetElement(id) ?? element;
            if (freshElement != null)
            {
                yield return new SnoopableObject(freshElement, document);
            }
        }
    }
}