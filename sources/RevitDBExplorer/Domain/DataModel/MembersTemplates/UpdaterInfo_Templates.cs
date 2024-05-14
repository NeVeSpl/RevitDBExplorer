using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class UpdaterInfo_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static UpdaterInfo_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
               MemberTemplate<UpdaterInfo>.Create(typeof(IUpdater), "GetUpdaterId", new MemberAccessorByFunc<UpdaterInfo, UpdaterId>((doc, target) => UpdaterInfoWizard.Get(target.ApplicationName, target.UpdaterName))),             
            };
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
