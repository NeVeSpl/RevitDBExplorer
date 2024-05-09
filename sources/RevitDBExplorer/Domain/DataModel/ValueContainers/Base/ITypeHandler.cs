using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal interface ITypeHandler : IHaveLabel, ISnoop, IHaveVisualization
    {
        Type Type { get; }
    }
    internal interface ITypeHandler<in T> : IHaveLabel<T>, ISnoop<T>, IHaveVisualization<T>
    {
        Type Type { get; }
        string GetTypeHandlerName(T value);
    }
}