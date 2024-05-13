using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors;
using RevitDBExplorer.Domain.DataModel.MembersTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Members;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class JoinGeometryUtils_Templates : IHaveMemberTemplates
    { 
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static JoinGeometryUtils_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
                SnoopableMemberTemplate<Element>.Create((doc, target) => JoinGeometryUtils.GetJoinedElements(doc, target)),
                SnoopableMemberTemplate<Element>.Create(typeof(JoinGeometryUtils), nameof(JoinGeometryUtils.IsCuttingElementInJoin), new JoinGeometryUtils_IsCuttingElementInJoin())
            };
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;         
        }
    }
}