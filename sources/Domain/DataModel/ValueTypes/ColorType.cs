using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class ColorType : Base.ValueType<Autodesk.Revit.DB.Color>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new ColorType();
        }


        protected override bool CanBeSnoooped(Autodesk.Revit.DB.Color color) => false;
        protected override string ToLabel(Autodesk.Revit.DB.Color color)
        {
            return color.IsValid
            ? $"R: {color.Red}; G: {color.Green}; B: {color.Blue}"
            : "invalid color value";
        }
    }
}