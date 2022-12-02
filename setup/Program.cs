using System;
using System.Linq;
using System.Diagnostics;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

namespace SetupBuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo($@"..\..\..\sources\bin\R2023\RevitDBExplorer.dll");
            var productVersion = fileVersionInfo.ProductVersion;

            var project = new Project()
            {
                Name = "Revit database explorer",
                GUID = new Guid("FF191452-5B6C-4A6D-AB57-CB4C20A7659F"),
                UpgradeCode = new Guid("E92B4658-0D37-4FF6-AD09-379881769A8D"),
                Platform = Platform.x64,
                UI = WUI.WixUI_InstallDir,
                InstallScope = InstallScope.perUser,
                MajorUpgrade = MajorUpgrade.Default,
                Version = new Version(productVersion),
                OutFileName = $"RevitDBExplorer",
                BackgroundImage = "Resources\\BackgroundImage.png",
                BannerImage = "Resources\\BannerImage.png"
            };

            project.ControlPanelInfo = new ProductInfo
            {
                Manufacturer = "github.com/NeVeSpl/RevitDBExplorer",
                HelpLink = "https://github.com/NeVeSpl/RevitDBExplorer",
                ProductIcon = "Resources\\RDBE.ico"
            };

            project.Dirs = new Dir[]
            {
                new Dir(@"%AppDataFolder%\Autodesk\Revit\Addins", CreateDirFor("2021"), CreateDirFor("2022"), CreateDirFor("2023"), CreateDirFor("2024"))
            };

            project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
            Compiler.BuildMsi(project);
        }

        readonly static string[] files = new string[]
        {
            "CircularBuffer.dll",
            "DocXml.dll",
            "RevitDBExplorer.dll",
            "RevitDBExplorer.dll.config",
            "SimMetrics.Net.dll",
            "Microsoft.Bcl.AsyncInterfaces.dll",
            "System.Buffers.dll",
            "System.Memory.dll",
            "System.Net.Http.Json.dll",
            "System.Numerics.Vectors.dll",
            "System.Runtime.CompilerServices.Unsafe.dll",
            "System.Text.Encodings.Web.dll",
            "System.Text.Json.dll",
            "System.Threading.Tasks.Extensions.dll",
            "System.ValueTuple.dll"
        };
        static Dir CreateDirFor(string year)
        {
            return new Dir(year,
                           new File($@"..\..\..\sources\bin\R{year}\RevitDBExplorer.addin"),
                           new Dir("RevitDBExplorer", files.Select(x => new File($@"..\..\..\sources\bin\R{year}\{x}")).ToArray())
                   );
        }
    }    
}