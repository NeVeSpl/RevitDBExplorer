using System.Collections.Generic;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopLoadedApplications : ISelector
    {
        public InfoAboutSource Info { get; } = new("UIApplication.LoadedApplications");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            AddInManifestWizard.WingardiumLeviosa(app);
            foreach (var externalApp in app.LoadedApplications)
            {
                yield return new SnoopableObject(app?.ActiveUIDocument?.Document, externalApp);
            }
        }
    }
}