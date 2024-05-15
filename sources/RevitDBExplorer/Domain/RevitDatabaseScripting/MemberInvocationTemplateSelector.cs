using System;
using System.Reflection;
using NSourceGenerators;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    public enum TemplateCmdKind { Select, Update };
    public enum TemplateInputsKind { Single, Multiple };

    internal class MemberInvocationTemplateSelector
    {
        public string Evaluate(MethodInfo getMethod, string invocation, TemplateInputsKind inputsKind)
        {
            if (string.IsNullOrEmpty(invocation))
            {
                var prefix = getMethod.IsStatic == false ? "item." : $"{getMethod.DeclaringType.Name}.";
                invocation = prefix + getMethod.GenerateInvocation();
            }
            var cmdkind = getMethod.ReturnType == typeof(void) ? TemplateCmdKind.Update : TemplateCmdKind.Select;

            return Evaluate(getMethod.DeclaringType, invocation, cmdkind, inputsKind);
        }
        public string Evaluate(Type type, string invocation, TemplateCmdKind cmdkind, TemplateInputsKind inputsKind)
        {
            var template = GetTemplateType(cmdkind, inputsKind);

            return EvaluateInternal(template, type, invocation);
        }
        private Type GetTemplateType(TemplateCmdKind cmdKind, TemplateInputsKind inputsKind)
        {
            if ((inputsKind == TemplateInputsKind.Single) && (cmdKind == TemplateCmdKind.Select))
            {
                return typeof(MemberInvocation_SelectSingle_Template);
            }
            if ((inputsKind == TemplateInputsKind.Single) && (cmdKind == TemplateCmdKind.Update))
            {
                return typeof(MemberInvocation_UpdateSingle_Template);
            }
            if ((inputsKind == TemplateInputsKind.Multiple) && (cmdKind == TemplateCmdKind.Select))
            {
                return typeof(MemberInvocation_SelectMultiple_Template);
            }
            if ((inputsKind == TemplateInputsKind.Multiple) && (cmdKind == TemplateCmdKind.Update))
            {
                return typeof(MemberInvocation_UpdateMultiple_Template);
            }

            throw new NotImplementedException();
        }

        

        private string EvaluateInternal(System.Type templateType, System.Type type, string invocation)
        {
            var template = CodeToStringRepo.GetText(templateType.Name, true);
            var result = template.ReplaceMany(new[]
            {
                ("TypePlaceholder", type.GetCSharpName()),
                ("item.MethodPlaceholder()", invocation)
            });

            return result;
        }
    }
}