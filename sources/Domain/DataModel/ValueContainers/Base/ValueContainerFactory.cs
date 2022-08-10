using System;
using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal static class ValueContainerFactory
    {        
        //private static readonly bool RunStaticConstructorASAP = true;

        private static readonly List<(Type type, Func<IValueContainer> factory)> FactoryMethodsForValueContainers = new List<(Type, Func<IValueContainer>)>();
        private static readonly IValueContainer[] ValueContainers = new IValueContainer[]
        {
            // System primitives
            new BoolContainer(),
            new IntContainer(),
            new StringContainer(),
            new DoubleContainer(),
            new GuidContainer(),
            new EnumContainer(),

            // 
            new DoubleNullableContainer(),

            // APIObject primitives
            new ForgeTypeIdContainer(),
            new ElementIdContainer(),
            new XYZContainer(),
            new UVContainer(),
            new CategoryContainer(),
            new ColorContainer(),
            new BoundingBoxXYZContainer(),      

            //
            new TransformContainer(),
            new ParameterContainer(),
            new FamilyParameterContainer(),
            new ParameterSetContainer(),
            new ParameterMapContainer(),
            new StructuralSectionContainer(),
            new IExternalApplicationContainer(),
            new UpdaterInfoContainer(),
            new BindingMapContainer(),
            new CategoryNameMapContainer(),
            new ScheduleFieldContainer(),  
            new FailuresProcessingEventArgsContainer(),
            new DocumentChangedEventArgsContainer(),
            new RevitApiEventArgsContainer(),            
            
            // generic
            new ElementContainer(),

            // collections
            new IListElementIdContainer(),
            new IEnumerableContainer()
        };


        static ValueContainerFactory()
        {
            foreach (var valueType in ValueContainers)
            {              
                FactoryMethodsForValueContainers.Add((valueType.Type, valueType.GetType().CompileFactoryMethod<IValueContainer>()));
            }
        }


        public static IValueContainer Create(Type type)
        {
            foreach (var pair in FactoryMethodsForValueContainers)
            {
                if (pair.type.IsAssignableFrom(type))
                {
                    var result = pair.factory();                   
                    return result;
                }
            }
            return new ObjectContainer(type);
        }
    }
}