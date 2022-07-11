using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableObject : BaseViewModel
    {
        private readonly object @object;
        private readonly Document document;
        private readonly string name;
        private readonly string typeName;
        private readonly List<SnoopableObject> items;


        public object Object => @object;
        public string Name
        {
            get
            {
                return name;
            }
        }
        public string TypeName
        {
            get
            {
                return typeName;
            }
        }
        public Document Document => document;
        public IEnumerable<SnoopableObject> Items => items;


        public SnoopableObject(object @object, Document document, string name = null, IEnumerable<SnoopableObject> subObjects = null)
        {
            this.@object = @object;
            this.document = document;
            this.name = name ?? Labels.GetNameForObject(@object?.GetType(), @object, document);
            this.typeName = @object?.GetType().Name;

            if (subObjects != null)
            {
                items = new List<SnoopableObject>(subObjects) ;
            }
            else
            {
                if (@object is IEnumerable enumerable && @object is not string)
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
            var type = @object.GetType();

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

                var comments = RevitDocumentationReader.GetPropertyComments(prop);

                var member = new SnoopableMember(this, SnoopableMember.Kind.Property, prop.Name, prop.DeclaringType, getMethod, null, comments);
                member.ReadValue(document, @object);
                yield return member;
            }

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (method.ReturnType == typeof(void)) continue;
                if (method.IsSpecialName) continue;
                if (method.DeclaringType == typeof(object)) continue;

                var comments = RevitDocumentationReader.GetMethodComments(method);

                var member = new SnoopableMember(this, SnoopableMember.Kind.Method, method.Name, method.DeclaringType, method, null, comments);
                member.ReadValue(document, @object);

                if (!member.HasExceptionCouldNotResolveAllArguments)
                    yield return member;
            }
        }
    }
}