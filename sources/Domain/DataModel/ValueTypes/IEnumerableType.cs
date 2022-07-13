using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal class IEnumerableType : Base.ValueType<IEnumerable>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new IEnumerableType();
        }


        protected override bool CanBeSnoooped(IEnumerable enumerable)
        {
            if (enumerable == null) return false;
            foreach(var item in enumerable)
            {
                return true;
            }
            return false;
        }
        protected override string ToLabel(IEnumerable enumerable)
        {
            var type = enumerable.GetType();
            var typeName = type.Name;

            if (type.FullName.StartsWith("System"))
            {
                typeName = "";
            }

            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                return $"{typeName}[{String.Join(", ", args.Select(x => x.Name))}]";
            }

            foreach (var item in enumerable)
            {
                var firstItemType = item.GetType();
                return $"{typeName}[{firstItemType.Name}]";               
            }

            return $"{typeName}[]";
        }


        protected override IEnumerable<SnoopableObject> Snooop(Document document, IEnumerable enumerable)
        {
            int index = -1;
            foreach (var item in enumerable)
            {
                index++;
                if (item is ElementId id)
                {
                    var element = document.GetElementOrCategory(id);
                    if (element != null)
                    {
                        yield return new SnoopableObject(element, document);
                    }
                }
                else
                {
                    yield return new SnoopableObject(item, document, index);
                }
            }
        }
    }
}
