using System.Collections.Generic;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopActiveView : ISelector
    {
        public InfoAboutSource Info { get; } = new("ActiveView");        


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var view = app?.ActiveUIDocument?.Document?.ActiveView;

            if (view == null) yield break;

            yield return new SnoopableObject(view.Document, view);
        }
    }
}