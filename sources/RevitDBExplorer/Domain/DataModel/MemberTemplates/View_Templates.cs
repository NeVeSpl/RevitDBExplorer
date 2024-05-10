using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class View_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static View_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {       
               SnoopableMemberTemplate<View>.Create((document, target) => SpatialFieldManager.GetSpatialFieldManager(target), kind: MemberKind.StaticMethod),
               SnoopableMemberTemplate<ViewSchedule>.Create((document, target) => TableView.GetAvailableParameters(document, target.Definition.CategoryId), kind: MemberKind.StaticMethod),
            }; 
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
