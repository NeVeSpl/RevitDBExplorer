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
            var fileVersionInfo = FileVersionInfo.GetVersionInfo($@"..\..\..\sources\bin\R2025\RevitDBExplorer.dll");
            var productVersion = fileVersionInfo.FileVersion;

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
                new Dir(@"%AppDataFolder%\Autodesk\Revit\Addins", CreateDirFor(2021), CreateDirFor(2022), CreateDirFor(2023), CreateDirFor(2024), CreateDirFor(2025))
            };

            project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
            Compiler.BuildMsi(project);
        }

        //readonly static string[] filesNet48 = new string[]
        //{
        //    "CircularBuffer.dll",
        //    "DocXml.dll",
        //    "Humanizer.dll",
        //    "ICSharpCode.AvalonEdit.dll",            
        //    "Microsoft.Bcl.AsyncInterfaces.dll",
        //    "Microsoft.CodeAnalysis.AnalyzerUtilities.dll",
        //    "Microsoft.CodeAnalysis.CSharp.dll",
        //    "Microsoft.CodeAnalysis.CSharp.Features.dll",
        //    "Microsoft.CodeAnalysis.CSharp.Scripting.dll",
        //    "Microsoft.CodeAnalysis.CSharp.Workspaces.dll",
        //    "Microsoft.CodeAnalysis.dll",
        //    "Microsoft.CodeAnalysis.Features.dll",
        //    "Microsoft.CodeAnalysis.Scripting.dll",
        //    "Microsoft.CodeAnalysis.Workspaces.dll",
        //    "Microsoft.DiaSymReader.dll",
        //    "RevitDBExplorer.dll",
        //    "RevitDBExplorer.dll.config",
        //    "RevitDBExplorer.API.dll",
        //    "RevitDBExplorer.Augmentations.dll",           
        //    "RevitDBScripting.dll",
        //    "RoslynPad.Editor.Windows.dll",
        //    "RoslynPad.Roslyn.dll",
        //    "RoslynPad.Roslyn.Windows.dll",
        //    "SimMetrics.Net.dll",
        //    "System.Buffers.dll",
        //    "System.Collections.Immutable.dll",
        //    "System.Composition.AttributedModel.dll",
        //    "System.Composition.Convention.dll",
        //    "System.Composition.Hosting.dll",
        //    "System.Composition.Runtime.dll",
        //    "System.Composition.TypedParts.dll",
        //    "System.Memory.dll",
        //    "System.Net.Http.Json.dll",
        //    "System.Numerics.Vectors.dll",
        //    "System.Reactive.dll",
        //    "System.Reactive.Linq.dll",
        //    "System.Reflection.Metadata.dll",
        //    "System.Runtime.CompilerServices.Unsafe.dll",
        //    "System.Text.Encoding.CodePages.dll",
        //    "System.Text.Encodings.Web.dll",
        //    "System.Text.Json.dll",
        //    "System.Threading.Tasks.Extensions.dll",
        //    "System.ValueTuple.dll",
        //    "TrieNet.dll"
        //};
        //readonly static string[] filesNet70 = new string[]
        //{
        //    "CircularBuffer.dll",
        //    "DocXml.dll",
        //    "Humanizer.dll",
        //    "ICSharpCode.AvalonEdit.dll",
        //    "Microsoft.Bcl.AsyncInterfaces.dll",
        //    "Microsoft.CodeAnalysis.AnalyzerUtilities.dll",
        //    "Microsoft.CodeAnalysis.CSharp.dll",
        //    "Microsoft.CodeAnalysis.CSharp.Features.dll",
        //    "Microsoft.CodeAnalysis.CSharp.Scripting.dll",
        //    "Microsoft.CodeAnalysis.CSharp.Workspaces.dll",
        //    "Microsoft.CodeAnalysis.dll",
        //    "Microsoft.CodeAnalysis.Features.dll",
        //    "Microsoft.CodeAnalysis.Scripting.dll",
        //    "Microsoft.CodeAnalysis.Workspaces.dll",
        //    "Microsoft.DiaSymReader.dll",
        //    "RevitDBExplorer.dll",
        //    "RevitDBExplorer.dll.config",
        //    "RevitDBExplorer.API.dll",
        //    "RevitDBExplorer.Augmentations.dll",
        //    "RevitDBScripting.dll",
        //    "RoslynPad.Editor.Windows.dll",
        //    "RoslynPad.Roslyn.dll",
        //    "RoslynPad.Roslyn.Windows.dll",
        //    "SimMetrics.Net.dll",
        //    "System.Composition.AttributedModel.dll",
        //    "System.Composition.Convention.dll",
        //    "System.Composition.Hosting.dll",
        //    "System.Composition.Runtime.dll",
        //    "System.Composition.TypedParts.dll",
        //    "System.Reactive.dll",
        //    "System.Reactive.Linq.dll",       
        //    "TrieNet.dll"
        //};

        static Dir CreateDirFor(int year)
        {
            return new Dir(year.ToString(),
                           new File($@"..\..\..\sources\bin\R{year}\RevitDBExplorer.addin"),
                           new Dir("RevitDBExplorer", new DirFiles($@"..\..\..\sources\bin\R{year}\*.dll*"))
                   );
        }
    }    
}