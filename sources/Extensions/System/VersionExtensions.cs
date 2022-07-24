// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System
{
    internal static class VersionExtensions
    {
        public static string ToGitHubTag(this Version version)
        {
            return $"v{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}