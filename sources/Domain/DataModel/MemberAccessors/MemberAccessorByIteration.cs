using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorByIteration : IMemberAccessor
    {
        private readonly MethodInfo getMethod;


        public MemberAccessorByIteration(MethodInfo getMethod)
        {
            this.getMethod = getMethod;
        }


        public ReadResult Read(SnoopableContext context, object @object)
        {
            var typeName = getMethod.ReturnType.GetCSharpName();
            return new ReadResult($"[{typeName}]", nameof(MemberAccessorByIteration), true);
        }
        public IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object)
        {            
            var result = new List<SnoopableObject>();
            var parameter = getMethod.GetParameters().First();

            var arg = new object[1];
            foreach (var input in StreamValues(parameter.ParameterType))
            {                
                try
                {
                    arg[0] = input;
                    var temp = getMethod.Invoke(@object, arg);

                    if (temp is ElementId { IntegerValue: > -1 } id)
                    {
                        temp = context.Document.GetElementOrCategory(id);
                    }

                    result.Add(SnoopableObject.CreateInOutPair(context.Document, input, temp, keyPrefix: parameter.Name + ":"));
                }
                catch
                {
                    break;
                }      
            }

            return result;
        }

        private IEnumerable<object> StreamValues(Type type)
        {
            if (type == typeof(int))
            {
                for (int i =  0; i < 757; ++i)
                {
                    yield return i;
                }
            }
            if (type.IsEnum)
            {
                foreach (var value in Enum.GetValues(type))
                {
                    yield return value;
                }
            }
            if (type == typeof(bool))
            {                
                yield return true;
                yield return false;
            }
        }


        public static Type[] HandledParameterTypes = new[] { typeof(int), typeof(bool), typeof(Enum) };
    }
}