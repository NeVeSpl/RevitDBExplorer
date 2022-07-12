using System;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorByRef : IMemberAccessor
    {
        private readonly MethodInfo getMethod;
        private readonly MethodInfo setMethod; 
        private readonly IValueType value;


        public MemberAccessorByRef(MethodInfo getMethod, MethodInfo setMethod)
        {                
            this.getMethod = getMethod;
            this.setMethod = setMethod;
            this.value = ValueTypeFactory.Create(getMethod.ReturnType);
        }


        public ReadResult Read(Document document, object @object)
        {
            value.SetValue(document, null);
            var paramsDef = getMethod.GetParameters();
            var resolvedArgs = ResolveArguments(paramsDef, document, @object);
            if (resolvedArgs.ex is not null)
            {
                return new ReadResult(String.Empty, value.TypeName, false, resolvedArgs.ex);
            }
            var result = getMethod.Invoke(@object, resolvedArgs.args);
            value.SetValue(document, result);
            return new ReadResult(value.ValueAsString, value.TypeName, value.CanBeSnooped, null);
        }
        public IEnumerable<SnoopableObject> Snoop(Document document, object @object)
        {
            return value.Snoop(document);
        }


        private (object[] args, Exception ex) ResolveArguments(ParameterInfo[] paramsDef, Document doc, object @object)
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
                            argument = new Options() { View = doc.ActiveView };
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

                    if (argument is null)
                    {
                        return (null, new CouldNotResolveAllArgumentsException(paramsDef[i].Name));
                    }

                    args[i] = argument;
                }
            }
            return (args, null);
        }        
    }

    class CouldNotResolveAllArgumentsException : ArgumentException
    {
        public CouldNotResolveAllArgumentsException(string argName) : base($"Could not resolve argument: {argName}")
        {

        }
    }
}