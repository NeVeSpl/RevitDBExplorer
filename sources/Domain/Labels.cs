using System;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain
{
    internal static class Labels
    {        
        public static string GetNameForObject(Type type, object @object, Document document)
        {
            var valueType = ValueTypeFactory.Create(type);
            valueType.SetValue(document, @object);           
            return valueType.ValueAsString;
        }

        public static string GetLabelForException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                if (ex.Message == "Exception has been thrown by the target of an invocation.")
                {
                    ex = ex.InnerException;
                }
            }

            return ex.InnerException == null ? $"{ex.Message}" : $"{ex.Message} ({ex.InnerException.Message})";
        }
    }
}
