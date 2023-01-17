using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal enum Icon { None, Empty, ProjectParameter, SharedParameter }
    internal class SnoopableObject : BaseViewModel
    {        
        private readonly List<SnoopableObject> items;   

        public SnoopableContext Context { get; }
        public object Object { get; }     
        public string Name { get; init; }
        public string NamePrefix { get; init; }
        public Icon NamePrefixIcon { get; init; }
        public string TypeName { get; }       
        public IEnumerable<SnoopableObject> Items => items;
        public int Index { get; init; } = -1;


        public SnoopableObject(Document document, object @object) : this(document, @object, null)
        {

        }
        public SnoopableObject(Document document, object @object, IEnumerable<SnoopableObject> subObjects)
        {
            if (@object is ElementId id)
            {
                var element = document?.GetElementOrCategory(id);
                if (element != null)
                {
                    @object = element;
                }
            }            
            this.Context = new SnoopableContext() { Document = document };
            this.Object = @object;               
            this.Name = @object is not null ? Labeler.GetLabelForObject(@object, this.Context) : "<null>";
            this.TypeName = @object?.GetType().GetCSharpName();

            if (subObjects != null)
            {
                items = new List<SnoopableObject>(subObjects);
            }
            else
            {
                if (@object is IEnumerable enumerable && @object?.GetType()?.FullName.StartsWith("System") == false)
                {
                    items = new List<SnoopableObject>();
                    foreach (var item in enumerable)
                    {
                        items.Add(new SnoopableObject(document, item));
                    }
                }
            }
            if (@object is Parameter parameter)
            {
                NamePrefixIcon = Icon.Empty;
                if (parameter.Id.Value() > -1)
                {
                    NamePrefixIcon = Icon.ProjectParameter;
                }
                if (parameter.IsShared)
                {
                    NamePrefixIcon = Icon.SharedParameter;
                }
            }
        }

                
        public virtual IEnumerable<SnoopableMember> GetMembers(UIApplication app)
        {
            if (isFrozen)
            {
                foreach (var frozenMember in frozenMembers)
                {
                    yield return frozenMember;
                }
                yield break;
            }
            if (Object == null)
            {
                yield break;
            }
            foreach (var member in CreateMembers(this))
            {                
                member.Read();                
                yield return member;                
            }
        }

        private static IEnumerable<SnoopableMember> CreateMembers(SnoopableObject snoopableObject)
        {
            foreach (var descriptor in MemberStreamer.StreamDescriptors(snoopableObject.Context, snoopableObject.Object))
            {
                var member = new SnoopableMember(snoopableObject, descriptor);
                yield return member;
            }
        }


        private bool isFrozen = false;
        IList<SnoopableMember> frozenMembers;
        private static readonly Type[] doNotFreeze = new Type[] { typeof(Document) , typeof(View), typeof(Element) };
        public void Freeze()
        {
            var objectType = Object.GetType();
            foreach (var forbiden in doNotFreeze)
            {
                if (forbiden.IsAssignableFrom(objectType))
                {
                    return;
                }
            }
            try
            {
                frozenMembers = GetMembers(null).ToList();
                foreach (SnoopableMember member in frozenMembers)
                {
                    member.Freeze();
                }
                isFrozen = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                //throw new Exception("" , ex);
#endif
            }
        }


        public static SnoopableObject CreateKeyValuePair(Document document, object key, object value, string keyPrefix = "key:", string valuePrefix = "value:")
        {
            return new SnoopableObject(document, key, new[] { new SnoopableObject(document, value) { NamePrefix = valuePrefix } }) { NamePrefix = keyPrefix };
        }
        public static SnoopableObject CreateInOutPair(Document document, object key, object value, string keyPrefix = "in:", string valuePrefix = "out:")
        {
            return new SnoopableObject(document, key, new[] { new SnoopableObject(document, value) { NamePrefix = valuePrefix } }) { NamePrefix = keyPrefix };
        }
    }
}