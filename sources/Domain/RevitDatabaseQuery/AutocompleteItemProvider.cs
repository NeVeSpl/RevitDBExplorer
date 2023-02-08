using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal class AutocompleteItemProvider : IAutocompleteItemProvider
    {
        private readonly static List<IAutocompleteItem> CommandAutocompleteItems;

        static AutocompleteItemProvider()
        {
            CommandAutocompleteItems = new List<IAutocompleteItem>(CommandParser.Definitions.Select(x => x.GetCommandAutocompleteItem()));
        }




        public IEnumerable<IAutocompleteItem> GetAutocompleteItems(string fullText, int caretPosition)
        {
            string textOnTheLeftSideOfCaret = fullText.Substring(0, caretPosition).Trim();
            //string textOnTheRightSideOfCaret = fullText.Substring(caretPosition).Trim();

            var items = new List<IAutocompleteItem>();

            if (string.IsNullOrEmpty(fullText) || IsSeparator(textOnTheLeftSideOfCaret.LastOrDefault()))
            {
                items.AddRange(CommandAutocompleteItems);
            }
            else
            {
                var lastCmd = QueryParser.SplitIntoCmdStrings(textOnTheLeftSideOfCaret).LastOrDefault();
                if (IsClassifierSymbol(lastCmd.Last()))
                {
                    var splittedByClassifier = lastCmd.Split(CommandParser.Separators, 2, System.StringSplitOptions.None);
                    if (splittedByClassifier.Length == 2)
                    {
                        var definition = CommandParser.GetCommandDefinitionForClassifier(splittedByClassifier[0]);
                        if (definition is IOfferArgumentAutocompletion argumentAutocompletion)
                        {
                            items.AddRange(argumentAutocompletion.GetAutocompleteItems(splittedByClassifier[1]));
                        }
                    }
                }
            }            

            return items;
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
    }
}