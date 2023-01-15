using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure.StructuralSections;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class Element_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();

        static Element_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
                SnoopableMemberTemplate<Element>.Create((doc, target) => doc.ActiveView.GetElementOverrides(target.Id), kind: MemberKind.AsArgument),
                SnoopableMemberTemplate<Element>.Create((doc, target) => doc.GetWorksetId(target.Id), kind: MemberKind.AsArgument),
                SnoopableMemberTemplate<FamilyInstance>.Create((doc, target) => StructuralSectionUtils.GetStructuralSection(doc, target.Id), kind: MemberKind.StaticMethod),
            };
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
