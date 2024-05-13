using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Parameter_Set : MemberAccessorTyped<Parameter>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() =>
        [
             (Parameter x) => x.Set("foo"),
             (Parameter x) => x.Set(7), 
             (Parameter x) => x.Set(7.77),
             (Parameter x) => x.Set(ElementIdFactory.Create(7)), 
        ];


        protected override IValueViewModel CreatePresenter(SnoopableContext context, Parameter parameter)
        {
            switch (parameter.StorageType)
            {
                case StorageType.Double:
                    return new DoubleEditor(this, () => parameter.AsDouble(), x => parameter.Set(x), () => !parameter.IsReadOnly);   
                case StorageType.Integer:
                    return new IntegerEditor(this, () => parameter.AsInteger(), x => parameter.Set(x), () => !parameter.IsReadOnly);
                case StorageType.ElementId:
                    return new IntegerEditor(this, () => parameter.AsInteger(), x => parameter.Set(ElementIdFactory.Create(x)), () => !parameter.IsReadOnly);              
            }
            return new StringEditor(this, () => parameter.AsString(), x => parameter.Set(x), () => !parameter.IsReadOnly);
        }
    }
}