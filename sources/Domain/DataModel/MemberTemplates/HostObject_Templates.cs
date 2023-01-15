using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Accessors;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class HostObject_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static HostObject_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
               SnoopableMemberTemplate<HostObject>.Create((doc, target) => HostObjectUtils.GetTopFaces(target), kind: MemberKind.StaticMethod),
               SnoopableMemberTemplate<HostObject>.Create((doc, target) => HostObjectUtils.GetBottomFaces(target), kind: MemberKind.StaticMethod),
               SnoopableMemberTemplate<HostObject>.Create(typeof(HostObjectUtils), "GetSideFaces", new HostObjectUtils_GetSideFaces(), kind: MemberKind.StaticMethod ),           
            }; 
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
