using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class ResultOfSnooping
    {
        public string Title { get; init; }
        public IList<SnoopableObject> Objects { get; }



        public ResultOfSnooping()
        {
            Objects = new SnoopableObject[0];
        }


        public ResultOfSnooping(IList<SnoopableObject> objects)
        {
            this.Objects = objects;
        }
    }
}
