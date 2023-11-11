using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopElementsOnScreen : ISelector
    {
        public InfoAboutSource Info { get; private set; } = new("elements on the screen ");

        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            if (document.ActiveView is ViewPlan viewPlan)
            {
                //var currentElevation =  viewPlan.GenLevel.Elevation;
                //var bottomElevation = -1000.0;

                //var viewRange = viewPlan.GetViewRange();
                //var depthPlaneLevelId = viewRange.GetLevelId(PlanViewPlane.ViewDepthPlane);
                //if (depthPlaneLevelId != ElementId.InvalidElementId)
                //{
                //    var depthOffset = viewRange.GetOffset(PlanViewPlane.ViewDepthPlane);
                //    var depthLevel = document.GetElement(depthPlaneLevelId) as Level;
                //    bottomElevation = depthLevel.Elevation + depthOffset;
                //}

                //var deltaBottom = currentElevation - bottomElevation;
            }

            var view = app.ActiveUIDocument.ActiveView;

            if (view.ViewDirection.IsParallelTo(XYZ.BasisX) == false && view.ViewDirection.IsParallelTo(XYZ.BasisY) == false && view.ViewDirection.IsParallelTo(XYZ.BasisZ) == false)
                return null;
            if (view.RightDirection.IsParallelTo(XYZ.BasisX) == false && view.RightDirection.IsParallelTo(XYZ.BasisY) == false && view.RightDirection.IsParallelTo(XYZ.BasisZ) == false)
                return null;

            var uiViews = app.ActiveUIDocument.GetOpenUIViews();
            var uiView = uiViews.FirstOrDefault(x => x.ViewId == document.ActiveView.Id);
            var zoomCorners = uiView.GetZoomCorners();
            var min = zoomCorners[0];
            var max = zoomCorners[1];
            
            var margin1 = document.ActiveView.ViewDirection * 54321;
            var margin2 = margin1.Negate();

            min = min + margin1.Min(margin2);
            max = max + margin1.Max(margin2);  

            var outline = new Outline(min, max);
            var collector = new FilteredElementCollector(document, document.ActiveView.Id).WherePasses(new BoundingBoxIntersectsFilter(outline));

            return collector.ToElements().Select(x => new SnoopableObject(document, x));
        }
    }  
}
