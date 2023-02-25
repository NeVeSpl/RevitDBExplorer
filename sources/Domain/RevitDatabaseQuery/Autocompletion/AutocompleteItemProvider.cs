using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion
{
    internal class AutocompleteItemProvider : IAutocompleteItemProvider
    {
        private readonly static List<IAutocompleteItem> CommandAutocompleteItems;

        static AutocompleteItemProvider()
        {
            CommandAutocompleteItems = new List<IAutocompleteItem>(CommandParser.Definitions.Select(x => x.GetCommandAutocompleteItem()));
        }




        public (IEnumerable<IAutocompleteItem>, int) GetAutocompleteItems(string fullText, int caretPosition)
        {
            string textOnTheLeftSideOfCaret = fullText.Substring(0, caretPosition);           

            var items = new List<IAutocompleteItem>();
            int prefixLength = 0;

            if (string.IsNullOrEmpty(fullText))
            {
                LoadDocumentSpecificData();
            }

            if (string.IsNullOrEmpty(fullText) || IsSeparator(textOnTheLeftSideOfCaret.Trim().LastOrDefault()))
            {
                items.AddRange(CommandAutocompleteItems);
                items.AddRange(FavoritesManager.GetFavorites());
            }
            else
            {
                var lastCmd = QueryParser.SplitIntoCmdStrings(textOnTheLeftSideOfCaret).LastOrDefault();
                if (!string.IsNullOrEmpty(lastCmd))
                {
                    var splittedByClassifier = lastCmd.Split(CommandParser.Separators, 2, System.StringSplitOptions.None);
                    if (splittedByClassifier.Length == 2)
                    {
                        var definition = CommandParser.GetCommandDefinitionForClassifier(splittedByClassifier[0]);
                        if (definition is IOfferArgumentAutocompletion argumentAutocompletion)
                        {
                            var prefix = splittedByClassifier[1].Trim().ToLowerInvariant();
                            //
                            prefixLength = splittedByClassifier[1].TrimStart().Length;
                            //
                            items.AddRange(argumentAutocompletion.GetAutocompleteItems(prefix));
                        }
                    }
                }
            }
            
            return (items, prefixLength);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsSeparator(char c)
        {
            foreach (var separator in QueryParser.Separators)
            {
                if (c == separator) return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsClassifierSymbol(char c)
        {
            foreach (var separator in CommandParser.Separators)
            {
                if (c == separator) return true;
            }
            return false;
        }


        private void LoadDocumentSpecificData()
        {
            ExternalExecutor.ExecuteInRevitContextAsync(x =>
            {
                CommandParser.LoadDocumentSpecificData(x.ActiveUIDocument?.Document);

            }).Forget();
        }
    }
}