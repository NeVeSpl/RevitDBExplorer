using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueObjects
{
    internal readonly record struct DeclaringType
    {
        public string Name { get; init; }

        public int InheritanceLevel { get; init; }       



        public static DeclaringType Create(Type declaringType, Type snoopableObjectTyppe)
        {
            int level = declaringType.IsAssignableFrom(snoopableObjectTyppe) == false ? 13 : declaringType.NumberOfBaseTypes();

            return new DeclaringType()
            {
                Name = declaringType.GetCSharpName(),
                InheritanceLevel = level,
            };
        }
    }
}