using System.Collections.Generic;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion
{
    internal class FavoritesManager
    {
        internal static IEnumerable<IAutocompleteItem> GetFavorites()
        {
            yield return new AutocompleteItem("t: ParameterElement", "", "User defined paramters", AutocompleteItemGroups.Favourite);
        }
    }
}