using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.UIComponents.List.ValueEditors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Parameter_SetValueString : MemberAccessorTypedWithWrite<Parameter>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Parameter x) => x.SetValueString("foo"); }


        private readonly StringEditorVM editor = new();


        public override IValueEditor GetEditor(SnoopableContext context, Parameter typedObject)
        {
            return editor;
        }
        public override void Read(SnoopableContext context, Parameter parameter)
        {
            editor.Value = parameter.AsValueString();           
        }
        public override bool CanBeWritten(SnoopableContext context, Parameter parameter)
        {
            return !parameter.IsReadOnly;
        }   
        public override void Write(SnoopableContext context, Parameter typedObject)
        {            
            typedObject.SetValueString(editor.Value);
        }       
    }
}