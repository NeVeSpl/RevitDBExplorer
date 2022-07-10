using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorFactory
    {
        private static readonly IHaveFactoryMethod[] MemberAccessorFactories = new IHaveFactoryMethod[]
        {
            new Element_Geometry(),
            new Element_GetDependentElements()
        };
        private static readonly Dictionary<string, IHaveFactoryMethod> MemberAccessorFactoriesLookup;

        static MemberAccessorFactory()
        {
            var type = typeof(IHaveFactoryMethod);
            var types = type.Assembly.GetTypes().Where(p => type.IsAssignableFrom(p) && !p.IsInterface).ToList();
            var memberAccessorFactories = types.Select(x => Activator.CreateInstance(x) as IHaveFactoryMethod);

            MemberAccessorFactoriesLookup = memberAccessorFactories.ToDictionary(x => x.TypeAndMemberName);
        }


        public static IMemberAccessor Create(string memberName, Type declaringType, MethodInfo getMethod, MethodInfo setMethod)
        {
            if (MemberAccessorFactoriesLookup.TryGetValue($"{declaringType.Name}.{memberName}", out IHaveFactoryMethod factory))
            {
                return factory.Create();
            }

            return new MemberAccessorByRef(getMethod, setMethod);
        }
    }
}