using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using NSourceGenerators;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class ParameterShared_UpdateTemplate
    {
        public string Evaluate(Guid guid, string paramValue, TemplateInputsKind inputsKind)
        {
            var templateType = GetTemplateType(inputsKind);
            var template = CodeToStringRepo.GetText(templateType.Name, true);
            var result = template.ReplaceMany(new[]
            {
                ("11111111-2222-3333-4444-555555666666", guid.ToString()),
                ("313.931", paramValue)
            });

            return result;
        }


        public Type GetTemplateType(TemplateInputsKind inputsKind)
        {
            if (inputsKind == TemplateInputsKind.Single)
            {
                return typeof(ParameterShared_UpdateSingle_Template);
            }

            return typeof(ParameterShared_UpdateMultiple_Template);
        }
    }


    internal class ParameterShared_UpdateMultiple_Template
    {
        [CodeToString(nameof(ParameterShared_UpdateMultiple_Template))]
        void Update(Document document, IEnumerable<Element> elements)
        {
            var paramElement = SharedParameterElement.Lookup(document, new Guid("11111111-2222-3333-4444-555555666666"));
            foreach (var element in elements)
            {
                var param = element.get_Parameter(paramElement.GuidValue);
                if (param?.IsReadOnly == false)
                {
                    param.Set(313.931);
                }
            }
        }
    }


    internal class ParameterShared_UpdateSingle_Template
    {
        [CodeToString(nameof(ParameterShared_UpdateSingle_Template))]
        void Update(Document document, IEnumerable<Element> elements)
        {
            var paramElement = SharedParameterElement.Lookup(document, new Guid("11111111-2222-3333-4444-555555666666"));
            var element = elements.FirstOrDefault();            
            var param = element.get_Parameter(paramElement.GuidValue);
            if (param?.IsReadOnly == false)
            {
                param.Set(313.931);
            }            
        }
    }
}