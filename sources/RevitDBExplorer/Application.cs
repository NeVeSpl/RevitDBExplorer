using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Properties;
using RevitDBScripting;

namespace RevitDBExplorer
{
    public class Application : IExternalApplication, IScriptRunner
    {
        public static IntPtr RevitWindowHandle;
        public static UIApplication UIApplication;      
        public static RDSController RDSController;


        public Application()
        {
            
        }


        public Result OnStartup(UIControlledApplication application)
        {
            RevitWindowHandle = application.MainWindowHandle;
            RDSController = new RDSController(application.MainWindowHandle, this);

            var panel = application.CreateRibbonPanel("Explorer");
            var cmdType = typeof(Command);
            var pushButtonData = new PushButtonData(cmdType.FullName, "Revit DB\r\nExplorer", cmdType.Assembly.Location, cmdType.FullName);
            pushButtonData.Image = new BitmapImage(new Uri("pack://application:,,,/RevitDBExplorer;component/Resources/RDBE.Icon.16.png", UriKind.RelativeOrAbsolute));
            pushButtonData.LargeImage = new BitmapImage(new Uri("pack://application:,,,/RevitDBExplorer;component/Resources/RDBE.Icon.32.png", UriKind.RelativeOrAbsolute));
            panel.AddItem(pushButtonData);

            ExternalExecutor.CreateExternalEvent();
            MemberAccessorFactory.Init();
            ValueContainerFactory.Init();
            MemberStreamerForTemplates.Init();
            RevitDocumentationReader.Init();
            RevitDatabaseQueryService.Init();
            EventMonitor.Register(application);

            ApplicationModifyTab.Init(panel.GetRibbonPanel(), AppSettings.Default.AddRDBECmdToModifyTab);

            application.Idling += Application_Idling;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


        async Task IScriptRunner.Execute(ExecutionContext context)
        {
            if (context.Type == ScriptType.Query)
            {
                var source = new SourceOfObjects(new QueryExecutionContext(context)) { Title = "User query" };

                var sourceOfObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x =>
                {
                    source.ReadFromTheSource(x);
                    return source;
                });

                var window = new MainWindow(sourceOfObjects, RevitWindowHandle);
                window.Show();
            }
            if (context.Type != ScriptType.Query)
            {
                await ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync(x => context.Execute(x), null, "RDS update command");
            }            
        }


        private static DateTime LastTimeWhenInCharge; 
        private void Application_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            LastTimeWhenInCharge = DateTime.Now;            
        }        
        public static bool IsRevitBussy()
        {
            return (DateTime.Now - Application.LastTimeWhenInCharge).TotalSeconds > 0.5;
        }


        internal class QueryExecutionContext : IAmSourceOfEverything
        {
            private readonly ExecutionContext context;          

            public QueryExecutionContext(ExecutionContext context)
            {
                this.context = context;
            }

            public IEnumerable<SnoopableObject> Snoop(UIApplication app)
            {
                var document = app.ActiveUIDocument?.Document;
                if (document == null) return null;

                var returnValue = context.Execute(app);

                if (returnValue is FilteredElementCollector collector)
                {
                    return collector.ToElements().Select(x => new SnoopableObject(document, x));
                }
                if (returnValue is IEnumerable<object> enumerable)
                {
                    return enumerable.Select(x => new SnoopableObject(document, x));
                }

                return new[] { new SnoopableObject(document, returnValue) };
            }           
        }
    }
}