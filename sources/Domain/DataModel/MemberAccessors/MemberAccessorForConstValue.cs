using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal sealed class MemberAccessorForConstValue : IMemberAccessor, IMemberAccessorWithValue
    {
        private readonly IValueContainer value;
        IValueContainer IMemberAccessorWithValue.Value => value;


        public MemberAccessorForConstValue(Type type, Document document, object value)
        {
            this.value = ValueContainerFactory.Create(type);
            this.value.SetValue(document, value);
        }


        public ReadResult Read(SnoopableContext context, object @object)
        {          
            return new ReadResult(value.ValueAsString, value.TypeName, value.CanBeSnooped);
        }
        public IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object)
        {
            return value.Snoop(context.Document);
        }
    }
}