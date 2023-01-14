using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorByFunc<TSnoopedObjectType, TReturnType> : MemberAccessorTyped<TSnoopedObjectType>, IMemberAccessorWithValue
    {
        private readonly IValueContainer value;
        private readonly Func<Document, TSnoopedObjectType, TReturnType> get;
        IValueContainer IMemberAccessorWithValue.Value => value;


        public MemberAccessorByFunc(Func<Document, TSnoopedObjectType, TReturnType> get)
        {
            this.get = get;
            this.value = ValueContainerFactory.Create(typeof(TReturnType));
        }
        

        public override ReadResult Read(SnoopableContext context, TSnoopedObjectType @object)
        {
            value.SetValue(context, null);
            var result = get(context.Document, @object);
            value.SetValue(context, result);
            return new ReadResult(value.ValueAsString, value.TypeName, value.CanBeSnooped);
        }
        public override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType @object)
        {
            return value.Snoop();
        }
    }
}