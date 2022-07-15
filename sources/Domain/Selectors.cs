using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitDBExplorer.Domain.DataModel;

namespace RevitDBExplorer.Domain
{
    public enum Selector
    {
        Db,
        CurrentSelection,
        PickFace,
        PickEdge,
        LinkedElement,
        DependentElements,
        ActiveView,
        ActiveDocument,
        Application,
        Schemas,
        Categories
    }

    internal static class Selectors
    {
        public static IEnumerable<SnoopableObject> Snoop(UIApplication uiApplication, Selector selector)
        {
            var result = selector switch
            {
                Selector.Db => SnoopDb(uiApplication),
                Selector.CurrentSelection => SnoopCurrentSelection(uiApplication),
                Selector.PickFace => SnoopPick(uiApplication, ObjectType.Face),
                Selector.PickEdge => SnoopPick(uiApplication, ObjectType.Edge),
                Selector.LinkedElement => SnoopLinkedElement(uiApplication),
                Selector.DependentElements => SnoopDependentElements(uiApplication),                
                Selector.Application => SnoopApplication(uiApplication),
                Selector.ActiveDocument => SnoopActiveDocument(uiApplication),
                Selector.ActiveView => SnoopActiveView(uiApplication),
                Selector.Schemas => SnoopSchemas(uiApplication),
                Selector.Categories => SnoopCategories(uiApplication),
                _ => throw new NotImplementedException()
            };
            return result ?? Enumerable.Empty<SnoopableObject>();
        }

        private static IEnumerable<SnoopableObject> SnoopDb(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var elementTypes = new FilteredElementCollector(document).WhereElementIsElementType();
            var elementInstances = new FilteredElementCollector(document).WhereElementIsNotElementType();
            var elementsCollector = elementTypes.UnionWith(elementInstances);
            var elements = elementsCollector.ToElements();

            return elements.Select(x => new SnoopableObject(x, document));
        }
        private static IEnumerable<SnoopableObject> SnoopCurrentSelection(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var selectedIds = app.ActiveUIDocument.Selection.GetElementIds();

            FilteredElementCollector collector = null;

            if (selectedIds.Any())
            {
                collector = new FilteredElementCollector(document, selectedIds);
            }
            else
            {
                collector = new FilteredElementCollector(document, document.ActiveView.Id);
            }

            return collector.WhereElementIsNotElementType().ToElements().Select(x => new SnoopableObject(x, document));
        }
        private static IEnumerable<SnoopableObject> SnoopPick(UIApplication app, ObjectType objectType)
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
            yield return new SnoopableObject(geoObject, uiDocument.Document);
        }
        private static IEnumerable<SnoopableObject> SnoopLinkedElement(UIApplication app)
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

            yield return new SnoopableObject(linkedElement, linkedDocument);
        }
        private static IEnumerable<SnoopableObject> SnoopDependentElements(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var selectedIds = app.ActiveUIDocument.Selection.GetElementIds();
            if (!selectedIds.Any())
            {
                return null;
            }

            var selectedElements = new FilteredElementCollector(document, selectedIds).WhereElementIsNotElementType().ToElements();
            var dependentElementIds = selectedElements.SelectMany(x => x.GetDependentElements(null)).ToList();
            var elements = new FilteredElementCollector(document, dependentElementIds).WhereElementIsNotElementType().ToElements();
            return elements.Select(x => new SnoopableObject(x, document));
        }
        private static IEnumerable<SnoopableObject> SnoopApplication(UIApplication app)
        {
            yield return new SnoopableObject(app.Application, null);
        }
        private static IEnumerable<SnoopableObject> SnoopSchemas(UIApplication app)
        {
            return Schema.ListSchemas().Select(x => new SnoopableObject(x, null));
        }
        private static IEnumerable<SnoopableObject> SnoopCategories(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var ids = ParameterFilterUtilities.GetAllFilterableCategories();
            var categorries = ids.Select(x => Category.GetCategory(document, x)).Where(x => x is not null).ToList();
           
            return categorries.Select(x => new SnoopableObject(x, document));
        }
        private static IEnumerable<SnoopableObject> SnoopActiveDocument(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) yield break;

            yield return new SnoopableObject(document, document);
        }
        private static IEnumerable<SnoopableObject> SnoopActiveView(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;
            var view = document?.ActiveView;

            if (view == null) yield break;

            yield return new SnoopableObject(view, document);
        }        
    }
}