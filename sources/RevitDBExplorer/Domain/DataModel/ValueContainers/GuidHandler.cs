using System;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class GuidHandler : TypeHandler<Guid>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Guid guid) => false;
        protected override string ToLabel(SnoopableContext context, Guid guid)
        {
            return guid.ToString();
        }
    }
}