using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class RebarConstrainedHandleHandler : TypeHandler<RebarConstrainedHandle>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, RebarConstrainedHandle handle) => true;

        protected override string ToLabel(SnoopableContext context, RebarConstrainedHandle handle) => handle.GetHandleName();
    }
}