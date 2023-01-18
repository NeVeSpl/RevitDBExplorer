using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

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
        FilterableCategories,
        FilterableParameters,
        ForgeParameterUtilsGetAllBuiltInGroups,
        ForgeParameterUtilsGetAllBuiltInParameters,
        ForgeUnitUtilsGetAllMeasurableSpecs,
        ForgeUnitUtilsGetAllUnits,
        ForgeUnitUtilsGetAllDisciplines,
        ForgeSpecUtilsGetAllSpecs,
        Updaters,
        LoadedApplications
    }

    internal static class Selectors
    {
        public static ResultOfSnooping Snoop(UIApplication uiApplication, Selector selector)
        {
            return new(SnoopInternal(uiApplication, selector).ToArray());
        }

        private static IEnumerable<SnoopableObject> SnoopInternal(UIApplication uiApplication, Selector selector)
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
                Selector.FilterableCategories => SnoopCategories(uiApplication),
                Selector.FilterableParameters => SnoopParameters(uiApplication),
                Selector.ForgeParameterUtilsGetAllBuiltInGroups => SnoopForge(uiApplication, selector),
                Selector.ForgeParameterUtilsGetAllBuiltInParameters => SnoopForge(uiApplication, selector),
                Selector.ForgeUnitUtilsGetAllMeasurableSpecs => SnoopForge(uiApplication, selector),
                Selector.ForgeUnitUtilsGetAllUnits => SnoopForge(uiApplication, selector),
                Selector.ForgeUnitUtilsGetAllDisciplines => SnoopForge(uiApplication, selector),
                Selector.ForgeSpecUtilsGetAllSpecs => SnoopForge(uiApplication, selector),
                Selector.Updaters => SnoopUpdaters(uiApplication),
                Selector.LoadedApplications => SnoopLoadedApplications(uiApplication),
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

            return elements.Select(x => new SnoopableObject(document, x));
        }
        private static IEnumerable<SnoopableObject> SnoopCurrentSelection(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var selectedIds = app.ActiveUIDocument.Selection.GetElementIds();

            FilteredElementCollector collector = null;

            if (selectedIds.Any())
            {
                collector = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(selectedIds));
            }
            else
            {
                collector = new FilteredElementCollector(document, document.ActiveView.Id); //.WherePasses(new VisibleInViewFilter(document, document.ActiveView.Id));
            }

            return collector.ToElements().Select(x => new SnoopableObject(document, x));
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
            yield return new SnoopableObject(uiDocument.Document, geoObject);
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

            yield return new SnoopableObject(linkedDocument, linkedElement);
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

            var selectedElements = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(selectedIds)).ToElements();
            var dependentElementIds = selectedElements.SelectMany(x => x.GetDependentElements(null)).ToList();
            var elements = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(dependentElementIds)).ToElements();
            return elements.Select(x => new SnoopableObject(document, x));
        }
        private static IEnumerable<SnoopableObject> SnoopApplication(UIApplication app)
        {
            yield return new SnoopableObject(null, app.Application);
        }
        private static IEnumerable<SnoopableObject> SnoopSchemas(UIApplication app)
        {
            return Schema.ListSchemas().Select(x => new SnoopableObject(app?.ActiveUIDocument?.Document, x));
        }
        private static IEnumerable<SnoopableObject> SnoopCategories(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var ids = ParameterFilterUtilities.GetAllFilterableCategories();
            var categorries = ids.Select(x => Category.GetCategory(document, x)).Where(x => x is not null).ToList();

            return categorries.Select(x => new SnoopableObject(document, x));
        }
        private static IEnumerable<SnoopableObject> SnoopParameters(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) yield break;

            var categoryIds = ParameterFilterUtilities.GetAllFilterableCategories();
            var categorries = categoryIds.Select(x => Category.GetCategory(document, x)).Where(x => x is not null).ToList();
            foreach (var category in categorries)
            {
                var paramIds = ParameterFilterUtilities.GetFilterableParametersInCommon(document, new[] { category.Id });
                IEnumerable<SnoopableObject> snoopableParameters = Enumerable.Empty<SnoopableObject>();
                if (paramIds.Any())
                {
                    var parameters = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(paramIds)).ToList();
                    snoopableParameters = parameters.Select(x => new SnoopableObject(document, x));
                }

                yield return new SnoopableObject(document, category, snoopableParameters);
            }
        }
        private static IEnumerable<SnoopableObject> SnoopActiveDocument(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) yield break;

            yield return new SnoopableObject(document, document);
        }
        private static IEnumerable<SnoopableObject> SnoopActiveView(UIApplication app)
        {
            var view = app?.ActiveUIDocument?.Document?.ActiveView;

            if (view == null) yield break;

            yield return new SnoopableObject(view.Document, view);
        }
        private static IEnumerable<SnoopableObject> SnoopForge(UIApplication app, Selector selector)
        {
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            IList<ForgeTypeId> ids = selector switch
            {
#if R2022b
                Selector.ForgeParameterUtilsGetAllBuiltInGroups => ParameterUtils.GetAllBuiltInGroups(),
                Selector.ForgeParameterUtilsGetAllBuiltInParameters => ParameterUtils.GetAllBuiltInParameters(),
                Selector.ForgeUnitUtilsGetAllMeasurableSpecs => UnitUtils.GetAllMeasurableSpecs(),
                Selector.ForgeUnitUtilsGetAllDisciplines => UnitUtils.GetAllDisciplines(),
                Selector.ForgeSpecUtilsGetAllSpecs => SpecUtils.GetAllSpecs(),
#endif
                Selector.ForgeUnitUtilsGetAllUnits => UnitUtils.GetAllUnits(),

            };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            return ids.Select(x => new SnoopableObject(app?.ActiveUIDocument?.Document, x));
        }
        private static IEnumerable<SnoopableObject> SnoopUpdaters(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            UpdaterInfoWizard.AvadaKedavra();

            return UpdaterRegistry.GetRegisteredUpdaterInfos(document).Select(x => new SnoopableObject(document, x));
        }
        private static IEnumerable<SnoopableObject> SnoopLoadedApplications(UIApplication app)
        {
            AddInManifestWizard.WingardiumLeviosa(app);
            foreach (var externalApp in app.LoadedApplications)
            {
                yield return new SnoopableObject(app?.ActiveUIDocument?.Document, externalApp);
            }
        }
    }
}