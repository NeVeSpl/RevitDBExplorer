using System;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams.Base
{

    internal class MemberDescriptor
    {
        private readonly Lazy<DocXml> documentation;

        public Type ForType { get; }
        public MemberKind Kind { get; }
        public string Name { get; }
        public DeclaringType DeclaringType { get; }
        public IMemberAccessor MemberAccessor { get; }
        public Func<DocXml> DocumentationFactoryMethod { get; }
        public DocXml Documentation => documentation?.Value ?? DocXml.Empty;


        public MemberDescriptor(Type forType, MemberKind memberKind, string name, Type declaringType, IMemberAccessor memberAccessor, Func<DocXml> documentationFactoryMethod)
        {
            ForType = forType;
            Kind = memberKind;
            Name = name;            
            MemberAccessor = memberAccessor;
            DocumentationFactoryMethod = documentationFactoryMethod;
            if (documentationFactoryMethod != null)
            {
                this.documentation = new Lazy<DocXml>(documentationFactoryMethod);
            }

            if (declaringType != null)
            {
                DeclaringType = DeclaringType.Create(declaringType, forType);
            }
            if (memberAccessor == null)
            {
                DeclaringType = DeclaringType.NotExposed;
            }
        }
    }
}