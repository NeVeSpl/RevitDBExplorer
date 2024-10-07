using System;
using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal sealed class ValueContainer<T> : IValueContainer
    {       
        private static readonly ITypeHandler<T> typeHandler;       
        private SnoopableContext context;
        private T value;


        public Type TypeHandlerType => typeHandler.GetType();
        public Type Type => typeHandler.Type;
        public T Value => value;       
        public string TypeHandlerName
        {
            get
            {
                var typeHandlerName = typeHandler.GetTypeHandlerName(value);
                string typeName = typeHandlerName != "Object" ? typeHandlerName : $"Object : unknown";
                return typeName;                
             }
        }


        static ValueContainer()
        {
            typeHandler ??= ValueContainerFactory.SelectTypeHandlerFor(typeof(T)) as ITypeHandler<T>;
        }
        public ValueContainer()
        {
            
        }


        public IValueContainer SetValue(SnoopableContext context, object value)
        {          
            SetValueTyped(context, value.CastValue<T>(Type));

            return this;
        }
        public ValueContainer<T> SetValueTyped(SnoopableContext context, T value)
        {
            this.context = context;
            this.value = value;

            return this;
        }


        public string ValueAsString => typeHandler?.ToLabel(context, value) ?? "RDBE Error";
        public bool CanBeSnooped => typeHandler.CanBeSnooped(context, value);
        public bool CanBeVisualized => typeHandler.CanBeVisualized(context, value);

        public string ToolTip
        {
            get
            {
                if (value is not null && typeHandler is IHaveToolTip<T> typeHandlerWithToolTip)
                {
                    return typeHandlerWithToolTip.GetToolTip(context, value);
                }
                return null;
            }
        }

        public IEnumerable<SnoopableObject> Snoop() => typeHandler.Snoop(context, value);
        public IEnumerable<VisualizationItem> GetVisualization() => typeHandler.GetVisualization(context, value);
    }
}