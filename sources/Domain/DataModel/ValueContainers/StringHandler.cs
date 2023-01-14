using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class StringHandler : TypeHandler<string>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, string stringValue) => false;
        protected override string ToLabel(SnoopableContext context, string stringValue)
        {            
            return stringValue;
        }
    }
}