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
        public string Evaluate(string name)
        {
            var template = GetTemplate();
            var result = template.ReplaceMany(new[]
            {
                ("_p_angle", name),
            });

            return result;
        }


        public string GetTemplate()
        {
            return CodeToStringRepo.GetText(nameof(ParameterProject_UpdateTemplate), true);
        }


        [CodeToString(nameof(ParameterProject_UpdateTemplate))]
        void Update(Document document, IEnumerable<Element> elements)
        {           
            foreach (var element in elements)
            {
                var param = element.GetParameters("_p_angle").FirstOrDefault();
                if (param?.IsReadOnly == false)
                {
                    param.Set(0);
                }
            }
        }
    }
}