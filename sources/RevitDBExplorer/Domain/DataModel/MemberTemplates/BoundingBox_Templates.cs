﻿using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Accessors;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class BoundingBox_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static BoundingBox_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
                SnoopableMemberTemplate<BoundingBoxXYZ>.Create(typeof(BoundingBoxXYZ), "BoundingBoxIntersectsFilter", new BoundingBox_BoundingBoxIntersectsFilter(), kind: MemberKind.Extra),               
            };
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
