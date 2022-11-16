using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ElementIdContainer : Base.ValueContainer<ElementId>
    {
        private bool canBeSnoooped;
        private Element element;

        public override IValueContainer SetValue(Document document, object value)
        {
            base.SetValue(document, value);
            if (value is ElementId id)
            {
                var elementOrCat = document?.GetElementOrCategory(id);
                canBeSnoooped = elementOrCat != null;
                element = elementOrCat as Element;
            }
            return this;
        }
        protected override bool CanBeSnoooped(ElementId id) => canBeSnoooped;
        protected override string ToLabel(ElementId id)
        {
            if (id == ElementId.InvalidElementId) return ElementId.InvalidElementId.ToString();
            if (id.Value() < -1)
            {                
                var parName = Enum.GetName(typeof(BuiltInParameter), id.Value());
                var catName = Enum.GetName(typeof(BuiltInCategory), id.Value());
                if (parName != null) return $"BuiltInParameter.{parName} ({id})";
                if (catName != null) return $"BuiltInCategory.{catName} ({id})";
                if(id == PlanViewRange.Unlimited) return $"PlanViewRange.Unlimited";
                if (id == PlanViewRange.Current) return $"PlanViewRange.Current";
                if (id == PlanViewRange.LevelBelow) return $"PlanViewRange.LevelBelow";
                if (id == PlanViewRange.LevelAbove) return $"PlanViewRange.LevelAbove";
                return $"{id}";
            }
            if (element != null)
            {
                return new ElementContainer().SetValue(null, element).ValueAsString;
            }
            return $"{id}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ElementId id)
        {
            yield return new SnoopableObject(document, id);            
        }
    }
}