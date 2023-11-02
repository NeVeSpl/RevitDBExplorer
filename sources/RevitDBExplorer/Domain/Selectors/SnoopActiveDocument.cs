using System.Collections.Generic;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopActiveDocument : ISelector
    {
        public InfoAboutSource Info { get; } = new("Document");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) yield break;

            yield return new SnoopableObject(document, document);
        }
    }
}