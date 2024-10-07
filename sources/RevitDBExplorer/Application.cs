using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Properties;
using RevitDBExplorer.Utils;
using RevitExplorer.Scripting;
using RevitExplorer.Visualizations;

namespace RevitDBExplorer
{
    public class Application : IExternalApplication, IScriptRunner
    {
        private static UIView UIView;
        private static View View;
        public static IntPtr RevitWindowHandle;
        public static UIApplication UIApplication;      
        public static RDSController RDSController;
        

        public Application()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            if (name.Name == "System.Runtime.CompilerServices.Unsafe")
            {
                return typeof(Unsafe).Assembly;
            }
            return null;
        }


        public Result OnStartup(UIControlledApplication application)
        {
            RevitWindowHandle = application.MainWindowHandle;
            UIApplication = application.GetUIApplication();
            RDSController = new RDSController(application.MainWindowHandle, this);

            var panel = application.CreateRibbonPanel("Explorer");
            var cmdType = typeof(Command);
            var pushButtonData = new PushButtonData(cmdType.FullName, "Revit\r\nExplorer", cmdType.Assembly.Location, cmdType.FullName);
            pushButtonData.Image = new BitmapImage(new Uri("pack://application:,,,/RevitDBExplorer;component/Resources/RDBE.Icon.16.png", UriKind.RelativeOrAbsolute));
            pushButtonData.LargeImage = new BitmapImage(new Uri("pack://application:,,,/RevitDBExplorer;component/Resources/RDBE.Icon.32.png", UriKind.RelativeOrAbsolute));
            panel.AddItem(pushButtonData);

            ExternalExecutor.CreateExternalEvent();
            MemberAccessorFactory.Init();
            ValueContainerFactory.Init();
            MemberStreamerForTemplates.Init();
            RevitDocumentationReader.Init();
            RevitDatabaseQueryService.Init();
            RevitVisualizationFactory.Init(UIApplication, new ServerIdentity("Revit Explorer Visualizations", "RevitDBExplorer", ""));
            EventMonitor.Register(application);

            ApplicationModifyTab.Init(panel.GetRibbonPanel(), AppSettings.Default.AddRDBECmdToModifyTab);

            application.Idling += Application_Idling;
            UIApplication.ViewActivated += UIApplication_ViewActivated;

            return Result.Succeeded;
        }              
        public Result OnShutdown(UIControlledApplication application)
        {
            application.Idling -= Application_Idling;
            UIApplication.ViewActivated -= UIApplication_ViewActivated;

            return Result.Succeeded;
        }


        async Task IScriptRunner.Execute(ExecutionContext context)
        {
            if (context.Type == ScriptType.Query)
            {
                var source = new SourceOfObjects(new QueryExecutionContext(context)) { Info = new InfoAboutSource("Script") };

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


        private void UIApplication_ViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs e)
        {
            if (e.CurrentActiveView != null)
            {
                var uiViews = UIApplication.ActiveUIDocument.GetOpenUIViews();
                UIView = uiViews.FirstOrDefault(x => x.ViewId  == e.CurrentActiveView.Id);
                View = e.CurrentActiveView;
            }
            else
            {
                UIView = null;
                View = null;
            }
        }
        public static (string, XYZ, XYZ, bool) GetMouseStatus()
        {
            var uiView = Application.UIView;
            var view = Application.View;
            if ((uiView != null) && (view != null) && (view.IsValidObject) && (uiView.IsValidObject) && (view.DetailLevel != ViewDetailLevel.Undefined))
            {
                // source : https://thebuildingcoder.typepad.com/blog/2012/10/uiview-windows-coordinates-referenceintersector-and-my-own-tooltip.html

                try
                {
                    var rect = uiView.GetWindowRectangle();
                    var p = System.Windows.Forms.Cursor.Position;

                    double dx = (double)(p.X - rect.Left) / (rect.Right - rect.Left);
                    double dy = (double)(p.Y - rect.Bottom) / (rect.Top - rect.Bottom);

                    var corners = uiView.GetZoomCorners();
                    var min = corners[0];
                    var max = corners[1];
                    var v = max - min;
                    var l = v.GetLength();

                    var vr = dx * new XYZ(v.X * view.RightDirection.X, v.Y * view.RightDirection.Y, v.Z * view.RightDirection.Z);
                    var vu = dy * new XYZ(v.X * view.UpDirection.X, v.Y * view.UpDirection.Y, v.Z * view.UpDirection.Z);
                    var vv = 1.0 * new XYZ(min.X * view.ViewDirection.X, min.Y * view.ViewDirection.Y, min.Z * view.ViewDirection.Z);

                    var q = min + vr + vu - vv;

                    min -= vv;
                    max -= vv;

                    if ((Math.Abs(view.RightDirection.X) < 0.999) && (Math.Abs(view.RightDirection.Y) < 0.999) && (Math.Abs(view.RightDirection.Z) < 0.999))
                    {
                        return ("(?,,)", min, max, false);
                    }
                    if ((Math.Abs(view.UpDirection.X) < 0.999) && (Math.Abs(view.UpDirection.Y) < 0.999) && (Math.Abs(view.UpDirection.Z) < 0.999))
                    {
                        return ("(,?,)", min, max, false);
                    }
                    if ((Math.Abs(view.ViewDirection.X) < 0.999) && (Math.Abs(view.ViewDirection.Y) < 0.999) && (Math.Abs(view.ViewDirection.Z) < 0.999))
                    {
                        return ("(,,?)", min, max, false);
                    }

                    if (view.ViewDirection.IsParallelTo(XYZ.BasisX))
                    {
                        return ($"(-,--, {q.Y:f3}, {q.Z:f3})", min, max, true);
                    }
                    if (view.ViewDirection.IsParallelTo(XYZ.BasisY))
                    {
                        return ($"({q.X:f3}, -,--, {q.Z:f3})", min, max, true);
                    }
                    if (view.ViewDirection.IsParallelTo(XYZ.BasisZ))
                    {
                        return ($"({q.X:f3}, {q.Y:f3}, -,--)", min, max, true);
                    }
                }
                catch
                {

                }
            }
            return ("", XYZ.Zero, XYZ.Zero, false);
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