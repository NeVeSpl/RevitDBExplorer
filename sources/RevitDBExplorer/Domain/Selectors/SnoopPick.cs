using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopPick : ISelector
    {
        private readonly ObjectType objectType;

        public SnoopPick(ObjectType objectType)
        {
            this.objectType = objectType;
        }


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var uiDocument = app?.ActiveUIDocument;

            if (uiDocument == null || uiDocument.Document == null) yield break;

            Reference reference;
            try
            {
                reference = uiDocument.Selection.PickObject(objectType);
            }
            catch
            {
                // User can cancel picking
                yield break;
            }
            var geoObject = uiDocument.Document.GetElement(reference).GetGeometryObjectFromReference(reference);
            yield return new SnoopableObject(uiDocument.Document, geoObject);
        }
    }
}