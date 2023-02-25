using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Providers.Internals
{
    internal abstract class Provider
    {
        public string Syntax
        {
            get;
            init;
        } = String.Empty;


        public abstract IEnumerable<ElementId> GetIds(UIDocument uiDocument);
    }
}