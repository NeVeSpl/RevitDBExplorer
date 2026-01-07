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
            BuildMsi(InstallScope.perUser, @"%AppDataFolder%\Autodesk\Revit\Addins", $"RevitDBExplorer");           
            BuildMsi(InstallScope.perMachine, @"%CommonAppDataFolder%\Autodesk\Revit\Addins\", $"RevitDBExplorer-perMachine");
        }

        private static void BuildMsi(InstallScope scope, string rootPath, string outFileName)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo($@"..\..\..\sources\bin\R2027\RevitDBExplorer.dll");
            var productVersion = fileVersionInfo.FileVersion;

            var project = new Project()
            {
                Name = "Revit database explorer",
                GUID = new Guid("FF191452-5B6C-4A6D-AB57-CB4C20A7659F"),
                UpgradeCode = new Guid("E92B4658-0D37-4FF6-AD09-379881769A8D"),
                Platform = Platform.x64,
                UI = WUI.WixUI_InstallDir,
                MajorUpgrade = MajorUpgrade.Default,
                Version = new Version(productVersion),
                BackgroundImage = "Resources\\BackgroundImage.png",
                BannerImage = "Resources\\BannerImage.png"
            };

            project.InstallScope = scope;
            project.OutFileName = outFileName;

            project.ControlPanelInfo = new ProductInfo
            {
                Manufacturer = "github.com/NeVeSpl/RevitDBExplorer",
                HelpLink = "https://github.com/NeVeSpl/RevitDBExplorer",
                ProductIcon = "Resources\\RDBE.ico"
            };

            project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);

            project.Dirs = new Dir[]
            {
                new Dir(rootPath, CreateDirFor(2021), CreateDirFor(2022), CreateDirFor(2023), CreateDirFor(2024), CreateDirFor(2025), CreateDirFor(2026), CreateDirFor(2027))
            };
            Compiler.BuildMsi(project);
        }

        static Dir CreateDirFor(int year)
        {
            return new Dir(year.ToString(),
                           new File($@"..\..\..\sources\bin\R{year}\RevitDBExplorer.addin"),
                           new Dir("RevitDBExplorer", new DirFiles($@"..\..\..\sources\bin\R{year}\*.dll*"))
                   );
        }
    }    
}