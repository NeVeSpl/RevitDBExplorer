using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class GuidContainer : Base.ValueContainer<Guid>
    {
        protected override bool CanBeSnoooped(Guid guid) => false;
        protected override string ToLabel(Guid guid)
        {
            return guid.ToString();
        }
    }
}