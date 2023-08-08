using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

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
                    return new DoubleEditor(this, () => parameter.AsDouble(), x => parameter.Set(x));   
                case StorageType.Integer:
                    return new IntegerEditor(this, () => parameter.AsInteger(), x => parameter.Set(x));
                case StorageType.ElementId:
                    return new IntegerEditor(this, () => parameter.AsInteger(), x => parameter.Set(ElementIdFactory.Create(x)));              
            }
            return new StringEditor(this, () => parameter.AsString(), x => parameter.Set(x));
        }
       
        public override bool CanBeWritten(SnoopableContext context, Parameter parameter)
        {
            return !parameter.IsReadOnly;
        } 
    }
}