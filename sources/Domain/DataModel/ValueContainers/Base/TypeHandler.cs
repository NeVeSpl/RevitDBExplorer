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



        bool ICanBeSnooped.CanBeSnooped(SnoopableContext context, object value)
        {
            T typedValue = value.CastValue<T>(type);
            return (this as ICanBeSnooped<T>).CanBeSnooped(context, typedValue);
        }
        bool ICanBeSnooped<T>.CanBeSnooped(SnoopableContext context, T value)
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
            return Snooop(context.Document, value) ?? Enumerable.Empty<SnoopableObject>();
        }
        protected virtual IEnumerable<SnoopableObject> Snooop(Document document, T value) => null;



        string IToLabel.ToLabel(SnoopableContext context, object value)
        {
            T typedValue = value.CastValue<T>(type);
            return (this as IToLabel<T>).ToLabel(context, typedValue);
        }
        string IToLabel<T>.ToLabel(SnoopableContext context, T value)
        {
            if (value is null) return "<null>";
            var label = ToLabel(context, value);
            if (string.IsNullOrEmpty(label)) return "<empty>";
            return label;
        }
        protected abstract string ToLabel(SnoopableContext context, T value);
    }
}