using System;
using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams.Base
{

    internal class MemberDescriptor
    {
        private readonly Lazy<DocXml> documentation;

        public Type ForType { get; }
        public MemberKind Kind { get; init; }
        public string Name { get; init; }
        public DeclaringType DeclaringType { get; init; }
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
                bool withoutMemberAccessor = memberAccessor == null;
                DeclaringType = DeclaringType.Create(declaringType, forType, withoutMemberAccessor);
            }            
        }

        private MemberDescriptor()
        {
           
        }
        public static IEnumerable<MemberDescriptor> CreateSeparators()
        {
            yield return new MemberDescriptor() { DeclaringType = DeclaringType.Separator, Name="", Kind = MemberKind.None };
            yield return new MemberDescriptor() { DeclaringType = DeclaringType.NotExposed, Name = "", Kind = MemberKind.None };
        }
    }
}