using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class IEnumerableHandler : TypeHandler<IEnumerable>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, IEnumerable enumerable)
        {
            if (enumerable == null) return false;          
            foreach (var item in enumerable)
            {
                return true;
            }
            return false;
        }
        protected override string ToLabel(SnoopableContext context, IEnumerable enumerable)
        {
            var type = enumerable.GetType();
            var typeName = type.GetCSharpName();
            var size = enumerable.TryGetPropertyValue(propertyThatContainsSize);

            if (!string.IsNullOrEmpty(size))
            {
                size = $": {size}";
            }

            if (type.FullName.StartsWith("System"))
            {
                typeName = "";
            }

            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                return $"{typeName}[{String.Join(", ", args.Select(x => x.GetCSharpName()))}{size}]";
            }

            foreach (var item in enumerable)
            {
                var firstItemType = item.GetType();
                return $"{typeName}[{firstItemType.GetCSharpName()}{size}]";               
            }

            return $"{typeName}[{size}]";
        }

        private static readonly string[] propertyThatContainsSize = new[] { "Count", "Size", "Length" };


        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, IEnumerable enumerable)
        {
            int index = -1;
            foreach (var item in enumerable)
            {
                index++;
                if (item is ElementId id)
                {                   
                    yield return new SnoopableObject(context.Document, id);                    
                }
                else
                {
                    yield return new SnoopableObject(context.Document, item) { Index = index };
                }
            }
        }
    }
}