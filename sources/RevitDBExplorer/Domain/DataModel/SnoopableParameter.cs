using System;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Parameters;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableParameter : SnoopableItem
    {
        private readonly Parameter parameter;


        public override string Name => parameter.Definition.Name;
        public ParameterOrgin Orgin { get; }
        public override bool CanGenerateCode => true;


        public SnoopableParameter(SnoopableObject parent, Parameter parameter) : base(parent, new ParameterAccessor(parameter))
        {
            this.parameter = parameter;
            this.Orgin = parameter.GetOrgin();
        }


        public override SourceOfObjects Snoop()
        {
            var title = $"{parent.Name}->{this.Name}";
            return new SourceOfObjects(this) { Info = new InfoAboutSource(title) };
        }

        public SourceOfObjects SnoopParameter()
        {
            var snoopableObject = new SnoopableObject(parent.Context.Document, parameter);
            var title = $"{parent.Name}: {snoopableObject.Name}";
            var source = new SourceOfObjects(new[] { snoopableObject }) { Info = new InfoAboutSource(title) };
            return source;
        }
        public string GenerateQueryForForParameterValue()
        {
            string name = Name;
            if ((parameter.Definition is InternalDefinition internalDefinition) && (internalDefinition.BuiltInParameter != BuiltInParameter.INVALID))
            {
                name = Enum.GetName(typeof(BuiltInParameter), internalDefinition.BuiltInParameter);
            }
            string value = "";

            if ((ValueViewModel is DefaultPresenter presenter) && (presenter.ValueContainer != null))
            {
                switch (parameter.StorageType)
                {
                    case StorageType.None:
                        break;
                    case StorageType.Integer:
                        var vci = presenter.ValueContainer as ValueContainer<int>;
                        value = $"= {vci.Value}";
                        break;
                    case StorageType.Double:
                        var vcd = presenter.ValueContainer as ValueContainer<double>;


                        var units = Application.UIApplication?.ActiveUIDocument?.Document?.GetUnits();
#if R2022_MIN
                        if (units != null)
                        {
                            var options = new FormatValueOptions { AppendUnitSymbol = true };
                            var dataType = parameter.Definition.GetDataType();
                            var copy = new FormatOptions(units.GetFormatOptions(dataType));
                            if (Units.IsModifiableSpec(dataType))
                            {
                                if (copy.Accuracy > 0.000001)
                                {
                                    copy.Accuracy = 0.000001;
                                }
                                options.SetFormatOptions(copy);
                            }
                            var formatted = UnitFormatUtils.Format(units, dataType, vcd.Value, false, options);
                            value = $"= {formatted}";

                        }

                        else
#endif
                        {
                            value = $"= {vcd.Value}";
                        }
                        break;
                    case StorageType.String:
                        var vcs = presenter.ValueContainer as ValueContainer<string>;
                        value = $"= {vcs.Value}";
                        break;
                    case StorageType.ElementId:
                        var vce = presenter.ValueContainer as ValueContainer<ElementId>;
                        value = $"= {vce.Value}";
                        break;
                }
            }

            return $"p: {name} {value}";
        }

        
        public override int CompareTo(SnoopableItem other)
        {
            if (other is SnoopableParameter snoopableParameter)
            {
                return parameter.Id.Compare(snoopableParameter.parameter.Id);
            }
            return -1;
        }
        public override bool Equals(SnoopableItem other)
        {
            if (other is SnoopableParameter snoopableParameter)
            {
                return parameter.Id.Equals(snoopableParameter.parameter.Id);
            }
            return false;
        }
    }
}