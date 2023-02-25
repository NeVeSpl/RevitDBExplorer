// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals
{
    public interface IAutocompleteItem
    {
        string TextToInsert { get; }
        string Label { get; }
        string Description { get; }
        bool IsChosenOne { get; set; }
        string GroupName { get; }
    }
}