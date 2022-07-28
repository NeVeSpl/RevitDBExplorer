using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ParameterSetContainer : Base.ValueContainer<ParameterSet>
    {
        protected override bool CanBeSnoooped(ParameterSet parameterSet) => parameterSet?.IsEmpty == false;
        protected override string ToLabel(ParameterSet parameterSet)
        {
            return $"Parameters : {parameterSet.Size}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ParameterSet parameterSet)
        {          
            foreach (Parameter param in parameterSet)
            {
                yield return new SnoopableObject(document, param);
            }
        }
    }
}