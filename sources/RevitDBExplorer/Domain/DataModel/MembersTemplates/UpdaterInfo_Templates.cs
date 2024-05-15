using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class UpdaterInfo_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<UpdaterInfo>.WithCustomAC(typeof(IUpdater), "GetUpdaterId", new MemberAccessorByFunc<UpdaterInfo, UpdaterId>((doc, target) => UpdaterInfoWizard.Get(target.ApplicationName, target.UpdaterName))),             
        ];        
    }
}