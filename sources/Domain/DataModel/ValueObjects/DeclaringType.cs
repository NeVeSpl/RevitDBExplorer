using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueObjects
{
    internal readonly record struct DeclaringType
    {
        public string Name { get; init; }

        public int InheritanceLevel { get; init; }       



        public static DeclaringType Create(Type type)
        {
            return new DeclaringType()
            {
                Name = type.GetCSharpName(),
                InheritanceLevel = type.NumberOfBaseTypes(),
            };
        }
    }
}