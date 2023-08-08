using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class GeometryElementHandler : TypeHandler<GeometryElement>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, GeometryElement value) => value is not null;

        protected override string ToLabel(SnoopableContext context, GeometryElement value) => Labeler.GetLabelForObjectWithId("GeometryElement", value.Id);

        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, GeometryElement value)
        {
            var geometryObjects = Enumarate(context.Document, value);
            yield return new SnoopableObject(context.Document, value, geometryObjects);
        }


        private IEnumerable<SnoopableObject> Enumarate(Document document, GeometryElement geometryElement)
        {
            int index = -1;
            foreach (var item in geometryElement)
            {
                index++;               
                yield return new SnoopableObject(document, item) { Index = index };
            }
        }
    }
}
