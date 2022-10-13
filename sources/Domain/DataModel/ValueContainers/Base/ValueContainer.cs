using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal abstract class ValueContainer<T> : IValueContainer
    {
        private static readonly Type type = typeof(T);
        private T value;

        public T Value => value;

        public Type Type => type;
        public virtual string TypeName
        {
            get
            {
                var containerTypeName = Type.GetCSharpName();
                var valueTypeName = value?.GetType()?.GetCSharpName() ?? containerTypeName;

                return valueTypeName == containerTypeName ? containerTypeName : $"{containerTypeName} : {valueTypeName}";
            }
        }

        protected Units Units { get; private set; }

        public virtual IValueContainer SetValue(Document document, object value)
        {
            Units = document?.GetUnits();
            if (value == null)
            {
                this.value = (T)type.GetDefaultValue();
            }
            else
            {
                this.value = (T)value;
            }
            return this;
        }

        public string ValueAsString
        {
            get
            {
                if (value is null) return "<null>";
                var label = ToLabel(value);
                if (string.IsNullOrEmpty(label)) return "<empty>";
                return label;
            }
        }
        protected abstract string ToLabel(T value);

        public bool CanBeSnooped
        {
            get 
            {
                if (value is null) return false;
                return CanBeSnoooped(value);
            }
        }
        protected abstract bool CanBeSnoooped(T value);

        public IEnumerable<SnoopableObject> Snoop(Document document) => Snooop(document, value) ?? Enumerable.Empty<SnoopableObject>();
        protected virtual IEnumerable<SnoopableObject> Snooop(Document document, T value) => null;
    }
}