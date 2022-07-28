using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class ParameterMapContainer : Base.ValueContainer<ParameterMap>
    {
        protected override bool CanBeSnoooped(ParameterMap parameterMap) => parameterMap?.IsEmpty == false;
        protected override string ToLabel(ParameterMap parameterMap)
        {
            return $"Parameters : {parameterMap.Size}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ParameterMap parameterMap)
        {            
            foreach (Parameter param in parameterMap)
            {
                yield return new SnoopableObject(document, param);
            }
        }
    }
}