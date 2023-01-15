using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates.Accessors
{
    internal class Schema_EraseSchemaAndAllEntities : MemberAccessorTypedWithWrite<Schema>
    {
        public override void Read(SnoopableContext context, Schema @object, IValueEditor valueEditor)
        {
            //Label = $"Erase",           
        }

        public override bool CanBeWritten(SnoopableContext context, Schema schema)
        {
            var result = schema.WriteAccessGranted() && schema.ReadAccessGranted();
            return result;
        }

        public override void Write(SnoopableContext context, Schema schema, IValueEditor valueEditor)
        {  
            var elements = new FilteredElementCollector(context.Document).WherePasses(new ExtensibleStorageFilter(schema.GUID)).ToElements();
            foreach (var element in elements)
            {
                element.DeleteEntity(schema);
            }
            context.Document.EraseSchemaAndAllEntities(schema); // does not work usually            
        }
    }
}
