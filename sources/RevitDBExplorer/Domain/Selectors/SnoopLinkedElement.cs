using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopLinkedElement : ISelector
    {
        public InfoAboutSource Info { get; private set; } = new("<>");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) yield break;

            Reference reference;
            try
            {
                reference = app.ActiveUIDocument.Selection.PickObject(ObjectType.LinkedElement);
            }
            catch
            {
                // User can cancel picking
                yield break;
            }


            var representation = reference.ConvertToStableRepresentation(document).Split(':')[0];
            var parsedReference = Reference.ParseFromStableRepresentation(document, representation);
            var revitLinkInstance = (RevitLinkInstance)document.GetElement(parsedReference);
            var linkedDocument = revitLinkInstance.GetLinkDocument();
            var linkedElement = linkedDocument.GetElement(reference.LinkedElementId);

            var snoopableObject = new SnoopableObject(linkedDocument, linkedElement);
            Info.ShortTitle = snoopableObject.Name;

            yield return snoopableObject;
        }
    }
}