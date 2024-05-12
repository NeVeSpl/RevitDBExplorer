using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class ForgeTypeId_Clear : MemberAccessorByType<ForgeTypeId>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (ForgeTypeId x) => x.Clear() ];    

              
        public override ReadResult Read(SnoopableContext context, ForgeTypeId forgeTypeId) => ReadResult.Forbidden;
    }
}