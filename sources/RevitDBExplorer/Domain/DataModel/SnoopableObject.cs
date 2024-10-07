using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure.StructuralSections;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
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
        public string NamePrefix { get; init; }
        public string Name { get; init; }
        public string NamePostfix { get; private set; }
        public Icon NamePrefixIcon { get; init; }
        public string TypeName { get; }       
        public IEnumerable<SnoopableObject> Items => items;
        public int Index { get; init; } = -1;
        public bool IsFrozen
        {
            get
            {
                return isFrozen;
            }
            set
            {
                isFrozen = value;
                OnPropertyChanged();
            }
        }
        public bool HasParameters => Object is Element;


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
                if (@object is IEnumerable enumerable && @object?.GetType()?.FullName.StartsWith("System") == false && (@object is not StructuralSection))
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
                NamePrefixIcon = parameter.GetOrgin() switch
                {
                    ParameterOrgin.Project => Icon.ProjectParameter, 
                    ParameterOrgin.Shared => Icon.SharedParameter,
                    _ => Icon.Empty 
                };               
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


        public IEnumerable<SnoopableParameter> GetParameters(UIApplication app)
        {
            if (Object is Element element)
            {
                var parameters = element.Parameters;
                foreach (var parameter in parameters)
                {
                    var snoopableParameter = new SnoopableParameter(this, parameter as Parameter);
                    snoopableParameter.Read();
                    yield return snoopableParameter;
                }
            }          
        }


        public IEnumerable<VisualizationItem> GetVisualization()
        {
            if (Object is not null)
            {
                var typeHandler = ValueContainerFactory.SelectTypeHandlerFor(Object.GetType());  
                return typeHandler.GetVisualization(this.Context, Object);                
            }
            return Enumerable.Empty<VisualizationItem>();
        }


        private bool isFrozen = false;
        IList<SnoopableMember> frozenMembers;
        private static readonly Type[] doNotFreeze = new Type[] { typeof(Document) , typeof(View), typeof(Element), typeof(Transform) };
        public void Freeze(int candies = 0)
        {
            if (candies > 2) return;

            if (Object == null)
            {
                return;
            }

            if (candies != 0)
            {
                var objectType = Object?.GetType();
                foreach (var forbiden in doNotFreeze)
                {
                    if (forbiden.IsAssignableFrom(objectType))
                    {
                        return;
                    }
                }
            }

            try
            {
                frozenMembers = GetMembers(null).ToList();
                frozenMembers.ForEach(x => x.Freeze(candies));               
                isFrozen = true;
                NamePostfix = "*" + DateTime.Now.ToString("HH:mm:ss");
                OnPropertyChanged(nameof(NamePostfix));   
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