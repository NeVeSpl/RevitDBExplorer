using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams.Base
{
    internal class JoinGeometryUtilsStream : BaseStream
    { 
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForElement = Enumerable.Empty<ISnoopableMemberTemplate>();


        static JoinGeometryUtilsStream()
        {
            ForElement = new ISnoopableMemberTemplate[]
            {
                SnoopableMemberTemplate<Element>.Create((doc, target) => JoinGeometryUtils.GetJoinedElements(doc, target)),
                SnoopableMemberTemplate<Element>.Create(typeof(JoinGeometryUtils), nameof(JoinGeometryUtils.IsCuttingElementInJoin), new JoinGeometryUtils_IsCuttingElementInJoin())
            };
        }


        public JoinGeometryUtilsStream()
        {
            RegisterTemplates(typeof(Element), ForElement);           
        }
    }
}