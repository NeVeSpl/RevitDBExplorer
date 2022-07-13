using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorForPrimitive : IMemberAccessor
    {
        private readonly IValueType value;

        public MemberAccessorForPrimitive(Type type)
        {
            this.value = ValueTypeFactory.Create(type);
        }

        public ReadResult Read(Document document, object @object)
        {
            value.SetValue(document, null);          
            value.SetValue(document, @object);
            return new ReadResult(value.ValueAsString, value.TypeName, value.CanBeSnooped, null);
        }

        public IEnumerable<SnoopableObject> Snoop(Document document, object @object)
        {
            return value.Snoop(document);
        }
    }
}