using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorByFunc<TSnoopedObjectType, TReturnType> : MemberAccessorTyped<TSnoopedObjectType>
    {
        private readonly IValueContainer value;
        private readonly Func<Document, TSnoopedObjectType, TReturnType> get;


        public MemberAccessorByFunc(Func<Document, TSnoopedObjectType, TReturnType> get)
        {
            this.get = get;
            this.value = ValueContainerFactory.Create(typeof(TReturnType));
        }


        public override ReadResult Read(Document document, TSnoopedObjectType @object)
        {
            value.SetValue(document, null);
            var result = get(document, @object);
            value.SetValue(document, result);
            return new ReadResult(value.ValueAsString, value.TypeName, value.CanBeSnooped, null);
        }


        public override IEnumerable<SnoopableObject> Snoop(Document document, TSnoopedObjectType @object)
        {
            return value.Snoop(document);
        }
    }
}