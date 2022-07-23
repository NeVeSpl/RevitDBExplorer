using System;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.RevitDatabaseQuery;

namespace RevitDBExplorer
{
    public class Application : IExternalApplication
    {
        public static IntPtr RevitWindowHandle;


        public Result OnStartup(UIControlledApplication application)
        {
            RevitWindowHandle = application.MainWindowHandle;

            var panel = application.CreateRibbonPanel("RevitDBExplorer");
            var cmdType = typeof(Command);
            var pushButtonData = new PushButtonData(cmdType.FullName, "Revit DB\r\nExplorer", cmdType.Assembly.Location, cmdType.FullName);
            pushButtonData.LargeImage = new BitmapImage(new Uri("pack://application:,,,/RevitDBExplorer;component/Resources/RDBE.Icon.32.png", UriKind.RelativeOrAbsolute));
            panel.AddItem(pushButtonData);

            ExternalExecutor.CreateExternalEvent();
            FactoryOfFactories.Init();
            RevitDocumentationReader.Init();
            RevitDatabaseQueryService.Init();            

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}