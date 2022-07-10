using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes.Base
{
    internal interface IValueType
    {
        Type Type { get; }
        string TypeName { get; }
        IValueType SetValue(Document document, object value);
        string ValueAsString { get; }        
        bool CanBeSnooped { get; }
        IEnumerable<SnoopableObject> Snoop(Document document);        
    }
}