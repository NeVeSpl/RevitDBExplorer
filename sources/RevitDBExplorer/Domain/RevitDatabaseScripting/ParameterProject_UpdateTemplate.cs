using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using NSourceGenerators;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class ParameterProject_UpdateTemplate
    {
        public string Evaluate(string name, string paramValue, TemplateInputsKind inputsKind)
        {
            var templateType = GetTemplateType(inputsKind);
            var template = CodeToStringRepo.GetText(templateType.Name, true);
            var result = template.ReplaceMany(new[]
            {
                ("_p_angle", name),
                ("313.931", paramValue)
            });

            return result;
        }


        public Type GetTemplateType(TemplateInputsKind inputsKind)
        {
            if (inputsKind == TemplateInputsKind.Single)
            {
                return typeof(ParameterProject_UpdateSingle_Template);
            }

            return typeof(ParameterProject_UpdateMultiple_Template);
        }
    }

    internal class ParameterProject_UpdateMultiple_Template
    {
        [CodeToString(nameof(ParameterProject_UpdateMultiple_Template))]
        void Update(Document document, IEnumerable<Element> elements)
        {
            foreach (var element in elements)
            {
                var param = element.GetParameters("_p_angle").FirstOrDefault();
                if (param?.IsReadOnly == false)
                {
                    param.Set(313.931);
                }
            }
        }
    }


    internal class ParameterProject_UpdateSingle_Template
    {
        [CodeToString(nameof(ParameterProject_UpdateSingle_Template))]
        void Update(Document document, IEnumerable<Element> elements)
        {
            var element = elements.FirstOrDefault();
            var param = element.GetParameters("_p_angle").FirstOrDefault();
            if (param?.IsReadOnly == false)
            {
                param.Set(313.931);
            }            
        }
    }
}