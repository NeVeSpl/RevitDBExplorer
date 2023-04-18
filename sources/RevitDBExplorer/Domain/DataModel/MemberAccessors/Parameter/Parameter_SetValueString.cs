using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ViewModels;
using RevitDBExplorer.Domain.DataModel.ViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Parameter_SetValueString : MemberAccessorTypedWithWrite<Parameter>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Parameter x) => x.SetValueString("foo"); }


       


        public override IValueEditor CreateEditor(SnoopableContext context, Parameter typedObject)
        {
            return new StringEditorVM();
        }
        public override void Read(SnoopableContext context, Parameter parameter, IValueEditor valueEditor)
        {
            (valueEditor as StringEditorVM).Value = parameter.AsValueString();           
        }
        public override bool CanBeWritten(SnoopableContext context, Parameter parameter)
        {
            return !parameter.IsReadOnly;
        }   
        public override void Write(SnoopableContext context, Parameter typedObject, IValueEditor valueEditor)
        {            
            typedObject.SetValueString((valueEditor as StringEditorVM).Value);
        }       
    }
}