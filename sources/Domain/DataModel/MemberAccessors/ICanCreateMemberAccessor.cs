using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface ICanCreateMemberAccessor
    {
        IMemberAccessor Create();
        IEnumerable<string> GetHandledMembers(); 
    }
}