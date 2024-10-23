using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Base
{
    internal interface IMemberOverride
    {
        public string UniqueId { get; }
        public Func<IAccessor> MemberAccessorFactory { get; init; }
    }



    internal class MemberOverride<TForType> : IMemberOverride
    {
        public string UniqueId { get; init; }
        public Func<IAccessor> MemberAccessorFactory { get; init; }
                

        public static IMemberOverride ByFunc<TReturnType>(Expression<Func<Document, TForType, TReturnType>> getter)
        {
            var compiledGetter = getter.Compile();   
            string syntax = getter.ToCeSharp();           
            var uniqueId = getter.GetUniqueId();

            return new MemberOverride<TForType>()
            {
                UniqueId = uniqueId,
                MemberAccessorFactory = () =>  
                { 
                    var accessor = new MemberAccessorByFunc<TForType, TReturnType>(compiledGetter); 
                    accessor.DefaultInvocation.Syntax = syntax;
                    return accessor;
                }
            };            
        }

        public static IMemberOverride ByFuncWithParam<TParam0Type, TReturnType>(Expression<Func<Document, TForType, TParam0Type, TReturnType>> getter, Func<Document, TForType, IEnumerable<TParam0Type>> param_0_arguments)
        {
            var compiledGetter = getter.Compile();
            string syntax = getter.ToCeSharp();
            var uniqueId = getter.GetUniqueId();

            var param_0_name = getter.Parameters[2].Name;

            return new MemberOverride<TForType>()
            {
                UniqueId = uniqueId,
                MemberAccessorFactory = () =>
                {
                    var accessor = new MemberAccessorByFuncUltra<TForType, TParam0Type, TReturnType>(compiledGetter, param_0_arguments, param_0_name);
                    accessor.DefaultInvocation.Syntax = syntax;
                    return accessor;
                }
            };
        }
    }
}
