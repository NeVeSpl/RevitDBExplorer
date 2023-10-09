using Autodesk.Revit.DB;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableParameter : BaseViewModel
    {
        private Parameter parameter;



        public string Name => parameter.Definition.Name;
        public string Value { get; private set; }


        public SnoopableParameter(Parameter parameter)
        {
            this.parameter = parameter;
            
        }


        public void Read()
        {
            var dataType = parameter.Definition.GetDataType();
            bool isMeasurableSpec = UnitUtils.IsMeasurableSpec(dataType);
            string value = "";
            if (isMeasurableSpec)
            {
                value = parameter.AsValueString();
            }
            else
            {

                switch (parameter.StorageType)
                {
                    case StorageType.String:
                        value = parameter.AsString();
                        break;
                    case StorageType.Integer:
                        value = parameter.AsInteger().ToString();
                        break;
                    case StorageType.ElementId:
                        value = parameter.AsValueString();
                        break;
                    case StorageType.Double:
                        value += parameter.AsDouble().ToString();
                        break;

                }
            }
            Value = value;
            OnPropertyChanged(nameof(Value));
        }


    }
}