using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorByEnum : IMemberAccessor
    {
        private readonly MethodInfo getMethod;

        public MemberAccessorByEnum(MethodInfo getMethod)
        {
            this.getMethod = getMethod;
        }

        public ReadResult Read(SnoopableContext context, object @object)
        {
            var typeName = getMethod.ReturnType.GetCSharpName();
            return new ReadResult($"[{typeName}]", "MemberAccessorByEnum", true, null);
        }

        public IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object)
        {
            var enumParameter = getMethod.GetParameters().First();

            var arg = new object[1];
            foreach (var value in Enum.GetValues(enumParameter.ParameterType))
            {
                arg[0] = value;
                var result = getMethod.Invoke(@object, arg);
               
                yield return SnoopableObject.CreateInOutPair(context.Document, value, result);
            }
        }
    }
}