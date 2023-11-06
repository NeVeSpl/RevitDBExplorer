using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class ParameterShared_UpdateTemplate
    {
        public string Evaluate(Guid guid)
        {
            var template = GetTemplate();
            var result = template.ReplaceMany(new[]
            {
                ("11111111-2222-3333-4444-555555666666", guid.ToString()),
            });

            return result;
        }


        public string GetTemplate()
        {
            return CodeToStringRepo.GetText(nameof(ParameterShared_UpdateTemplate), true);
        }


        [CodeToString(nameof(ParameterShared_UpdateTemplate))]
        void Update(Document document, IEnumerable<Element> elements)
        {
            var paramElement = SharedParameterElement.Lookup(document, new Guid("11111111-2222-3333-4444-555555666666"));           
            foreach (var element in elements)
            {
                var param = element.get_Parameter(paramElement.GuidValue);
                if (param?.IsReadOnly == false)
                {
                    param.Set(0);
                }
            }
        }
    }
}