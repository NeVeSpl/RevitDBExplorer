using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class BindingMapContainer : Base.ValueContainer<BindingMap>
    {   
        protected override bool CanBeSnoooped(BindingMap map) => map is not null && !map.IsEmpty;
        protected override string ToLabel(BindingMap map) => $"Bindings : {map.Size}";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, BindingMap map)
        {
            var iterator = map.ForwardIterator();
            while (iterator.MoveNext())
            {
                var definition = iterator.Key;
                var binding = iterator.Current;

                yield return SnoopableObject.CreateKeyValuePair(document, definition, binding);
            }
        }
    }
}