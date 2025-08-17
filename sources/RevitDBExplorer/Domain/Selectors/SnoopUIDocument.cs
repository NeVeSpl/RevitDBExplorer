﻿using System.Collections.Generic;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopUIDocument : ISelector
    {
        public InfoAboutSource Info { get; } = new("UIDocument");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument;

            if (document == null) yield break;

            yield return new SnoopableObject(app.ActiveUIDocument.Document, document);
        }
    }
}