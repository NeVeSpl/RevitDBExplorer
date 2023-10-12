using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Parameters;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableParameter : SnoopableItem
    {
        private readonly Parameter parameter;
    

        public string Name => parameter.Definition.Name;
        

        public SnoopableParameter(SnoopableObject parent, Parameter parameter) : base(parent, new ParameterAccessor(parameter))
        {
            this.parameter = parameter;            
        }       


       
        public override SourceOfObjects Snoop()
        {
            return new SourceOfObjects(this);
        }
    }
}