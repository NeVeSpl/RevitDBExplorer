using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorByFunc<TSnoopedObjectType, TReturnType> : MemberAccessorTyped<TSnoopedObjectType>
    {     
        private readonly Func<Document, TSnoopedObjectType, TReturnType> get;      


        public MemberAccessorByFunc(Func<Document, TSnoopedObjectType, TReturnType> get)
        {
            this.get = get;
            
        }
        

        public override ReadResult Read(SnoopableContext context, TSnoopedObjectType @object)
        {
            var value = ValueContainerFactory.Create(typeof(TReturnType));            
            var result = get(context.Document, @object);
            value.SetValue(context, result);
            return new ReadResult(value.ValueAsString, value.TypeName, value.CanBeSnooped, value);
        }
        public override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType @object, IValueContainer state)
        {
            return state.Snoop();
        }
    }
}