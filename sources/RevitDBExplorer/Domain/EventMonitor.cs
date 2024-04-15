using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using CircularBuffer;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Properties;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal class EventMonitor
    {
        static CircularBuffer<SnoopableObject> events = new CircularBuffer<SnoopableObject>(27);


        public static void Register(UIControlledApplication application)
        {
            application.ApplicationClosing += GenericEventHandler;
            application.DialogBoxShowing += GenericEventHandler;
            application.DisplayingOptionsDialog += GenericEventHandler;
            application.DockableFrameFocusChanged += GenericEventHandler;
            application.DockableFrameVisibilityChanged += GenericEventHandler;
            application.FabricationPartBrowserChanged += GenericEventHandler;
            application.FormulaEditing += GenericEventHandler;
#if R2023_MIN
            application.SelectionChanged += GenericEventHandler;
#endif
#if R2024_MIN 
            application.ThemeChanged += GenericEventHandler;
#endif
            application.TransferredProjectStandards += GenericEventHandler;
            application.TransferringProjectStandards += GenericEventHandler;
            application.ViewActivated += GenericEventHandler;
            application.ViewActivating += GenericEventHandler;

            application.ControlledApplication.ApplicationInitialized += GenericEventHandler;
            application.ControlledApplication.DocumentChanged += GenericEventHandler;
            application.ControlledApplication.DocumentClosed += GenericEventHandler;
            application.ControlledApplication.DocumentClosing += GenericEventHandler;
            application.ControlledApplication.DocumentCreated += GenericEventHandler;
            application.ControlledApplication.DocumentCreating += GenericEventHandler;
            application.ControlledApplication.DocumentOpened += GenericEventHandler;
            application.ControlledApplication.DocumentOpening += GenericEventHandler;
            application.ControlledApplication.DocumentPrinted += GenericEventHandler;
            application.ControlledApplication.DocumentPrinting += GenericEventHandler;
            application.ControlledApplication.DocumentReloadedLatest += GenericEventHandler;
            application.ControlledApplication.DocumentReloadingLatest += GenericEventHandler;
            application.ControlledApplication.DocumentSaved += GenericEventHandler;
            application.ControlledApplication.DocumentSavedAs += GenericEventHandler;
            application.ControlledApplication.DocumentSaving += GenericEventHandler;
            application.ControlledApplication.DocumentSavingAs += GenericEventHandler;
            application.ControlledApplication.DocumentSynchronizedWithCentral += GenericEventHandler;
            application.ControlledApplication.DocumentSynchronizingWithCentral += GenericEventHandler;
            application.ControlledApplication.ElementTypeDuplicated += GenericEventHandler;
            application.ControlledApplication.ElementTypeDuplicating += GenericEventHandler;
            application.ControlledApplication.FailuresProcessing += GenericEventHandler;
            application.ControlledApplication.FamilyLoadedIntoDocument += GenericEventHandler;
            application.ControlledApplication.FamilyLoadingIntoDocument += GenericEventHandler;
            application.ControlledApplication.FileExported += GenericEventHandler;
            application.ControlledApplication.FileExporting += GenericEventHandler;
            application.ControlledApplication.FileImported += GenericEventHandler;
            application.ControlledApplication.FileImporting += GenericEventHandler;
            application.ControlledApplication.LinkedResourceOpened += GenericEventHandler;
            application.ControlledApplication.LinkedResourceOpening += GenericEventHandler;
            application.ControlledApplication.ViewPrinted += GenericEventHandler;
            application.ControlledApplication.ViewPrinting += GenericEventHandler;
            application.ControlledApplication.WorksharedOperationProgressChanged += GenericEventHandler;

        }

        private static void GenericEventHandler(object sender, RevitAPIEventArgs e)
        {
            if (!AppSettings.Default.IsEventMonitorEnabled)
            {
                return;
            }

            Document document = null;

            if (e is FormulaEditingEventArgs formulaEditingEventArgs)
            {
                document = formulaEditingEventArgs.CurrentDocument;
            }
#if R2023_MIN
if (e is SelectionChangedEventArgs selectionChangedEventArgs)
            {
                document = selectionChangedEventArgs.GetDocument();
            }
#endif

            if (e is RevitAPIPostDocEventArgs revitAPIPostDocEventArgs)
            {
                document = revitAPIPostDocEventArgs.Document;
            }
            if (e is DocumentChangedEventArgs documentChangedEventArgs)
            {
                document = documentChangedEventArgs.GetDocument();
            }
            if (e is RevitAPIPreDocEventArgs revitAPIPreDocEventArgs)
            {
                document = revitAPIPreDocEventArgs.Document;
            }
            if (e is DocumentWorksharingEnabledEventArgs documentWorksharingEnabledEventArgs)
            {
                document = documentWorksharingEnabledEventArgs.GetDocument();
            }
            if (e is FailuresProcessingEventArgs failuresProcessingEventArgs)
            {
                document = failuresProcessingEventArgs.GetFailuresAccessor()?.GetDocument();
            }
            if ((document == null) && (sender is UIApplication uiApplication))
            {
                document = uiApplication?.ActiveUIDocument?.Document;
            }


            var frozenArgs = new SnoopableObject(document, e) { NamePrefix = "args:" };
            frozenArgs.Freeze();
            var senderSO = new SnoopableObject(document, sender, new[] { frozenArgs }) { Name = DateTime.Now.ToLongTimeString(), NamePrefix = "sender:" };
            events.PushBack(senderSO);
        }

        public static IEnumerable<SnoopableObject> GetEvents()
        {
            var copy = events.ToArray();
            events.Clear();
            return copy;
        }
    }
}