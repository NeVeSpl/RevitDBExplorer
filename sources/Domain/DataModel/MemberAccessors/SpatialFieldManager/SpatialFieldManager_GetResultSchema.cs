using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors.SpatialFieldManager
{
    internal class SpatialFieldManager_GetResultSchema : MemberAccessorByType<Autodesk.Revit.DB.Analysis.SpatialFieldManager>, ICanCreateMemberAccessor
    {
        public IEnumerable<LambdaExpression> GetHandledMembers() { yield return (Autodesk.Revit.DB.Analysis.SpatialFieldManager x) => x.GetResultSchema(7); }

        protected override bool CanBeSnoooped(Document document, Autodesk.Revit.DB.Analysis.SpatialFieldManager value) => value.GetRegisteredResults().Count > 0;

        protected override string GetLabel(Document document, Autodesk.Revit.DB.Analysis.SpatialFieldManager value) => Labeler.GetLabelForCollection(nameof(AnalysisResultSchema), value.GetRegisteredResults().Count);

        protected override IEnumerable<SnoopableObject> Snooop(Document document, Autodesk.Revit.DB.Analysis.SpatialFieldManager value)
        {
            foreach (var id in value.GetRegisteredResults())
            {
                var resultSchema = value.GetResultSchema(id);
                yield return new SnoopableObject(document, resultSchema);
            }
        }
    }
}
