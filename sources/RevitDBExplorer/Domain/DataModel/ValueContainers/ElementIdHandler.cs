using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ElementIdHandler : TypeHandler<ElementId>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, ElementId id)
        {
            var elementOrCat = context.Document?.GetElementOrCategory(id);
            bool canBeSnoooped = elementOrCat != null;
            return canBeSnoooped;
        }
        protected override string ToLabel(SnoopableContext context, ElementId id)
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
            var element = context.Document?.GetElement(id);
            if (element != null)
            {
                return (new ElementHandler() as IHaveLabel<Element>).ToLabel(context, element);
            }
            return $"{id}";
        }

        [CodeToString]
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, ElementId id)
        {
            yield return new SnoopableObject(context.Document, id);            
        }
    }
}