using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NSourceGenerators;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class Query_SelectTemplate
    {
        public string Evaluate(string query)
        {
            var template = GetTemplate();
            var result = template.ReplaceMany(new[]
            {
                ("    return null;", query),               
            });

            return result;
        }


        public string GetTemplate()
        {
            return CodeToStringRepo.GetText(nameof(Query_SelectTemplate), true);
        }


        [CodeToString(nameof(Query_SelectTemplate))]
        IEnumerable<object> Select(Document document, UIApplication uia)
        {
            return null;
        }
    }
}
