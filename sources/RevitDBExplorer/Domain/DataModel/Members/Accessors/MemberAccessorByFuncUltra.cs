using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Accessors
{
    internal class MemberAccessorByFuncUltra<TSnoopedObjectType, TParam0Type, TReturnType> : MemberAccessorTypedWithDefaultPresenter<TSnoopedObjectType>
    {
        private readonly Func<Document, TSnoopedObjectType, TParam0Type, TReturnType> get;
        private readonly IEnumerable<TParam0Type> param_0_arguments;
        private readonly string param_0_Name;


        public MemberAccessorByFuncUltra(Func<Document, TSnoopedObjectType, TParam0Type, TReturnType> get, IEnumerable<TParam0Type> param_0_arguments, string param_0_Name)
        {
            this.get = get;
            this.param_0_arguments = param_0_arguments;
            this.param_0_Name = param_0_Name;
        }


        protected override ReadResult Read(SnoopableContext context, TSnoopedObjectType snoopedObject)
        {
            var value = new ValueContainer<ExecuteResultCollection>();
            var resultCollection = ExecuteResultCollection.Create<TReturnType>(param_0_Name);            

            foreach (var arg in param_0_arguments)
            {
                var result = get(context.Document, snoopedObject, arg);
                resultCollection.Add(arg, result);
            }

            value.SetValueTyped(context, resultCollection);

            return new ReadResult(value.ValueAsString, "[ByFuncUltra] " + value.TypeHandlerName, value.CanBeSnooped, value.CanBeVisualized, value);
        }
    }   
}