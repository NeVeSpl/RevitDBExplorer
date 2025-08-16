using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class ModelPath_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
#if R2022_MIN
           
#endif
            //MemberTemplate<ModelPath>.Create((doc, target) => ModelPathUtils.CloudRegionEMEA),
            //MemberTemplate<ModelPath>.Create((doc, target) => ModelPathUtils.CloudRegionUS),
            MemberTemplate<ModelPath>.Create((doc, target) => ModelPathUtils.ConvertModelPathToUserVisiblePath(target)),          
        ];
    }
}
