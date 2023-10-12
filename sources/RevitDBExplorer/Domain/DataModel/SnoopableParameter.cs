using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Parameters;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableParameter : SnoopableItem
    {
        private readonly Parameter parameter;
    

        public override string Name => parameter.Definition.Name;
        

        public SnoopableParameter(SnoopableObject parent, Parameter parameter) : base(parent, new ParameterAccessor(parameter))
        {
            this.parameter = parameter;            
        }       


       
        public override SourceOfObjects Snoop()
        {
            return new SourceOfObjects(this);
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