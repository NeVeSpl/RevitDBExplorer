using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.RevitAddIns;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Streams;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class IExternalApplication_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static IExternalApplication_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
               SnoopableMemberTemplate<IExternalApplication>.Create(typeof(AddInManifestUtility), "GetRevitAddInManifest", new MemberAccessorByFunc<IExternalApplication, RevitAddInManifest>((doc, target) => AddInManifestWizard.Get(target.GetType().Assembly.Location))),              
            };
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
