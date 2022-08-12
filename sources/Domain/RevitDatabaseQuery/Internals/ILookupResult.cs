// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal interface ILookupResult
    {
        string Name { get;  }
        string Label { get; }
        double LevensteinScore { get;  }
        public CmdType CmdType { get; init; }
    }

    internal abstract class LookupResult<T> : ILookupResult
    {
        public T Value { get; init; }
        public string Name { get; init; }
        public string Label { get; init; }
        public double LevensteinScore { get; init; }
        public CmdType CmdType { get; init; }        


        public LookupResult(T value, double levensteinScore)
        {
            Value = value;
            LevensteinScore = levensteinScore;
        }
    }
}