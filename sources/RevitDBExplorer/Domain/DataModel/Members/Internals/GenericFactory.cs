using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Internals
{
    internal interface IGenericFactory
    {

    }
    internal interface IGenericFactory2
    {
        Func<object, object> CreateCompiledLambda(MethodInfo getMethod);
        IAccessor CreateMemberAccessorByRefCompiled(MethodInfo getMethod);
        IAccessor CreateMemberAccessorByIteration(MethodInfo getMethod);
    }

    internal static class GenericFactory
    {
        private static readonly Dictionary<Type, IGenericFactory> Cache_Factories = new();
        private static readonly Dictionary<Type, Dictionary<Type, IGenericFactory2>> Cache_Factories_2 = new();

        public static IGenericFactory GetInstance(Type snoopedObjectType)
        {
            if (!Cache_Factories.TryGetValue(snoopedObjectType, out var factory))
            {
                var closedType = typeof(GenericFactory<>).MakeGenericType(new Type[] { snoopedObjectType });
                factory = Activator.CreateInstance(closedType) as IGenericFactory;
                Cache_Factories[snoopedObjectType] = factory;
            }
            return factory;
        }

        public static IGenericFactory2 GetInstance(Type snoopedObjectType, Type returnType)
        {
            if (!Cache_Factories_2.TryGetValue(snoopedObjectType, out var returnTypeCache))
            {
                returnTypeCache = new Dictionary<Type, IGenericFactory2>();
                Cache_Factories_2[snoopedObjectType] = returnTypeCache;
            }

            if (!returnTypeCache.TryGetValue(returnType, out var factory))
            {
                var closedType = typeof(GenericFactory2<,>).MakeGenericType(new Type[] { snoopedObjectType, returnType });
                factory = Activator.CreateInstance(closedType) as IGenericFactory2;
                returnTypeCache[returnType] = factory;
            }

            return factory;
        }
    }
    internal class GenericFactory<TSnoopedObjectType> : IGenericFactory
    {

    }

    internal class GenericFactory2<TSnoopedObjectType, TReturnType> : IGenericFactory2
    {
        private static readonly Dictionary<MethodInfo, Func<TSnoopedObjectType, TReturnType>> Cache_Lambdas = new();


        public Func<TSnoopedObjectType, TReturnType> CreateLambda(MethodInfo getMethod)
        {
            var lambda = Cache_Lambdas.GetOrCreate(getMethod, CreateLambdaInternal);
            return lambda;
        }
        private Func<TSnoopedObjectType, TReturnType> CreateLambdaInternal(MethodInfo getMethod)
        {
            var instance = Expression.Parameter(typeof(TSnoopedObjectType));
            var call = Expression.Call(instance, getMethod);
            var lambda = Expression.Lambda<Func<TSnoopedObjectType, TReturnType>>(call, instance);
            var func = lambda.Compile();
            return func;
        }
        public Func<TSnoopedObjectType, TParamType, TReturnType> CreateLambdaInternalWithOneParam<TParamType>(MethodInfo getMethod)
        {
            var instance = Expression.Parameter(typeof(TSnoopedObjectType));
            var inputPar = Expression.Parameter(typeof(TParamType));

            var parameters = getMethod.GetParameters();
            //var paramExprs = parameters.Select(x => Expression.Parameter(x.ParameterType)).ToArray();
            var inputParConverted = Expression.Convert(inputPar, parameters[0].ParameterType);
            var call = Expression.Call(instance, getMethod, inputParConverted);
            var lambda = Expression.Lambda<Func<TSnoopedObjectType, TParamType, TReturnType>>(call, instance, inputPar);
            var func = lambda.Compile();
            return func;
        }


        public Func<object, object> CreateCompiledLambda(MethodInfo getMethod)
        {
            var lambda = CreateLambda(getMethod);
            return (input) => { return lambda((TSnoopedObjectType)input); };
        }

        public IAccessor CreateMemberAccessorByRefCompiled(MethodInfo getMethod)
        {
            var func = CreateLambda(getMethod);
            var accessor = new MemberAccessorByRefCompiled<TSnoopedObjectType, TReturnType>(getMethod, func);

            return accessor;
        }

        public IAccessor CreateMemberAccessorByIteration(MethodInfo getMethod)
        {
            var accessor = new MemberAccessorByIteration<TSnoopedObjectType, TReturnType>(getMethod);

            return accessor;
        }
    }
}