using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using LoxSmoke.DocXml;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class RevitDocumentationReader
    {
        static DocXmlReader docXml;


        static public void Init()
        {
            var assemblyPath = typeof(Element).Assembly.Location;
            var assemblyXmlDocPath = Path.ChangeExtension(assemblyPath, "xml");
            if (File.Exists(assemblyXmlDocPath))
            {
                docXml = new DocXmlReader(assemblyXmlDocPath);
            }
        }

        //public static DocXml GetTypeComments(Type type)
        //{
        //    var typeComments = docXml.GetTypeComments(type);
        //    return new DocXml(typeComments.Summary?.Trim(), typeComments.Remarks?.Trim());
        //}
        public static DocXml GetPropertyComments(PropertyInfo info)
        {
            var memberComments = docXml?.GetMemberComments(info);
            var doc = new DocXml()
            {
                Summary = CleanString(memberComments?.Summary),              
                Remarks = CleanString(memberComments?.Remarks),
                ReturnType = info.PropertyType.ToCSharpString(),
                Name = info.Name,
                Invocation = " { get; }"
            };
            return doc;
        }
        public static DocXml GetMethodComments(MethodInfo info)
        {
            var methodComments = docXml?.GetMethodComments(info);
            var doc = new DocXml()
            {
                Summary = CleanString(methodComments?.Summary),
                Returns = CleanString(methodComments?.Returns),
                Remarks = CleanString(methodComments?.Remarks),
                ReturnType = info.ReturnType.ToCSharpString(),
                Name = info.Name,
                Invocation = "(" + String.Join(",", info.GetParameters().Select(p => $"{p.ParameterType.ToCSharpString()} {p.Name}").ToArray()) + ")"
        };
            return doc;
        }

        private static string CleanString(string input)
        {
            string result = input?.Trim()?.Replace(System.Environment.NewLine, "").StripTags();
            return result;
        }
    }

    public class DocXml
    {
        public string Summary    { get; init; }
        public string Returns    { get; init; }
        public string Remarks    { get; init; }
        public string ReturnType { get; init; }
        public string Name       { get; init; }
        public string Invocation { get; init; }

    }
}