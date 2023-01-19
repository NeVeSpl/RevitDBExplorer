using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors;
using RevitDBExplorer.Domain.Selectors.Base;

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

    internal static class SelectorExecutor
    {
        public static ResultOfSnooping Snoop(UIApplication uiApplication, Selector selector)
        {
            var snooper = CreateSelector(uiApplication, selector);
            var result = snooper.Snoop(uiApplication) ?? Enumerable.Empty<SnoopableObject>();
            return new(result.ToArray());
        }

        private static ISelector CreateSelector(UIApplication uiApplication, Selector selector)
        {
            ISelector result = selector switch
            {
                Selector.Db => new SnoopDatabase(),
                Selector.CurrentSelection => new SnoopCurrentSelection(),
                Selector.PickFace => new SnoopPick(ObjectType.Face),
                Selector.PickEdge => new SnoopPick(ObjectType.Edge),
                Selector.LinkedElement => new SnoopLinkedElement(),
                Selector.DependentElements => new SnoopDependentElements(),
                Selector.Application => new SnoopApplication(),
                Selector.ActiveDocument => new SnoopActiveDocument(),
                Selector.ActiveView => new SnoopActiveView(),
                Selector.Schemas => new SnoopSchemas(),
                Selector.FilterableCategories => new SnoopCategories(),
                Selector.FilterableParameters => new SnoopParameters(),
                Selector.ForgeParameterUtilsGetAllBuiltInGroups => new SnoopForge(selector),
                Selector.ForgeParameterUtilsGetAllBuiltInParameters => new SnoopForge(selector),
                Selector.ForgeUnitUtilsGetAllMeasurableSpecs => new SnoopForge(selector),
                Selector.ForgeUnitUtilsGetAllUnits => new SnoopForge(selector),
                Selector.ForgeUnitUtilsGetAllDisciplines => new SnoopForge(selector),
                Selector.ForgeSpecUtilsGetAllSpecs => new SnoopForge(selector),
                Selector.Updaters => new SnoopUpdaters(),
                Selector.LoadedApplications => new SnoopLoadedApplications(),
                _ => throw new NotImplementedException()
            };
            return result;
        }
    }
}