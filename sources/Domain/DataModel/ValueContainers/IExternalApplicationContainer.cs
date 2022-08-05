using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class IExternalApplicationContainer : Base.ValueContainer<IExternalApplication>
    {
        protected override bool CanBeSnoooped(IExternalApplication value) => false;

        protected override string ToLabel(IExternalApplication app)
        {
            var appType = app.GetType();
            var name = appType.Name;
            var dllName = Path.GetFileName(appType.Assembly.Location);
            return $"{name}: {dllName}";
        }
    }
}
