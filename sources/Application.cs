using System;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain;

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
            var pushButtonData = new PushButtonData(cmdType.FullName, "Revit DB Explorer", cmdType.Assembly.Location, cmdType.FullName);
            pushButtonData.LargeImage = new BitmapImage(new Uri("pack://application:,,,/RevitDBExplorer;component/Resources/RDBE.Icon.32.b.png", UriKind.RelativeOrAbsolute));
            panel.AddItem(pushButtonData);

            ExternalExecutor.CreateExternalEvent();
            RevitDocumentationReader.Init();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}