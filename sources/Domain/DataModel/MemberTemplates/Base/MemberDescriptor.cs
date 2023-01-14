using System;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using static RevitDBExplorer.Domain.DataModel.SnoopableMember;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates.Base
{
    internal class MemberDescriptor
    {
        public Kind MemberKind { get; }
        public string Name { get; }
        public Type DeclaringType { get; }
        public IMemberAccessor MemberAccessor { get; }
        public Func<DocXml> DocumentationFactoryMethod { get; }


        public MemberDescriptor(Kind memberKind, string name, Type declaringType, IMemberAccessor memberAccessor, Func<DocXml> documentationFactoryMethod)
        {
            MemberKind = memberKind;
            Name = name;
            DeclaringType = declaringType;
            MemberAccessor = memberAccessor;
            DocumentationFactoryMethod = documentationFactoryMethod;
        }
    }
}