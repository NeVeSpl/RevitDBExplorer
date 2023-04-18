using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class CategoryNameMapHandler : TypeHandler<CategoryNameMap>
    {    
        protected override bool CanBeSnoooped(SnoopableContext context, CategoryNameMap categoryNameMap) => categoryNameMap?.IsEmpty == false;
        protected override string ToLabel(SnoopableContext context, CategoryNameMap categoryNameMap)
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