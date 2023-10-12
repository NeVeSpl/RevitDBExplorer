using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Accessors
{
    internal readonly ref struct ReadResult
    {
        public string Label { get; init; }
        public string AccessorName { get; init; }
        public bool CanBeSnooped { get; init; }
        public IValueContainer State { get; init; }


        public ReadResult(string value, string accessorName, bool canBeSnooped, IValueContainer state = null)
        {
            Label = value;
            AccessorName = accessorName;
            CanBeSnooped = canBeSnooped;
            State = state;
        }
    }
}