using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Accessors
{
    internal class Invocation
    {
        public string Syntax { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
    }
}
