using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class ParameterBuiltIn_UpdateTemplate
    {
        public string Evaluate(BuiltInParameter parameter)
        {
            var template = GetTemplate();
            var result = template.ReplaceMany(new[]
            {
                ("HOST_AREA_COMPUTED", parameter.ToString()),
            });

            return result;
        }


        public string GetTemplate()
        {
            return CodeToStringRepo.GetText(nameof(ParameterBuiltIn_UpdateTemplate), true);
        }


        [CodeToString(nameof(ParameterBuiltIn_UpdateTemplate))]
        void Update(Document document, IEnumerable<Element> elements)
        {           
            foreach (var element in elements)
            {
                var param = element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
                if (param?.IsReadOnly == false)
                {
                    param.Set(0);
                }
            }
        }
    }
}