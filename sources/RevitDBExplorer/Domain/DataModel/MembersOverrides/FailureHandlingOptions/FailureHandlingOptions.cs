using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class FailureHandlingOptions_ : MemberAccessorByType<FailureHandlingOptions>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() =>
        [
            (FailureHandlingOptions x) => x.SetClearAfterRollback(false),
            (FailureHandlingOptions x) => x.SetDelayedMiniWarnings(false),
            (FailureHandlingOptions x) => x.SetFailuresPreprocessor(null),
            (FailureHandlingOptions x) => x.SetForcedModalHandling(false),
            (FailureHandlingOptions x) => x.SetTransactionFinalizer(null),
        ];


        protected override ReadResult Read(SnoopableContext context, FailureHandlingOptions options) => ReadResult.Forbidden;
    }
}