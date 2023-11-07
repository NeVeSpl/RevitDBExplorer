// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Accessors
{
    internal interface IAccessorWithCodeGeneration : IAccessor
    {
        string GenerateInvocationForScript();
    }
}