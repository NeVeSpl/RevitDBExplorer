using System;
using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal sealed class MemberAccessorByRefCompiled<TSnoopedObjectType, TReturnType> : MemberAccessorTyped<TSnoopedObjectType>
    {
        private readonly Func<TSnoopedObjectType, TReturnType> func;

        public MemberAccessorByRefCompiled(Func<TSnoopedObjectType, TReturnType> func)
        {
            this.func = func;
        }

        public override ReadResult Read(SnoopableContext context, TSnoopedObjectType typedObject)
        {
            var value = new ValueContainer<TReturnType>();        
            var result = func(typedObject);
            value.SetValueTyped(context, result);
            return new ReadResult(value.ValueAsString, "[ByRefComp] " + value.TypeName, value.CanBeSnooped, value);
        }

        public override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType typedObject, IValueContainer state)
        {
            return state.Snoop();
        }
    }
}