using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal interface ITypeHandler : IToLabel, ICanBeSnooped, ISnoop, IHaveVisualization
    {
        Type Type { get; }
    }
    internal interface ITypeHandler<in T> : IToLabel<T>, ICanBeSnooped<T>, ISnoop<T>, IHaveVisualization<T>
    {
        Type Type { get; }
        string GetTypeHandlerName(T value);
    }
}