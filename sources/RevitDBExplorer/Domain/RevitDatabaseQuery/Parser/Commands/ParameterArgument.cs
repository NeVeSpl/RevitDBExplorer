using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class ParameterArgument : CommandArgument<ElementId>
    {
        private static readonly Dictionary<ElementId, StorageType> storageTypeCache= new();
        private static readonly Dictionary<ElementId, ForgeTypeId> dataTypeCache = new();
        public bool IsBuiltInParameter { get; }
        public BuiltInParameter BuiltInParameter { get; init; }
        public StorageType StorageType { get; private set; } = StorageType.None;
        public ForgeTypeId DataType { get; private set; } = SpecTypeId.Custom; 

        public ParameterArgument(BuiltInParameter value) : base(new ElementId(value))
        {            
            IsBuiltInParameter = true;
            BuiltInParameter = value;
            Name = $"BuiltInParameter.{value}";
            Label = LabelUtils.GetLabelFor(value);
        }
        public ParameterArgument(ElementId value, string name) : base(value)
        {
           
            IsBuiltInParameter = false;
            Name = $"new ElementId({value})";
            Label = name;
        }


        public void ResolveStorageType(Document document)
        {
            if (storageTypeCache.TryGetValue(Value, out StorageType storageType))
            {
                StorageType = storageType;
                if (dataTypeCache.TryGetValue(Value, out ForgeTypeId dataType))
                {
                    DataType = dataType;
                    return;
                }
            }

            var collector = new FilteredElementCollector(document)
                     .WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)))
                     .WherePasses(new LogicalOrFilter(new ElementParameterFilter(ParameterFilterRuleFactory.CreateHasValueParameterRule(Value)), new ElementParameterFilter(ParameterFilterRuleFactory.CreateHasNoValueParameterRule(Value))));
            var first = collector.FirstElement();

            if (IsBuiltInParameter)
            {
                StorageType = document.get_TypeOfStorage(BuiltInParameter);
                storageTypeCache[Value] = StorageType;

                var parameter = first?.get_Parameter(BuiltInParameter);
                if (parameter != null)
                {
#if R2022_MIN
                    DataType = parameter.Definition.GetDataType();
                    dataTypeCache[Value] = DataType;
#endif
                }
            }
            else
            {
                var parameterElement = document.GetElement(Value) as ParameterElement;
                if (parameterElement != null)
                {
                    var definition = parameterElement.GetDefinition();
#if R2022_MIN
                    DataType = definition.GetDataType();
                    dataTypeCache[Value] = DataType;
#endif
                    var parameter = first?.get_Parameter(definition);
                    if (parameter != null)
                    {
                        StorageType = parameter.StorageType;
                        storageTypeCache[Value] = StorageType;
                    }
                }
            }            
        }
    }
}