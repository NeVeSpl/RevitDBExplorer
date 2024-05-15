using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members.Base;
using RevitDBExplorer.Domain.DataModel.Members;
using Autodesk.Revit.DB.Structure;

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
