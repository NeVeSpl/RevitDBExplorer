using System.Reflection;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.Domain.RevitDatabaseScripting;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Accessors
{
    internal sealed class MemberAccessorForStatic : IAccessorWithCodeGeneration
    {
        private readonly MethodInfo getMethod;

        public string UniqueId { get; set; }
        public Invocation DefaultInvocation { get; } = new Invocation();


        public MemberAccessorForStatic(MethodInfo getMethod)
        {
            this.getMethod = getMethod;
        }


        public IValueViewModel CreatePresenter(SnoopableContext context, object @object)
        {
            return EmptyPresenter.Instance;
        }
        public string GenerateInvocationForScript(TemplateInputsKind inputsKind)
        {
            return new MemberInvocationTemplateSelector().Evaluate(getMethod, "", inputsKind);
        }
    }
}