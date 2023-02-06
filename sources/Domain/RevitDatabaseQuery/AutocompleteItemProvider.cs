using System.Collections.ObjectModel;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal class AutocompleteItemProvider : IAutocompleteItemProvider
    {
        public ObservableCollection<IAutocompleteItem> GetAutocompleteItems(string fullText, string textOnTheLeftSideOfCaret)
        {
            var items = new ObservableCollection<IAutocompleteItem>();

            if (string.IsNullOrEmpty(fullText) ||
                textOnTheLeftSideOfCaret.EndsWith(";") || 
                textOnTheLeftSideOfCaret.EndsWith("; ") || 
                textOnTheLeftSideOfCaret.EndsWith(",") || 
                textOnTheLeftSideOfCaret.EndsWith(", ")
                )
            {
                foreach (var factory in CommandParser.Factories)
                {                   
                    items.Add(factory.GetAutocompleteItem());
                }
            }
            else
            {

            }
            

            return items;
        }
    }
}