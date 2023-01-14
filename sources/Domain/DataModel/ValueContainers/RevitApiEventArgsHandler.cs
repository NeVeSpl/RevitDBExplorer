using System;
using Autodesk.Revit.DB.Events;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers 
{
    internal class RevitApiEventArgsHandler : TypeHandler<RevitAPIEventArgs>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, RevitAPIEventArgs args) => true;
        protected override string ToLabel(SnoopableContext context, RevitAPIEventArgs args)
        {
            return args.GetType().Name.RemoveFromEnd("EventArgs");
        }
    }
}
