using System.Collections.Generic;
using System.Linq;
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
            var view = document?.ActiveView;
            if (view == null ) return null;

            var elements = app.ActiveUIDocument.GetElementsOnTheScreen().Select(x => new SnoopableObject(document, x)).ToArray();
            
            return elements;
        }        
    }  
}