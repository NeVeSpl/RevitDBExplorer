using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates.Accessors
{
    internal class Schema_EraseSchemaAndAllEntities : MemberAccessorTypedWithWrite<Schema>
    {


        public override IValueEditor CreateEditor(SnoopableContext context, Schema schema)
        {
            return new ExecuteEditor(this, () => Execute(context, schema));
        }

        private void Execute(SnoopableContext context, Schema schema)
        {
            var elements = new FilteredElementCollector(context.Document).WherePasses(new ExtensibleStorageFilter(schema.GUID)).ToElements();
            foreach (var element in elements)
            {
                element.DeleteEntity(schema);
            }
            context.Document.EraseSchemaAndAllEntities(schema); // does not work usually            
        }


        public override bool CanBeWritten(SnoopableContext context, Schema schema)
        {
            var result = schema.WriteAccessGranted() && schema.ReadAccessGranted();
            return result;
        }
    }
}