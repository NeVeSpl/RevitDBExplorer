using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class MemberAccessorByIteration : MemberAccessorTyped<object>
    {
        private readonly MethodInfo getMethod;


        public MemberAccessorByIteration(MethodInfo getMethod)
        {
            this.getMethod = getMethod;
        }


        public override ReadResult Read(SnoopableContext context, object @object)
        {
            var typeName = getMethod.ReturnType.GetCSharpName();
            return new ReadResult($"[{typeName}]", nameof(MemberAccessorByIteration), true);
        }
        public override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object)
        {            
            var result = new List<SnoopableObject>();
            var parameter = getMethod.GetParameters().First();

            var arg = new object[1];
            foreach (var input in StreamValues(context, parameter.ParameterType))
            {    
                arg[0] = input;     
                object resultOfInvocation = null;
                try
                {

                    resultOfInvocation = getMethod.Invoke(@object, arg);                    
                }
                catch (Exception ex)
                {
                    if (parameter.ParameterType == typeof(int))
                    {
                        break;
                    }
                    resultOfInvocation = Labeler.GetLabelForException(ex);
                }
                result.Add(SnoopableObject.CreateInOutPair(context.Document, input, resultOfInvocation, keyPrefix: parameter.Name + ":"));
            }

            return result;
        }

        private IEnumerable<object> StreamValues(SnoopableContext context, Type type)
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
            if (type == typeof(Phase))
            {
                foreach (Phase phase in context.Document.Phases)
                {
                    yield return phase;
                }
            }
            if (type == typeof(Level))
            {
                foreach (Level level in new FilteredElementCollector(context.Document).OfClass(typeof(Level)).ToElements())
                {
                    yield return level;
                }
            }

        }


        public static Type[] HandledParameterTypes = new[] { typeof(int), typeof(bool), typeof(Enum), typeof(Phase), typeof(Level) };
    }
}