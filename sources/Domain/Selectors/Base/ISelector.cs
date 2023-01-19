using System.Collections.Generic;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors.Base
{
    internal interface ISelector
    {
        IEnumerable<SnoopableObject> Snoop(UIApplication app);
    }
}
