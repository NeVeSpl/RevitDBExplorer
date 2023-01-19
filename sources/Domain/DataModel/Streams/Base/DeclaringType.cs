using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams.Base
{
    internal class DeclaringType: IEquatable<DeclaringType>, IComparable<DeclaringType>
    {
        public static readonly DeclaringType NotExposed = new DeclaringType(" ", 256);
        public static readonly DeclaringType Separator = new DeclaringType("", 14);
        private readonly Lazy<DocXml> documentation = null;

        public string Name { get; init; }
        public int InheritanceLevel { get; init; }
        public DocXml Documentation => documentation?.Value ?? DocXml.Empty;


        private DeclaringType(string name, int level,  Func<DocXml> documentationFactoryMethod = null)
        {
            Name = name;
            InheritanceLevel = level;
            if (documentationFactoryMethod != null)
            {
                documentation = new Lazy<DocXml>(documentationFactoryMethod);
            }
        }


        public static DeclaringType Create(Type declaringType, Type snoopableObjectType, bool withoutMemberAccessor)
        {
            string name = declaringType.GetCSharpName();            
            int level = declaringType.NumberOfBaseTypes();

            if (declaringType.IsAssignableFrom(snoopableObjectType) == false)
            {
                level = 13;
                if (!string.IsNullOrEmpty(name))
                {
                    level += name[0];
                }
            }

            if (withoutMemberAccessor)
            {
                name += " - Not exposed";
                level += 256;
            }

            var result = new DeclaringType(name, level, () => RevitDocumentationReader.GetTypeComments(declaringType));            

            return result;
        }

        public bool Equals(DeclaringType other)
        {
            return this.Name.Equals(other.Name);
        }

        public int CompareTo(DeclaringType other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}