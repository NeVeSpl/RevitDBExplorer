using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopUpdaters : ISelector
    {
        public InfoAboutSource Info { get; } = new("UpdaterRegistry.GetRegisteredUpdaterInfos()");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            UpdaterInfoWizard.AvadaKedavra();

            return UpdaterRegistry.GetRegisteredUpdaterInfos(document).Select(x => new SnoopableObject(document, x));
        }
    }
}