using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates.Accessors
{
    internal class HostObjectUtils_GetSideFaces : MemberAccessorTyped<HostObject>
    {
        public override ReadResult Read(SnoopableContext context, HostObject element)
        {            
            return new ReadResult()
            {
                CanBeSnooped = true,
                Label = $"[Elements]",
                AccessorName = nameof(HostObjectUtils_GetSideFaces)
            };

        }

        public override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, HostObject element, IValueContainer state)
        {
            var interior = HostObjectUtils.GetSideFaces(element, ShellLayerType.Interior);
            var exterior = HostObjectUtils.GetSideFaces(element, ShellLayerType.Exterior);

            yield return new SnoopableObject(context.Document, ShellLayerType.Interior, interior.Select(x => new SnoopableObject(context.Document, x)));
            yield return new SnoopableObject(context.Document, ShellLayerType.Exterior, exterior.Select(x => new SnoopableObject(context.Document, x)));
        }
    }
}
