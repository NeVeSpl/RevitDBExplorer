using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal static class MemberStreamer
    {
        public static IEnumerable<SnoopableMember> Stream(SnoopableObject target)
        {
            var type = target.Object.GetType();

            bool shouldEndAllStreaming = false;
            foreach (var member in MemberStreamerForSystemType.Stream(target))
            {
                shouldEndAllStreaming = true;
                yield return member;
            }
            if (shouldEndAllStreaming) yield break;


            foreach (var member in MemberStreamerForTemplates.Stream(target))
            {
                yield return member;
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in properties)
            {
                if (prop.Name == "Parameter")
                {
                    continue;
                }

                var getMethod = prop.GetGetGetMethod();
                if (getMethod == null)
                {
                    continue;
                }

                var comments = () => RevitDocumentationReader.GetPropertyComments(prop);
                var memberAccessor = MemberAccessorFactory.CreateMemberAccessor(getMethod, null);
                var member = new SnoopableMember(target, SnoopableMember.Kind.Property, prop.Name, prop.DeclaringType, memberAccessor, comments);
                var desc = new MemberDescriptor(SnoopableMember.Kind.Property, prop.Name, prop.DeclaringType, null, comments);
                yield return member;
            }

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (method.ReturnType == typeof(void) && method.Name != "GetOverridableHookParameters") continue;
                if (method.IsSpecialName) continue;
                if (method.DeclaringType == typeof(object)) continue;

                if (method.Name == "Set" && target.Object is Parameter parameter)
                {
                    Type expectedParameterType = parameter.StorageType switch
                    {
                        StorageType.None => null,
                        StorageType.Integer => typeof(int),
                        StorageType.Double => typeof(double),
                        StorageType.String => typeof(string),
                        StorageType.ElementId => typeof(ElementId)
                    };
                    var parameterType = method.GetParameters().FirstOrDefault()?.ParameterType;
                    if (parameterType != expectedParameterType)
                    {
                        continue;
                    }
                }

                var comments = () => RevitDocumentationReader.GetMethodComments(method);
                var memberAccessor = MemberAccessorFactory.CreateMemberAccessor(method, null);
                var member = new SnoopableMember(target, SnoopableMember.Kind.Method, method.Name, method.DeclaringType, memberAccessor, comments);
                yield return member;
            }
        }
    }
}
