using System;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.RevitDatabaseScripting.Dummies;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class MemberInvocation_SelectTemplate
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
            return CodeToStringRepo.GetText(nameof(MemberInvocation_SelectTemplate), true);
        }


        [CodeToString(nameof(MemberInvocation_SelectTemplate))]
        IEnumerable<object> Select(Document document, IEnumerable<TypePlaceholder> inputs)
        {
            foreach (var item in inputs)
            {
                yield return item.MethodPlaceholder();
            }
        }
    }

    internal class MemberInvocation_Template
    {
        public string Evaluate(MethodInfo getMethod, string invocation)
        {
            invocation ??= getMethod.GenerateInvocation();

            if (getMethod.ReturnType == typeof(void))
            {
                return new MemberInvocation_UpdateTemplate().Evaluate(getMethod.DeclaringType, invocation);
            }

            return new MemberInvocation_SelectTemplate().Evaluate(getMethod.DeclaringType, invocation);
        }
    }
}
