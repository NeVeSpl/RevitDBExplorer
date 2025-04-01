using System;
using System.Collections.Generic;
using System.Linq;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal static class ValueContainerFactory
    {   
        private static readonly List<(Type type, Func<IValueContainer> factory)> FactoryMethodsForValueContainers = new List<(Type, Func<IValueContainer>)>();
        private static readonly ITypeHandler[] TypeHandlers = new ITypeHandler[]
        {
            // System primitives
            new BoolHandler(),
            new IntHandler(),
            new StringHandler(),
            new DoubleHandler(),
            new GuidHandler(),
            new EnumHandler<System.Enum>(),
            new TypeHandler(),
            // 
            new DoubleNullableHandler(),

            // APIObject primitives
            new ForgeTypeIdHandler(),
            new ElementIdHandler(),
            new XYZHandler(),
            new UVHandler(),
            new CategoryHandler(),
            new ColorHandler(),
            new CurveHandler(),
            new EdgeHandler(),
            new FaceHandler(),
            new SolidHandler(),
            new PointHandler(),
            new BoundingBoxXYZHandler(),
            new LocationHandler(),  

            //
            new RebarHandler(),
            new RebarConstrainedHandleHandler(),
            new GeometryElementHandler(),
            new TransformHandler(),
            new ParameterHandler(),
#if R2024_MIN
            new EvaluatedParameterHandler(),
#endif
            new FamilyParameterHandler(),
            new ParameterSetHandler(),
            new ParameterMapHandler(),
            new StructuralSectionHandler(),
            new IExternalApplicationHandler(),
            new UpdaterInfoHandler(),
            new BindingMapHandler(),
            new CategoryNameMapHandler(),
            new ScheduleFieldHandler(),
            new SchedulableFieldHandler(),
            new FailuresProcessingEventArgsHandler(),
            new DocumentChangedEventArgsHandler(),
            new RevitApiEventArgsHandler(),   
            new WorksetIdHandler(),
            new FailureDefinitionIdHandler(),
            new SpatialElementBoundaryOptionsHandler(),
            new BoundarySegmentHandler(),
            
            // generic
            new ElementHandler(),

            // collections
            new ExecuteResultCollectionHandler(),
            new IListElementIdHandler(),
            new IEnumerableHandler(),

            // one to rule them all
            new ObjectHandler(),
        };


        public static void Init()
        {

        }
        static ValueContainerFactory()
        {
            foreach (var typeHandler in TypeHandlers)
            {
                var closedType = typeof(ValueContainer<>).MakeGenericType(new Type[] { typeHandler.Type });
                //var field = closedType.GetField("typeHandler", BindingFlags.NonPublic | BindingFlags.Static);
                //field.SetValue(null, typeHandler);

                FactoryMethodsForValueContainers.Add((typeHandler.Type, closedType.CompileFactoryMethod<IValueContainer>()));
            }
        }
                


        private static readonly Dictionary<Type, Func<IValueContainer>> Cache_Factories = new();
        public static IValueContainer Create(Type type)
        {           
            var factory = Cache_Factories.GetOrCreate(type, SelectValueContainerFactory);           
            return factory();
        }
        private static Func<IValueContainer> SelectValueContainerFactory(Type type)
        {
            foreach (var pair in FactoryMethodsForValueContainers)
            {
                if (pair.type.IsAssignableFrom(type))
                {
                    var result = pair.factory;
                    return result;
                }
            }
            return FactoryMethodsForValueContainers.Last().factory;
        }



        private static readonly Dictionary<Type, ITypeHandler> Cache_TypeHandlers = new();
        public static ITypeHandler SelectTypeHandlerFor(Type type)
        {
            var typeHandler = Cache_TypeHandlers.GetOrCreate(type, SelectTypeHandlerInternal);         
            return typeHandler;
        }
        private static ITypeHandler SelectTypeHandlerInternal(Type type)
        {
            if (type.IsEnum)
            {
                var closedType = typeof(EnumHandler<>).MakeGenericType(new Type[] { type });
                var typehandler = Activator.CreateInstance(closedType) as ITypeHandler;
                return typehandler;
            }

            var selectedHandler = TypeHandlers.Last();

            foreach (var typeHandler in TypeHandlers)
            {
                if (typeHandler.Type.IsAssignableFrom(type))
                {
                    selectedHandler = typeHandler;
                    break;
                }
            }     
            
            if (type.IsValueType && selectedHandler is ObjectHandler)
            {
                var closedType = typeof(ValueTypeHandler<>).MakeGenericType(new Type[] { type });
                var typehandler = Activator.CreateInstance(closedType) as ITypeHandler;
                return typehandler;
            }

            return selectedHandler;
        }
    }
}