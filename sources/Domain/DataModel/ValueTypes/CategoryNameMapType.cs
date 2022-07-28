using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal class CategoryNameMapType : Base.ValueType<CategoryNameMap>
    {    
        protected override bool CanBeSnoooped(CategoryNameMap categoryNameMap) => categoryNameMap?.IsEmpty == false;
        protected override string ToLabel(CategoryNameMap categoryNameMap)
        {
            return $"Categories : {categoryNameMap.Size}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, CategoryNameMap categoryNameMap)
        {
            foreach (Category cat in categoryNameMap)
            {
                yield return new SnoopableObject(document, cat);
            }
        }
    }
}