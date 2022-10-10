using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class XYZContainer : Base.ValueContainer<XYZ>, IHaveDetailInformation
    {
        protected override bool CanBeSnoooped(XYZ xyz) => false;
        protected override string ToLabel(XYZ xyz)
        {
            return $"({xyz.X}, {xyz.Y}, {xyz.Z})";
        }

        /// <summary>
        /// Retrieve xyz in mm
        /// </summary>
        public string DetailInformationText
        {
            get => $"{ToLabel(Value)}\n({Value.X.ToLengthDisplayString(Units)}, {Value.Y.ToLengthDisplayString(Units)},{Value.Z.ToLengthDisplayString(Units)})";
        }
    }
}