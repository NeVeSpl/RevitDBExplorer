using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal class CategoryNameMapType : Base.ValueType<CategoryNameMap>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new CategoryNameMapType();
        }


        protected override bool CanBeSnoooped(CategoryNameMap categoryNameMap) => categoryNameMap?.IsEmpty == false;
        protected override string ToLabel(CategoryNameMap categoryNameMap)
        {
            return $"Categories : {categoryNameMap.Size}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, CategoryNameMap categoryNameMap)
        {
            foreach (Category cat in categoryNameMap)
            {
                yield return new SnoopableObject(cat, document);
            }
        }
    }
}