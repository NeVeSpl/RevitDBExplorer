using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members
{
    internal static class MemberStreamer
    {
        public static IEnumerable<MemberDescriptor> StreamDescriptors(SnoopableContext context, object snoopableObject)
        {
            bool shouldEndAllStreaming = false;
            foreach (var member in MemberStreamerForSystemType.Stream(context, snoopableObject))
            {
                shouldEndAllStreaming = true;
                yield return member;
            }
            if (shouldEndAllStreaming) yield break;

            foreach (var member in MemberDescriptor.CreateSeparators())
            {  
                yield return member;
            }

            foreach (var member in MemberStreamerForTemplates.Stream(snoopableObject))
            {
                yield return member;
            }

            var type = snoopableObject.GetType();
            var cachableDescriptors = Cache_DescriptorsForPropsAndMethods.GetOrCreate(type, _ => StreamDescriptorsForPropsAndMethods(snoopableObject).ToArray());

            foreach (var member in cachableDescriptors)
            {
                yield return member;
            }
        }
        private static readonly Dictionary<Type, IReadOnlyList<MemberDescriptor>> Cache_DescriptorsForPropsAndMethods = new();

        private static IEnumerable<MemberDescriptor> StreamDescriptorsForPropsAndMethods(object snoopableObject)
        {
            var type = snoopableObject.GetType();

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
                var member = new MemberDescriptor(type, MemberKind.Property, prop.Name, prop.DeclaringType, memberAccessor, comments);
                yield return member;
            }

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            foreach (var method in methods)
            {                
                if (method.IsSpecialName) continue;
                if (method.DeclaringType == typeof(object) && method.Name != "GetType") continue;

                if (method.Name == "Set" && snoopableObject is Parameter parameter)
                {
                    Type expectedParameterType = parameter.StorageType switch
                    {
                        StorageType.None => null,
                        StorageType.Integer => typeof(int),
                        StorageType.Double => typeof(double),
                        StorageType.String => typeof(string),
                        StorageType.ElementId => typeof(ElementId),
                        _ => throw new NotImplementedException()
                    };
                    var parameterType = method.GetParameters().FirstOrDefault()?.ParameterType;
                    if (parameterType != expectedParameterType)
                    {
                        continue;
                    }
                }

                var comments = () => RevitDocumentationReader.GetMethodComments(method);
                IAccessor memberAccessor = null;
                MemberKind memberKind = method.IsStatic == true ? MemberKind.StaticMethod : MemberKind.Method;               
                memberAccessor = MemberAccessorFactory.CreateMemberAccessor(method, null);
                    
                var member = new MemberDescriptor(type, memberKind, method.Name, method.DeclaringType, memberAccessor, comments);
                yield return member;
            }
        }
    }
}