using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using NSourceGenerators;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class ParameterBuiltIn_UpdateTemplate
    {
        public string Evaluate(BuiltInParameter parameter, string paramValue, TemplateInputsKind inputsKind)
        {
            var templateType = GetTemplateType(inputsKind);
            var template = CodeToStringRepo.GetText(templateType.Name, true);
            var result = template.ReplaceMany(new[]
            {
                ("HOST_AREA_COMPUTED", parameter.ToString()),
                ("313.931", paramValue)
            });

            return result;
        }


        public Type GetTemplateType(TemplateInputsKind inputsKind)
        {
            if (inputsKind == TemplateInputsKind.Single)
            {
                return typeof(ParameterBuiltIn_UpdateSingle_Template);
            }

            return typeof(ParameterBuiltIn_UpdateMultiple_Template);
        }
    }


    internal class ParameterBuiltIn_UpdateMultiple_Template
    {
        [CodeToString(nameof(ParameterBuiltIn_UpdateMultiple_Template))]
        void Update(Document document, IEnumerable<Element> elements)
        {
            foreach (var element in elements)
            {
                var param = element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
                if (param?.IsReadOnly == false)
                {
                    param.Set(313.931);
                }
            }
        }
    }


    internal class ParameterBuiltIn_UpdateSingle_Template
    {
        [CodeToString(nameof(ParameterBuiltIn_UpdateSingle_Template))]
        void Update(Document document, IEnumerable<Element> elements)
        {
            var element = elements.FirstOrDefault();
            var param = element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
            if (param?.IsReadOnly == false)
            {
                param.Set(313.931);
            }            
        }
    }
}