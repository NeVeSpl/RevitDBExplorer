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
        

        public SnoopableParameter(SnoopableObject parent, Parameter parameter) : base(parent, new ParameterAccessor(parameter))
        {
            this.parameter = parameter;
            this.Orgin = parameter.GetOrgin();
        }       

       
        public override SourceOfObjects Snoop()
        {
            return new SourceOfObjects(this);
        }

        public SnoopableObject SnoopParameter()
        {
            return new SnoopableObject(parent.Context.Document, parameter);
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
#if R2022b
                        if (units != null)
                        {
                            
                            var formatted = UnitFormatUtils.Format(units, parameter.Definition.GetDataType(), vcd.Value, false, new FormatValueOptions { AppendUnitSymbol = true });
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


        #region IComparable & IEquatable
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
        #endregion
    }
}