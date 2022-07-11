using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal interface IMemberAccessor
    {
        ReadResult Read(Document document, object @object);      
        IEnumerable<SnoopableObject> Snoop(Document document, object @object);
    }

    readonly ref struct ReadResult
    {
        public readonly string Value;
        public readonly string ValueTypeName;
        public readonly bool CanBeSnooped;
        public readonly Exception Exception;

        public ReadResult(string value, string valueTypeName, bool canBeSnooped, Exception exception = null)
        {
            Value = value;
            ValueTypeName = valueTypeName;
            CanBeSnooped = canBeSnooped;
            Exception = exception;
        }
    }
}