using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.UI.Events;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class DialogBoxShowingEventArgs_OverrideResult : MemberAccessorByType<DialogBoxShowingEventArgs>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (DialogBoxShowingEventArgs x) => x.OverrideResult(0) ];


        protected override ReadResult Read(SnoopableContext context, DialogBoxShowingEventArgs value) => ReadResult.Forbidden;

    }
}