using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal abstract class TypeHandler<T> : ITypeHandler<T>, ITypeHandler
    {
        private static readonly Type type = typeof(T);

        public Type Type => type;

        public string GetTypeHandlerName(T value)
        {
            var containerTypeName = Type.GetCSharpName();
            var valueTypeName = value?.GetType()?.GetCSharpName() ?? containerTypeName;

            return valueTypeName == containerTypeName ? containerTypeName : $"{containerTypeName} : {valueTypeName}";            
        }



        bool ISnoop.CanBeSnooped(SnoopableContext context, object value)
        {
            T typedValue = value.CastValue<T>(type);
            return (this as ISnoop<T>).CanBeSnooped(context, typedValue);
        }
        bool ISnoop<T>.CanBeSnooped(SnoopableContext context, T value)
        {
            if (value is null) return false;
            return CanBeSnoooped(context, value);
        }
        protected abstract bool CanBeSnoooped(SnoopableContext context, T value);



        IEnumerable<SnoopableObject> ISnoop.Snoop(SnoopableContext context, object value)
        {
            T typedValue = value.CastValue<T>(type);
            return (this as ISnoop<T>).Snoop(context, typedValue);
        }
        IEnumerable<SnoopableObject> ISnoop<T>.Snoop(SnoopableContext context, T value)
        {
            return Snooop(context, value) ?? Enumerable.Empty<SnoopableObject>();
        }
        protected virtual IEnumerable<SnoopableObject> Snooop(SnoopableContext context, T value)
        {
            yield return new SnoopableObject(context.Document, value);
        }



        string IHaveLabel.ToLabel(SnoopableContext context, object value)
        {
            T typedValue = value.CastValue<T>(type);
            return (this as IHaveLabel<T>).ToLabel(context, typedValue);
        }
        string IHaveLabel<T>.ToLabel(SnoopableContext context, T value)
        {
            if (value is null) return "<null>";
            var label = ToLabel(context, value);
            if (string.IsNullOrEmpty(label)) return "<empty>";
            return label;
        }
        protected abstract string ToLabel(SnoopableContext context, T value);



        bool IHaveVisualization.CanBeVisualized(SnoopableContext context, object value)
        {
            T typedValue = value.CastValue<T>(type);
            return (this as IHaveVisualization<T>).CanBeVisualized(context, typedValue);
        }
        bool IHaveVisualization<T>.CanBeVisualized(SnoopableContext context, T value)
        {
            if (value is null) return false;
            return CanBeVisualized(context, value);
        }
        protected virtual bool CanBeVisualized(SnoopableContext context, T value) => false;



        IEnumerable<VisualizationItem> IHaveVisualization.GetVisualization(SnoopableContext context, object value)
        {
            T typedValue = value.CastValue<T>(type);
            return (this as IHaveVisualization<T>).GetVisualization(context, typedValue);
        }
        IEnumerable<VisualizationItem> IHaveVisualization<T>.GetVisualization(SnoopableContext context, T value)
        {            
            if (value is not null) 
            {
                if ((value is Element element) && (!element.IsValidObject))
                {
                    return Enumerable.Empty<VisualizationItem>();
                }

                return GetVisualization(context, value) ?? Enumerable.Empty<VisualizationItem>();
            }
            return Enumerable.Empty<VisualizationItem>();
        }
        protected virtual IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, T value) => null;
    }
}