using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.RevitDatabaseScripting.Dummies;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class MemberInvocation_UpdateTemplate
    {
        public string Evaluate(System.Type type, string invocation)
        {
            var template = GetTemplate();
            var result = template.ReplaceMany(new[]
            { 
                ("TypePlaceholder", type.GetCSharpName()),
                ("MethodPlaceholder()", invocation)
            });
            
            return result;
        }


        public string GetTemplate()
        {
            return CodeToStringRepo.GetText(nameof(MemberInvocation_UpdateTemplate), true);
        }


        [CodeToString(nameof(MemberInvocation_UpdateTemplate))]
        void Update(Document document, IEnumerable<TypePlaceholder> inputs)
        {
            foreach (var item in inputs)
            {
                item.MethodPlaceholder();
            }
        }
    }
}
