using System;
using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal sealed class ValueContainer<T> : IValueContainer
    {       
        private static readonly ITypeHandler<T> typeHandler;
        private Type nativeType;
        private SnoopableContext context;
        private T value;

        public Type TypeHandlerType => typeHandler.GetType();
        public Type Type => typeHandler.Type;
        public T Value => value;       
        public string TypeName
        {
            get
            {
                var typeHandlerName = typeHandler.GetTypeHandlerName(value);
                string TypeName = typeHandlerName != "Object" ? typeHandlerName : $"Object : {nativeType?.GetCSharpName()}";
                return TypeName;                
             }
        }


        public ValueContainer()
        {           
            
        }


        public IValueContainer SetValue(SnoopableContext context, object value)
        {
            this.context = context;           
            this.value = value.CastValue<T>(Type);
         
            return this;
        }

        public string ValueAsString => typeHandler.ToLabel(context, value);
        public bool CanBeSnooped => typeHandler.CanBeSnooped(context, value);

        public string ToolTip
        {
            get
            {
                if (typeHandler is IHaveToolTip<T> typeHandlerWithToolTip)
                {
                    return typeHandlerWithToolTip.GetToolTip(context, value);
                }
                return null;
            }
        }

        public IEnumerable<SnoopableObject> Snoop() => typeHandler.Snoop(context, value);
       
    }
}