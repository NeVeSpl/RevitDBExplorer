using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.API;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;

namespace RevitDBExplorer
{
    internal class APIAdapter : IRDBEController
    {
        public void Snoop(object document, IEnumerable<object> elements)
        {
            var revitDocument = document as Document;
            var snoopableObjects = elements.Select(x => new SnoopableObject(revitDocument, x)).ToArray();
            var sourceOfObjects = new SourceOfObjects(snoopableObjects) { Title = "API.Snoop()" };            

            var window = new MainWindow(sourceOfObjects, Application.RevitWindowHandle);
            window.Show();
        }
    }
}