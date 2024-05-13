using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB.Analysis;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides.SpatialFieldManager
{
    internal class SpatialFieldManager_GetResultSchema : MemberAccessorByType<Autodesk.Revit.DB.Analysis.SpatialFieldManager>, ICanCreateMemberAccessor
    {
        public IEnumerable<LambdaExpression> GetHandledMembers() => [ (Autodesk.Revit.DB.Analysis.SpatialFieldManager x) => x.GetResultSchema(7) ];


        protected override ReadResult Read(SnoopableContext context, Autodesk.Revit.DB.Analysis.SpatialFieldManager spatialFieldManager) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(AnalysisResultSchema), spatialFieldManager.GetRegisteredResults().Count),
            CanBeSnooped = spatialFieldManager.GetRegisteredResults().Count > 0
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Autodesk.Revit.DB.Analysis.SpatialFieldManager value)
        {
            foreach (var id in value.GetRegisteredResults())
            {
                var resultSchema = value.GetResultSchema(id);
                yield return new SnoopableObject(context.Document, resultSchema);
            }
        }
    }
}