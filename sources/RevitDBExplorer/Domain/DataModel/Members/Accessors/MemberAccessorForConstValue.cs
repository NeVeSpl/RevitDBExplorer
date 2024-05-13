using System;
using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Accessors
{
    internal sealed class MemberAccessorForConstValue : MemberAccessorTypedWithDefaultPresenter<object>
    {
        private readonly IValueContainer value;


        public MemberAccessorForConstValue(Type type, SnoopableContext context, object value)
        {
            this.value = ValueContainerFactory.Create(type);
            this.value.SetValue(context, value);
        }


        protected override ReadResult Read(SnoopableContext context, object @object)
        {
            return new ReadResult(value.ValueAsString, value.TypeHandlerName, value.CanBeSnooped, value.CanBeVisualized, value);
        }
        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object, IValueContainer state)
        {
            return state.Snoop();
        }
    }
}