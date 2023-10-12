using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorByFunc<TSnoopedObjectType, TReturnType> : MemberAccessorTyped<TSnoopedObjectType>
    {     
        private readonly Func<Document, TSnoopedObjectType, TReturnType> get;
        private readonly Func<Document, TSnoopedObjectType, IEnumerable<SnoopableObject>> snoop;

        public MemberAccessorByFunc(Func<Document, TSnoopedObjectType, TReturnType> get, Func<Document, TSnoopedObjectType, IEnumerable<SnoopableObject>> snoop = null)
        {
            this.get = get;            
            this.snoop = snoop;
        }
        

        public override ReadResult Read(SnoopableContext context, TSnoopedObjectType @object)
        {
            var value = new ValueContainer<TReturnType>();
            var result = get(context.Document, @object);
            value.SetValueTyped(context, result);
            return new ReadResult(value.ValueAsString, "[ByFunc] " + value.TypeName, value.CanBeSnooped, value);
        }
        public override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType @object, IValueContainer state)
        {
            if (snoop != null)
            {
                return snoop(context.Document, @object);
            }
            return state.Snoop();
        }
    }
}