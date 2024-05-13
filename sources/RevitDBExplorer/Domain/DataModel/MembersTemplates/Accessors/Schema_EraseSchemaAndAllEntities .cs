using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors
{
    internal class Schema_EraseSchemaAndAllEntities : MemberAccessorTyped<Schema>
    {
        protected override IValueViewModel CreatePresenter(SnoopableContext context, Schema schema)
        {
            return new ExecuteEditor(this, () => Execute(context, schema), () => CanBeWritten(context, schema));
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


        private bool CanBeWritten(SnoopableContext context, Schema schema)
        {
            var result = schema.WriteAccessGranted() && schema.ReadAccessGranted();
            return result;
        }
    }
}