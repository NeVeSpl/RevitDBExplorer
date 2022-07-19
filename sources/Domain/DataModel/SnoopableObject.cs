using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.Streams.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableObject : BaseViewModel
    {        
        private readonly List<SnoopableObject> items;         

        public object Object { get; }
        public Document Document { get; }
        public string Name { get; init; }
        public string NamePrefix { get; init; }
        public string TypeName { get; }       
        public IEnumerable<SnoopableObject> Items => items;
        public int Index { get; init; } = -1;

        
        public SnoopableObject(object @object, Document document, SnoopableObject child) : this(@object, document, new[] {child})
        {

        }
        public SnoopableObject(object @object, Document document, IEnumerable<SnoopableObject> subObjects = null)
        {
            this.Object = @object;
            this.Document = document;            
            this.Name = @object is not null ? Labels.GetLabelForObject(@object, document) : "";
            this.TypeName = @object?.GetType().GetCSharpName();

            if (subObjects != null)
            {
                items = new List<SnoopableObject>(subObjects);
            }
            else
            {
                if (@object is IEnumerable enumerable && @object is not string && @object is not IDictionary)
                {
                    items = new List<SnoopableObject>();
                    foreach (var item in enumerable)
                    {
                        items.Add(new SnoopableObject(item, document));
                    }
                }
            }
        }

                
        public IEnumerable<SnoopableMember> GetMembers(UIApplication app)
        {
            if (Object == null)
            {
                yield break;
            }
            foreach (var member in GetMembersFromStreams(app))
            {
                member.ReadValue(Document, Object);
                if (!member.HasExceptionCouldNotResolveAllArguments)
                {
                    yield return member;
                }
            }
        }

        private static readonly BaseStream[] Streams = new BaseStream[]
        {
            new SystemTypeStream(),
            new ForgeTypeIdStream(),
            new PartUtilsStream(),
            new SchemaTypeStream(),
        };
        private IEnumerable<SnoopableMember> GetMembersFromStreams(UIApplication app)
        {
            var type = Object.GetType();

            foreach (var stream in Streams)
            {
                foreach (var member in stream.Stream(this))
                {
                    yield return member;
                }
                if (stream.ShouldEndAllStreaming())
                {
                    yield break;
                }
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
                var memberAccessor = MemberAccessorFactory.Create(getMethod, null);
                var member = new SnoopableMember(this, SnoopableMember.Kind.Property, prop.Name, prop.DeclaringType, memberAccessor, comments);              
                yield return member;
            }

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (method.ReturnType == typeof(void)) continue;
                if (method.IsSpecialName) continue;
                if (method.DeclaringType == typeof(object)) continue;               

                var comments = () => RevitDocumentationReader.GetMethodComments(method);
                var memberAccessor = MemberAccessorFactory.Create(method, null);
                var member = new SnoopableMember(this, SnoopableMember.Kind.Method, method.Name, method.DeclaringType, memberAccessor, comments);
                 yield return member;
            }
        }


        public static SnoopableObject CreateKeyValuePair(Document document, object key, object value)
        {
            return new SnoopableObject(key, document, new SnoopableObject(value, document) { NamePrefix = "value:" }) { NamePrefix = "key:" };
        }
    }
}