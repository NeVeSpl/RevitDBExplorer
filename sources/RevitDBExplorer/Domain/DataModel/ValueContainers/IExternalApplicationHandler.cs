using System.IO;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class IExternalApplicationHandler : TypeHandler<IExternalApplication>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, IExternalApplication value) => false;

        protected override string ToLabel(SnoopableContext context, IExternalApplication app)
        {
            var appType = app.GetType();
            var name = appType.Name;
            var dllName = Path.GetFileName(appType.Assembly.Location);
            return $"{name}: {dllName}";
        }
    }
}
