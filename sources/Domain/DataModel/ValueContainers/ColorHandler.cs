using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ColorHandler : TypeHandler<Autodesk.Revit.DB.Color>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Autodesk.Revit.DB.Color color) => false;
        protected override string ToLabel(SnoopableContext context, Autodesk.Revit.DB.Color color)
        {
            return color.IsValid
            ? $"R: {color.Red}; G: {color.Green}; B: {color.Blue}"
            : "<invalid color value>";
        }
    }
}