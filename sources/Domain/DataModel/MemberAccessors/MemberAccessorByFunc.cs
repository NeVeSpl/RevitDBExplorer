using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{

    delegate TReturn FuncWithCtx<TInput, TReturn>(Document document, TInput @object) where TInput : class;

    internal class MemberAccessorByFunc<TInput, TReturn> : IMemberAccessor where TInput : class
    {
        private readonly IValueType value;
        private readonly FuncWithCtx<TInput, TReturn> get;


        public MemberAccessorByFunc(FuncWithCtx<TInput, TReturn> get)
        {
            this.get = get;
            this.value = ValueTypeFactory.Create(typeof(TReturn));
        }



        public ReadResult Read(Document document, object @object)
        {
            value.SetValue(document, null);            
            var result = get(document, @object as TInput);
            value.SetValue(document, result);
            return new ReadResult(value.ValueAsString, value.TypeName, value.CanBeSnooped, null);
        }

        public IEnumerable<SnoopableObject> Snoop(Document document, object @object)
        {
            return value.Snoop(document);
        }
    }
}
