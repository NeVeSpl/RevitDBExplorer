using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableMember : BaseViewModel
    {
        public enum Kind { Property, Method, StaticMethod }

        private readonly SnoopableObject parent;
        private readonly Kind memberKind;
        private readonly string name;
        private readonly Type declaringType;
        private readonly int declaringTypeLevel;
        private readonly IMemberAccessor memberAccessor;
        private Exception valueAccessException;
        private string value;
        private string valueTypeName;
        private bool canBeSnooped;
        private Lazy<DocXml> documentation;


        public Kind MemberKind => memberKind;
        public string Name => name;
        public string DeclaringType => declaringType.Name;
        public int DeclaringTypeLevel => declaringTypeLevel;
        public bool HasException => valueAccessException is not null;
        public bool HasExceptionCouldNotResolveAllArguments => valueAccessException is CouldNotResolveAllArgumentsException;
        public string Value
        {
            get
            {
                if (valueAccessException is not null)
                {
                    return Labels.GetLabelForException(valueAccessException);
                }
                return value;
            }
        }
        public string ValueTypeName => valueTypeName;
        public bool CanBeSnooped => canBeSnooped;
        public DocXml Documentation => documentation?.Value ?? DocXml.Empty;
        
        public SnoopableMember(SnoopableObject parent, Kind memberKind, string name, Type declaringType, IMemberAccessor memberAccessor, Func<DocXml> documentationFactoryMethod)
        {
            this.parent = parent;
            this.memberKind = memberKind;
            this.name = name;
            this.declaringType = declaringType;
            this.declaringTypeLevel = declaringType.NumberOfBaseTypes();
            this.memberAccessor = memberAccessor;
            if (documentationFactoryMethod != null)
            {
                this.documentation = new Lazy<DocXml>(documentationFactoryMethod);
            }
        }


        public void ReadValue(Document document, object @object)
        {
            try
            {
                var result = memberAccessor.Read(document, @object);
                if (result.Exception is not null)
                {
                    valueAccessException = result.Exception;
                }
                else
                {
                    value = result.Value; 
                }
                valueTypeName = result.ValueTypeName;
                canBeSnooped = result.CanBeSnooped;
            }
            catch (Exception ex)
            {
                valueAccessException = ex;
            }
        }
        public IEnumerable<SnoopableObject> Snooop(UIApplication app)
        {
            return memberAccessor.Snoop(parent.Document, parent.Object);
        }
    }
}