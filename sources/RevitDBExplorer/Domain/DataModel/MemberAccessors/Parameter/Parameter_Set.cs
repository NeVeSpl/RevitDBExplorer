using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ViewModels;
using RevitDBExplorer.Domain.DataModel.ViewModels.Base;

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
              


        public override IValueEditor CreateEditor(SnoopableContext context, Parameter parameter)
        {
            switch (parameter.StorageType)
            {
                case StorageType.Double:
                    return new DoubleEditorVM();   
                case StorageType.Integer:
                case StorageType.ElementId:
                    return new IntegerEditorVM();              
            }
            return new StringEditorVM();
        }
        public override void Read(SnoopableContext context, Parameter parameter, IValueEditor valueEditor)
        {
            switch (valueEditor)
            {
                case DoubleEditorVM doubleEditor:
                    doubleEditor.Value = parameter.AsDouble();
                    break;
                case IntegerEditorVM integerEditor:
                    integerEditor.Value = parameter.AsInteger();
                    break;
                case StringEditorVM stringEditor:
                    stringEditor.Value = parameter.AsString();
                    break;
            }
        }
        public override bool CanBeWritten(SnoopableContext context, Parameter parameter)
        {
            return !parameter.IsReadOnly;
        }   
        public override void Write(SnoopableContext context, Parameter parameter, IValueEditor valueEditor)
        {
            switch (parameter.StorageType)
            {
                case StorageType.Double:
                    parameter.Set((valueEditor as DoubleEditorVM).Value);
                    break;
                case StorageType.Integer:
                    parameter.Set((valueEditor as IntegerEditorVM).Value);
                    break;
                case StorageType.ElementId:
                    parameter.Set(ElementIdFactory.Create((valueEditor as IntegerEditorVM).Value));
                    break;
                case StorageType.String:
                    parameter.Set((valueEditor as StringEditorVM).Value);
                    break;
            }
        }       
    }
}