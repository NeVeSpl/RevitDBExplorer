using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitExplorer.Visualizations.DrawingVisuals;

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

        [CodeToString]
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, Element element)
        {
            var freshElement = context.Document?.GetElement(element.Id) ?? element;
            yield return new SnoopableObject(context.Document, freshElement);
        }

        protected override bool CanBeVisualized(SnoopableContext context, Element element) => element is not ElementType;
        [CodeToString]
        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, Element element)
        {            
            var bb = element.get_BoundingBox(null);

            if (bb != null && (bb.Max != null) && (bb.Min != null))
            {
                yield return new VisualizationItem("Element", "get_BoundingBox(null).Min", new CrossDrawingVisual(bb.Min, VisualizationItem.StartColor));
                yield return new VisualizationItem("Element", "get_BoundingBox(null).Max", new CrossDrawingVisual(bb.Max, VisualizationItem.EndColor));
            }            
        }
    }
}