using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal class ParameterMapType : Base.ValueType<ParameterMap>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new ParameterMapType();
        }


        protected override bool CanBeSnoooped(ParameterMap parameterMap) => parameterMap?.IsEmpty == false;
        protected override string ToLabel(ParameterMap parameterMap)
        {
            return $"Parameters : {parameterMap.Size}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ParameterMap parameterMap)
        {            
            foreach (Parameter param in parameterMap)
            {
                yield return new SnoopableObject(param, document);
            }
        }
    }
}
