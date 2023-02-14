using Autodesk.Revit.DB;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseView
{

    internal interface ICell
    {
        void Read();
    }


    internal class Cell : BaseViewModel, ICell
    {
        private readonly Row row;
        private readonly Column column;
        private string value;


        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }


        public Cell(Row row, Column column)
        {
            this.row = row;
            this.column = column;
        }
      

        public void Read()
        {
            var parameter = row.get_Parameter(column.Definition);           

            if (parameter != null)
            {
                var dataType = parameter.Definition?.GetDataType();
                bool isMeasurableSpec = UnitUtils.IsMeasurableSpec(dataType);

                if (true)
                {
                    Value = parameter.AsValueString();
                }
                else
                {

                    switch (parameter.StorageType)
                    {
                        case StorageType.String:
                            Value = parameter.AsString();
                            break;
                        case StorageType.Integer:
                            Value = parameter.AsInteger().ToString();
                            break;
                        case StorageType.ElementId:
                            Value = parameter.AsValueString();
                            break;
                        case StorageType.Double:
                            Value += parameter.AsDouble().ToString();
                            break;

                    }
                }
            }
        }
    }
}