using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.WPF.Controls;
using SimMetrics.Net;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch
{
    internal class DataBucket<T> where T : ICommandArgument
    {
        private readonly List<DataBucketItem<T>> items = new List<DataBucketItem<T>>();
        private readonly double fuzzySearchMatchingThreshold;


        public DataBucket(double fuzzySearchMatchingThreshold)
        {
            this.fuzzySearchMatchingThreshold = fuzzySearchMatchingThreshold;
        }


        public void Add(IAutocompleteItem item, T argument, params string[] keys)
        {
            items.Add(new DataBucketItem<T>(item, argument, keys.Select(x => Clean(x)).Where(x => string.IsNullOrWhiteSpace(x) == false).ToArray()));
        }
        public void Rebuild()
        {
            if (items.Count > 0 && items.First().autocompleteItem != null)
            {
                items.Sort((x, y) => x.autocompleteItem.Label.CompareTo(y.autocompleteItem.Label));
            }
        }
        public void Clear()
        {
            items.Clear();
        }

        public IEnumerable<IFuzzySearchResult> FuzzySearch(string text)
        {
            var sorted = FuzzySearchInternal(text).OrderByDescending(x => x.LevensteinScore);
            
            if (!sorted.Any()) yield break;

            double prevScore = sorted.First().LevensteinScore;
            foreach (var item in sorted.Take(27))
            {
                if (Math.Abs(item.LevensteinScore - prevScore) < 0.13)
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
            foreach (var item in items)
            {
                if (item.autocompleteItem != null)
                {
                    yield return item.autocompleteItem;
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

        private class FuzzySearchResult<T2> : ICommandArgument, IFuzzySearchResult where T2 : ICommandArgument
        {
            private readonly T2 argument;
            public ICommandArgument Argument => argument;
            public double LevensteinScore { get; init; }
            public string Name => argument.Name;
            public string Label => argument.Label;
            public CmdType CmdType => argument.CmdType;


            public FuzzySearchResult(T2 argument, double levensteinScore)
            {
                this.argument = argument;
                LevensteinScore = levensteinScore;
            }
        }
    }

    internal interface IFuzzySearchResult : ICommandArgument
    {
        ICommandArgument Argument { get; }
        public double LevensteinScore { get;  }

    }
}