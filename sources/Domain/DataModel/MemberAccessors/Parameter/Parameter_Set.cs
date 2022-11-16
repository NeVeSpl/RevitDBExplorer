using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.UIComponents.List.ValueEditors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Parameter_Set : MemberAccessorTypedWithWrite<Parameter>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() 
        {
            yield return (Parameter x) => x.Set("foo");
            yield return (Parameter x) => x.Set(7); 
            yield return (Parameter x) => x.Set(7.77);
            yield return (Parameter x) => x.Set(ElementIdFactory.Create(7)); 
        }


        private readonly StringEditorVM stringEditor = new();
        private readonly IntegerEditorVM intEditor = new();
        private readonly DoubleEditorVM doubleEditor = new();


        public override IValueEditor GetEditor(SnoopableContext context, Parameter parameter)
        {
            switch (parameter.StorageType)
            {
                case StorageType.Double:
                    return doubleEditor;   
                case StorageType.Integer:
                case StorageType.ElementId:
                    return intEditor;              
            }
            return stringEditor;
        }
        public override void Read(SnoopableContext context, Parameter parameter)
        {
            stringEditor.Value = parameter.AsString();
            intEditor.Value = parameter.AsInteger();
            doubleEditor.Value = parameter.AsDouble();
        }
        public override bool CanBeWritten(SnoopableContext context, Parameter parameter)
        {
            return !parameter.IsReadOnly;
        }   
        public override void Write(SnoopableContext context, Parameter parameter)
        {
            switch (parameter.StorageType)
            {
                case StorageType.Double:
                    parameter.Set(doubleEditor.Value);
                    break;
                case StorageType.Integer:
                    parameter.Set(intEditor.Value);
                    break;
                case StorageType.ElementId:
                    parameter.Set(ElementIdFactory.Create(intEditor.Value));
                    break;
                case StorageType.String:
                    parameter.Set(stringEditor.Value);
                    break;
            }
        }       
    }
}