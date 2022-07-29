using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ValueObjects;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableMember : BaseViewModel
    {
        public enum Kind { Property, Method, StaticMethod, Extra }

        private readonly SnoopableObject parent;
        private readonly DeclaringType declaringType;      
        private readonly IMemberAccessor memberAccessor;
        private readonly Lazy<DocXml> documentation;
        private Exception valueAccessException;
        private string value;
        private string valueTypeName;
        private bool canBeSnooped;
        


        public Kind MemberKind { get; }
        public string Name { get; }
        public string DeclaringTypeName => declaringType.Name;
        public int DeclaringTypeLevel => declaringType.InheritanceLevel;
        public DocXml Documentation => documentation?.Value ?? DocXml.Empty;


        public bool HasException => valueAccessException is not null;
        public bool HasAccessor => memberAccessor is null;
        public string Value
        {
            get
            {
                if (valueAccessException is not null)
                {
                    return Labeler.GetLabelForException(valueAccessException);
                }
                return value;
            }
        }
        public string ValueTypeName => valueTypeName;
        public bool CanBeSnooped => canBeSnooped;       
        

        public SnoopableMember(SnoopableObject parent, Kind memberKind, string name, Type declaringType, IMemberAccessor memberAccessor, Func<DocXml> documentationFactoryMethod)
        {
            this.parent = parent;
            this.MemberKind = memberKind;
            this.Name = name;
            this.declaringType = DeclaringType.Create(declaringType);      
            this.memberAccessor = memberAccessor;
            if (documentationFactoryMethod != null)
            {
                this.documentation = new Lazy<DocXml>(documentationFactoryMethod);
            }
        }


        public void ReadValue()
        {
            ReadValue(parent.Context, parent.Object);
        }
        private void ReadValue(SnoopableContext document, object @object)
        {
            
            try
            {
                var result = memberAccessor.Read(document, @object);    
                value = result.Label;                 
                valueTypeName = result.ValueTypeName;
                canBeSnooped = result.CanBeSnooped;
            }
            catch (Exception ex)
            {
                valueAccessException = ex;
            }
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(ValueTypeName));
            OnPropertyChanged(nameof(CanBeSnooped));
        }
        public IEnumerable<SnoopableObject> Snooop(UIApplication app)
        {
            return memberAccessor.Snoop(parent.Context, parent.Object);
        }
    }
}