// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals
{
    public class AutocompleteItem : IAutocompleteItem
    {
        public string Label { get; init; }
        public string TextToInsert { get; init; }
        public string Description { get; init; }
        public bool IsChosenOne { get; set; }
        public string GroupName { get; init; }

        public AutocompleteItem(string textToInsert, string label, string description, string group = null)
        {
            Label = label;
            TextToInsert = textToInsert;
            Description = description;
            GroupName = group;
        }
    }
}