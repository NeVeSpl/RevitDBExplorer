using System;
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
            docXml = new DocXmlReader(assemblyXmlDocPath);
        }

        public static DocXml GetTypeComments(Type type)
        {
            var typeComments = docXml.GetTypeComments(type);
            return new DocXml(typeComments.Summary?.Trim(), typeComments.Remarks?.Trim());
        }
        public static DocXml GetPropertyComments(MemberInfo info)
        {
            var memberComments = docXml.GetMemberComments(info); 

            return new DocXml(CleanString(memberComments.Summary), CleanString(memberComments.Remarks));
        }
        public static DocXml GetMethodComments(MethodInfo info)
        {
            var methodComments = docXml.GetMethodComments(info);

            return new DocXml(CleanString(methodComments.Summary), CleanString(methodComments.Remarks));
        }

        private static string CleanString(string input)
        {
            string result = input?.Trim()?.Replace(System.Environment.NewLine, "");
            return result;
        }
    }

    public record class DocXml(string Summary, string Remarks);
}