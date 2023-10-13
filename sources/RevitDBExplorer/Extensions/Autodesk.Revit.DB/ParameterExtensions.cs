// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.DB
{
    internal enum ParameterOrgin { Shared, Project, BuiltIn }

    internal static class ParameterExtensions
    {
        public static ParameterOrgin GetOrgin(this Parameter parameter)
        {
            var result = ParameterOrgin.BuiltIn;
            if (parameter.Id.Value() > -1)
            {
                result = ParameterOrgin.Project;
            }
            if (parameter.IsShared)
            {
                result = ParameterOrgin.Shared;
            }
            return result;
        }
    }
}