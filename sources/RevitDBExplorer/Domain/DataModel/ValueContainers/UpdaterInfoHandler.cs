using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class UpdaterInfoHandler : TypeHandler<UpdaterInfo>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, UpdaterInfo value) => false;

        protected override string ToLabel(SnoopableContext context, UpdaterInfo updater)
        {
            return $"{updater.ApplicationName}: {updater.UpdaterName}";
        }
    }
}