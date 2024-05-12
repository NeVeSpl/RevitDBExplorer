using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class View_Duplicate : MemberAccessorByType<View>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (View x) => x.Duplicate(ViewDuplicateOption.Duplicate)];


        public override ReadResult Read(SnoopableContext context, View view) => ReadResult.Forbidden;  
    }
}