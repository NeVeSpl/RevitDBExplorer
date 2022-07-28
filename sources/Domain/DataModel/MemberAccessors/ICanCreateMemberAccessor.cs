using System.Collections.Generic;
using System.Linq.Expressions;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface ICanCreateMemberAccessor
    {        
        IEnumerable<LambdaExpression> GetHandledMembers(); 
    }
}