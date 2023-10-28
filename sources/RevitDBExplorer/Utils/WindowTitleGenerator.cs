using System;
using System.Reflection;

namespace RevitDBExplorer.Utils
{
    internal static class WindowTitleGenerator
    {
        public static string Get()
        {
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            var revit_ver = typeof(Autodesk.Revit.DB.Element).Assembly.GetName().Version;
            var title = $"Revit database explorer 20{revit_ver.Major} - {ver.ToGitHubTag()}";
            return title;
        }
    }
}