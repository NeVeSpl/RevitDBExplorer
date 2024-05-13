using System;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class Labeler
    {
        public static string GetLabelForObject(object @object, SnoopableContext context)
        {
            return GetLabelForObject(@object.GetType(), @object, context);
        }
        public static string GetLabelForObject(Type type, object @object, SnoopableContext context)
        {
            var valueType = ValueContainerFactory.SelectTypeHandlerFor(type);          
            return valueType.ToLabel(context, @object);
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
            if (categoryId != null)
            {
                if (Category.IsBuiltInCategoryValid((BuiltInCategory)categoryId.Value()))
                {
                    return LabelUtils.GetLabelFor((BuiltInCategory)categoryId.Value());
                }

                return "<invalid category>";
            }

            return "<null>";
        }


        public static string GetLabelForCollection(string typeName, int? count)
        {
            if (count.HasValue)
            {
                return $"[{typeName.ReduceTypeName()}: {count.Value}]";
            }
            return $"[{typeName.ReduceTypeName()}: ?]";
        }
        public static string GetLabelForObjectWithId(string typeName, long? id)
        {
            if (id.HasValue)
            {
                return $"{typeName} ({id.Value})";
            }
            return $"{typeName}";
        }
    }
}