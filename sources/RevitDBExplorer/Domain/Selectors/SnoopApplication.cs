using System.Collections.Generic;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopApplication : ISelector
    {
        public InfoAboutSource Info { get; } = new("Application");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            yield return new SnoopableObject(null, app.Application);
        }
    }
}