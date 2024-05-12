using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class PrintManager_SubmitPrint : MemberAccessorByType<PrintManager>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (PrintManager x) => x.SubmitPrint(), (PrintManager x, View v) => x.SubmitPrint(v) ];


        public override ReadResult Read(SnoopableContext context, PrintManager printManager) => ReadResult.Forbidden;
    }
}