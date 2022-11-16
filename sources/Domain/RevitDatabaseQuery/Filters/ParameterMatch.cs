using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
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
            Name = $"new ElementId({value})";
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
}