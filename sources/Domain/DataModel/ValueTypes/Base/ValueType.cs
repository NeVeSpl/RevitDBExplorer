using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes.Base
{
    internal abstract class ValueType<T> : IValueType
    {
        private static readonly Type type = typeof(T);
        private T value;


        public ValueType()
        {

        }


        public Type Type => type;
        public virtual string TypeName
        {
            get
            {
                var finalType = value?.GetType()?.Name ?? Type.Name;

                return $"{Type.Name}"+ (finalType != Type.Name ? $"({finalType})" : "");
            }
        }

        public virtual IValueType SetValue(Document document, object value)
        {
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
                return ToLabel(value);
            }            
        }
        protected abstract string ToLabel(T value);

        public bool CanBeSnooped => CanBeSnoooped(value);
        protected abstract bool CanBeSnoooped(T value);

        public IEnumerable<SnoopableObject> Snoop(Document document) => Snooop(document, value);
        protected virtual IEnumerable<SnoopableObject> Snooop(Document document, T value) => Enumerable.Empty<SnoopableObject>();
    }
}