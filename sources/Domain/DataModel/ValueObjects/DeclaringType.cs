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
            string name = declaringType.GetCSharpName();            
            int level = declaringType.NumberOfBaseTypes();

            if (declaringType.IsAssignableFrom(snoopableObjectTyppe) == false)
            {
                level = 13;
                if (!string.IsNullOrEmpty(name))
                {
                    level += name[0];
                }
            }

            return new DeclaringType()
            {
                Name = name,
                InheritanceLevel = level,
            };
        }
    }
}