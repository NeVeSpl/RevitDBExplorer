using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class EdgeHandler : TypeHandler<Edge>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Edge edge) => edge is not null;
        protected override string ToLabel(SnoopableContext context, Edge edge) => edge.GetType()?.GetCSharpName();
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, Edge edge)
        {
            yield return new SnoopableObject(context.Document, edge);
        }


    }
}
