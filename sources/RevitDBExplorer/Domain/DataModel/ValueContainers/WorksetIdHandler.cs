using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class WorksetIdHandler : TypeHandler<WorksetId>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, WorksetId value)
        {
            return true;
        }

        protected override string ToLabel(SnoopableContext context, WorksetId value)
        {
            return Labeler.GetLabelForObjectWithId("WorksetId", value.IntegerValue);
        }
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, WorksetId value)
        {
            var workset = context.Document.GetWorksetTable().GetWorkset(value);
            yield return new SnoopableObject(context.Document, workset);
        }
    }
}
