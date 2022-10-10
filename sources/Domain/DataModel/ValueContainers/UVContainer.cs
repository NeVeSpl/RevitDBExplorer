using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class UVContainer : Base.ValueContainer<UV>, IHaveDetailInformation
    {
        protected override bool CanBeSnoooped(UV uv) => false;
        protected override string ToLabel(UV uv)
        {
            return $"({uv.U}, {uv.V})";
        }

        /// <summary>
        /// Retrieve uv in mm
        /// </summary>
        public string DetailInformationText
        {
            get => $"{ToLabel(Value)}\n({Value.U.ToLengthDisplayString(Units)}, {Value.V.ToLengthDisplayString(Units)})";
        }
    }
}