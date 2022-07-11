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
            Type firstItemType = null;
            bool isEnum = false;

            foreach (var item in enumerable)
            {
                firstItemType = item.GetType();
                isEnum = typeof(IEnumerable).IsAssignableFrom(firstItemType) && item is not string;

                break;
            }

            if (!isEnum)
            {
                typeName = "";
            }

            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                return $"{typeName}[{args.First().Name}]";
            }

            if (firstItemType != null)          
            {
                return $"{typeName}[{firstItemType.Name}]";                
            }

            return $"{typeName}[]";
        }


        protected override IEnumerable<SnoopableObject> Snooop(Document document, IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                if (item is ElementId id)
                {
                    var element = document.GetElement(id);
                    if (element != null)
                    {
                        yield return new SnoopableObject(element, document);
                    }
                }
                else
                {
                    yield return new SnoopableObject(item, document);
                }
            }
        }
    }
}
