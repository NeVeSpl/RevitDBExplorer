using System;
using System.Collections.Generic;
using System.Linq;
using Gma.DataStructures.StringSearch;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using SimMetrics.Net;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch
{
    internal class DataBucket<T> where T : ICommandArgument
    {
        private readonly List<DataBucketItem<T>> items = new List<DataBucketItem<T>>();
        private readonly double fuzzySearchMatchingThreshold;

        private readonly List<IAutocompleteItem> autocompleteItems = new List<IAutocompleteItem>();
        private ITrie<IAutocompleteItem> autocompleteTrie = new Trie<IAutocompleteItem>();

        public DataBucket(double fuzzySearchMatchingThreshold)
        {
            this.fuzzySearchMatchingThreshold = fuzzySearchMatchingThreshold;
        }


        public void Add(IAutocompleteItem item, T argument, params string[] keys)
        {
            items.Add(new DataBucketItem<T>(item, argument, keys.Select(x => Clean(x)).Where(x => string.IsNullOrWhiteSpace(x) == false).ToArray()));
            if (item != null)
            {
                autocompleteItems.Add(item);
            }
        }
        public void Rebuild()
        {            
            autocompleteItems.Sort((x, y) => x.Label.CompareTo(y.Label));

            autocompleteTrie = new UkkonenTrie<IAutocompleteItem>();
            foreach (var item in autocompleteItems)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(item.Label))
                    {
                        autocompleteTrie.Add(item.Label.ToLowerInvariant(), item);
                    }
                    if (!string.IsNullOrWhiteSpace(item.Description))
                    {
                        autocompleteTrie.Add(item.Description.ToLowerInvariant(), item);
                    }
                }
                catch(Exception e)
                {
#if DEBUG
                    throw;
#endif
                }
            }
                            
        }
        public void Clear()
        {
            items.Clear();
            autocompleteItems.Clear();
        }

        public IEnumerable<IFuzzySearchResult> FuzzySearch(string text)
        {
            var sorted = FuzzySearchInternal(text).OrderByDescending(x => x.LevensteinScore);
            
            if (!sorted.Any()) yield break;

            double prevScore = sorted.First().LevensteinScore;
            double cutOffTreshold = prevScore == 1.0 ? 0.05 : 0.13;

            foreach (var item in sorted.Take(27))
            {
                if (Math.Abs(item.LevensteinScore - prevScore) < cutOffTreshold)
                {
                    yield return item;
                }
                else
                {
                    yield break;
                }
            }
        }

        private IEnumerable<IFuzzySearchResult> FuzzySearchInternal(string text)
        {
            var needle = Clean(text);

            foreach (var item in items)
            {
                foreach (var key in item.keys)
                {
                    var score = needle.ApproximatelyEquals(key, SimMetricType.Levenstein);
                    if (score > fuzzySearchMatchingThreshold)
                    {
                        yield return new FuzzySearchResult<T>(item.argument, score);
                    }
                }
            }
        }
        public IEnumerable<IFuzzySearchResult> CreateMatch(params T[] args)
        {
            foreach (var item in args)
            {
                yield return new FuzzySearchResult<T>(item, 1.0);
            }
        }


        public IEnumerable<IAutocompleteItem> ProvideAutoCompletion(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                foreach (var item in autocompleteItems)
                {                                        
                     yield return item;  
                }
            }
            else
            {
                autocompleteItems.ForEach(x => x.IsChosenOne = false);

                foreach (var item in autocompleteTrie.Retrieve(prefix))
                {
                    item.IsChosenOne = true;                    
                }

                foreach (var item in autocompleteItems)
                {
                    if (item.IsChosenOne)
                    {
                        yield return item;
                    }
                }
            }
        }



        private string Clean(string text)
        {
            return text?.RemoveWhitespace()?.ToLower();
        }

        

        private class DataBucketItem<T1>
        {
            public IAutocompleteItem autocompleteItem;
            public T1 argument;
            public IList<string> keys;


            public DataBucketItem(IAutocompleteItem autocompleteItem, T1 argument, IList<string> keys)
            {
                this.autocompleteItem = autocompleteItem;
                this.argument = argument;
                this.keys = keys;
            }
        }

        private class FuzzySearchResult<T2> : IFuzzySearchResult where T2 : ICommandArgument
        {
            private readonly T2 argument;
            public ICommandArgument Argument => argument;
            public double LevensteinScore { get; init; }         
         


            public FuzzySearchResult(T2 argument, double levensteinScore)
            {
                this.argument = argument;
                LevensteinScore = levensteinScore;
            }
        }
    }

    internal interface IFuzzySearchResult
    {
        ICommandArgument Argument { get; }
        public double LevensteinScore { get;  }

    }
}