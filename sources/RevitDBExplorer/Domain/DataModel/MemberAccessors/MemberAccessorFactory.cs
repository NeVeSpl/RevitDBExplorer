using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal static class MemberAccessorFactory
    {        
        private static readonly Dictionary<string, Func<IMemberAccessor>> forTypeMembers = new();


        public static void Init()
        {

        }
        static MemberAccessorFactory()
        {
            var accessors = GetAllInstancesThatImplement<ICanCreateMemberAccessor>();
            foreach (var accessor in accessors)
            {
                foreach (var handledMember in accessor.GetHandledMembers())
                {
                    var memberId = handledMember.GetUniqueId();
                    forTypeMembers[memberId] = accessor.GetType().CompileFactoryMethod<IMemberAccessor>();
                }
            }
        }
        private static IEnumerable<T> GetAllInstancesThatImplement<T>() where T : class
        {
            var type = typeof(T);
            var types = type.Assembly.GetTypes().Where(p => type.IsAssignableFrom(p) && !p.IsInterface).ToList();
            var instances = types.Select(x => Activator.CreateInstance(x) as T);
            return instances;
        }
       

        
        public static IMemberAccessor CreateMemberAccessor(MethodInfo getMethod, MethodInfo setMethod)
        {
            if (getMethod.ReturnType == typeof(void) && getMethod.Name != "GetOverridableHookParameters") return null;

            if (forTypeMembers.TryGetValue(getMethod.GetUniqueId(), out Func<IMemberAccessor> factory))
            {
                return factory();
            }

            var @params = getMethod.GetParameters();
          
            if (@params.Length == 0)
            {
                var genericFactory = GenericFactory.GetInstance(getMethod.DeclaringType, getMethod.ReturnType);
                var accessor = genericFactory.CreateMemberAccessorByRefCompiled(getMethod);
                return accessor;
            }            

            if (@params.Length == 1)
            {
                if (MemberAccessorByIteration.HandledParameterTypes.Contains(@params[0].ParameterType) || @params[0].ParameterType.IsEnum)
                {
                    var genericFactory = GenericFactory.GetInstance(getMethod.DeclaringType, getMethod.ReturnType);
                    var accessor = genericFactory.CreateMemberAccessorByIteration(getMethod);
                    return accessor;
                }
            }
            if (@params.All(x => MemberAccessorByRef.HandledParameterTypes.Contains(x.ParameterType)))
            {
                return new MemberAccessorByRef(getMethod, setMethod);
            }
            
            return null;
        }
    }

    internal interface ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> GetHandledMembers();
    }
}