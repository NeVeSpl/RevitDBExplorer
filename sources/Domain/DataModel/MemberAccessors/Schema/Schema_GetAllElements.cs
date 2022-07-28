using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Schema_GetAllElements : MemberAccessorTyped<Schema>
    {
        public override ReadResult Read(SnoopableContext context, Schema schema)
        {
            int count = new FilteredElementCollector(context.Document).WherePasses(new ExtensibleStorageFilter(schema.GUID)).GetElementCount();
            return new ReadResult() 
            { 
                CanBeSnooped = count > 0, 
                Value = $"Elements : {count}",
                ValueTypeName = nameof(Schema_GetAllElements) 
            };            
        }

        public override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Schema schema)
        {
            var elements = new FilteredElementCollector(context.Document).WherePasses(new ExtensibleStorageFilter(schema.GUID)).ToElements();
            return elements.Select(x => new SnoopableObject(context.Document, x));
        }
    }
}