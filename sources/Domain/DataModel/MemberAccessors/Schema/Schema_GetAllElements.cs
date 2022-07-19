using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Schema_GetAllElements : MemberAccessorByType<Schema>
    {
        protected override IEnumerable<LambdaExpression> HandledMembers => Enumerable.Empty<LambdaExpression>();

     
        protected override bool CanBeSnoooped(Document document, Schema schema)
        {
            int count = new FilteredElementCollector(document).WherePasses(new ExtensibleStorageFilter(schema.GUID)).GetElementCount();           
            return count > 0;
        }
        protected override string GetLabel(Document document, Schema schema)
        {
            int count = new FilteredElementCollector(document).WherePasses(new ExtensibleStorageFilter(schema.GUID)).GetElementCount();
            return $"Elements : {count}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Schema schema)
        {
            var elements = new FilteredElementCollector(document).WherePasses(new ExtensibleStorageFilter(schema.GUID)).ToElements();
            return elements.Select(x => new SnoopableObject(x, document));
        }
    }
}