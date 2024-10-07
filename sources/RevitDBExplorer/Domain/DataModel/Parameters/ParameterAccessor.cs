using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.Domain.RevitDatabaseScripting;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Parameters
{
    internal sealed class ParameterAccessor : IAccessor, IAccessorForDefaultPresenter, IAccessorWithCodeGeneration
    {
        private readonly Parameter parameter;
        private readonly IValueContainer value;
       
        public string UniqueId { get; set; }
        public Invocation DefaultInvocation { get; } = new Invocation();


        public ParameterAccessor(Parameter parameter)
        {
            this.parameter = parameter;
            this.value = CreateValueContainer(parameter.StorageType);
        }


        public IValueViewModel CreatePresenter(SnoopableContext context, object @object)
        {
            return new DefaultPresenter(this);
        }

        
        public ReadResult Read(SnoopableContext context, object @object)
        {
            if (!parameter.HasValue)
            {
                return new ReadResult("<has no value>", "[ByParam]", false, false);
            }

            switch (parameter.StorageType)
            {
                case StorageType.String:
                    var stringValue = parameter.AsString();
                    (value as ValueContainer<string>).SetValueTyped(context, stringValue);
                    break;
                case StorageType.Integer:
                    var intValue = parameter.AsInteger();
                    (value as ValueContainer<int>).SetValueTyped(context, intValue);
                    break;
                case StorageType.ElementId:
                    var idValue = parameter.AsElementId();
                    (value as ValueContainer<ElementId>).SetValueTyped(context, idValue);
                    break;
                case StorageType.Double:
                    var doubleValue = parameter.AsDouble();
                    (value as ValueContainer<double>).SetValueTyped(context, doubleValue);
                    break;
                case StorageType.None:
                    break;
            }
            return new ReadResult(value.ValueAsString, "[ByParam] " + value.TypeHandlerName, value.CanBeSnooped, value.CanBeVisualized, value);

        }
        public IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object, IValueContainer state)
        {
            return state.Snoop();
        }
        public IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, object @object, IValueContainer state)
        {
            return state.GetVisualization();
        }


        public string GenerateInvocationForScript(TemplateInputsKind inputsKind)
        {
            string paramValue = parameter.StorageType switch { StorageType.String => @"""""", StorageType.Integer => "0", StorageType.Double => "0.0", _ => "new ElementId(0)" };

            if (parameter.IsShared)
            {
                return new ParameterShared_UpdateTemplate().Evaluate(parameter.GUID, paramValue, inputsKind);
            }
            if (parameter.Definition is InternalDefinition internalDef)
            {
                if (internalDef.BuiltInParameter != BuiltInParameter.INVALID)
                {
                    return new ParameterBuiltIn_UpdateTemplate().Evaluate(internalDef.BuiltInParameter, paramValue, inputsKind);
                }
            }
            return new ParameterProject_UpdateTemplate().Evaluate(parameter.Definition.Name, paramValue, inputsKind);
        }


        private IValueContainer CreateValueContainer(StorageType storageType)
        {
            switch (storageType)
            {
                case StorageType.None:
                case StorageType.String:
                    return new ValueContainer<string>();
                case StorageType.Integer:
                    return new ValueContainer<int>();
                case StorageType.Double:
                    return new ValueContainer<double>();
                case StorageType.ElementId:
                    return new ValueContainer<ElementId>();
               
            }
            throw new NotImplementedException();
        }      
    }
}