using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class UIDocument_Close : MemberAccessorByType<UIDocument>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (UIDocument x) => x.SaveAndClose() ];


        protected override ReadResult Read(SnoopableContext context, UIDocument value) => ReadResult.Forbidden;
    }
}