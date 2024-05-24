using System;
using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitExplorer.Augmentations.Services;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Base
{
    internal class MemberDescriptor: IComparable<MemberDescriptor>, IEquatable<MemberDescriptor>
    {
        private readonly Lazy<DocXml> documentation;

        public Type ForType { get; }
        public MemberKind Kind { get; init; }
        public string Name { get; init; }
        public DeclaringType DeclaringType { get; init; }
        public IAccessor MemberAccessor { get; }
        public Func<DocXml> DocumentationFactoryMethod { get; }
        public DocXml Documentation => documentation?.Value ?? DocXml.Empty;
        public string IntroducedInRevitVersion { get; init; }


        public MemberDescriptor(Type forType, MemberKind memberKind, string name, Type declaringType, IAccessor memberAccessor, Func<DocXml> documentationFactoryMethod)
        {
            ForType = forType;
            Kind = memberKind;
            Name = name;            
            MemberAccessor = memberAccessor;
            DocumentationFactoryMethod = documentationFactoryMethod;
            IntroducedInRevitVersion = WhatIsNew.WhenIntroduced(memberAccessor.UniqueId);
            if (documentationFactoryMethod != null)
            {
                this.documentation = new Lazy<DocXml>(documentationFactoryMethod);
            }

            if (declaringType != null)
            {
                bool withoutMemberAccessor = memberAccessor == null || memberAccessor is MemberAccessorForNotExposed || memberAccessor is MemberAccessorForStatic;
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

        public int CompareTo(MemberDescriptor other)
        {
            if (this.DeclaringType.InheritanceLevel != other.DeclaringType.InheritanceLevel)
            {
                return this.DeclaringType.InheritanceLevel.CompareTo(other.DeclaringType.InheritanceLevel);
            }
            if (this.DeclaringType.Name != other.DeclaringType.Name)
            {
                return this.DeclaringType.Name.CompareTo(other.DeclaringType.Name);
            }
            if (this.Kind != other.Kind)
            {
                return this.Kind.CompareTo(other.Kind);
            }
            if (this.Name != other.Name)
            {
                return this.Name.CompareTo(other.Name);
            }
            return 0;
        }
        public bool Equals(MemberDescriptor other)
        {
            if (this.DeclaringType.InheritanceLevel != other.DeclaringType.InheritanceLevel)
            {
                return false;
            }
            if (this.DeclaringType.Name != other.DeclaringType.Name)
            {
                return false;
            }
            if (this.Kind != other.Kind)
            {
                return false;
            }
            if (this.Name != other.Name)
            {
                return false;
            }
            return true;
        }
        public string ComputeKey()
        {
            var key = $"{DeclaringType.InheritanceLevel:000}_{DeclaringType.Name}_{(int)Kind}_{Name}";
            return key;
        }       
    }
}