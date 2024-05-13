using System;
using System.Reflection;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.RevitDatabaseScripting;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Accessors
{
    internal sealed class MemberAccessorByRefCompiled<TSnoopedObjectType, TReturnType> : MemberAccessorTypedWithDefaultPresenter<TSnoopedObjectType>, IAccessorWithCodeGeneration
    {
        private readonly MethodInfo getMethod;
        private readonly Func<TSnoopedObjectType, TReturnType> func;


        public MemberAccessorByRefCompiled(MethodInfo getMethod, Func<TSnoopedObjectType, TReturnType> func)
        {
            this.getMethod = getMethod;
            this.func = func;
        }


        protected override ReadResult Read(SnoopableContext context, TSnoopedObjectType typedObject)
        {
            var value = new ValueContainer<TReturnType>();
            var result = func(typedObject);
            value.SetValueTyped(context, result);
            return new ReadResult(value.ValueAsString, "[ByRefComp] " + value.TypeHandlerName, value.CanBeSnooped, value.CanBeVisualized, value);
        }


        public string GenerateInvocationForScript(TemplateInputsKind inputsKind)
        {
            return new MemberInvocationTemplateSelector().Evaluate(getMethod, null, inputsKind);
        }
    }
}