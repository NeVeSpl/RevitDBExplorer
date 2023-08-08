using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Parameter_SetValueString : MemberAccessorTypedWithWrite<Parameter>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Parameter x) => x.SetValueString("foo"); }



        public override IValueEditor CreateEditor(SnoopableContext context, Parameter typedObject)
        {
            return new StringEditor(this, () => typedObject.AsValueString(), x => typedObject.SetValueString(x));
        }
        public override bool CanBeWritten(SnoopableContext context, Parameter parameter)
        {
            return !parameter.IsReadOnly;
        }         
    }
}