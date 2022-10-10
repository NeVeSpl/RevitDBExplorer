// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class DoubleContainer : Base.ValueContainer<double>, IHaveDetailInformation
    {
        protected override bool CanBeSnoooped(double doubleValue) => false;
        protected override string ToLabel(double doubleValue)
        {
            return doubleValue.ToString();
        }

        public string DetailInformationText
        {
            get => Value.ToLengthDisplayString(Units);
        }
    }
}