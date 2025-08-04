using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopUIView : ISelector
    {
        public InfoAboutSource Info { get; } = new("UIView");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var views = app?.ActiveUIDocument?.GetOpenUIViews();
            var view = views?.Where(x => x.ViewId == app.ActiveUIDocument.ActiveGraphicalView?.Id).FirstOrDefault();

            if (view == null) yield break;

            yield return new SnoopableObject(null, view);
        }
    }
}