using System;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal static class CodeGenerator
    {

        public static string GenerateQueryFor(string text)
        {
            var appArg = "";
            if (text.Contains("uia."))
            {
                appArg = ", UIApplication uia";
            }

            var scriptText =
@$"IEnumerable<object> Select(Document document{appArg})
{{
{text}    
}}";
            return scriptText;
        }



        
        public static string GenerateUpdateCommandForParameter(Autodesk.Revit.DB.Parameter revitParameter)
        {            
            var paramIdText = "";
            var paramGetText = "get_Parameter(paramId)";
            if (revitParameter.IsShared)
            {
                paramIdText = 
@$"var paramElement = SharedParameterElement.Lookup(document, new Guid(""{revitParameter.GUID}""));
    var paramId = paramElement.GuidValue; ";
            }
            if (revitParameter.Definition is InternalDefinition internalDef)
            {
                if (internalDef.BuiltInParameter != BuiltInParameter.INVALID)
                {
                    paramIdText = $"var paramId = BuiltInParameter.{internalDef.BuiltInParameter};";
                }
            }
            // todo : add case for project parameter
            if (string.IsNullOrEmpty(paramIdText))
            {
                paramIdText = $"var paramId = \"{revitParameter.Definition.Name}\";";
                paramGetText = "GetParameters(paramId).FirstOrDefault();";
            }

            return
@$"void Update(Document document, IEnumerable<Element> elements)
{{
    {paramIdText}
    foreach (var element in elements)
    {{
        var param = element.{paramGetText};
        if (param?.IsReadOnly == false)
        {{
            param.Set();
        }}
    }}
}}
";
        }
        public static string GenerateUpdateCommandForType(System.Type type)
        {
            return
@$"void Update(IEnumerable<{type.GetCSharpName()}> inputs)
{{
    foreach (var item in inputs)
    {{
        item.
    }}
}}
";
        }

    }
}