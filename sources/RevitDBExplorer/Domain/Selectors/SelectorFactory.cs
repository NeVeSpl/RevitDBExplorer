using System;
using Autodesk.Revit.UI.Selection;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    public enum Selector
    {
        Db,
        CurrentSelection,
        ElementsOnScreen,
        PickFace,
        PickEdge,
        PickPoint,
        PickSubelement,
        LinkedElement,
        DependentElements,
        ActiveView,
        ActiveDocument,
        Application,
        Schemas,
        FilterableCategories,       
        ForgeParameterUtilsGetAllBuiltInGroups,
        ForgeParameterUtilsGetAllBuiltInParameters,
        ForgeUnitUtilsGetAllMeasurableSpecs,
        ForgeUnitUtilsGetAllUnits,
        ForgeUnitUtilsGetAllDisciplines,
        ForgeSpecUtilsGetAllSpecs,
        Updaters,
        LoadedApplications,
        ExternalServices,
        VisibleInView,
    }

    internal static class SelectorFactory
    {
        public static SourceOfObjects Create(Selector selector)
        {
            var snooper = CreateSelector(selector);
            var sourceOfObjects = new SourceOfObjects(snooper);
            return sourceOfObjects;            
        }

        private static ISelector CreateSelector(Selector selector)
        {
            ISelector result = selector switch
            {
                Selector.Db => new SnoopDatabase(),
                Selector.CurrentSelection => new SnoopCurrentSelection(),
                Selector.ElementsOnScreen => new SnoopElementsOnScreen(),
                Selector.PickFace => new SnoopPick(ObjectType.Face),
                Selector.PickEdge => new SnoopPick(ObjectType.Edge),
                Selector.PickPoint => new SnoopPick(ObjectType.PointOnElement),
                Selector.PickSubelement => new SnoopPick(ObjectType.Subelement),
                Selector.LinkedElement => new SnoopLinkedElement(),
                //Selector.DependentElements => new SnoopDependentElements(),
                Selector.Application => new SnoopApplication(),
                Selector.ActiveDocument => new SnoopActiveDocument(),
                Selector.ActiveView => new SnoopActiveView(),
                Selector.Schemas => new SnoopSchemas(),
                Selector.FilterableCategories => new SnoopCategories(),
                Selector.ForgeParameterUtilsGetAllBuiltInGroups => new SnoopForge(selector),
                Selector.ForgeParameterUtilsGetAllBuiltInParameters => new SnoopForge(selector),
                Selector.ForgeUnitUtilsGetAllMeasurableSpecs => new SnoopForge(selector),
                Selector.ForgeUnitUtilsGetAllUnits => new SnoopForge(selector),
                Selector.ForgeUnitUtilsGetAllDisciplines => new SnoopForge(selector),
                Selector.ForgeSpecUtilsGetAllSpecs => new SnoopForge(selector),
                Selector.Updaters => new SnoopUpdaters(),
                Selector.LoadedApplications => new SnoopLoadedApplications(),
                Selector.ExternalServices => new SnoopExternalServices(),
                Selector.VisibleInView => new SnoopVisibleInView(),
                _ => throw new NotImplementedException()
            }; ;
            return result;
        }
    }
}