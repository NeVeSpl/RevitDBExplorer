using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopExternalServices : ISelector
    {
        public InfoAboutSource Info { get; } = new("ExternalServiceRegistry.GetServices()");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            return ExternalServiceRegistry.GetServices().Select(x => new SnoopableObject(document, x));
        }
    }
}