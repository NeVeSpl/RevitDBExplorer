using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal class BindingMapType : Base.ValueType<BindingMap>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new BindingMapType();
        }


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