using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ParameterSetHandler : TypeHandler<ParameterSet>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, ParameterSet parameterSet) => parameterSet?.IsEmpty == false;
        protected override string ToLabel(SnoopableContext context, ParameterSet parameterSet)
        {
            return $"Parameters : {parameterSet.Size}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, ParameterSet parameterSet)
        {          
            foreach (Parameter param in parameterSet)
            {
                yield return new SnoopableObject(context.Document, param);
            }
        }
    }
}