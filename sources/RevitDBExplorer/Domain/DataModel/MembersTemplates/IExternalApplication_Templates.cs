using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.RevitAddIns;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class IExternalApplication_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<IExternalApplication>.WithCustomAC(typeof(AddInManifestUtility), "GetRevitAddInManifest", new MemberAccessorByFunc<IExternalApplication, RevitAddInManifest>((doc, target) => AddInManifestWizard.Get(target.GetType().Assembly.Location))),              
        ];       
    }
}