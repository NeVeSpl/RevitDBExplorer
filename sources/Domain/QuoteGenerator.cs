using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class QuoteGenerator
    {
        private static readonly Random random = new();

        private static readonly string[] quotes = new[] 
        {
            "'cannot be done' - Colin",
            "'I wouldn't do that if I were you' - Anthony",
            "'not this time' - Benedict"
        };

        public static string Deny()
        {
            var luckyNumber = random.Next(quotes.Length);
            return quotes[luckyNumber];
        }
    }
}