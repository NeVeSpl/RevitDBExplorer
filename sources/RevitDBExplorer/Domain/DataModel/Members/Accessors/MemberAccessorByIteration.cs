using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Internals;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Accessors
{
    internal sealed class MemberAccessorByIteration<TSnoopedObjectType, TReturnType> : MemberAccessorTypedWithDefaultPresenter<TSnoopedObjectType>
    {
        private readonly string getMethodReturnTypeName;
        private readonly ParameterInfo getMethodParameter;
        private readonly Func<TSnoopedObjectType, object, TReturnType> func;


        public MemberAccessorByIteration(MethodInfo getMethod)
        {
            getMethodReturnTypeName = getMethod.ReturnType.GetCSharpName();
            getMethodParameter = getMethod.GetParameters().First();

            var factory = new GenericFactory2<TSnoopedObjectType, TReturnType>();
            func = factory.CreateLambdaInternalWithOneParam<object>(getMethod);
        }


        protected override ReadResult Read(SnoopableContext context, TSnoopedObjectType @object)
        {
            var count = CountValues(context, getMethodParameter.ParameterType);
            var canBeSnooped = count > 0 || count.HasValue == false;
            return new ReadResult(Labeler.GetLabelForCollection(getMethodReturnTypeName, count), "[ByIteration]", canBeSnooped, false);
        }
        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, TSnoopedObjectType @object, IValueContainer state)
        {
            var result = new List<SnoopableObject>();

            foreach (var input in StreamValues(context, getMethodParameter.ParameterType))
            {
                object resultOfInvocation = null;
                try
                {
                    resultOfInvocation = func(@object, input);
                }
                catch (Exception ex)
                {
                    if (getMethodParameter.ParameterType == typeof(int) && IsNotExpectedException(ex))
                    {
                        break;
                    }
                    resultOfInvocation = ex;// Labeler.GetLabelForException(ex);
                }
                result.Add(SnoopableObject.CreateInOutPair(context.Document, input, resultOfInvocation, keyPrefix: getMethodParameter.Name + ":"));
            }

            return result;
        }
        private bool IsNotExpectedException(Exception ex)
        {
            if (ex is Autodesk.Revit.Exceptions.ArgumentOutOfRangeException)
            {
                return true;
            }

            if (ex is Autodesk.Revit.Exceptions.ArgumentException argumentException)
            {
                if (argumentException.ParamName == "barEnd")
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<object> StreamValues(SnoopableContext context, Type type)
        {
            if (type == typeof(int))
            {
                for (int i = 0; i < 757; ++i)
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
        private int? CountValues(SnoopableContext context, Type type)
        {
            if (type == typeof(int))
            {
                return null;
            }
            if (type.IsEnum)
            {
                return Enum.GetNames(type).Length;
            }
            if (type == typeof(bool))
            {
                return 2;
            }
            if (type == typeof(Phase))
            {
                return context.Document.Phases.Size;
            }
            if (type == typeof(Level))
            {
                return null;
            }

            return null;
        }
    }

    internal sealed class MemberAccessorByIteration
    {
        public static Type[] HandledParameterTypes = new[] { typeof(int), typeof(bool), typeof(Enum), typeof(Phase), typeof(Level) };
    }
}