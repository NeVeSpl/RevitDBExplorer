using System;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class TypeHandler : TypeHandler<Type>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Type value) => false;

        protected override string ToLabel(SnoopableContext context, Type value) => $"{value.Namespace}.{value.GetCSharpName()}";
    }
}