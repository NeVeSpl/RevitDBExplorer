using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class BindingMapHandler : TypeHandler<BindingMap>
    {   
        protected override bool CanBeSnoooped(SnoopableContext context, BindingMap map) => !map.IsEmpty;
        protected override string ToLabel(SnoopableContext context, BindingMap map) => Labeler.GetLabelForCollection("Binding", map.Size);

        [CodeToString]
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, BindingMap map)
        {
            var iterator = map.ForwardIterator();
            while (iterator.MoveNext())
            {
                var definition = iterator.Key;
                var binding = iterator.Current;

                yield return SnoopableObject.CreateKeyValuePair(context.Document, definition, binding);
            }
        }
    }
}