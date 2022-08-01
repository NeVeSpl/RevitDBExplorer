using System;
using Autodesk.Revit.DB.Events;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers 
{
    internal class RevitApiEventArgsContainer : Base.ValueContainer<RevitAPIEventArgs>
    {
        protected override bool CanBeSnoooped(RevitAPIEventArgs args) => true;
        protected override string ToLabel(RevitAPIEventArgs args)
        {
            return args.GetType().Name.RemoveFromEnd("EventArgs");
        }
    }
}
