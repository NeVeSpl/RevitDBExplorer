using System;
using System.Collections.Generic;
using System.Linq.Expressions;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes.Base
{
    internal static class ValueTypeFactory
    {        
        //private static readonly bool RunStaticConstructorASAP = true;

        private static readonly List<(Type type, Func<IValueType> factory)> FactoryMethodsForValueTypes = new List<(Type, Func<IValueType>)>(); 
        private static readonly IValueType[] ValueTypes = new IValueType[]
        {
            // System primitives
            new BoolType(),
            new IntType(),
            new StringType(),
            new DoubleType(),
            new GuidType(),
            new EnumType(),

            // 
            new DoubleNullableType(),

            // APIObject primitives
            new BoundingBoxXYZType(),
            new CategoryType(),
            new XYZType(),
            new UVType(),
            new ColorType(),
            new ForgeTypeIdType(),

            //            
            new BindingMapType(),
            new CategoryNameMapType(),
            new ScheduleFieldType(),
            new ParameterType(),
            new FamilyParameterType(),
            new ParameterSetType(),
            new ParameterMapType(),
            new ElementIdType(),
            new ElementType(),
            
            //
            new IEnumerableType()
        };


        static ValueTypeFactory()
        {
            foreach (var valueType in ValueTypes)
            {
                var newExpression = Expression.New(valueType.GetType());
                var factoryLambda = Expression.Lambda<Func<IValueType>>(newExpression);
                var FactoryMethod = factoryLambda.Compile();
                FactoryMethodsForValueTypes.Add((valueType.Type, FactoryMethod));
            }
        }


        public static IValueType Create(Type type)
        {
            foreach (var pair in FactoryMethodsForValueTypes)
            {
                if (pair.type.IsAssignableFrom(type))
                {
                    var result = pair.factory();                   
                    return result;
                }
            }
            return new ObjectType(type);
        }
    }
}