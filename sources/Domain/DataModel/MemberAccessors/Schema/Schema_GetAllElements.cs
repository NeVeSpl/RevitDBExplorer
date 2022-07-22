using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Schema_GetAllElements : MemberAccessorTyped<Schema>
    {
        public override ReadResult Read(Document document, Schema schema)
        {
            int count = new FilteredElementCollector(document).WherePasses(new ExtensibleStorageFilter(schema.GUID)).GetElementCount();
            return new ReadResult() 
            { 
                CanBeSnooped = count > 0, 
                Value = $"Elements : {count}",
                ValueTypeName = nameof(Schema_GetAllElements) 
            };            
        }

        public override IEnumerable<SnoopableObject> Snoop(Document document, Schema schema)
        {
            var elements = new FilteredElementCollector(document).WherePasses(new ExtensibleStorageFilter(schema.GUID)).ToElements();
            return elements.Select(x => new SnoopableObject(document, x));
        }
    }
}