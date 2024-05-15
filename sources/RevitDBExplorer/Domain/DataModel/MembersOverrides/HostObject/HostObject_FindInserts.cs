using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class HostObject_FindInserts : MemberAccessorByFunc<HostObject, IList<ElementId>>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (HostObject x) => x.FindInserts(true, true, true, true) ];


        public HostObject_FindInserts() : base((document, hostObject) => hostObject.FindInserts(true, true, true, true))
        {
            DefaultInvocation.Syntax = "item.FindInserts(true, true, true, true)";
        }
    }
}