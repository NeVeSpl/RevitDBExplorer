using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class ParameterMapHandler : TypeHandler<ParameterMap>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, ParameterMap parameterMap) => parameterMap?.IsEmpty == false;
        protected override string ToLabel(SnoopableContext context, ParameterMap parameterMap)
        {
            return $"Parameters : {parameterMap.Size}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, ParameterMap parameterMap)
        {            
            foreach (Parameter param in parameterMap)
            {
                yield return new SnoopableObject(context.Document, param);
            }
        }
    }
}