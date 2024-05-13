using System;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.RevitDatabaseScripting;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Accessors
{
    internal sealed class MemberAccessorByRef : MemberAccessorTypedWithDefaultPresenter<object>, IAccessorWithCodeGeneration
    {
        private readonly MethodInfo getMethod;
        private readonly ParameterInfo[] parameters;


        public MemberAccessorByRef(MethodInfo getMethod)
        {
            this.getMethod = getMethod;
            parameters = getMethod.GetParameters();
        }


        protected override ReadResult Read(SnoopableContext context, object @object)
        {
            var value = ValueContainerFactory.Create(getMethod.ReturnType);
            var resolvedArgs = ResolveArguments(parameters, context.Document, @object);
            var result = getMethod.Invoke(@object, resolvedArgs);
            value.SetValue(context, result);
            return new ReadResult(value.ValueAsString, "[ByRef] " + value.TypeHandlerName, value.CanBeSnooped, value.CanBeVisualized, value);
        }


        public static Type[] HandledParameterTypes = new[] { typeof(Document), typeof(Options), typeof(View), typeof(SpatialElementBoundaryOptions) };

        private object[] ResolveArguments(ParameterInfo[] paramsDef, Document doc, object @object)
        {
            object[] args = null;
            if (paramsDef.Length > 0)
            {
                args = new object[paramsDef.Length];

                for (int i = 0; i < paramsDef.Length; i++)
                {
                    object argument = null;
                    if (paramsDef[i].ParameterType == typeof(Document))
                    {
                        argument = doc;
                    }
                    if (paramsDef[i].ParameterType == typeof(Options))
                    {
                        if (@object is Element { ViewSpecific: true })
                        {
                            argument = new Options() { View = doc.ActiveView, ComputeReferences = true };
                        }
                        else
                        {
                            argument = new Options();
                        }
                    }
                    if (paramsDef[i].ParameterType == typeof(View))
                    {
                        argument = doc.ActiveView;
                    }
                    if (paramsDef[i].ParameterType == typeof(SpatialElementBoundaryOptions))
                    {
                        argument = new SpatialElementBoundaryOptions()
                        {
                            StoreFreeBoundaryFaces = true,
                            SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center
                        };
                    }

                    args[i] = argument;
                }
            }
            return args;
        }


        public string GenerateInvocationForScript(TemplateInputsKind inputsKind)
        {
            return new MemberInvocationTemplateSelector().Evaluate(getMethod, null, inputsKind);
        }
    }
}