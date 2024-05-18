using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class RebarHostData_Templates : IHaveMemberTemplates
    {

        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [    
            MemberTemplate<Element>.Create((doc, target) =>  RebarHostData.IsValidHost(target), kind: MemberKind.StaticMethod),
            MemberTemplate<Element>.Create((doc, target) =>  RebarHostData.GetRebarHostData(target), canBeUsed: element => RebarHostData.IsValidHost(element) , kind: MemberKind.StaticMethod),
#if R2025_MIN
            MemberTemplate<Element>.Create((doc, target) =>  RebarHostData.GetRebarHostDirectNeighbors(target), canBeUsed: element => RebarHostData.IsValidHost(element) , kind: MemberKind.StaticMethod),
#endif                
        ];
    }
}