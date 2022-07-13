using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorWithConst : IMemberAccessor
    {
        private readonly IValueType value;

        public MemberAccessorWithConst(Type type, Document document, object value)
        {
            this.value = ValueTypeFactory.Create(type);
            this.value.SetValue(document, value);
        }

        public ReadResult Read(Document document, object @object)
        {          
            return new ReadResult(value.ValueAsString, value.TypeName, value.CanBeSnooped, null);
        }

        public IEnumerable<SnoopableObject> Snoop(Document document, object @object)
        {
            return value.Snoop(document);
        }
    }
}