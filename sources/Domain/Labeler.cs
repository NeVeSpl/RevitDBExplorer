using System;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class Labeler
    {
        public static string GetLabelForObject(object @object, Document document)
        {
            return GetLabelForObject(@object.GetType(), @object, document);
        }
        public static string GetLabelForObject(Type type, object @object, Document document)
        {
            var valueType = ValueContainerFactory.Create(type);
            valueType.SetValue(document, @object);           
            return valueType.ValueAsString;
        }

        public static string GetLabelForException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                if ((ex.Message == "Exception has been thrown by the target of an invocation.") && (!String.IsNullOrEmpty(ex.InnerException?.Message)))
                {
                    ex = ex.InnerException;
                }
            }            

            return String.IsNullOrEmpty(ex.InnerException?.Message) ? $"{ex.Message}" : $"{ex.Message} ({ex.InnerException.Message})";
        }
        public static string GetLabelForCategory(ElementId categoryId)
        {
            if ((categoryId != null) && (Category.IsBuiltInCategoryValid((BuiltInCategory)categoryId.Value())))
            {
                return LabelUtils.GetLabelFor((BuiltInCategory)categoryId.Value());
            }
            return "[invalid category]";
        }
    }
}