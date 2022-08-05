using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class UpdaterInfoContainer : Base.ValueContainer<UpdaterInfo>
    {
        protected override bool CanBeSnoooped(UpdaterInfo value) => false;

        protected override string ToLabel(UpdaterInfo updater)
        {
            return $"{updater.ApplicationName}: {updater.UpdaterName}";
        }
    }
}