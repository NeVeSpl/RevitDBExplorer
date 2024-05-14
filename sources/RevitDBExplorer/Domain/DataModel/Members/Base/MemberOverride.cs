using System;
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
            var methodCallExpression = getter.Body as MethodCallExpression;
            //var syntax = methodCallExpression.ToString();
            var uniqueId = getter.GetUniqueId();

            return new MemberOverride<TForType>()
            {
                UniqueId = uniqueId,
                MemberAccessorFactory = () =>  
                { 
                    var accessor = new MemberAccessorByFunc<TForType, TReturnType>(compiledGetter); 
                    //accessor.DefaultInvocation.Syntax = syntax;
                    return accessor;
                }
            };            
        }
    }
}
