using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Media;
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

        public static DocXml GetTypeComments(Type type)
        {
            var typeComments = docXml.GetTypeComments(type);            

            var doc = new DocXml()
            {
                Summary = CleanString(typeComments?.Summary),
                Remarks = CleanString(typeComments?.Remarks),              
                Name = type.GetCSharpName(),              
            };

            return doc;
        }
        public static DocXml GetPropertyComments(PropertyInfo info)
        {
            var memberComments = docXml?.GetMemberComments(info);

            var invocation = " { get; }";
            var name = info.Name;
            if (info.CanWrite)
            {
                invocation = " { get; set; }";
            }
            var parameters = info?.GetGetGetMethod().GetParameters();
            if (parameters.Any())
            {
                name = info.GetGetGetMethod().Name;
                invocation = "(" + String.Join(", ", parameters.Select(p => $"{p.ParameterType.GetCSharpName()} {p.Name}").ToArray()) + ")";
            }


            var returnType = info.PropertyType.GetCSharpName();
            IEnumerable<Inline> titleCollored;

            if (parameters.Any())
            {
                titleCollored = ToInlinesMethod(returnType, info.DeclaringType.Name, name, parameters).ToArray();
            }
            else
            {
                titleCollored = ToInlinesProp(returnType, name, true, info.CanWrite).ToArray();
            }

            var doc = new DocXml(returnType, name, invocation, titleCollored)
            {
                Summary = CleanString(memberComments?.Summary),
                Remarks = CleanString(memberComments?.Remarks),
            };

            return doc;
        }
        public static DocXml GetMethodComments(MethodInfo info)
        {
            var methodComments = docXml?.GetMethodComments(info);
            var returnType = info.ReturnType.GetCSharpName();
            var invocation = "(" + String.Join(", ", info.GetParameters().Select(p => $"{p.ParameterType.GetCSharpName()} {p.Name}").ToArray()) + ")";
            var titleCollored = ToInlinesMethod(returnType, info.DeclaringType.Name, info.Name, info.GetParameters()).ToArray();

            var doc = new DocXml(returnType, info.Name, invocation, titleCollored)
            {
                Summary = CleanString(methodComments?.Summary),
                Returns = CleanString(methodComments?.Returns),
                Remarks = CleanString(methodComments?.Remarks),
            };           
            return doc;
        }

        private static string CleanString(string input)
        {
            string result = input?.Trim()?.Replace(System.Environment.NewLine, "").StripTags();
            return result;
        }

        private static IEnumerable<Inline> ToInlinesProp(string returnType, string name, bool hasGet, bool hasSet)
        {
            yield return new Run(returnType) { Foreground = returnType.IsPrimitiveTypeName() ? PropTypeBrush : TypeBrush };
            yield return new Run(" ");
            yield return new Run(name) { Foreground = NameBrush };
            yield return new Run(" { ");
            if (hasGet)
            {
                yield return new Run("get; ") { Foreground = PropTypeBrush };
            }
            if (hasSet)
            {
                yield return new Run("set; ") { Foreground = PropTypeBrush };
            }
            yield return new Run("}");
        }
        private static IEnumerable<Inline> ToInlinesMethod(string returnType, string declaringType, string name, ParameterInfo[] parameterInfos)
        {
            yield return new Run(returnType) { Foreground = returnType.IsPrimitiveTypeName() ? PropTypeBrush : TypeBrush };
            yield return new Run(" ");
            yield return new Run(declaringType) { Foreground = TypeBrush };
            yield return new Run(".");
            yield return new Run(name) { Foreground = NameBrush };
            yield return new Run("(");
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                var p = parameterInfos[i];
                var typeName = p.ParameterType.GetCSharpName();
                yield return new Run(typeName) { Foreground = typeName.IsPrimitiveTypeName() ? PropTypeBrush : TypeBrush };
                yield return new Run(" ");
                yield return new Run(p.Name) { Foreground = VarNameBrush };
                if (i < parameterInfos.Length - 1)
                {
                    yield return new Run(", ");
                }
            }            

            yield return new Run(")");
        }


        private static readonly SolidColorBrush PropTypeBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0000FF");
        private static readonly SolidColorBrush NameBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#745320");
        private static readonly SolidColorBrush TypeBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#3A96BC");
        private static readonly SolidColorBrush VarNameBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#21377F");
    }

    public class DocXml
    {
        public static readonly DocXml Empty = new DocXml();


        public string Summary { get; init; }
        public string Returns { get; init; }
        public string Remarks { get; init; }
        public string Name { get; init; }

        public string Title { get; init; }
        public IEnumerable<Inline> TitleCollored { get; init; }


        public DocXml()
        {
            
        }

        public DocXml(string returnType, string name, string invocation, IEnumerable<Inline> titleCollored)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Name = name;
                Title = $"{returnType} {name}{invocation}";
                TitleCollored = titleCollored;
            }
        }
    }
}