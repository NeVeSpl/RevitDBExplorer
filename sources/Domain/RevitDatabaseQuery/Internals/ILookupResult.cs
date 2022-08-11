using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

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

    internal class CategoryMatch : LookupResult<BuiltInCategory>
    {
        public CategoryMatch(BuiltInCategory value, double levensteinScore) : base(value, levensteinScore)
        {
            CmdType = CmdType.Category;
            Name = $"BuiltInCategory.{value}";
            Label = LabelUtils.GetLabelFor(value);
        }
    }
    internal class TypeMatch : LookupResult<Type>
    {
        public TypeMatch(Type value, double levensteinScore) : base(value, levensteinScore)
        {
            CmdType = CmdType.Class;
            Name = $"typeof({value.Name})";
        }
    }
    internal class ElementIdMatch : LookupResult<ElementId>
    {
        public ElementIdMatch(ElementId value, double levensteinScore) : base(value, levensteinScore)
        {
            CmdType = CmdType.ElementId;
            Name = $"new ElementId({value.IntegerValue})";
        }
    }
    internal class ParameterMatch : LookupResult<ElementId>
    {
        private static readonly Dictionary<ElementId, StorageType> storageTypeForUserParameters = new();
        public bool IsBuiltInParameter { get; }
        public BuiltInParameter BuiltInParameter { get; init; }
        public StorageType StorageType { get; private set; } = StorageType.None;


        public ParameterMatch(BuiltInParameter value, double levensteinScore) : base(new ElementId(value), levensteinScore)
        {
            CmdType = CmdType.Parameter;
            IsBuiltInParameter = true;
            BuiltInParameter = value;
            Name = $"BuiltInParameter.{value}";
            Label = LabelUtils.GetLabelFor(value);
        }
        public ParameterMatch(ElementId value, double levensteinScore, string name) : base(value, levensteinScore)
        {
            CmdType = CmdType.Parameter;
            IsBuiltInParameter = false;
            Name = name;
            Label = name;
        }


        public void ResolveStorageType(Document document)
        {
            if (IsBuiltInParameter)
            {
                StorageType = document.get_TypeOfStorage(BuiltInParameter);
            }
            else
            {
                if (!storageTypeForUserParameters.TryGetValue(Value, out StorageType storage))
                {
                    var collector = new FilteredElementCollector(document)
                        .WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)))
                        .WherePasses(new LogicalOrFilter(new ElementParameterFilter(ParameterFilterRuleFactory.CreateHasValueParameterRule(Value)), new ElementParameterFilter(ParameterFilterRuleFactory.CreateHasNoValueParameterRule(Value))));
                    var first = collector.FirstElement();
                    if (first != null)
                    {
                        var parameterElement = document.GetElement(Value) as ParameterElement;
                        if (parameterElement != null)
                        {
                            var definition = parameterElement.GetDefinition();
                            var parameter = first.get_Parameter(definition);
                            if (parameter != null)
                            {
                                storage = parameter.StorageType;
                                storageTypeForUserParameters[Value] = storage;
                            }
                        }
                    }
                }
                StorageType = storage;
            }
        }
    }
    internal class StructuralTypeMatch : LookupResult<StructuralType>
    {
        public StructuralTypeMatch(StructuralType value, double levensteinScore) : base(value, levensteinScore)
        {
            CmdType = CmdType.StructuralType;
            Name = $"StructuralType.{value}";
        }
    }
}